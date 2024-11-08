using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using LucidFactory.Cards.UI.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace LucidFactory.Cards.UI.Hand
{
    internal readonly struct CardStateContainerCollection
    {
        private readonly Dictionary<ICardUI, CardContainer> containers;
        private readonly RectTransform containerRoot;
        private readonly RectTransform cardRoot;

        public CardStateContainerCollection(RectTransform containerRoot, RectTransform cardRoot)
        {
            this.containerRoot = containerRoot;
            this.cardRoot = cardRoot;
            containers = new();
        }


        public bool TryGetContainer(ICardUI cardUI, out CardContainer container) => containers.TryGetValue(cardUI, out container);
        public Vector3 Position(ICardUI cardUI) => containers.TryGetValue(cardUI, out CardContainer container) ? container.Position :  containerRoot.position;

        public Vector2 AnchoredPosition(ICardUI cardUI) => containers.TryGetValue(cardUI, out CardContainer container) ? container.AnchoredPosition :  containerRoot.anchoredPosition;
        public Vector2 AnchoredPosition3D(ICardUI cardUI) => containers.TryGetValue(cardUI, out CardContainer container) ? container.AnchoredPosition :  containerRoot.anchoredPosition3D;
        public Quaternion Rotation(ICardUI cardUI) => containers.TryGetValue(cardUI, out var container) ? container.Rotation :  containerRoot.rotation;

        public void Add(ICardUI cardUI, CardContainer prefab, IEnumerable<CardStateSettings> settings)
        {
            if(containers.ContainsKey(cardUI))
                return;

            CardContainer container = Object.Instantiate(prefab, containerRoot);
            container.name = $"{cardUI.gameObject.name} [Container]";

            container.ConnectToCard(cardUI);
            IEnumerable<CardStateSettings> cardStateSettingsEnumerable = settings as CardStateSettings[] ?? settings.ToArray();
            if(cardStateSettingsEnumerable.Any())
                container.Deactivate(cardStateSettingsEnumerable.First());

            containers.Add(cardUI, container);
            LayoutRebuilder.MarkLayoutForRebuild(containerRoot);
        }
        public void Remove(ICardUI cardUI)
        {
            if (containers.Remove(cardUI, out var container))
                container.RemoveReference();
        }

        public bool IsInContainer(ICardUI cardUI) => containers.ContainsKey(cardUI);


        public void MoveInContainer(ICardUI cardUI, CardStateSettings settings)
        {
            if (containers.TryGetValue(cardUI, out var container))
            {
                cardUI.transform.SetParent(cardRoot);

                container.Activate(settings);
                container.ApplyLayoutToCard(settings);

            }
        }

        public void MoveOutContainer(ICardUI cardUI, CardStateSettings settings)
        {
            if (containers.TryGetValue(cardUI, out var container))
            {
                container.Deactivate(settings);
            }
        }

        internal void UpdateSiblingIndexes(Dictionary<ICardUI, int> cardsIndexes)
        {
            foreach ((ICardUI cardUI, CardContainer cardContainer) in containers)
            {
                if (cardsIndexes.TryGetValue(cardUI, out int index))
                {
                    cardUI.RectTransform.DOComplete();

                    if(cardContainer.gameObject.activeInHierarchy)
                        cardContainer.transform.SetSiblingIndex(index);
                }
            }
        }
    }
}