using System.Collections.Generic;

namespace LucidFactory.Cards.UI.Interfaces
{
    public interface ICardDraggableController
    {
        IEnumerable<ICardDropSlot> GetSlots(ICardUI cardUI);
    }
}