using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LucidFactory.Cards.UI.Interfaces;
using LucidFactory.Cards.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LucidFactory.Cards.UI.Hand
{
    public abstract class HandUI<T, TU> : CardCollectionUI<T, TU>, ICardDraggableController
        where T : ICard
        where TU : ICardCollection<T>
    {
        [Serializable]
        public struct CardStateSettingsEntry
        {
            [field: SerializeField]
            [field: ListDrawerSettings(DefaultExpandedState = true, ShowFoldout = false)]
            public CardState[] States { get; private set; }

            [field: SerializeField]
            public CardStateSettings Settings { get; private set; }
        }

        [SerializeField, BoxGroup("Hand UI")]
        private CardStateSettingsEntry[] entries;

        [SerializeField, BoxGroup("Hand UI")]
        private bool placeholderCards;

        [field: SerializeField, AssetsOnly, BoxGroup("Hand UI")]
        internal CardContainer CardContainerPrefab { get; private set; }

        [ShowInInspector, ReadOnly, BoxGroup("Hand UI")]
        private CardStateContainerCollection[] containerCollections;

        private Dictionary<T, ICardUI<T>> cards;

        [ShowInInspector, ReadOnly, BoxGroup("Hand UI")]
        private Dictionary<CardState, int> statesToEntryIndex;
        [ShowInInspector, ReadOnly, BoxGroup("Hand UI")]
        private Dictionary<CardState, int> statesToContainerCollectionIndex;

        private readonly Dictionary<ICardUI,List<CardState>> preservedStates = new();
        private readonly Dictionary<ICardUI, EntryCollection> cardEventEntries = new();
        private readonly List<ICardUI> cardsToSnap = new();

        private RectTransform root;

        protected sealed override void Awake()
        {
            base.Awake();
            statesToContainerCollectionIndex = new();
            statesToEntryIndex = new();
            cards = new();

            root = CreateCardRoot("Default Card Container");
            CreateContainersCollections();

            OnValidate();

            (transform as RectTransform)?.ForceUpdateRectTransforms();
        }

        protected virtual void OnEnable()
        {
            Canvas.preWillRenderCanvases += UpdateCards;
        }

        protected virtual void OnDisable()
        {
            Canvas.preWillRenderCanvases -= UpdateCards;
        }

        private void OnValidate()
        {
            (statesToEntryIndex??=new()).Clear();
            if(entries == null)
                return;

            for (int i = 0; i < entries.Length; i++)
            {
                var cardStateSettings = entries[i];
                CardState[] cardStates = cardStateSettings.States;
                if(cardStates == null)
                    continue;

                for (int j = 0; j < cardStates.Length; j++)
                {
                    var state = cardStates[j];
                    if (state)
                        statesToEntryIndex.TryAdd(state, i);
                }
            }
        }

        protected virtual void UpdateCards()
        {
            if (transform is RectTransform rectTransform)
            {
                rectTransform.ForceUpdateRectTransforms();
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
            //Caching delta time
            float deltaTime = Time.unscaledDeltaTime;
            if (statesToEntryIndex == null) return;

            foreach ((_, ICardUI<T> cardUI) in cards)
            {
                CardState state = cardUI.CurrentState;
                if (state == null)
                    continue;

                if (!GetSettingsForState(state, out CardStateSettings stateSettings) ||
                    !GetContainerCollectionForState(state, out CardStateContainerCollection containerCollection))
                    continue;

                if (!stateSettings.ConstraintCard)
                {
                    cardUI.transform.rotation = Quaternion.Slerp(
                        cardUI.transform.rotation,
                        Quaternion.identity,
                        15 * deltaTime);
                    continue;
                }

                //No snapping applied
                float stateSettingsSnapStrength = stateSettings.ConstraintStrength;
                if (stateSettingsSnapStrength <= 0 || cardUI.IsDragged)
                    continue;

                int indexOf = cardsToSnap.IndexOf(cardUI);
                if (indexOf == -1)
                {
                    Vector3 containerPos = containerCollection.Position(cardUI);
                    Quaternion containerRot = containerCollection.Rotation(cardUI);
                    cardUI.transform.position = Vector3.Lerp(
                        cardUI.transform.position,
                        containerPos,
                        stateSettingsSnapStrength * deltaTime);

                    cardUI.transform.rotation = Quaternion.Slerp(
                        cardUI.transform.rotation,
                        containerRot,
                        stateSettingsSnapStrength * deltaTime);
                }
                else
                {
                    cardUI.RectTransform.anchoredPosition = containerCollection.AnchoredPosition(cardUI);
                    cardsToSnap.RemoveAt(indexOf);
                }
            }
        }

        private RectTransform CreateCardRoot(string name)
        {
            GameObject child = new GameObject(name);
            child.layer = gameObject.layer;

            child.transform.SetParent(transform);

            var rectTransform = child.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = Vector2.one * 0.5f;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.localPosition = Vector2.zero;
            rectTransform.localScale = Vector3.one;

            return rectTransform;
        }

        private void CreateContainersCollections()
        {
            var dic = new Dictionary<(RectTransform, RectTransform), int>(entries.Length);
            List<Transform> validTransformes = new();
            List<CardStateContainerCollection> collectionsList = new();

            statesToContainerCollectionIndex.Clear();
            for (int i = 0; i < entries.Length; i++)
            {
                CardStateSettingsEntry entry = entries[i];
                CardStateSettings stateSettings = entry.Settings;

                RectTransform containerRoot = stateSettings.CardContainerParent == null
                    ? root
                    : stateSettings.CardContainerParent;

                RectTransform cardRoot = stateSettings.CardParent == null
                    ? root
                    : stateSettings.CardParent;

                if(!validTransformes.Contains(containerRoot))
                    validTransformes.Add(containerRoot);
                if(!validTransformes.Contains(cardRoot))
                    validTransformes.Add(cardRoot);

                //If no similar container, creating a new one
                if (!dic.TryGetValue((containerRoot, cardRoot), out int index))
                {
                    //Clearing every objet that was put in the scene for testing purposes
                    if (stateSettings is { ConstraintCard: true, ClearContainerOnAwake: true } && containerRoot != root)
                    {
                        int childCount = containerRoot.childCount;
                        for (int j = 0; j < childCount; j++)
                        {
                            Transform child = containerRoot.GetChild(j);
                            if(!validTransformes.Contains(child))
                                Destroy(child.gameObject);
                        }
                    }

                    index = collectionsList.Count;
                    collectionsList.Add(new CardStateContainerCollection(containerRoot, cardRoot));

                    dic.Add((containerRoot, cardRoot), index);
                }

                CardState[] entryStates = entry.States;
                for (int j = 0; j < entryStates.Length; j++)
                    statesToContainerCollectionIndex.TryAdd(entryStates[j], index);
            }

            containerCollections = collectionsList.ToArray();
        }



        protected abstract bool TryCreateCardUIForCard(T card, out ICardUI<T> createdCard);
        protected virtual void DestroyCardUI(T card, ICardUI<T> cardUI) => Destroy(cardUI.gameObject);
        protected virtual void SetupCardUI(T card, ICardUI<T> cardUI) { }

        public abstract IEnumerable<ICardDropSlot> GetSlots(ICardUI cardUI);

        protected override void OnCardAdded(T card)
        {
            if (!TryCreateCardUIForCard(card, out ICardUI<T> cardUI))
                return;

            if (!cards.TryAdd(card, cardUI))
            {
                Debug.LogError("Couldn't add card");
                DestroyCardUI(card, cardUI);
                return;
            }

            //Registering inside container
            for (int i = 0; i < containerCollections.Length; i++)
            {
                var containerCollection = containerCollections[i];
                if (!containerCollection.IsInContainer(cardUI))
                {
                    if(CardContainerPrefab == null)
                        Debug.LogError("No prefab was assigned.", this);
                    else
                        containerCollection.Add(cardUI, CardContainerPrefab, entries
                            .Where(ctx => ctx.States.Any(state => state == cardUI.CurrentState))
                            .Select(ctx => ctx.Settings));

                }
            }
            RefreshContainersSiblingIndexes();

            preservedStates.Add(cardUI, new());
            cardEventEntries.Add(cardUI, new EntryCollection(cardUI));

            cardUI.OnStateChanged += SyncWithCardState;
            cardUI.BindToDraggableController(this);
            cardUI.Setup(card, placeholderCards);
            SetupCardUI(card, cardUI);
            SetState(cardUI, cardUI.CurrentState);

            // SnapCardToCurrentState(cardUI);
            cardsToSnap.Add(cardUI);

            Canvas.ForceUpdateCanvases();
        }

        protected override void OnCardRemoved(T card)
        {
            if (TryGetCardUI(card, out ICardUI<T> cardUI))
            {
                cards.Remove(card);
                for (int i = 0; i < containerCollections.Length; i++)
                {
                    CardStateContainerCollection cardStateContainerCollection = containerCollections[i];
                    if(cardStateContainerCollection.IsInContainer(cardUI))
                        cardStateContainerCollection.Remove(cardUI);
                }

                preservedStates.Remove(cardUI);
                if (cardEventEntries.Remove(cardUI, out var collection))
                    collection.Dispose();

                cardUI.OnStateChanged -= SyncWithCardState;
                cardUI.BindToDraggableController(this);
                cardUI.Dispose(card);

                DestroyCardUI(card, cardUI);
                RefreshContainersSiblingIndexes();
                Canvas.ForceUpdateCanvases();
            }
        }

        private void RefreshContainersSiblingIndexes()
        {
            Dictionary<ICardUI, int> cardsIndexes = new();
            if (CurrentCardCollection == null)
            {
                foreach ((_, ICardUI<T> cardUI) in cards)
                    cardsIndexes.Add(cardUI,  cardUI.transform.GetSiblingIndex());
            }
            else
            {
                int idx = 0;
                foreach (T card in CurrentCardCollection.Cards)
                {
                    if (TryGetCardUI(card, out ICardUI<T> cardUI))
                        cardsIndexes.Add(cardUI, idx++);
                }
            }

            for (int i = 0; i < containerCollections.Length; i++)
                containerCollections[i].UpdateSiblingIndexes(cardsIndexes);
        }
/*
        private void SnapCardToCurrentState(ICardUI createdCard)
        {
            if (!GetSettingsForState(createdCard.CurrentState, out var stateSettings) ||
                !GetContainerCollectionForState(createdCard.CurrentState, out CardStateContainerCollection container))
                return;

            if (!stateSettings.ConstraintCard)
                return;
            //No snapping applied
            float stateSettingsSnapStrength = stateSettings.ConstraintStrength;
            if (stateSettingsSnapStrength <= 0)
                return;

            createdCard.transform.SetPositionAndRotation(container.Position(createdCard), container.Rotation(createdCard));
        }
*/
        private void SyncWithCardState(ICardUI cardUI, StateTransition stateTransition)
        {
            CardStateContainerCollection containerCollection;

            CardState to = stateTransition.To;
            CardState from = stateTransition.From;

            bool preserve = GetSettingsForState(to, out var settings) && settings.PreserveLastState;

            List<CardState> states = preservedStates[cardUI];

            if (GetSettingsForState(from, out settings) &&
                GetContainerCollectionForState(from, out containerCollection))
                states.Add(from);

            if (!preserve)
            {
                foreach (CardState preserved in states)
                {
                    if (GetContainerCollectionForState(preserved, out containerCollection) &&
                        GetSettingsForState(preserved, out settings))
                        containerCollection.MoveOutContainer(cardUI, settings);
                }

                states.Clear();
            }

            SetState(cardUI, to);
        }

        private void SetState(ICardUI cardUI, CardState to)
        {
            if(!isActiveAndEnabled)
                return;

            if(GetSettingsForState(to, out CardStateSettings settings) &&
               GetContainerCollectionForState(to, out var containerCollection))
            {
                containerCollection.MoveInContainer(cardUI, settings);
            }
        }

        private bool GetSettingsForState(CardState state, out CardStateSettings settings)
        {
            if (state != null && statesToEntryIndex.TryGetValue(state, out int index))
            {
                settings = entries[index].Settings;
                return true;
            }

            settings = default;
            return false;
        }

        private bool GetContainerCollectionForState(CardState state, out CardStateContainerCollection collection)
        {
            if (state != null && statesToContainerCollectionIndex.TryGetValue(state, out int index))
            {
                collection = containerCollections[index];
                return true;
            }

            collection = default;
            return false;
        }

        public bool TryGetCardUI<TC>(T card, out TC cardUI) where TC : ICardUI<T>
        {
            if (TryGetCardUI(card, out ICardUI<T> result) && result is TC tc)
            {
                cardUI = tc;
                return true;
            }

            cardUI = default;
            return false;
        }


        public bool AddEntriesToCardUI(ICardUI cardUI, params (EventTriggerType, UnityAction<BaseEventData>)[] eventEntries)
        {
            var array = new EventTrigger.Entry[eventEntries.Length];
            for (int i = 0; i < eventEntries.Length; i++)
            {
                var triggerEvent = new EventTrigger.TriggerEvent();
                var tuple = eventEntries[i];

                triggerEvent.AddListener(tuple.Item2);

                array[i] = new EventTrigger.Entry()
                {
                    eventID = tuple.Item1,
                    callback = triggerEvent,
                };
            }

            return AddEntriesToCardUI(cardUI, array);
        }
        public bool AddEntriesToCardUI(ICardUI cardUI, params EventTrigger.Entry[] entries)
        {
            if (cardEventEntries.TryGetValue(cardUI, out var collection))
            {
                collection.AddEntries(entries);
                return true;
            }

            return false;
        }
        public bool RemoveEntriesToCardUI(ICardUI cardUI, params EventTrigger.Entry[] entries)
        {
            if (cardEventEntries.TryGetValue(cardUI, out var collection))
            {
                collection.RemoveEntries(entries);
                return true;
            }

            return false;
        }

        public bool TryGetCardUI(T card, out ICardUI<T> cardUI) => cards.TryGetValue(card, out cardUI);
    }
}