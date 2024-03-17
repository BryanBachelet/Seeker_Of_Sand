using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InteractionEvent : MonoBehaviour
{
    [SerializeField] private float radiusInteraction;
    [SerializeField] private LayerMask InteractibleObject;
    [SerializeField] private Vector2 offsetPopUpDisplay;
    private float lastInteractionCheck;
    public float intervalCheckInteraction;
    public GameObject currentInteractibleObject;
    private Transform m_socleTransform;

    public GameObject ui_HintInteractionObject;
    private RectTransform ui_RectTransformHintInteraction;
    public Animator m_lastHintAnimator;
    [HideInInspector] public Animator m_lastArtefactAnimator;
    public RectTransform parentHintTransform;

    public string[] eventDataInfo;

    public TMP_Text txt_ObjectifDescription;
    public TMP_Text txt_ObjectifDescriptionPnj;
    public TMP_Text txt_RewardDescription;
    public UnityEngine.UI.Image img_ImageReward;
    public Sprite[] sprite_List;
    public Image img_progressionBar;

    public LayerMask traderLayer;
    public float rangeTrader;

    public Animator lastTrader;

    public LayerMask artefactLayer;
    public float rangeArtefact;
    public ArtefactHolder lastArtefact;
    [SerializeField] private RectTransform CanvasRect;

    public Collider[] colliderProche;

    public HintInteractionManager m_hintInteractionManager;

    [SerializeField] private Camera mainCamera;

    public GameObject currentInteractibleObjectActive = null;
    // Start is called before the first frame update
    void Start()
    {
        lastInteractionCheck = 0;
        ui_RectTransformHintInteraction = ui_HintInteractionObject.transform.parent.GetComponent<RectTransform>();
        parentHintTransform = ui_RectTransformHintInteraction;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > lastInteractionCheck + intervalCheckInteraction)
        {
            if (currentInteractibleObjectActive == null) { NearPossibleInteraction(); }
            NearTrader();
            NearArtefact();
            lastInteractionCheck = Time.time;
        }
        if(currentInteractibleObject != null)
        {
            CalculateWorldPosition(currentInteractibleObject);
        }
        else if(lastArtefact != null)
        {
            CalculateWorldPosition(lastArtefact.gameObject);
        }
    }

    public void NearPossibleInteraction()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, radiusInteraction, InteractibleObject);
        if (col.Length > 0)
        {
            if (ui_HintInteractionObject != null)
            {
                //ui_HintInteractionObject.SetActive(true);
                m_hintInteractionManager.activateAutelData(true);
            }
            if (currentInteractibleObject != col[0].transform.gameObject)
            {
                currentInteractibleObject = col[0].transform.gameObject;
                m_socleTransform = GameObject.Find("low_Socle").transform;
                eventDataInfo = currentInteractibleObject.GetComponent<AltarBehaviorComponent>().GetAltarData();
                m_lastHintAnimator.SetBool("InteractionOn", true);
                if (eventDataInfo[0] == "0")
                {
                    img_ImageReward.sprite = sprite_List[int.Parse(eventDataInfo[3])]; //Cristal Associated
                }
                else if (eventDataInfo[0] == "1")
                {
                    img_ImageReward.sprite = sprite_List[4]; // Experience Icon
                }
                else if (eventDataInfo[0] == "2")
                {

                    img_ImageReward.sprite = CapsuleManager.instance.capsules[int.Parse(eventDataInfo[3])].sprite;
                    //Trouver methode pour r�cup�rer le sprite du sort obtenu
                }
                else if (eventDataInfo[0] == "3")
                {
                    img_ImageReward.sprite = sprite_List[5]; //Health Quarter icon
                }
                txt_ObjectifDescription.text = eventDataInfo[1];
                txt_RewardDescription.text = eventDataInfo[2] + "x";
                //img_progressionBar.fillAmount = float.Parse(eventDataInfo[4]);
            }


            float nearest = Vector3.Distance(transform.position, col[0].transform.position);
            if (col.Length > 1)
            {
                for (int i = 0; i < col.Length; i++)
                {
                    float newDistance = Vector3.Distance(transform.position, col[i].transform.position);
                    if (nearest >= newDistance)
                    {
                        currentInteractibleObject = col[i].transform.gameObject;
                        m_socleTransform = GameObject.Find("low_Socle").transform;
                        nearest = newDistance;
                    }
                }
            }

        }
        else if (col.Length == 0 && currentInteractibleObject != null)
        {
            currentInteractibleObject = null;
            m_socleTransform = null;
            StartCoroutine(CloseUIWithDelay(2));
        }
    }

    public void ActionInteraction()
    {
        if (currentInteractibleObject != null)
        { 
            currentInteractibleObject.GetComponent<AltarBehaviorComponent>().ActiveEvent(this);
            currentInteractibleObjectActive = currentInteractibleObject;
            m_socleTransform = null;
            StartCoroutine(CloseUIWithDelay(2));
        }

    }

    public void NearTrader()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, rangeTrader, traderLayer);
        colliderProche = col;
        if (col.Length > 0)
        {
            if (ui_HintInteractionObject != null)
            {
                //ui_HintInteractionObject.SetActive(true);
                m_hintInteractionManager.activatePnjData(true);

            }
            for (int i = 0; i < col.Length; i++)
            {
                if (lastTrader != null)
                {
                    if (lastTrader.gameObject != col[i].gameObject)
                    {
                        lastTrader.SetBool("StandUp", false);
                        NewTrader(col[i].gameObject.GetComponent<Animator>());
                        m_lastHintAnimator.SetBool("InteractionOn", true);
                        txt_ObjectifDescriptionPnj.text = col[i].GetComponent<DataInteraction.InteractionData>().instructionOnActivation;

                    }
                }
                else
                {
                    NewTrader(col[i].gameObject.GetComponent<Animator>());
                    m_lastHintAnimator.SetBool("InteractionOn", true);
                    txt_ObjectifDescriptionPnj.text = col[i].GetComponent<DataInteraction.InteractionData>().instructionOnActivation;
                }

            }
        }
        else if (col.Length == 0 && lastTrader != null)
        {

            lastTrader.SetBool("StandUp", false);
            m_lastHintAnimator.SetBool("InteractionOn", false);
            txt_ObjectifDescription.text = "";
            txt_ObjectifDescriptionPnj.text = "";
            lastTrader = null;
            StartCoroutine(CloseUIWithDelay(2));
        }
    }

    public void NearArtefact()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, rangeArtefact, artefactLayer);
        colliderProche = col;
        if (col.Length > 0)
        {
            if (ui_HintInteractionObject != null)
            {
                //ui_HintInteractionObject.SetActive(true);
                m_hintInteractionManager.activeArtefactData(true);

            }
            for (int i = 0; i < col.Length; i++)
            {
                if (lastArtefact != null)
                {
                    if (lastArtefact.gameObject != col[i].gameObject)
                    {

                        NewArtefact(col[i].gameObject.GetComponent<ArtefactHolder>());
                        m_lastHintAnimator.SetBool("InteractionOn", true);
                        txt_ObjectifDescriptionPnj.text = lastArtefact.m_artefactsInfos.description;
                        m_lastArtefactAnimator.SetBool("PlayerProxi", true);

                    }
                }
                else
                {
                    NewArtefact(col[i].gameObject.GetComponent<ArtefactHolder>());
                    m_lastHintAnimator.SetBool("InteractionOn", true);
                    txt_ObjectifDescriptionPnj.text = lastArtefact.m_artefactsInfos.description;
                    m_lastArtefactAnimator.SetBool("PlayerProxi", true);
                }

            }
        }
        else if (col.Length == 0 && m_lastHintAnimator.GetBool("InteractionOn") && lastTrader == null && currentInteractibleObject == null)
        {
            lastArtefact = null;
            m_lastArtefactAnimator.SetBool("PlayerProxi", false);
            m_lastArtefactAnimator = null;
            txt_ObjectifDescription.text = "";
            txt_ObjectifDescriptionPnj.text = "";

            StartCoroutine(CloseUIWithDelay(2));
        }
    }

    public void NewTrader(Animator trader)
    {
        lastTrader = trader;
        GlobalSoundManager.PlayOneShot(39, transform.position);
        lastTrader.SetBool("StandUp", true);
    }

    public void NewArtefact(ArtefactHolder artefact)
    {
        lastArtefact = artefact;
        m_lastArtefactAnimator = artefact.transform.parent.GetComponentInChildren<Animator>();
    }
    public IEnumerator CloseUIWithDelay(float time)
    {
        m_lastHintAnimator.SetBool("InteractionOn", false);
        m_lastArtefactAnimator.SetBool("PlayerProxi", false);
        txt_ObjectifDescriptionPnj.text = "";
        yield return new WaitForSeconds(time);
        //ui_HintInteractionObject.SetActive(false);
        m_hintInteractionManager.activateAutelData(false);
        m_hintInteractionManager.activatePnjData(false);
        m_hintInteractionManager.activeArtefactData(false);
    }

    public void CalculateWorldPosition(GameObject objectToDisplayInfo)
    {
        Vector2 ViewportPosition = mainCamera.WorldToViewportPoint(objectToDisplayInfo.transform.position);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

        //now you can set the position of the ui element
        parentHintTransform.anchoredPosition = WorldObject_ScreenPosition + offsetPopUpDisplay;
    }
}
