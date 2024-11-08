namespace LucidFactory.UI
{
    public interface IDropSlot
    {
        int Priority { get; }
        bool IsDraggableOver(IDraggable draggable);
    }

    public interface IDropSlotWithCallbacks : IDropSlot
    {
        void OnDragBegin(IDraggable draggable);
        void OnDragEnd(IDraggable draggable);
        
        void OnDraggableEnter(IDraggable draggable);
        void OnDraggableExit(IDraggable draggable);
        void OnDraggableDrop(IDraggable draggable);
    }
}