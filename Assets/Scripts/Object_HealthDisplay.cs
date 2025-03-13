using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_HealthDisplay : MonoBehaviour
{
    public GameObject healthHolder;
    public GameObject healthBarFill;
    public GameObject healthBackground;
    public SpriteRenderer healthBar;
    public Vector3 posLowHealth;
    public Vector3 posFullHealth;
    public Vector3 scaleLow;
    public Vector3 scaleHigh;

    public bool isHealthDisplay = false;
    [Range(0,1)]
    public float progressDebug = 1;

    public SpriteRenderer spriteRender;
    public Material m_progressMaterial;
    // Start is called before the first frame update
    void Start()
    {
        healthHolder.SetActive(isHealthDisplay);
        m_progressMaterial = spriteRender.material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateLifeBar(float progress)
    {
        if (isHealthDisplay)
        {
            //healthBarFill.transform.localPosition = Vector3.Lerp(posFullHealth, posLowHealth, progress);
            //healthBarFill.transform.localScale = Vector3.Lerp(scaleLow, scaleHigh, progress);
            m_progressMaterial.SetFloat("_Progress", progress);
            //healthBackground.transform.localPosition = Vector3.Lerp(posLowHealth, new Vector3(-posFullHealth.x, posFullHealth.y, posFullHealth.z), progress);
            //healthBackground.transform.localScale = new Vector3(Mathf.Lerp(0, 0.35f, progress), 0.25f, 0.22f);
        }
    }

    public void UpdateAlpha(float progress)
    {
        if (isHealthDisplay)
        {
            //healthBarFill.transform.localPosition = Vector3.Lerp(posFullHealth, posLowHealth, progress);
            //healthBarFill.transform.localScale = Vector3.Lerp(scaleLow, scaleHigh, progress);
            m_progressMaterial.SetFloat("_Alpha", progress);
            //healthBackground.transform.localPosition = Vector3.Lerp(posLowHealth, new Vector3(-posFullHealth.x, posFullHealth.y, posFullHealth.z), progress);
            //healthBackground.transform.localScale = new Vector3(Mathf.Lerp(0, 0.35f, progress), 0.25f, 0.22f);
        }
    }

}
