using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterChainEffect : MonoBehaviour
{
    [HideInInspector] public List<SpellSystem.ChainEffect> chainEffectsList = new List<SpellSystem.ChainEffect>();

    public void AddChainEffect(SpellSystem.ChainEffect newChainEffect)
    {
        newChainEffect.hasBeenAdd = true;
        newChainEffect.Reset();
        chainEffectsList.Add(newChainEffect);
    }

    public void ApplyChainEffect(SpellSystem.SpellProfil spellTarget)
    {
        for (int i = 0; i < chainEffectsList.Count; i++)
        {
            chainEffectsList[i].Apply(spellTarget);
            if (chainEffectsList[i].IsFinish())
            {
                chainEffectsList[i].Reset();
                chainEffectsList.RemoveAt(i);
            }
        }
    }
}
