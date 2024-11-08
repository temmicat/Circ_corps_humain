using LucidFactory.Cards.UI.Interfaces;

namespace LucidFactory.Cards.UI
{
    public struct StateTransition
    {
        public readonly ICardUI cardUI;
        public CardState From { get; internal set; }
        public CardState To { get; internal set; }


        public StateTransition(ICardUI cardUI)
        {
            this.cardUI = cardUI;
            From = null;
            To = null;
        }

        public bool IsDraggedState() => cardUI.CardStateCollection && To == cardUI.CardStateCollection.DraggedState;
        public bool IsDropSuccessState() => cardUI.CardStateCollection && To == cardUI.CardStateCollection.DraggedSuccessState;
        public bool IsDropFailedState() => cardUI.CardStateCollection && To == cardUI.CardStateCollection.DraggedCanceledState;

        public bool IsIdleState() => cardUI.CardStateCollection && To == cardUI.CardStateCollection.DraggedSuccessState;

        public bool IsHoveredState() => cardUI.CardStateCollection && To == cardUI.CardStateCollection.HoveredState;
        public bool IsUnhoveredState() => cardUI.CardStateCollection && From == cardUI.CardStateCollection.HoveredState;

        public bool IsSelectedState() => cardUI.CardStateCollection && To == cardUI.CardStateCollection.SelectedState;
        public bool IsDeselectedState() => cardUI.CardStateCollection && From == cardUI.CardStateCollection.SelectedState;
    }
}