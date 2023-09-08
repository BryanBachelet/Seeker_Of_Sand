using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Main_UI_Controle : MonoBehaviour
{
    public GameObject[] buttonHolderPositionLayer1;
    public GameObject[] buttonHolderPositionLayer2;
    public Quaternion[] buttonInactiveHolderRotation1;
    public Quaternion[] buttonInactiveHolderRotation2;
    private Quaternion buttonActiveQuaternion;

    public Transform buttonActiveHolderPosition;

    private bool activeRotation = false;
    public bool activeRotationTrigger = false;
    private bool resetRotation = false;
    public bool resetRotationTrigger = false;
    public float timeActive;
    public AnimationCurve transitionSpeed;

    [SerializeField] private int currentLayer = 0;

    public int indexButtonActive = 0;
    public GameObject selectionCircle;
    public GameObject currentButtonSelection;


    public Vector3[] positionButtonToScreenPointLayer1;
    public Vector3[] positionButtonToScreenPointLayer2;
    public RectTransform[] textObjectLayer1;
    public RectTransform[] textObjectLayer2;

    public Vector3 buttonOffsetPosition;
    public int currentButtonOver = 0;

    private Animator m_animator;
    // Start is called before the first frame update
    void Start()
    {
        buttonActiveQuaternion = buttonActiveHolderPosition.rotation;
        currentLayer = 0;
        m_animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(activeRotationTrigger)
        {
            activeRotation = true;
            activeRotationTrigger = false;

            timeActive = Time.time;
        }
        if (activeRotation)
        {
            RotationMainUi();
            RotationSecondUI();
            //initialRotPosition[]
        }
        if (resetRotationTrigger)
        {
            resetRotation = true;
            resetRotationTrigger = false;

            timeActive = Time.time;
        }
        if (resetRotation)
        {
            ResetRotationMainUi();
            //initialRotPosition[]
        }
        if(currentLayer == 0)
        {
            selectionCircle.transform.position = buttonHolderPositionLayer1[currentButtonOver].transform.GetChild(0).position;
        }
        else if(currentLayer == 1)
        {

        }

    }

    public void ActiveTriggerSecondButton(int index)
    {
        indexButtonActive = index;
        currentLayer = 2;
        activeRotationTrigger = true;
    }
    public void ActiveTriggerButton(int index)
    {
        indexButtonActive = index;
        currentLayer = 1;
        activeRotationTrigger = true;
    }
    public void ActiveResetbutton()
    {
        resetRotationTrigger = true;
        activeRotation = false;
        currentLayer = 0;
        indexButtonActive = 0;
        currentButtonOver = 0;
        for(int i = 0; i < buttonHolderPositionLayer1.Length; i++)
        {
            positionButtonToScreenPointLayer1[i] = Vector3.zero;
        }
    }
    public void RotationMainUi()
    {
        for(int i = 0; i < buttonHolderPositionLayer1.Length; i++)
        {
            if(i != indexButtonActive)
            {
                buttonHolderPositionLayer1[i].transform.eulerAngles = Vector3.Lerp(Vector3.zero, buttonInactiveHolderRotation1[i].eulerAngles, transitionSpeed.Evaluate(Time.time - timeActive));
            }
            else
            {
                buttonHolderPositionLayer1[i].transform.eulerAngles = Vector3.Lerp(Vector3.zero, buttonActiveHolderPosition.eulerAngles + new Vector3(0,0, 20 * i), transitionSpeed.Evaluate(Time.time - timeActive));
                currentButtonSelection.transform.position = buttonHolderPositionLayer1[i].transform.GetChild(0).position;
            }


            MoveTextWithbutton();
            if (Time.time - timeActive > 1) { activeRotation = false; }
            positionButtonToScreenPointLayer1[i] = Camera.main.WorldToScreenPoint(buttonHolderPositionLayer1[i].transform.GetChild(0).position);
        }
        m_animator.SetBool("MainUi", false);
    }

    public void RotationSecondUI()
    {
        for(int i = 0; i < buttonHolderPositionLayer2.Length; i++)
        {
            buttonHolderPositionLayer2[i].transform.eulerAngles = Vector3.Lerp(Vector3.zero, buttonInactiveHolderRotation2[i].eulerAngles, transitionSpeed.Evaluate(Time.time - timeActive));
            positionButtonToScreenPointLayer2[i] = Camera.main.WorldToScreenPoint(buttonHolderPositionLayer2[i].transform.GetChild(0).position);
        }

        if (Time.time - timeActive > 1) { activeRotation = false; }
    }
    public void ResetRotationMainUi()
    {
        for (int i = 0; i < buttonHolderPositionLayer1.Length; i++)
        {
            
            if (i != indexButtonActive)
            {
                buttonHolderPositionLayer1[i].transform.eulerAngles = Vector3.Lerp(buttonInactiveHolderRotation1[i].eulerAngles, Vector3.zero, transitionSpeed.Evaluate(Time.time - timeActive));
            }
            else
            {
                buttonHolderPositionLayer1[i].transform.eulerAngles = Vector3.Lerp(buttonActiveHolderPosition.eulerAngles + new Vector3(0, 0, 20 * i), Vector3.zero, transitionSpeed.Evaluate(Time.time - timeActive));
                selectionCircle.transform.position = buttonHolderPositionLayer1[i].transform.position;
                currentButtonSelection.transform.position = buttonHolderPositionLayer1[i].transform.position;
            }

            MoveTextWithbutton();
            if (Time.time - timeActive > 1) { resetRotation = false;  }
            positionButtonToScreenPointLayer1[i] = Camera.main.WorldToScreenPoint(buttonHolderPositionLayer1[i].transform.GetChild(0).position);
        }
        m_animator.SetBool("MainUi", true);
    }


    public void MoveTextWithbutton()
    {
        for(int i = 0; i < textObjectLayer1.Length; i++)
        {
            textObjectLayer1[i].position = positionButtonToScreenPointLayer1[i] + buttonOffsetPosition;
        }
        for(int i = 0; i < textObjectLayer2.Length; i++)
        {
            textObjectLayer2[i].position = positionButtonToScreenPointLayer2[i] + buttonOffsetPosition;
        }
    }

    public void ButtonOver(int indexButton)
    {
        currentButtonOver = indexButton;
        if( indexButton == -1)
        {
            currentButtonOver = indexButtonActive;
        }
    }
}
