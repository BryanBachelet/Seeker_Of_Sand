using SpellSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using GuerhoubaGames.UI;
public class spell_Attribution : MonoBehaviour
{
    [SerializeField] private int preLeveledDisplay = 0;

    [HideInInspector] private Material materialUse;
    [HideInInspector] private string spell_name;
    [HideInInspector] private string description;
    [HideInInspector] private int level;
    [HideInInspector] private int rank;

    [SerializeField] private SpellProfil spellProfil;

    [Header("Component In Child")]
    [SerializeField] public Image imageSpell;
    [SerializeField] private TMP_Text nameSpell;
    [SerializeField] private Image rankSpell_material;
    [SerializeField] private Image tabSpell_material;
    [SerializeField] private Animator animatorSpell;

    [SerializeField] private Image[] levelDisplayObject = new Image[13];
    [SerializeField] private Material[] materialLevel = new Material[2];
    [SerializeField] private Material[] materialRank = new Material[4];
    [SerializeField] private Texture[] textureRank = new Texture[4];
    [SerializeField] private bool[] animationBool = new bool[2]; //0 --> ImageOnly, correspond à l'affichage de sort dans le nom + bandeau //1 --> OpenTab, affichage avec le nom du sort + bandeau

    [HideInInspector] private Material spellCadre_cloneMaterial;
    [HideInInspector] private Material spellTab_cloneMaterial;

    [HideInInspector] public TooltipTrigger tooltipTrigger;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (animatorSpell == null) { animatorSpell = this.GetComponentInChildren<Animator>(); }
        tooltipTrigger = imageSpell.GetComponent<TooltipTrigger>();
        ChangeAnimatorBool();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AcquireSpellData(SpellProfil spell)
    {
        bool generateTexture = false;
        if (spellProfil != null)
        {
            if (spellProfil != spell)
            {
                generateTexture = true;
            }

        }
        else
        {
            generateTexture = true;
        }
        spellProfil = spell;
        spellProfil.m_SpellAttributionAssociated = this;
        materialUse = spell.matToUse;
        imageSpell.material = materialUse;
        spell_name = spell.name;
        description = spell.description;
        nameSpell.text = spell_name;

        if(generateTexture)
        {
            Texture2D texture = (Texture2D)materialUse.GetTexture("_Symbole");
            imageSpell.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
        }
        //UpdateSpellLevel(spell);
        StartCoroutine(UpdateSpellLevelDelay(spell));
    }

    public void UpdateToolTipInfo()
    {
        tooltipTrigger.header = spellProfil.name;
        tooltipTrigger.content = spellProfil.description;
        //tooltipTrigger.additionnalDatas
    }
    public void UpdateSpellLevel(SpellProfil spell)
    {
        if (preLeveledDisplay > 0) { level = preLeveledDisplay; }
        else { level = spell.spellExp; }
        rank = level % 4;
        rankSpell_material.material = materialRank[rank];
        for (int i = 0; i < levelDisplayObject.Length; i++)
        {
            if (i <= level)
            {
                levelDisplayObject[i].material = materialLevel[1];
            }
            else
            {
                levelDisplayObject[i].material = materialLevel[0];
            }
        }
    }

    public void ChangeNameForSpecificText(string text)
    {
        nameSpell.text = text;
    }
    public IEnumerator UpdateSpellLevelDelay(SpellProfil spell)
    {
        if (preLeveledDisplay > 0) { level = preLeveledDisplay; }
        else { level = spell.spellExp; }
        rank = level / 4;
        //rankSpell_material.material = materialRank[rank];
        if(spellCadre_cloneMaterial == null)
        {
            spellCadre_cloneMaterial = new Material(rankSpell_material.material);
            rankSpell_material.material = spellCadre_cloneMaterial;
        }
        if(spellTab_cloneMaterial == null)
        {
            spellTab_cloneMaterial = new Material(tabSpell_material.material);
            tabSpell_material.material = spellTab_cloneMaterial;
        }
        spellCadre_cloneMaterial.SetTexture("_Gradient", textureRank[rank]);
        spellTab_cloneMaterial.SetTexture("_Gradient", textureRank[rank]);
        for (int i = 0; i < levelDisplayObject.Length; i++)
        {
            if (i <= level)
            {
                levelDisplayObject[i].material = materialLevel[1];
            }
            else
            {
                levelDisplayObject[i].material = materialLevel[0];
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
    public void ChangeAnimatorBool()
    {
        for (int i = 0; i < animationBool.Length; i++)
        {
            animatorSpell.SetBool("" + i, animationBool[i]);
        }
    }
}
