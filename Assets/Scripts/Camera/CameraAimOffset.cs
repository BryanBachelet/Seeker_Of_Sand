using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Render.Camera
{

    public class CameraAimOffset : CameraEffect
    {

        [SerializeField] private Character.CharacterAim m_characterAim;


        private Vector3 m_offsetAim;
        public override Vector3 GetEffectPos()
        {
            return m_offsetAim;
        }

        public override Vector3 GetEffectRot()
        {
            return base.GetEffectRot();
        }

        public void Update()
        {
            Vector3 aimDirection = m_characterAim.GetAimDirection();
            aimDirection = new Vector3(aimDirection.x, 0, aimDirection.z);
            float magnitude = m_characterAim.GetRangeFromPlayerToFinalPoint();

            // m_offsetAim = aimDirection.normalized * 1 * m_sizeDistance;
            m_offsetAim = Vector3.zero;
        }
    }
}