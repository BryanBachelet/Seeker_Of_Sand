using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[System.Serializable, VolumeComponentMenu("Post-processing/Custom/CustomPostProcessEffectHDRP")]
public sealed class CustomPostProcessEffectHDRPSettings : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
    public TextureParameter deformationTexture = new TextureParameter(null);
    public MaterialParameter customPostProcessMaterial = new MaterialParameter(null);

    public bool IsActive() => customPostProcessMaterial.value != null && intensity.value > 0f;
    public bool IsTileCompatible() => false;

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (customPostProcessMaterial.value == null)
            return;

        // Configure the material with the intensity and deformation texture
        customPostProcessMaterial.value.SetFloat("_Intensity", intensity.value);
        customPostProcessMaterial.value.SetTexture("_DeformationTex", deformationTexture.value);
        customPostProcessMaterial.value.SetTexture("_MainTex", source);

        // Render the effect
        HDUtils.DrawFullScreen(cmd, customPostProcessMaterial.value, destination);
    }
}
