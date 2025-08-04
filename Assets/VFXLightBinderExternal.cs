using UnityEngine.VFX;

namespace UnityEngine.VFX.Utility
{
    [AddComponentMenu("VFX/Property Binders/Light Binder External")]
    [VFXBinder("Utility/LightExternal")]
    class VFXLightBinderExternal : VFXBinderBase
    {
        public string ColorProperty { get { return (string)m_ColorProperty; } set { m_ColorProperty = value; } }
        public string BrightnessProperty { get { return (string)m_BrightnessProperty; } set { m_ColorProperty = value; } }
        public string RadiusProperty { get { return (string)m_RadiusProperty; } set { m_RadiusProperty = value; } }
        public string TemperatureProperty { get { return (string)m_TemperatureProperty; } set { m_TemperatureProperty = value; } }

        [VFXPropertyBinding("UnityEngine.Color"), SerializeField, UnityEngine.Serialization.FormerlySerializedAs("m_ColorParameter")]
        protected ExposedProperty m_ColorProperty = "Color";
        [VFXPropertyBinding("System.Single"), SerializeField, UnityEngine.Serialization.FormerlySerializedAs("m_BrightnessParameter")]
        protected ExposedProperty m_BrightnessProperty = "Brightness";
        [VFXPropertyBinding("System.Single"), SerializeField, UnityEngine.Serialization.FormerlySerializedAs("m_RadiusParameter")]
        protected ExposedProperty m_RadiusProperty = "Radius";
        [VFXPropertyBinding("System.Single"), SerializeField, UnityEngine.Serialization.FormerlySerializedAs("m_TemperatureParameter")]
        protected ExposedProperty m_TemperatureProperty = "Temperature";
        public Light Target = null;
        static public Light lightTarget = null;

        public bool BindColor = true;
        public bool BindBrightness = false;
        public bool BindRadius = false;
        public bool BindTemperature = false;

        public override bool IsValid(VisualEffect component)
        {
            if (lightTarget == null && Target != null)
            {
                lightTarget = Target;
            }
            if (Target == null)
            {
                if (lightTarget == null)
                {
                }
                else
                {
                    Target = lightTarget;
                }

            }
            return Target != null
                && (!BindColor || component.HasVector4(ColorProperty))
                && (!BindBrightness || component.HasFloat(BrightnessProperty))
                && (!BindRadius || component.HasFloat(RadiusProperty))
                && (!BindTemperature || component.HasFloat(TemperatureProperty))
            ;
        }

        public override void UpdateBinding(VisualEffect component)
        {

            if (BindColor)
                component.SetVector4(ColorProperty, Target.color);
            if (BindBrightness)
                component.SetFloat(BrightnessProperty, Target.intensity);
            if (BindRadius)
                component.SetFloat(RadiusProperty, Target.range);
            if (BindTemperature)
                component.SetFloat(TemperatureProperty, Target.colorTemperature);
        }

        public override string ToString()
        {
            return string.Format("Light : '{0}' -> {1}", m_ColorProperty, Target == null ? "(null)" : Target.name);
        }
    }
}
