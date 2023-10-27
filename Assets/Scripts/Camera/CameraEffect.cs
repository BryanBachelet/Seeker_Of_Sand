using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Render.Camera
{

    public class CameraEffect : MonoBehaviour
    {
        virtual public Vector3 GetEffectPos()
        {
            return Vector3.zero;
        }

        virtual public Vector3 GetEffectRot()
        {
            return Vector3.zero;
        }
    }
}
