using System;
using LTX.ChanneledProperties;
using LucidFactory.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LucidFactory.Cards.UI.Interfaces
{
    public interface ICardUI : IDraggable
    {
        event Action<ICardUI, StateTransition> OnStateChanged;

        bool IsDragged { get; }
        bool IsHovered { get; }
        bool IsSelected { get; }
        public CardState LastState { get; }
        public CardState CurrentState { get; }

        EventTrigger EventTrigger { get; }

        PrioritisedProperty<CardState> StateProperty { get; }
        CardStateCollection CardStateCollection { get; }
        new RectTransform RectTransform { get; }

        // ReSharper disable once InconsistentNaming
        new Transform transform { get; }
        // ReSharper disable once InconsistentNaming
        GameObject gameObject { get; }
        ICard Card { get; }

        void BindToDraggableController(ICardDraggableController provider);
        void UnbindToDraggableController(ICardDraggableController provider);
    }

    public interface ICardUI<in T> : ICardUI where T : ICard
    {
        void Setup(T card, bool withPlaceholder = false);
        void Dispose(T card);
    }
}