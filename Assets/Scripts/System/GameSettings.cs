using FMOD.Studio;
using Render.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameSettings : MonoBehaviour
{    
    
    [SerializeField] private CameraBehavior m_cameraBehavior;


    static bool gp_MouseRotationSpeed = false;
    [Range(0.1f, 1)] public float mouseRotationSpeed = 1;
    [SerializeField] private TMP_Text m_textValueRotSpeed;

    static bool gp_AngleRotationSpeed = false;
    [Range(0.1f, 1)] public float angleRotationSpeed = 1;
    [SerializeField] private TMP_Text m_textValueAngleSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateRotationSpeed(Slider sliderValue)
    {
        float speed = sliderValue.value;
        if (speed > 1) speed = 1;
        else if (speed < 0.1f) speed = 0.1f;

        mouseRotationSpeed = speed;
        m_cameraBehavior.UpdateSettingRotationSpeed(mouseRotationSpeed);
        m_textValueRotSpeed.text = "" + mouseRotationSpeed.ToString("F2");
    }

    public void UpdateAngleSpeed(Slider sliderValue)
    {
        float speed = sliderValue.value;
        if (speed > 1) speed = 1;
        else if (speed < 0.1f) speed = 0.1f;

        angleRotationSpeed = speed;
        m_cameraBehavior.UpdateSettingAngleSpeed(angleRotationSpeed);
        m_textValueAngleSpeed.text = "" + angleRotationSpeed.ToString("F2");
    }
}
