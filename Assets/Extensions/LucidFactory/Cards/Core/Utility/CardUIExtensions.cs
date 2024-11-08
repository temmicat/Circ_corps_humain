using LucidFactory.Cards.UI.Interfaces;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace LucidFactory.Cards.Utility
{
    public static class CardUIExtensions
    {
        public static EventTrigger.Entry AddEventEntry<T>(this T cardUI, EventTriggerType eventTriggerType,
            UnityAction<BaseEventData> callback) where T : ICardUI
        {
            EventTrigger.Entry entry = new EventTrigger.Entry()
            {
                eventID = eventTriggerType,
            };
            entry.callback.AddListener(callback);

            return AddEventEntry(cardUI, entry);
        }
        public static EventTrigger.Entry AddEventEntry<T>(this T cardUI, EventTrigger.Entry entry) where T : ICardUI
        {
            EventTrigger eventTrigger = cardUI.EventTrigger;
            eventTrigger.triggers.Add(entry);
            return entry;
        }
        public static bool RemoveEventEntry(this ICardUI cardUI, EventTrigger.Entry entry)
        {
           return cardUI.EventTrigger.triggers.Remove(entry);
        }

        public static T GetCard<T>(this ICardUI<T> cardUI) where T : class, ICard => cardUI.Card as T;
    }
}