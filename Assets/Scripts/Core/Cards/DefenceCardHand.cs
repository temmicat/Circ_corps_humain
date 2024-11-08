using LucidFactory.Cards;
using UnityEngine;

namespace CorpsHumain.Core
{
    public class DefenceCardHand : Hand<DefenceCard>
    {
        public DefenceCardHand(int maxSize, params DefenceCard[] cardsArray) : base(maxSize, cardsArray)
        {
            
        }
    }
}
