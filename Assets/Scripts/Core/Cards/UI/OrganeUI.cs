using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CorpsHumain.Core
{
    //TODO deposer carte dessus et transvaser la carte de la main du jouer a la main des organes


    public class OrganeUI : MonoBehaviour
    {
        // This script handles the conversion between datas in OrganeData and the UI

        public List<OrganeData> organesList;
        public OrganeData organeDataScriptable { get; private set; }
        public GameData gameDataScriptable;

        [SerializeField]
        private Image icon;

        public void Start()
        {
            SelectOrgan();
            ApplyData();
            gameDataScriptable.answersNumber = organeDataScriptable.numberOfAnswers;
        }

        public void SelectOrgan()
        {
            // choose the right organ scriptable based on GameData.levelActive
            for (int i = 0; i < organesList.Count; i++)
            {
                if (gameDataScriptable.levelActive == organesList[i].thisOrgane)
                {
                    organeDataScriptable = organesList[i];
                }
            }
        }

        public void ApplyData()
        {
            icon.sprite = organeDataScriptable.Icon;
        }
    }
}
