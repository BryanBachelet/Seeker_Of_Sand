using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectHighLight : MonoBehaviour
{
    private bool currentlyApplyEffect = false;
    [SerializeField] private float effectDuration;
    [SerializeField] private AnimationCurve effectCurve;
    private float currentEffectDuration;
    private float timeLast;

    [ColorUsage(true, true)]
    [SerializeField] private Color[] colorsUsed = new Color[2];
    [SerializeField] private float[] colorsIntensity = new float[2];

    [SerializeField] private SkinnedMeshRenderer m_meshRenderer;
    [SerializeField] private int indexMatAffected = 0;
    [SerializeField] private Material m_materialAffected;
    private MaterialPropertyBlock _propBlock;

    public Object_HealthDisplay m_objectHealthDisplay;
    // Start is called before the first frame update
    void Start()
    {
        m_materialAffected = m_meshRenderer.materials[indexMatAffected];
        _propBlock = new MaterialPropertyBlock();
        m_meshRenderer.GetPropertyBlock(_propBlock, indexMatAffected);
    }

    // Update is called once per frame
    void Update()
    {
        if(!currentlyApplyEffect) { return; }
        if(currentEffectDuration < effectDuration)
        {
            currentEffectDuration += Time.deltaTime;
        }
        else
        {
            currentlyApplyEffect = false;
            currentEffectDuration = effectDuration;
        }
        float progression = effectCurve.Evaluate(currentEffectDuration / effectDuration);
        _propBlock.SetColor("_HighlightColor", Color.Lerp(colorsUsed[0], colorsUsed[1], progression));
        _propBlock.SetFloat("_HighlightColorPower", Mathf.Lerp(colorsIntensity[0], colorsIntensity[1], progression));
        if(m_objectHealthDisplay)  m_objectHealthDisplay.UpdateAlpha(progression);
        m_meshRenderer.SetPropertyBlock(_propBlock, 0);
        //m_materialAffected.SetColor("_HighlightColor", Color.Lerp(colorsUsed[0], colorsUsed[1], progression));
        //m_materialAffected.SetFloat("_HighlightColorPower", Mathf.Lerp(colorsIntensity[0], colorsIntensity[1], progression));


    }

    public void ReceiveHit()
    {
        timeLast = Time.time;

        currentlyApplyEffect = true;
        currentEffectDuration = 0;
    }
}
