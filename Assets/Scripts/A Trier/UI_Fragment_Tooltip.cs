using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.UI;
using UnityEngine.UI;
using GuerhoubaGames.GameEnum;
using TMPro;
using SeekerOfSand.Tools;
public class UI_Fragment_Tooltip : MonoBehaviour
{
    public List<GameObject> fragment_List = new List<GameObject>();
    public Sprite[] spriteType = new Sprite[4]; //0 Elec, 1 Eau, 2 Terre, 3 Feu
    public Sprite[] fragment_Type = new Sprite[4];
    public Sprite[] fragment_Rarity = new Sprite[3];

    [HideInInspector] public List<TooltipTrigger> tooltipTrigger = new List<TooltipTrigger>();
    [HideInInspector] public List<Image> imageFragmentTooltip = new List<Image>();
    [HideInInspector] public List<Image> fragmentTypeBackground = new List<Image>();
    [HideInInspector] public List<Image> fragmentRarity = new List<Image>();
    [HideInInspector] public List<TMP_Text> fragmentName = new List<TMPro.TMP_Text>();
    [HideInInspector] public List<ArtefactsInfos> fragmentInfo = new List<ArtefactsInfos>();
    [HideInInspector] private List<TMP_Text> fragmentCount = new List<TMP_Text>();

    public int currentFragmentNumber = 0;

    public Color[] colorOutlineByElement = new Color[5]; //Color by GameElement

    private CharacterArtefact m_characterArtefact;
    // Start is called before the first frame update
    void Awake()
    {
        if (fragment_List.Count > 0)
        {
            for (int i = 0; i < fragment_List.Count; i++)
            {
                tooltipTrigger.Add(fragment_List[i].GetComponent<TooltipTrigger>());
                imageFragmentTooltip.Add(fragment_List[i].GetComponent<Image>());
                fragmentCount.Add(fragment_List[i].GetComponentInChildren<TMP_Text>());
            }
        }


      
    }

    private void Start()
    {
        m_characterArtefact = GameState.s_playerGo.GetComponent<CharacterArtefact>();

    }

    public void AddNewFragment(ArtefactsInfos artefactInfo)
    {

        tooltipTrigger[currentFragmentNumber].header = artefactInfo.nameArtefact;
        tooltipTrigger[currentFragmentNumber].content = artefactInfo.description;
        tooltipTrigger[currentFragmentNumber].OnEnterData += UpdateDescription;

        imageFragmentTooltip[currentFragmentNumber].sprite = artefactInfo.icon;
        imageFragmentTooltip[currentFragmentNumber].gameObject.SetActive(true);
        SelectElement(fragment_List[currentFragmentNumber], artefactInfo);
        currentFragmentNumber += 1;
    }

    public void UpdateFragmentStack(int indexFragment, int stackNumber)
    {
        fragmentCount[indexFragment].text = "x<size=180%>" + stackNumber;
    }

    public void UpdateDescription(TooltipEventData data)
    {
        tooltipTrigger[data.index].content = m_characterArtefact.artefactsList[data.index].description;
    }
    public void RemoveFragment(int index)
    {
        imageFragmentTooltip[currentFragmentNumber-1].gameObject.SetActive(false);

        for (int i = index; i < currentFragmentNumber-1; i++)
        {
            tooltipTrigger[i].header = tooltipTrigger[i+1 ].header;
            tooltipTrigger[i].content = tooltipTrigger[i + 1].content;
            imageFragmentTooltip[i].sprite = imageFragmentTooltip[i + 1].sprite;
        }

      

        currentFragmentNumber--;
    }

    public void SelectElement(GameObject artefactObject, ArtefactsInfos artefactInfo)
    {
        artefactObject.transform.GetChild(0).gameObject.SetActive(true);
        artefactObject.GetComponentInChildren<Image>().sprite = artefactInfo.icon;
        FragmentDisplay_Elemental artefactElement = artefactObject.GetComponentInChildren<FragmentDisplay_Elemental>();
        artefactElement.ChangeFragmentDisplay(artefactInfo);


        //artefactElement.sprite = artefactInfo.icon;
        //artefactElement.color = colorOutlineByElement[GeneralTools.GetElementalArrayIndex( artefactInfo.gameElement,true)];
        
    }
}
