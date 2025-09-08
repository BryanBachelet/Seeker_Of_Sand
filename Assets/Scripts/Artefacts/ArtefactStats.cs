using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.Artefact
{


    [System.Serializable]
    [CreateAssetMenu(fileName = " Artefact Stats Data", menuName = "Artefacts/Artefact Stas", order = 2)]
    public class ArtefactStats : ArtefactBaseInfos
    {
        [Header("Stats Conditions")]
        [SerializeField] private bool isPermanent;
        [ShowIf("isPermanent", false, true)] public bool isTemporary;
        [ShowIf("isTemporary", true, true)] public float buffDuration = 0;
        private float buffTimer;

        public ArtefactStats()
        {
            type = ArtefactType.Stats;
        }

        public new ArtefactStats Clone()
        {
            return Instantiate(this);
        }

        public bool IsPermanent() { return isPermanent; }
 
    }
}