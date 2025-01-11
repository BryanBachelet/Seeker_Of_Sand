using GuerhoubaGames.GameEnum;
using SpellSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterChainEffect : MonoBehaviour
{
    [HideInInspector] public List<SpellSystem.ChainEffect> chainEffectsList = new List<SpellSystem.ChainEffect>();

    public void AddChainEffect(SpellSystem.ChainEffect newChainEffect,SpellProfil ownerSpellProfil)
    {
        newChainEffect.hasBeenAdd = true;
        newChainEffect.Reset();
        newChainEffect.launchSpellProfil = ownerSpellProfil;
        chainEffectsList.Add(newChainEffect);
    }

    public BehaviorLevel[] GetAllBehaviorLevel()
    {
        List<BehaviorLevel> behaviorLevels = new List<BehaviorLevel>();
        for (int i = 0; i < chainEffectsList.Count; i++)
        {
            for (int j = 0; j < chainEffectsList[i].levelSpells.Length; j++)
            {
                if (chainEffectsList[i].levelSpells[j].LevelType == SpellLevelType.BEHAVIOR)
                {
                    behaviorLevels.Add( (BehaviorLevel)chainEffectsList[i].levelSpells[j]);
                }
            }
           
        }
        return behaviorLevels.ToArray();
    }

    public void ApplyChainEffect(SpellSystem.SpellProfil spellTarget)
    {
        for (int i = 0; i < chainEffectsList.Count; i++)
        {
            chainEffectsList[i].Apply(gameObject,spellTarget);
            if (chainEffectsList[i].IsFinish())
            {
                chainEffectsList[i].Reset();
                chainEffectsList.RemoveAt(i);
                i--;
            }
        }
    }
}
