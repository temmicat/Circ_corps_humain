using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LucidFactory.UI
{

    public interface IDraggable :
        IDragHandler,
        IBeginDragHandler,
        IEndDragHandler,
        IInitializePotentialDragHandler,
        ICanvasElement
    {

        Canvas Canvas { get; }
        Component Fallback { get; set; }
        RectTransform RectTransform { get; }
        bool CanBeDragged { get; }
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T">DropSlot</typeparam>
    /// <typeparam name="TU">DragNDrop </typeparam>
    public interface IDraggable<out T> : IDraggable
        where T : IDropSlot
    {
        IEnumerable<T> Slots { get; }
        public T CurrentSlot { get; }

    }
}