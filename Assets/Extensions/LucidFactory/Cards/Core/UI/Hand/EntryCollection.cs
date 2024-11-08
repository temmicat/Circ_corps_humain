using System.Collections.Generic;
using LucidFactory.Cards.UI.Interfaces;
using LucidFactory.Cards.Utility;
using Sirenix.Serialization;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace LucidFactory.Cards.UI.Hand
{
    internal class EntryCollection
    {
        private readonly ICardUI cardUI;
        private readonly List<EventTrigger.Entry> entries;

        public EntryCollection(ICardUI cardUI)
        {
            this.cardUI = cardUI;
            this.entries = new();
        }

        public void AddEntries(params EventTrigger.Entry[] entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd.Length; i++)
            {
                EventTrigger.Entry entry = entriesToAdd[i];
                entries.Add(entry);
                cardUI.AddEventEntry(entry);
            }
        }
        public void RemoveEntries(params EventTrigger.Entry[] entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd.Length; i++)
            {
                EventTrigger.Entry entry = entriesToAdd[i];

                entries.Remove(entry);
                cardUI.RemoveEventEntry(entry);
            }
        }
        public void Dispose()
        {
            foreach (var entry in entries)
                cardUI.RemoveEventEntry(entry);

            entries.Clear();
        }
    }
}