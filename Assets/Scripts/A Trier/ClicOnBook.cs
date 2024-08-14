using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClicOnBook : MonoBehaviour
{
    private Camera m_camera;
    public GameObject lightPrefab;
    public LayerMask renderBookLayer;
    public Vector3 inputInfo;

    public Vector3 mousePiosition;

    public float offset;
    public Vector2 augmenteOffset;
    public Vector3 offSetPosition;


    // Start is called before the first frame update
    void Start()
    {
        m_camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void getClicEffect(Vector2 clicPosition)
    {
        Vector3 convert = new Vector3((clicPosition.x * augmenteOffset.x) / m_camera.pixelHeight, offset, (clicPosition.y * augmenteOffset.y )/ m_camera.pixelWidth);
        Vector3 direction = m_camera.ScreenToViewportPoint(convert);
        inputInfo = direction;
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(offSetPosition + m_camera.transform.position, -direction, out hit, Mathf.Infinity, renderBookLayer))
        {
            Debug.DrawRay(offSetPosition + m_camera.transform.position, -direction * hit.distance, Color.yellow);
            inputInfo = hit.point;
            Instantiate(lightPrefab, inputInfo, transform.rotation);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(offSetPosition + m_camera.transform.position, -direction * 1000, Color.white);
            Debug.Log("Did not Hit");
        }


    }
}
