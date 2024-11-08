using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LucidFactory.UI
{
    public abstract class LF_CurvedLayout : HorizontalOrVerticalLayoutGroup
    {
        protected abstract RectTransform.Axis Axis { get; }

        [Space]
        [SerializeField]
        private float curveStrength = 1;
        [SerializeField, Range(0, 15)]
        private float maxInfluenceAt = 5;
        [SerializeField]
        private float maxTilt;
        // [SerializeField]
        // private float curveCenter = 0.5f;

        /// <summary>
        /// Called by the layout system. Also see ILayoutElement
        /// </summary>
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalcAlongAxis(0, Axis == RectTransform.Axis.Vertical);
        }

        /// <summary>
        /// Called by the layout system. Also see ILayoutElement
        /// </summary>
        public override void CalculateLayoutInputVertical()
        {
            CalcAlongAxis(1, Axis == RectTransform.Axis.Vertical);
        }

        /// <summary>
        /// Called by the layout system. Also see ILayoutElement
        /// </summary>
        public override void SetLayoutHorizontal()
        {
            SetChildrenAlongAxis(0, Axis == RectTransform.Axis.Vertical);

            if (Axis == RectTransform.Axis.Vertical)
                OrientateElements(false);
        }
        /// <summary>
        /// Called by the layout system. Also see ILayoutElement
        /// </summary>
        public override void SetLayoutVertical()
        {
            SetChildrenAlongAxis(1, Axis == RectTransform.Axis.Vertical);

            if (Axis == RectTransform.Axis.Horizontal)
                OrientateElements(true);
        }

        private void OrientateElements(bool verticalOffset)
        {
            var rectChildrenCount = rectChildren.Count;
            if (rectChildrenCount == 1)
            {
                rectChildren[0].localRotation = Quaternion.Euler(0, 0, 0);
                return;
            }

            int startIndex = m_ReverseArrangement ? rectChildrenCount - 1 : 0;
            int endIndex = m_ReverseArrangement ? 0 : rectChildrenCount;
            int increment = m_ReverseArrangement ? -1 : 1;

            Vector2 baseOffset = (verticalOffset ? Vector2.up  : Vector2.right) * curveStrength;

            float influence = Mathf.Clamp01(Mathf.InverseLerp(0, maxInfluenceAt, rectChildrenCount));

            for (int i = startIndex; m_ReverseArrangement ? i >= endIndex : i < endIndex; i += increment)
            {
                RectTransform child = rectChildren[i];
                float curveNormPos = Mathf.InverseLerp(0, 1, (float)i / (rectChildrenCount - 1));

                var radAngle = curveNormPos * 180 * Mathf.Deg2Rad;
                float sin = Mathf.Sin(radAngle);

                child.anchoredPosition += baseOffset * sin * influence;
                var angle = rectChildrenCount == 1 ? 0 : Mathf.LerpAngle(-maxTilt, maxTilt, curveNormPos);

                Quaternion rot = Quaternion.Euler(0, 0, angle * influence);
                child.localRotation = rot;
            }
        }
    }
}