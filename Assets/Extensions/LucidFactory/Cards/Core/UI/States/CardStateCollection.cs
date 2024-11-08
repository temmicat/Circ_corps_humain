using UnityEngine;

namespace LucidFactory.Cards.UI
{
    [CreateAssetMenu(menuName = "LucidFactory/Cards/CardStateCollection")]
    public class CardStateCollection : ScriptableObject
    {
        [field: SerializeField]
        public CardState IdleState { get; private set; }

        [field: SerializeField]
        public CardState DraggedState { get; private set; }
        [field: SerializeField]
        public CardState DraggedCanceledState { get; private set; }
        [field: SerializeField]
        public CardState DraggedSuccessState { get; private set; }

        [field: SerializeField]
        public CardState SelectedState { get; private set; }
        [field: SerializeField]
        public CardState HoveredState { get; private set; }


        internal int RefCount { get; private set; } = 0;

        internal void IncrementRef()
        {
            RefCount++;
        }

        internal void DecrementRef()
        {
            RefCount--;
        }
    }
}