using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentManager : MonoBehaviour
{
    [SerializeField] private ArtefactsInfos[] m_listAllArtefact = new ArtefactsInfos[0];
    [SerializeField] List<ArtefactHolder> artefactHolder = new List<ArtefactHolder>();
    [Tooltip("The artefact prefab array need to match element enum")]
    [SerializeField] public GameObject[] artefactPrefab = new GameObject[4]; 

    public ArtefactsInfos GetArtefacts(int index) { return m_listAllArtefact[index]; }


    private void GenerateNewArtefact(int index) // Not Finish
    {
        GameObject newArtefact = Instantiate(artefactPrefab[(int)m_listAllArtefact[index].gameElement],
            transform.position , 
            transform.rotation,
            transform.Find("ArtefactContainer")
            );
    }

    public ArtefactsInfos GetFragment(int index)
    {
        return m_listAllArtefact[index];
    }

    public int GetRandomIndexFragment()
    {
        int rndArtefact = Random.Range(0, m_listAllArtefact.Length);
        return rndArtefact;
    }

    public void GiveArtefact(int index, GameObject target)
    {

        target.GetComponent<CharacterArtefact>().AddArtefact(m_listAllArtefact[index]);
        target.GetComponent<DropInventory>().AddNewArtefact(m_listAllArtefact[index]);
    }
}
