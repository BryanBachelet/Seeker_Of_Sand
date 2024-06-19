using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.UI;
using UnityEngine.UI;
using GuerhoubaGames.GameEnum;
public class UI_Fragment_Tooltip : MonoBehaviour
{
    public List<GameObject> fragment_List = new List<GameObject>();
    public Sprite[] spriteType = new Sprite[4]; //0 Elec, 1 Eau, 2 Terre, 3 Feu

    [HideInInspector] public List<TooltipTrigger> tooltipTrigger = new List<TooltipTrigger>();
    [HideInInspector] public List<Image> imageFragmentTooltip = new List<Image>();

    public int currentFragmentNumber = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (fragment_List.Count > 0)
        {
            for(int i = 0; i < fragment_List.Count; i++)
            {
                tooltipTrigger.Add(fragment_List[i].GetComponent<TooltipTrigger>());
                imageFragmentTooltip.Add(fragment_List[i].GetComponent<Image>());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddNewFragment(ArtefactsInfos artefactInfo)
    {
        if(!fragment_List[currentFragmentNumber].activeSelf)
        {
            fragment_List[currentFragmentNumber].SetActive(true);
        }
        tooltipTrigger[currentFragmentNumber].header = artefactInfo.nameArtefact;
        tooltipTrigger[currentFragmentNumber].content = artefactInfo.description;


        if(artefactInfo.gameElement == GameElement.AIR)
        {
            imageFragmentTooltip[currentFragmentNumber].sprite = spriteType[0];
        }
        else if (artefactInfo.gameElement == GameElement.WATER)
        {
            imageFragmentTooltip[currentFragmentNumber].sprite = spriteType[1];
        }
        else if (artefactInfo.gameElement == GameElement.EARTH)
        {
            imageFragmentTooltip[currentFragmentNumber].sprite = spriteType[2];
        }
        else if (artefactInfo.gameElement == GameElement.FIRE)
        {
            imageFragmentTooltip[currentFragmentNumber].sprite = spriteType[3];
        }
        currentFragmentNumber += 1;
    }
}
