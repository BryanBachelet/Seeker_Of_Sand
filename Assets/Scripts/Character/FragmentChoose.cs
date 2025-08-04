using GuerhoubaGames.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FragmentChoose : MonoBehaviour
{
    public ArtefactsInfos[] artefactInfoCurrent;
    public TMP_Text[] descriptionTmp_Text;
    public Transform m_playerTransform;

    public FragmentUIView fragmentUIView1;
    public Artefact_UI_View artefactUIView1;
    public FragmentUIView fragmentUIView2;
    public Artefact_UI_View artefactUIView2;

    public GameObject fonduAuNoir;

    private Animator m_animator;
    // Start is called before the first frame update
    void OnEnable()
    {
        if (m_playerTransform == null) { m_playerTransform = GameObject.Find("Player").transform; }
        m_animator = this.GetComponent<Animator>();
    }


    public void SetFragmentInfo(ArtefactsInfos[] artefactInfo)
    {

        fonduAuNoir.SetActive(true);
        artefactInfoCurrent = artefactInfo;
        fragmentUIView1.UpdateInteface(artefactInfo[0]);
        artefactUIView1.UpdateInteface(artefactInfo[0]);
        fragmentUIView2.UpdateInteface(artefactInfo[1]);
        artefactUIView2.UpdateInteface(artefactInfo[1]);
        descriptionTmp_Text[0].text = artefactInfo[0].name + "<br>" + artefactInfo[0].description;
        descriptionTmp_Text[1].text = artefactInfo[1].name + "<br>" + artefactInfo[1].description;
        m_animator.SetBool("OpenChoose", true);
        GameState.SetState(false);
    }

    public void GiveSelectedFragment(int fragmentNumber)
    {

        m_playerTransform.GetComponent<CharacterArtefact>().AddArtefact(artefactInfoCurrent[fragmentNumber]);
        m_playerTransform.GetComponent<DropInventory>().AddNewArtefact(artefactInfoCurrent[fragmentNumber]);
        m_animator.SetBool("OpenChoose", false);
        GameState.SetState(true);
    }


}
