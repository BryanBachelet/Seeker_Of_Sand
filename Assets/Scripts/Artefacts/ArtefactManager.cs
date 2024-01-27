using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtefactManager : MonoBehaviour
{
    [SerializeField] private ArtefactsInfos[] m_listAllArtefact = new ArtefactsInfos[0];

    public ArtefactsInfos GetArtefacts(int index) { return m_listAllArtefact[index]; }

}
