using UnityEngine;

namespace CorpsHumain.Core
{
    public class DefenceCard
    {
        public DefenceCardData Data { get; }

        public bool isPlayerCard;
        public DefenceCard(DefenceCardData data)
        {
            Data = data;
        }

    }
}
