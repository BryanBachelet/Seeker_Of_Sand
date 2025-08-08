using GuerhoubaGames.GameEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.VFX;

namespace GuerhoubaGames.VFX
{

    public class VFX_SpellArea : MonoBehaviour
    {
        public SpellSystem.AreaMeta areaMeta;
        public VisualEffect vfx;

        // Start is called before the first frame update
        void Start()
        {
            areaMeta = GetComponentInParent<SpellSystem.AreaMeta>();
            vfx.SetFloat("Size", areaMeta.areaData.spellProfil.GetFloatStat(GuerhoubaGames.GameEnum.StatType.Size));
            if (areaMeta.areaData.spellProfil.TagList.EqualsSpellParticularity(SpellParticualarity.Explosion))
            {
                vfx.SetFloat("Size", areaMeta.areaData.spellProfil.GetFloatStat(GuerhoubaGames.GameEnum.StatType.SizeExplosion));
            }
        }


    }
}
