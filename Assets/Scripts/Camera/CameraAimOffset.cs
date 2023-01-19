using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAimOffset : MonoBehaviour
{
    [SerializeField] private float m_sizeDistance = 2;
    [SerializeField] private Character.CharacterAim m_characterAim;


    private Vector3 m_offsetAim;
    public Vector3 GetAimOffset()
    {
        return m_offsetAim;
    }

    public void Update()
    {
        Vector3 aimDirection = m_characterAim.GetAim();
        aimDirection = new Vector3(aimDirection.x, 0, aimDirection.z);
        float magnitude = m_characterAim.GetAimMagnitude();
        m_offsetAim = aimDirection.normalized * magnitude * m_sizeDistance;
    }
}
