using System;
using System.Collections.Generic;
using LTX.ChanneledProperties;
using LucidFactory.Cards.UI.Interfaces;
using LucidFactory.Cards.Utility;
using LucidFactory.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LucidFactory.Cards.UI
{

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T">Card</typeparam>
    public abstract class CardUI<T> : Draggable<ICardDropSlot>,
        ICardUI<T>,
        IPointerEnterHandler,
        IPointerExitHandler,
        ISelectHandler,
        IDeselectHandler

        where T : ICard
    {
        public event Action<ICardUI, StateTransition> OnStateChanged;
        ////Events
        public event Action<ICardUI<T>, CardDropContext> OnDropSucceed;
        public event Action<ICardUI<T>, Vector2> OnDragged;
        public event Action<ICardUI<T>> OnDropCanceled;

        public event Action<ICardUI<T>> OnSetup;
        public event Action<ICardUI<T>> onDisposed;

        [field: SerializeField, ReadOnly]
        public CardState LastState { get; private set; }
        [field: SerializeField, ReadOnly]
        public CardState CurrentState { get; private set; }

        private new Camera camera;

        public bool IsHovered { get; private set; }
        public bool IsSelected { get; private set; }


        private EventTrigger eventTrigger;

        public EventTrigger EventTrigger
        {
            get
            {
                if (eventTrigger == null && !TryGetComponent(out eventTrigger))
                    eventTrigger = gameObject.AddComponent<EventTrigger>();

                return eventTrigger;
            }
        }

        ICard ICardUI.Card => Card;

        public T Card { get; private set; }
        public CardUI<T> PlaceholderCard { get; private set; }
        public ICardDraggableController DraggableController { get; private set; }
        public PrioritisedProperty<CardState> StateProperty { get; private set; }

        public CardStateCollection CardStateCollection { get; private set; }

        protected virtual string CardStateCollectionPath => "Cards/CardStateCollection";



        protected sealed override IEnumerable<ICardDropSlot> Slots =>
            DraggableController?.GetSlots(this) ??
            Array.Empty<ICardDropSlot>();

        ////Game Feel settings

        /// <summary>
        /// How fast the card will try to go to the destination
        /// </summary>
        protected virtual float DragSmoothness => 8f;

        /// <summary>
        /// How fast the card will go to a rotation applied by movement
        /// </summary>
        protected virtual float RotationInSmoothness => 8f;

        /// <summary>
        /// How fast the card will go back to neutral rotation
        /// </summary>
        protected virtual float RotationOutSmoothness => .5f;

        /// <summary>
        /// Is card enabled to tilt depending on movement
        /// </summary>
        protected virtual bool EnableTilt => true;

        /// <summary>
        /// How much the card will tilt in the current direction
        /// </summary>
        protected virtual float TiltStrength => 45;

        /// <summary>
        /// At what speed the card will have de max tilt
        /// </summary>
        protected virtual float MaxTiltSpeed => 10;

        /// <summary>
        /// Direction used to tilt the card
        /// </summary>
        protected virtual Vector3 FrontDirection =>
            transform.parent != null ? transform.parent.forward : Vector3.forward;

        /// <summary>
        /// Idle rotation, the one the card will have when not moving
        /// </summary>
        public Quaternion NeutralRot { get; protected set; }

        private Vector3 targetPos;


        protected override void Awake()
        {
            base.Awake();

            if (camera == null)
                camera = Canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Canvas.worldCamera;
        }

        private void OnEnable()
        {
            if (!CardStateCollection)
                CardStateCollection = Resources.Load<CardStateCollection>(CardStateCollectionPath);

            if (CardStateCollection)
            {
                CardStateCollection.IncrementRef();

                if (CardStateCollection)
                    StateProperty = new PrioritisedProperty<CardState>(CardStateCollection.IdleState);
            }

            StateProperty ??= new PrioritisedProperty<CardState>();
            StateProperty.AddPriority(this, PriorityTags.Smallest);
            if (CardStateCollection)
                StateProperty.Write(this, CardStateCollection.IdleState);

            StateProperty.AddOnValueChangeCallback(TriggerStateChangeEvent, true);
        }

        [HideInCallstack]
        private void TriggerStateChangeEvent(CardState newState)
        {
            try
            {
                var t = new StateTransition(this)
                {
                    From = CurrentState,
                    To = newState,
                };

                OnStateChanged?.Invoke(this, t);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                LastState = CurrentState;
                CurrentState = newState;
            }
        }

        private void OnDisable()
        {
            if (CardStateCollection)
            {
                CardStateCollection.DecrementRef();
                if (CardStateCollection.RefCount <= 0)
                    Resources.UnloadAsset(CardStateCollection);
            }

            CardStateCollection = null;
            StateProperty.Clear();
        }

        private void Start()
        {
            NeutralRot = transform.rotation;
        }

        private void LateUpdate()
        {
            if (IsDragged)
            {
                Vector3 smoothPos = Vector3.Lerp(transform.position, targetPos,
                    DragSmoothness * Time.unscaledDeltaTime);
                Quaternion smoothRot = Quaternion.Slerp(transform.rotation, NeutralRot,
                    RotationOutSmoothness * Time.unscaledDeltaTime);

                transform.SetPositionAndRotation(smoothPos, smoothRot);

                // Debug.Log($"{targetPos}=> {smoothPos} => {transform.position}");
            }
        }

        #region Initialisation

        public void Setup(T card, bool withPlaceholder = false)
        {
            Card = card;

            if (withPlaceholder)
            {
                PlaceholderCard = InstantiatePlaceHolder();
                PlaceholderCard.name = $"{PlaceholderCard.name} (PLACEHOLDER)";
                PlaceholderCard.InitAsPlaceHolder(this);
            }

            ApplyData(card);
            OnSetup?.Invoke(this);
        }


        /// <summary>
        /// Called within the card system himself.
        /// This sets this cards has a placeholder, a mirror of an other cardUI.
        /// </summary>
        /// <param name="cardUI">CardUI to mirror</param>
        protected virtual void InitAsPlaceHolder(CardUI<T> cardUI)
        {
            Setup(cardUI.Card, false);

            CanBeDragged = false;
            gameObject.SetActive(false);
        }

        public void Dispose(T card)
        {
            onDisposed?.Invoke(this);
            OnDispose(card);
            if (PlaceholderCard)
                PlaceholderCard.Dispose(card);
        }

        protected virtual CardUI<T> InstantiatePlaceHolder() =>
            Instantiate(this, transform.position, Quaternion.identity, transform.parent);

        /// <summary>
        /// Custom init implementation
        /// </summary>
        /// <param name="card"></param>
        public abstract void ApplyData(T card);

        /// <summary>
        /// Custom Deinit implementation
        /// </summary>
        /// <param name="card"></param>
        protected abstract void OnDispose(T card);

        #endregion

        public void BindToDraggableController(ICardDraggableController provider)
        {
            CanBeDragged = true;
            DraggableController = provider;
        }

        public void UnbindToDraggableController(ICardDraggableController provider)
        {
            CanBeDragged = false;
            DraggableController = provider;
        }

        #region Dragging

        protected override void OnDragInit(Vector2 screenPos)
        {

        }

        protected override void OnDragBegin()
        {
            if (CardStateCollection)
                StateProperty?.Write(this, CardStateCollection.DraggedState);

            if (PlaceholderCard)
                PlaceholderCard.gameObject.SetActive(true);

        }

        protected override void OnDragEnd(Vector2 position)
        {
            // Debug.Log("Ending drag");
            if (CurrentSlot == null)
            {
                if (CardStateCollection)
                    StateProperty?.Write(this, CardStateCollection.DraggedCanceledState);

                OnDropCanceled?.Invoke(this);
            }
            else
            {
                if (CardStateCollection)
                    StateProperty?.Write(this, CardStateCollection.DraggedSuccessState);

                OnDropSucceed?.Invoke(this,
                    new CardDropContext { DropSlot = CurrentSlot, Position = position });
            }

            if (PlaceholderCard)
                PlaceholderCard.gameObject.SetActive(false);

            if (CardStateCollection)
                StateProperty?.Write(this, CardStateCollection.IdleState);
        }

        protected override void OnDrag(Vector2 screenPos)
        {
            //Movement
            Camera targetCamera = camera != null ? camera : Camera.main;
            targetPos = CardUtilities.GetPosOfCard(screenPos, Canvas, targetCamera);


            //Tilt
            if (EnableTilt)
            {
                Vector3 currentPosition = transform.position;
                Vector3 speed = (targetPos - currentPosition);
                Vector3 tiltAxis = Vector3.Cross(speed.normalized, FrontDirection.normalized);

                float maxTiltSpeed = MaxTiltSpeed * MaxTiltSpeed * Time.unscaledDeltaTime;
                float speedSqrMagnitude = speed.sqrMagnitude * Time.unscaledDeltaTime;

                float interpolation = Mathf.InverseLerp(0, maxTiltSpeed, speedSqrMagnitude);

                float angle = TiltStrength * interpolation;
                Quaternion smoothRot = NeutralRot * Quaternion.AngleAxis(angle, tiltAxis);

                Quaternion target = Quaternion.Slerp(transform.rotation, smoothRot,
                    RotationInSmoothness * Time.unscaledDeltaTime);

                if (Quaternion.Angle(NeutralRot, target) > Quaternion.Angle(NeutralRot, transform.rotation))
                    transform.eulerAngles = target.eulerAngles;
            }

            OnDragged?.Invoke(this, screenPos);
        }

        protected override bool IsDragValid(PointerEventData eventData) => CanBeDragged;


        #endregion

        public virtual void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }


        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (!IsDragged && !IsSelected && CardStateCollection)
                StateProperty.Write(this, CardStateCollection.HoveredState);

            eventData.Use();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (!IsDragged && !IsSelected && CardStateCollection)
                StateProperty.Write(this, CardStateCollection.IdleState);

            eventData.Use();
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            if (!IsDragged && CardStateCollection)
                StateProperty.Write(this, CardStateCollection.SelectedState);

            eventData.Use();
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            if (!IsDragged && CardStateCollection)
                StateProperty.Write(this, CardStateCollection.IdleState);

            eventData.Use();
        }
    }
}