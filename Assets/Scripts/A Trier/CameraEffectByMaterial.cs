using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraEffectByMaterial : MonoBehaviour
{
    public Material material;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(material == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        Graphics.Blit(source, destination, material, pass: 0);
    }
}
