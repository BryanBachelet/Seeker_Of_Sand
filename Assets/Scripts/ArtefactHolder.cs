using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtefactHolder : MonoBehaviour
{
    public ArtefactsInfos m_artefactsInfos;

    private bool m_firstTime = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" )
        {
            other.GetComponent<CharacterArtefact>().AddArtefact(m_artefactsInfos);
            other.GetComponent<DropInventory>().AddNewArtefact(m_artefactsInfos);

             m_firstTime = true;
            transform.parent.GetComponent<ExperienceMouvement>().DestoyObject();
           // transform.parent.GetComponent<ExperienceMouvement>().m_timeBeforeDestruction = 0.2f;
          

          
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !m_firstTime)
        {
            other.GetComponent<CharacterArtefact>().AddArtefact(m_artefactsInfos);
            other.GetComponent<DropInventory>().AddNewArtefact(m_artefactsInfos);

            m_firstTime = true;
            transform.parent.GetComponent<ExperienceMouvement>().DestoyObject();
          //  transform.parent.GetComponent<ExperienceMouvement>().m_timeBeforeDestruction = 0.2f;



        }
    }

}
