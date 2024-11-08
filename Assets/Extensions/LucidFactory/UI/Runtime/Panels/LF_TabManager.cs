using System;
using System.Collections.Generic;
using LTX.ChanneledProperties;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace LucidFactory.UI.Panels
{
    [AddComponentMenu( "LucidFactory/UI/TabManager" )]
    public class LF_TabManager : MonoBehaviour
    {
        [SerializeField, BoxGroup("Settings"), PropertyRange(0, "@tabs.Length - 1")]
        protected int startTab = 0;
        [SerializeField, BoxGroup("Settings")]
        private LF_Tab[] tabs;


        [BoxGroup("PanelManager"), ShowInInspector, OnValueChanged(nameof(OnTabChange)), PropertyRange(0, "@tabs.Length - 1")]
        private int currentTabIndex;

        protected int CurrentTabIndex
        {
            get
            {
                return currentTabIndex;
            }
            private set
            {
                if (value != CurrentTabIndex)
                {
                    currentTabIndex = value;
                    OnTabChange(value);
                }

            }
        }

        public string CurrentTab
        {
            get => tabs[currentTabIndex].name;
            private set
            {
                for (int i = 0; i < tabs.Length; i++)
                {
                    if (tabs[i].name == value)
                    {
                        CurrentTabIndex = i;
                        return;
                    }
                }

                CurrentTabIndex = -1;
            }
        }

        private void OnTabChange(int index)
        {
            for (int i = 0; i < tabs.Length; i++)
            {
                var panel = tabs[i];

                if (i == index)
                {
                    // Debug.Log($"Opening {panel.name}");
                    panel.Open();
                }
                else
                {
                    // Debug.Log($"Closing {panel.name}");
                    panel.Close();
                }
            }
        }

        public void OpenTab(string tab)
        {
            currentTabIndex = -1;
            CurrentTab = tab;
        }

        public void OpenTab(int tab)
        {
            currentTabIndex = -1;
            CurrentTabIndex = tab;
        }

        public void CloseAllTabs()
        {
            CurrentTabIndex = -1;
        }
        protected virtual void Awake()
        {
            currentTabIndex = startTab;
            OnTabChange(startTab);
        }
    }
}