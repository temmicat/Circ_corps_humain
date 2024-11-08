using System.Collections.Generic;
using LucidFactory.UI;

namespace LucidFactory.Cards.UI.Interfaces
{

    public interface IHandCardCollectionUI<T>  where T : ICard
    {
        public IEnumerable<ICardDropSlot> GetSlotsForCard(ICardUI<T> cardUI);
    }
}