using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ManualDepthTraitement : MonoBehaviour
{
    private Camera m_camera;

    public int width;
    public int height;

    public RenderTexture rt1;
    public RenderTexture rt2;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateRenderTexture()
    {
        m_camera = this.GetComponent<Camera>();
        m_camera.depthTextureMode = DepthTextureMode.Depth;
        rt1 = new RenderTexture(width, height, 0, RenderTextureFormat.Default);
        rt2 = new RenderTexture(width, height, 24, RenderTextureFormat.Depth);
        m_camera.SetTargetBuffers(rt1.colorBuffer, rt2.depthBuffer);
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination);
    }
}
