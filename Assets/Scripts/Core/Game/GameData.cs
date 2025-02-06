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
        public List<CardType> playerAnswers;

        public int answersNumber;

        // Game answers
        public Dictionary<levels, List<GameObject>> gameAnswers = new Dictionary<levels, List<GameObject>>()
        {
            {levels.Rein, new List<GameObject>()},
            {levels.Ovaires, new List<GameObject>() },
            {levels.Sein, new List<GameObject>() },
            {levels.Pancreas, new List<GameObject>() },
            {levels.Peau, new List<GameObject>() },
            {levels.ColDeLUterus, new List<GameObject>() },
            {levels.ColonRectum, new List<GameObject>() },
            {levels.Endometre, new List<GameObject>() },
            {levels.Estomac, new List<GameObject>() },
            {levels.Foie, new List<GameObject>() },
            {levels.Oesophage, new List<GameObject>() },
            {levels.Poumon, new List<GameObject>() },
            {levels.Vessie, new List<GameObject>() },
        };
    }
}
