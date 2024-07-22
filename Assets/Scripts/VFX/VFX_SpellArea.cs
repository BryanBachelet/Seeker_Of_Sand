using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class VFX_SpellArea : MonoBehaviour
{
    public SpellSystem.AreaMeta areaMeta;
    public VisualEffect vfx;

    // Start is called before the first frame update
    void Start()
    {
        areaMeta = GetComponentInParent<SpellSystem.AreaMeta>();
        vfx.SetFloat("Size", areaMeta.areaData.spellProfil.GetFloatStat(GuerhoubaGames.GameEnum.StatType.Size));
    }

  
}
