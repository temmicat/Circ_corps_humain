
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LucidFactory.UI
{
    public class LF_FoldoutButtonImage : MonoBehaviour
    {
        [FormerlySerializedAs("foldout")] [SerializeField] private LF_Foldout lfFoldout;

        [SerializeField] private Image image;
        [SerializeField] private Sprite open;
        [SerializeField] private Sprite close;
        private void OnEnable()
        {
            if (lfFoldout == null)
                enabled = false;

            lfFoldout.OnOpened += OnOpened;
            lfFoldout.OnClosed += OnClosed;
        }


        private void OnDisable()
        {
            if (lfFoldout == null)
                return;
            
            lfFoldout.OnOpened -= OnOpened;
            lfFoldout.OnClosed -= OnClosed;
        }
        
        private void OnOpened(LF_Foldout obj)
        {
            image.sprite = close;
        }

        private void OnClosed(LF_Foldout obj)
        {
            image.sprite = open;
        }
    }
}
