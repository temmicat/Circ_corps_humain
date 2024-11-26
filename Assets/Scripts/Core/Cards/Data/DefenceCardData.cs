using UnityEngine;

namespace CorpsHumain.Core
{
    [CreateAssetMenu(fileName = "CardData", menuName = "CorpsHumain/CardData")]
    public class DefenceCardData : ScriptableObject
    {

        // This is the base Scriptable Object containing each var for each cards
        // Cards are created based on this Scriptable Object, on Ressource -> Cards

        // DefenceCardUI takes these informations and convert them to be seen on UI

        [SerializeField] 
        private string title;
        [SerializeField]
        private string description;
        [SerializeField]
        private Sprite icon;
        
        public enum cards
        {
            Depistage, Soleil, GazRadon,
            AlimentationSaine, Vaccination,
            Consignes, Fumer, TabagismePassif,
            Poids, ActifPhysiquement, Allaiter,
            Alcool
        }

        public cards thisCard;


        public string Title => title;
        public string Description => description;
        public Sprite Icon => icon;

    }
}
