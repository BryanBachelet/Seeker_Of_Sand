using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class FluxBehavior : MonoBehaviour
{
    public GameObject fluxStart;
    public GameObject fluxEnd;
    private LineRenderer m_LineRenderer;
    private Vector3[] linePositions = new Vector3[2];

    public bool updatePosition = false;
    // Start is called before the first frame update
    void Start()
    {
        UpdateFluxPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if(updatePosition)
        {
            updatePosition = false;
            UpdateFluxPosition();
        }
    }

    public void UpdateFluxPosition()
    {
        if(m_LineRenderer == null) { m_LineRenderer = this.GetComponent<LineRenderer>(); }
        linePositions[0] = fluxStart.transform.position;
        linePositions[1] = fluxEnd.transform.position;
        m_LineRenderer.SetPositions(linePositions);
    }
}
