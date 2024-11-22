using LucidFactory.Cards.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CorpsHumain.Core
{
    public class DefenceCardUI : CardUI<DefenceCard>
    {

        // This script handles the conversion between datas in DefenceCardData and the UI

        [SerializeField]
        private TextMeshProUGUI title;
        [SerializeField]
        private TextMeshProUGUI description;

        [SerializeField]
        private Image icon;

        public override void ApplyData(DefenceCard card)
        {
            title.text = card.Data.Title;
            description.text = card.Data.Description;

            icon.sprite = card.Data.Icon;
        }

        protected override void OnDispose(DefenceCard card)
        {

        }
    }
}
