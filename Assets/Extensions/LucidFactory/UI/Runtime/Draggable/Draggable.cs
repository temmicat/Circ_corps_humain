using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LucidFactory.UI
{
    public abstract class Draggable : MonoBehaviour, IDraggable, IScrollHandler
    {
        public Component Fallback { get; set; }
        public bool CanBeDragged { get; protected set; }
        public bool IsDragged { get; private set; }

        public Canvas Canvas { get; private set; }
        public RectTransform RectTransform { get; private set; }


        private bool isDestroy = false;

        protected virtual void Awake()
        {
            isDestroy = false;

            RectTransform = GetComponent<RectTransform>();
            OnCanvasHierarchyChanged();
        }
        protected virtual void OnDestroy()
        {
            isDestroy = true;
        }

        private void OnCanvasHierarchyChanged()
        {
            Canvas = transform.GetComponentInParent<Canvas>();
            if(Canvas == null)
                return;

            while (!Canvas.isRootCanvas)
                Canvas = Canvas.rootCanvas;
        }

        void IInitializePotentialDragHandler.OnInitializePotentialDrag(PointerEventData eventData)
        {
            // Debug.Log("ici?");
            TryInitDrag(eventData);
        }
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            TryDrag(eventData);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            // Debug.Log("Beginning drag");
            if (TryBeginDrag(eventData))
                IsDragged = true;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            // Debug.Log("Ending drag");
            if(TryEndDrag(eventData))
                IsDragged = false;
        }

        protected virtual bool TryInitDrag(PointerEventData eventData)
        {
            if (Fallback is IInitializePotentialDragHandler potential)
            {
                 potential.OnInitializePotentialDrag(eventData);
            }

            OnDragInit(eventData.position);
            return true;
        }

        protected virtual bool TryBeginDrag(PointerEventData eventData)
        {
            if (!CanBeDragged)
                return false;


            if (!IsDragValid(eventData))
            {
                if (Fallback == null)
                {
                    eventData.pointerDrag = null;
                }
                else if (Fallback is IBeginDragHandler dragHandler)
                {
                    dragHandler.OnBeginDrag(eventData);
                    eventData.pointerDrag = Fallback.gameObject;
                }

                return false;
            }

            OnDragBegin();
            eventData.Use();
            return true;
        }

        protected virtual bool TryDrag(PointerEventData eventData)
        {
            if (!CanBeDragged)
                return false;

            if (!IsDragValid(eventData))
            {
                if (Fallback == null)
                {
                    eventData.pointerDrag = null;
                }
                else if (Fallback is IDragHandler dragHandler)
                {
                    dragHandler.OnDrag(eventData);
                    eventData.pointerDrag = Fallback.gameObject;
                }

                return false;
            }

            OnDrag(eventData.position);
            eventData.Use();
            return true;
        }

        protected virtual bool TryEndDrag(PointerEventData eventData)
        {
            if (!CanBeDragged)
                return false;

            if (!IsDragValid(eventData))
            {
                if (Fallback == null)
                {
                    eventData.pointerDrag = null;
                }
                else if (Fallback is IEndDragHandler dragHandler)
                {
                    dragHandler.OnEndDrag(eventData);
                    eventData.pointerDrag = Fallback.gameObject;
                }

                return false;
            }
            //Debug.Log("end drag");

            OnDragEnd(eventData.position);
            eventData.Use();
            return true;
        }

        protected abstract void OnDragInit(Vector2 screenPos);
        protected abstract void OnDragBegin();
        protected abstract void OnDragEnd(Vector2 position);
        protected abstract void OnDrag(Vector2 screenPos);
        protected abstract bool IsDragValid(PointerEventData eventData);

        void ICanvasElement.Rebuild(CanvasUpdate executing)
        {

        }

        void ICanvasElement.LayoutComplete()
        {

        }

        void ICanvasElement.GraphicUpdateComplete()
        {

        }

        bool ICanvasElement.IsDestroyed()
        {
            return isDestroy;
        }

        void IScrollHandler.OnScroll(PointerEventData eventData)
        {
            if (Fallback is IScrollHandler scrollHandler)
            {
                eventData.pointerDrag = Fallback.gameObject;
                scrollHandler.OnScroll(eventData);
                eventData.Use();
            }
        }
    }

    public abstract class Draggable<T> : Draggable, IDraggable<T> where T : IDropSlot
    {
        private readonly static List<T> Buffer = new();

        public T CurrentSlot { get; private set; }
        protected abstract IEnumerable<T> Slots { get; }

        T IDraggable<T>.CurrentSlot => CurrentSlot;
        IEnumerable<T> IDraggable<T>.Slots => Slots;


        protected override bool TryBeginDrag(PointerEventData eventData)
        {
            if (!base.TryBeginDrag(eventData))
                return false;

            foreach (var slot in Slots)
            {
                if(slot is IDropSlotWithCallbacks callbacks)
                    callbacks.OnDragBegin(this);
            }

            return base.TryBeginDrag(eventData);
        }

        protected override bool TryEndDrag(PointerEventData eventData)
        {
            if (!base.TryEndDrag(eventData))
                return false;

            if(CurrentSlot is IDropSlotWithCallbacks dropSlotWithCallbacks)
                dropSlotWithCallbacks.OnDraggableDrop(this);

            foreach (var slot in Slots)
            {
                if(slot is IDropSlotWithCallbacks callbacks)
                    callbacks.OnDragEnd(this);
            }
            return true;
        }


        protected override bool TryDrag(PointerEventData eventData)
        {
            if (!base.TryDrag(eventData))
                return false;

            //Looking for other slots
            var currentSlotWithCallbacks = CurrentSlot as IDropSlotWithCallbacks;
            Buffer.Clear();

            foreach (T slot in Slots)
            {
                if (slot == null)
                {
                    Debug.Log($"No slot found for { gameObject.name }");
                    continue;
                }


                if (slot.Priority > 0 && slot.IsDraggableOver(this))
                {
                    // Debug.Log("Over slot");
                    Buffer.Add(slot);
                }
            }

            bool containsCurrentSlot = CurrentSlot != null && Buffer.Contains(CurrentSlot);
            T selectedSlot = containsCurrentSlot ? CurrentSlot : default;

            foreach (var slot in Buffer)
            {
                if (selectedSlot == null || selectedSlot.Priority < slot.Priority)
                    selectedSlot = slot;
            }

            bool isOverSlot = selectedSlot != null;
            if (isOverSlot)
            {
                var dropSlotWithCallbacks = selectedSlot as IDropSlotWithCallbacks;
                if (!selectedSlot.Equals(CurrentSlot))
                {
                    dropSlotWithCallbacks?.OnDraggableEnter(this);
                    currentSlotWithCallbacks?.OnDraggableExit(this);
                }

                CurrentSlot = selectedSlot;
            }

            if (!isOverSlot && CurrentSlot != null)
            {
                currentSlotWithCallbacks?.OnDraggableExit(this);
                CurrentSlot = default;
            }

            return true;
        }

    }
}