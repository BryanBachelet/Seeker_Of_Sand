using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using GuerhoubaGames.Enemies;

public class Sound_Masse_Behavior : MonoBehaviour
{
    public int typeEnemyObserved = 0;
    public string variableFMOD;
    public int[] seuil = new int[5];
    public int currentSeuil = 0;
    public EventReference masseSoundAssociated;
    public EventInstance masseSoundInstance;

    public Transform player;
    private Vector3 playerPos;

    public LayerMask enemiLayer;
    public int rangeDetection;
    public Collider[] nearEnemi;
    public int nearEnemiCount = 0;

    public float cooldownDetection = 1;
    private float lastTimeDetection;

    // Start is called before the first frame update
    void Start()
    {
        if(player == null) { player = GameObject.Find("Player").transform; }

        masseSoundInstance = RuntimeManager.CreateInstance(masseSoundAssociated);
        masseSoundInstance.start();
        masseSoundInstance.setVolume(1);
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > lastTimeDetection + cooldownDetection)
        {
            CountNearEnnemi();
        }
        //transform.position = player.transform.position;
    }

    public void CountNearEnnemi()
    {
        int tempEnemiCount = 0;
        Vector3 positionMoyenne = Vector3.zero;
        playerPos = player.transform.position;
        nearEnemi = Physics.OverlapSphere(player.position,rangeDetection,enemiLayer);
        //nearEnemiCount = nearEnemi.Length;
        lastTimeDetection = Time.time;
        foreach (Collider item in nearEnemi)
        {
            if (item.GetComponent<NpcMetaInfos>())
            {
                NpcMetaInfos metaObserved = item.GetComponent<NpcMetaInfos>();
                if((int)metaObserved.type == typeEnemyObserved || typeEnemyObserved == -1)
                {
                    //Debug.Log("EnemiAdd");
                    tempEnemiCount++;
                    positionMoyenne += metaObserved.transform.position;
                }
                else
                {
                    //Debug.Log("EnemiNotAdd");
                }
            }
             
        }
        if(tempEnemiCount != nearEnemiCount) { nearEnemiCount = tempEnemiCount; }
        if (nearEnemiCount > 0)
        {
            for(int i = 0; i < seuil.Length; i++)
            {
                if(i == 0)
                {
                    if(nearEnemiCount > 0 && nearEnemiCount < seuil[i+1])
                    {
                        currentSeuil = i;
                    }
                }
                else
                {
                    if(i < seuil.Length - 1)
                    {
                        if (nearEnemiCount > seuil[i] && nearEnemiCount < seuil[i + 1])
                        {
                            currentSeuil = i ;
                        }
                    }
                    else
                    {
                        if (nearEnemiCount >= seuil[i])
                        {
                            currentSeuil = i ;
                        }
                    }
                }
            }
            positionMoyenne /= nearEnemiCount;
            transform.position = positionMoyenne;
            transform.position = new Vector3(transform.position.x, playerPos.y, transform.position.z);
        }
        else
        {
            currentSeuil = 0;
        }
        masseSoundInstance.setParameterByName(variableFMOD, currentSeuil);
        masseSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        //Debug.Log("Send Seuil Masse : [" + currentSeuil + "]");
    }
}
