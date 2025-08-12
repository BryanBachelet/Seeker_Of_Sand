using Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAggroActivation : MonoBehaviour
{
    [SerializeField] private int rangeAggro = 100;
    [HideInInspector] private GameLayer m_gameLayer;
    public Collider[] colliderInRange;
    // Start is called before the first frame update
    void Start()
    {
        m_gameLayer = GameLayer.instance;
    }

    // Update is called once per frame
    void Update()
    {
        colliderInRange = Physics.OverlapSphere(transform.position, rangeAggro, m_gameLayer.enemisLayerMask);
        foreach (Collider col in colliderInRange)
        {
            if(col.gameObject.GetComponent<NpcMetaInfos>())
            {
                NpcMetaInfos tempNPCMetaInfos = col.gameObject.GetComponent<NpcMetaInfos>();
                if(tempNPCMetaInfos.state == NpcState.IDLE)
                {
                    tempNPCMetaInfos.state = NpcState.MOVE;
                }
            }
        }
    }
}
