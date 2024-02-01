using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtefactHolder : MonoBehaviour
{
    public ArtefactsInfos m_artefactsInfos;
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<CharacterArtefact>().AddArtefact(m_artefactsInfos);
            other.GetComponent<DropInventory>().AddNewArtefact(m_artefactsInfos);
            Destroy(transform.parent.gameObject);

        }
    }
}
