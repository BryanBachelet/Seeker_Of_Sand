using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using GuerhoubaTools.Gameplay;


namespace Enemies
{
    public struct NPCMoveAttData
    {
        public GameObject npcGo;
        public Transform targetTransform;
        public float maxHeight;

    }


    [CreateAssetMenu(fileName = "DashAttack", menuName = "Enemmis/Move/Dash", order = 1)]
    public class MA_Dash : NPCMoveAttackObject
    {
        public float distance = 0;
        public float duration = 0;

        private float speed;

        private Vector3 dashDirection;
        private Vector3 destination;

        private float timerMovement;
        public Vector3 originPoint1;
        public Vector3 originPoint;

        public override void StartMvt(NPCMoveAttData moveData)
        {
            timerMovement = 0.0f;
            data = moveData;
            dashDirection = moveData.targetTransform.position - ( moveData.npcGo.transform.position  - new Vector3(0, moveData.targetTransform.localScale.y* moveData.maxHeight/2, 0));
            speed = distance / duration;

            RaycastHit hit = new RaycastHit();

            bool hasHit = Physics.Raycast(
                moveData.npcGo.transform.position,
                dashDirection.normalized,
                out hit,
                distance,
                GameLayer.instance.propsGroundLayerMask);

            if (hasHit)
            {
                if (Tools.IsBelongToLayer(LayerMask.NameToLayer("Ground"), hit.collider.gameObject))
                {
                    destination = hit.point;
                    originPoint1 = hit.point;
                }
                if (Tools.IsBelongToLayer(LayerMask.NameToLayer("Deco"), hit.collider.gameObject))
                {
                    NavMeshHit navHit = new NavMeshHit();
                    bool hasFoundNavMesh = false;
                    int indexMeter = 0;

                   
                    while (!hasFoundNavMesh && indexMeter< 300)
                    {
                        Vector3 originPoint = hit.point - (dashDirection * indexMeter * .5f);
                        hasFoundNavMesh = NavMesh.SamplePosition(originPoint, out navHit, data.npcGo.transform.localScale.y, NavMesh.AllAreas);
                        indexMeter++;
                    }
                    if (indexMeter >= 300)
                        Debug.LogError("Infinite Loop ");
                    destination = navHit.position;
                }
            }
            else
            {
                NavMeshHit navHit = new NavMeshHit();
                bool hasFoundNavMesh = false;
                int indexMeter = 0;
                originPoint1 = data.npcGo.transform.position + dashDirection.normalized * distance;
                while (!hasFoundNavMesh && indexMeter < 300)
                {
                     originPoint = data.npcGo.transform.position + dashDirection.normalized * distance + (-dashDirection.normalized * indexMeter * .5f);
                    hasFoundNavMesh = NavMesh.SamplePosition(originPoint, out navHit, data.npcGo.transform.localScale.y , NavMesh.AllAreas);
                    indexMeter++;
                }
                if (indexMeter >= 300)
                    Debug.LogError("Infinite Loop ");
                destination = navHit.position;
            }

            dashDirection = destination - moveData.npcGo.transform.position; 
        }

        public override void UpdateMvt()
        {
            if (timerMovement > duration)
            {
                if (EndMovement != null) EndMovement.Invoke();
            }
            else
            {
                data.npcGo.transform.position += dashDirection.normalized * speed * Time.deltaTime; 
                timerMovement += Time.deltaTime;
            }
        }

        

    }
}
