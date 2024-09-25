using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemies
{
    public class TS_RingSkill : MonoBehaviour
    {
        [Header("Ring Object Parameters")]
        public GameObject ringGO;

        private Vector3 centerRing;

        [Header("Ring Stats Parameters")]
        public float radius = 200;
        

        // Update is called once per frame
        void Update()
        {

        }

        public void ActiveRingSkill()
        {
            ringGO.transform.position = new Vector3(centerRing.x, centerRing.y, centerRing.z);
        }

        private void SetPositionRing()
        {
            centerRing = transform.position;
        }

        public void UpdateRingSkill()
        {

        }
    }
}