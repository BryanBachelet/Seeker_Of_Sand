using Klak.Wiring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ui_Display_Timeline : MonoBehaviour
{
    public GameObject dayHolder;
    public GameObject nightHolder;
    public RectTransform timeIndicator;
    public RectTransform start_timeline;
    public RectTransform end_timeline;
    [Range(0,1)] public float currentProgress;
    public bool currentDayState = true;
    public Animator m_animator;
    // Start is called before the first frame update
    void Start()
    {
        StartDay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        UpdateIndicatorProgress(currentProgress);
    }
    public void UpdateIndicatorProgress(float progress)
    {
        currentProgress = progress;
        timeIndicator.transform.position = Vector3.Lerp(start_timeline.position, end_timeline.position, progress);

    }

    public void ChangeDayState()
    {
        currentDayState = !currentDayState;
        if (currentDayState)
        {
            StartDay();
        }
        else
        {
            StartNight();
        }
    }

    public void StartDay()
    {
        currentProgress = 0;
        Vector3 newRot = new Vector3(0, 0, 0);
        timeIndicator.position = Vector3.Lerp(start_timeline.position, end_timeline.position, currentProgress);
        timeIndicator.localRotation = Quaternion.Euler(newRot);
        m_animator.SetTrigger("StartDay");
        m_animator.ResetTrigger("StartNight");
    }

    public void StartNight()
    {
        currentProgress = 1;
        Vector3 newRot = new Vector3 (0, 0, -180);
        timeIndicator.position = Vector3.Lerp(start_timeline.position, end_timeline.position, currentProgress);
        timeIndicator.localRotation = Quaternion.Euler(newRot);
        m_animator.ResetTrigger("StartDay");
        m_animator.SetTrigger("StartNight");
    }
}
