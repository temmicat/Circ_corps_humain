using System.Collections.Generic;
using UnityEngine;

namespace CorpsHumain.Core
{
    [CreateAssetMenu(fileName = "OrganeData", menuName = "CorpsHumain/OrganeData")]
    public class OrganeData : ScriptableObject
    {

        // This is the base Scriptable Object Containing each var for each organ
        public GameData.levels thisOrgane;
        public List<CardType> thisOrganAnswers;

        public int numberOfAnswers;

        [SerializeField]
        private Sprite icon;

        public Sprite Icon => icon;


    }
}
