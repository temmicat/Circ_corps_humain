using System;
using LucidFactory.Cards.UI.Hand;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LucidFactory.Cards.UI
{
    [Serializable]
    public struct CardStateSettings
    {
        [Serializable]
        public struct LayoutSettings
        {
            private const float WIDTH = 75f;

            [field: SerializeField]
            public bool IgnoreLayout { get; private set; }

            [field: HideIf(nameof(IgnoreLayout))]
            [field: SerializeField, HorizontalGroup("Min", WIDTH), LabelText("Min")]
            public bool HasMin { get; private set; }

            [field: HideIf(nameof(IgnoreLayout))]
            [field: SerializeField, HorizontalGroup("Min"), HideLabel, ShowIf(nameof(HasMin))]
            public Vector2Int Min { get; private set; }


            [field: HideIf(nameof(IgnoreLayout))]
            [field: SerializeField, HorizontalGroup("Preferred", WIDTH), LabelText("Preferred")]
            public bool HasPreferred { get; private set; }

            [field: HideIf(nameof(IgnoreLayout))]
            [field: SerializeField, HorizontalGroup("Preferred"), HideLabel, ShowIf(nameof(HasPreferred))]
            public Vector2Int Preferred { get; private set; }


            [field: HideIf(nameof(IgnoreLayout))]
            [field: SerializeField, HorizontalGroup("Flexible", WIDTH), LabelText("Flexible")]
            public bool HasFlexible { get; private set; }

            [field: HideIf(nameof(IgnoreLayout))]
            [field: SerializeField, HorizontalGroup("Flexible"), HideLabel, ShowIf(nameof(HasFlexible))]
            public Vector2Int Flexible { get; private set; }
        }

        [field: SerializeField]
        public float TransitionDuration { get; private set; }
        [field: SerializeField]
        public float CardScale { get; private set; }
        [field: SerializeField]
        public LayoutSettings InactiveLayout { get; private set; }
        [field: SerializeField]
        public LayoutSettings ActiveLayout { get; private set; }

        [field: Space]
        [field: SerializeField]
        public bool PreserveLastState { get; private set; }
        [field: SerializeField]
        public RectTransform CardContainerParent { get; private set; }

        [field: SerializeField]
        public RectTransform CardParent { get; private set; }

        [field: SerializeField]
        public bool ConstraintCard { get; private set; }

        [field: Space]
        [field: ShowIf(nameof(ConstraintCard))]
        [field: SerializeField]
        public bool ClearContainerOnAwake { get; private set; }
        [field: ShowIf(nameof(ConstraintCard))]

        [field: ShowIf(nameof(ConstraintCard))]
        [field: SerializeField, Tooltip(" 0 : No strength (free movement) \n > 0 : Strength (constrained movement)"), Range(0, 20)]
        public float ConstraintStrength { get; private set; }

    }
}