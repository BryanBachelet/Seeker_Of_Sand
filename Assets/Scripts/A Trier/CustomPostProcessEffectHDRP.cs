using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEngine.Rendering.PostProcessing
{
    class CustomPostProcessEffectHDRP : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public Material customPostProcessMaterial;

        public bool IsActive() => customPostProcessMaterial != null;
        public bool IsTileCompatible() => false;

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (customPostProcessMaterial == null) return;

            // Apply the custom post-process effect
            cmd.Blit(source, destination, customPostProcessMaterial, pass: 0);
        }
    }
}
