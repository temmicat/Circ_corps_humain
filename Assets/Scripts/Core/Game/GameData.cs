using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace CorpsHumain.Core
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
    public class GameData : ScriptableObject
    {
        public enum levels
        {
            Peau, Oesophage, Poumon, Sein, 
            Estomac, Foie, Rein, 
            Pancreas, ColonRectum, Vessie, 
            Endometre, ColDeLUterus, Ovaires
        }

        public levels levelActive;

        // list levelsCleared
        public List<levels> levelsCleared;

        // list organ : answers
        public List<levels> playerAnswers;

    }
}
