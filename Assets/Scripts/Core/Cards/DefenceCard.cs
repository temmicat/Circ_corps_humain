using UnityEngine;
using LucidFactory.Cards;

namespace CorpsHumain.Core
{
    public class DefenceCard : ICard
    {
        public DefenceCardData Data { get; }

        public DefenceCard(DefenceCardData data)
        {
            Data = data;
        }

    }
}
