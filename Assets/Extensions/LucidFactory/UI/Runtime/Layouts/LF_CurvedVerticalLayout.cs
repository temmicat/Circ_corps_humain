using UnityEngine;

namespace LucidFactory.UI
{
    /// <summary>
    /// TODO:
    /// - add automatic child sizing, like in the HorizontalOrVerticalLayoutGroup.cs
    /// - nicer anchor handling for initial child positions
    /// </summary>
    [AddComponentMenu( "LucidFactory/Layout/Curved Vertical Layout" )]
    public class LF_CurvedVerticalLayout : LF_CurvedLayout
    {
        protected override RectTransform.Axis Axis => RectTransform.Axis.Vertical;
    }
}
