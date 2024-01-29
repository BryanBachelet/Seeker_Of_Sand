using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Artefact
{
    public class SpellLauncher : MonoBehaviour
    {
        private ArtefactData m_artefactData;
        private Character.CharacterShoot m_characterShoot ;
        private float radiusEffect;
        public LayerMask enemyMask;
        public int spellOffset = 0;
        public void Start()
        {
            m_artefactData = GetComponent<ArtefactData>();
            radiusEffect = m_artefactData.radius;
            m_characterShoot = m_artefactData.characterGo.GetComponent<Character.CharacterShoot>();
            ApplyEffect();
        }

       
        private void ApplyEffect()
        {
            m_characterShoot.LaunchShootUniqueSpell(m_characterShoot.GetCapsuleIndex(spellOffset));
        }
    }
}