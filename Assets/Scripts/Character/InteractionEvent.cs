using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using GuerhoubaGames.Resources;
using GuerhoubaGames.UI;


/// <summary>
/// Manage Player interaction with the world environement 
/// </summary>
public class InteractionEvent : MonoBehaviour
{
    [SerializeField] private float radiusInteraction;
    [HideInInspector] private GameLayer m_gameLayer;
    [HideInInspector] private float lastInteractionCheck = 0.5f;
    [HideInInspector] private float intervalCheckInteraction = 0.2f;
    [SerializeField] private GameObject currentInteractibleObject;
    private Transform m_socleTransform;

    [HideInInspector] public Animator m_lastHintAnimator;
    [HideInInspector] public Animator m_lastArtefactAnimator;

    [HideInInspector] private TMP_Text txt_ObjectifDescriptionPnj;

    private Animator lastTrader;

    [HideInInspector] public ArtefactHolder lastArtefact;
    [SerializeField] private RectTransform CanvasRect;

    [HideInInspector] private Collider[] colliderProche;

    [HideInInspector] private HintInteractionManager m_hintInteractionManager;

    [HideInInspector] private Camera mainCamera;
    private InteractionInterface currentInteractionInterface;
    private InteractionInterface saveInteractionInterface;

    [HideInInspector] public GameObject currentInteractibleObjectActive = null;

    [SerializeField] private Animator bandeDiscussion;
    [SerializeField] private GameObject hintInputInteraction;
    [SerializeField] private TMP_Text interactionAction_Txt;
    [SerializeField] private GameObject hintInputInteraction_Detail;
    [HideInInspector] private TMP_Text interactionDetail_Txt;
    [SerializeField] private Animator hintAnimator;

    [SerializeField] private GameObject costCristal;
    ///[SerializeField] private TMP_Text costText;

    [HideInInspector] private GameResources m_gameRessources;

    private Character.CharacterMouvement m_characterMouvement;
    private CristalInventory m_CristalInventory;

    private GameObject hintDetailCost;

    private bool m_hasNewInteractibleElement;
    private bool m_hasInteractibleElementNear;

    #region Unity Functions
    // Start is called before the first frame update
    void Start()
    {
        lastInteractionCheck = 0;
        m_characterMouvement = GetComponent<Character.CharacterMouvement>();
        if (m_gameLayer == null) { m_gameLayer = GameLayer.instance; }
        if (mainCamera == null) { mainCamera = Camera.main; }
        interactionDetail_Txt = hintInputInteraction_Detail.GetComponentInChildren<TMP_Text>();
        m_gameRessources = GameResources.instance;
        m_CristalInventory = this.gameObject.GetComponent<CristalInventory>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > lastInteractionCheck + intervalCheckInteraction)
        {
            Collider[] col = Physics.OverlapSphere(transform.position, radiusInteraction, m_gameLayer.interactibleLayerMask);
            FindInteractiveElementAround(col);

            if (currentInteractionInterface && m_hasNewInteractibleElement && currentInteractionInterface.isInteractable)
            {
                UpdateInteraction();
            }

            if (col.Length == 0 && currentInteractionInterface || currentInteractionInterface && !currentInteractionInterface.isInteractable || currentInteractionInterface == null && hintInputInteraction.activeSelf)
            {
                RemoveInteraction();
                currentInteractionInterface = null;
            }
            lastInteractionCheck = Time.time;
        }
    }
    #endregion

    #region Finding Functions
    public void FindInteractiveElementAround(Collider[] col)
    {
        if (col.Length == 0)
        {
            return;
        }

        GameObject interactiveObject = FindClosestElement(col);

        if (interactiveObject == null)
        {
            return;
        }

        InteractionInterface interactionInterface = interactiveObject.GetComponent<InteractionInterface>();

        if (currentInteractionInterface != interactionInterface) m_hasNewInteractibleElement = true;
        currentInteractibleObject = interactiveObject;
        currentInteractionInterface = interactionInterface;
    }

    public void UpdateInteraction()
    {
        m_hasNewInteractibleElement = false;
        interactionAction_Txt.text = currentInteractionInterface.verbeInteraction;
        hintInputInteraction.SetActive(true);
        if (currentInteractionInterface.hasAdditionalDescription)
        {
            hintInputInteraction_Detail.SetActive(true);
            hintAnimator.SetBool("OpenHintDetail", true);
            interactionDetail_Txt.text = currentInteractionInterface.additionalDescription;
            if (currentInteractionInterface.cost > 0)
            {
                if (hintDetailCost != null) { Destroy(hintDetailCost); }

                hintDetailCost = Instantiate(m_gameRessources.costPrefab[currentInteractionInterface.cristalID], costCristal.transform.position, costCristal.transform.rotation, costCristal.transform);
                TMP_Text textCost = hintDetailCost.GetComponentInChildren<TMP_Text>();
                if (currentInteractionInterface.cristalID < 4) //On regarde la nature du cristal (si < 4, le cristal est élémentaire
                {
                    if (currentInteractionInterface.cost > m_CristalInventory.cristalCount[currentInteractionInterface.cristalID]) { textCost.color = Color.red; }
                    else { textCost.color = Color.white; }
                }
                else if (currentInteractionInterface.cristalID == 4) //On regarde la nature du cristal (si == 4, le cristal est dissonance
                {
                    if (currentInteractionInterface.cost > m_CristalInventory.dissonanceCout) { textCost.color = Color.red; }
                    else { textCost.color = Color.white; }
                }
                textCost.text = "x " + currentInteractionInterface.cost;
            }
        }

        Selection_Feedback selection = currentInteractibleObject.GetComponent<Selection_Feedback>();
        if (currentInteractibleObject.GetComponent<AltarBehaviorComponent>())
        {
            if (selection != null && !currentInteractibleObject.GetComponent<AltarBehaviorComponent>().hasBeenActivate) { selection.ChangeLayerToSelection(); }
        }
        else
        {
            if (selection != null)
            {
                selection.ChangeLayerToSelection();
            }
        }
    }

    private void RemoveInteraction()
    {
        if (hintInputInteraction.activeSelf)
        {
            interactionAction_Txt.text = "";

            if (currentInteractionInterface && !currentInteractionInterface.hasAdditionalDescription)
            {

                hintAnimator.SetBool("OpenHintDetail", false);
                interactionDetail_Txt.text = "";
            }
            hintInputInteraction_Detail.SetActive(false);
            hintInputInteraction.SetActive(false);
        }
        //bandeDiscussion.SetBool("NearNPC", false);
        m_hasNewInteractibleElement = false;
        m_hasInteractibleElementNear = false;

        if (currentInteractibleObject == null) return;
        Selection_Feedback selection = currentInteractibleObject.GetComponent<Selection_Feedback>();
        if (selection != null) { selection.ChangeLayerToDefault(); }
        currentInteractibleObject = null;
        m_socleTransform = null;
    }


    private GameObject FindClosestElement(Collider[] col)
    {
        if (col.Length == 1)
        {
            if (col[0].GetComponent<InteractionInterface>().isInteractable)
                return col[0].gameObject;
            else return null;
        }

        int colliderIndex = 0;
        float distance = 1000;
        for (int i = 0; i < col.Length; i++)
        {
            float tempDistance = Vector3.Distance(transform.position, col[i].transform.position);
            InteractionInterface interactionInterface = col[i].GetComponent<InteractionInterface>();
            if (interactionInterface == null)
            {
                Debug.LogWarning("This object " + interactionInterface.name + " don't have an InteractionInterface component");
                continue;
            }
            if (tempDistance < distance)
            {
                distance = tempDistance;
                colliderIndex = i;
            }
        }

        return col[colliderIndex].gameObject;
    }

    public void NearPossibleInteraction(Collider[] col)
    {

        if (col.Length > 0)
        {
            if (!hintInputInteraction.activeSelf)
            {
                interactionAction_Txt.text = currentInteractionInterface.verbeInteraction;
                hintInputInteraction.SetActive(true);
                if (currentInteractionInterface.hasAdditionalDescription)
                {
                    hintInputInteraction_Detail.SetActive(true);
                    hintAnimator.SetBool("OpenHintDetail", true);
                    interactionDetail_Txt.text = currentInteractionInterface.additionalDescription;
                    if (currentInteractionInterface.cost > 0)
                    {
                        if (hintDetailCost != null) { Destroy(hintDetailCost); }

                        hintDetailCost = Instantiate(m_gameRessources.costPrefab[currentInteractionInterface.cristalID], costCristal.transform.position, costCristal.transform.rotation, costCristal.transform);
                        TMP_Text textCost = hintDetailCost.GetComponentInChildren<TMP_Text>();
                        if (currentInteractionInterface.cristalID < 4) //On regarde la nature du cristal (si < 4, le cristal est élémentaire
                        {
                            if (currentInteractionInterface.cost > m_CristalInventory.cristalCount[currentInteractionInterface.cristalID]) { textCost.color = Color.red; }
                            else { textCost.color = Color.white; }
                        }
                        else if (currentInteractionInterface.cristalID == 4) //On regarde la nature du cristal (si == 4, le cristal est dissonance
                        {
                            if (currentInteractionInterface.cost > m_CristalInventory.dissonanceCout) { textCost.color = Color.red; }
                            else { textCost.color = Color.white; }
                        }
                        textCost.text = "x " + currentInteractionInterface.cost;
                    }
                }
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
                        GameObject socle = GameObject.Find("low_Socle");
                        if (socle) m_socleTransform = socle.transform;
                        nearest = newDistance;
                    }
                }
            }

            Selection_Feedback selection = currentInteractibleObject.GetComponent<Selection_Feedback>();
            if (currentInteractibleObject.GetComponent<AltarBehaviorComponent>())
            {
                if (selection != null && !currentInteractibleObject.GetComponent<AltarBehaviorComponent>().hasBeenActivate) { selection.ChangeLayerToSelection(); }
            }
            else
            {
                if (selection != null)
                {
                    selection.ChangeLayerToSelection();
                }
            }


        }
        else if (col.Length == 0 && currentInteractibleObject != null)
        {
            if (hintInputInteraction.activeSelf)
            {
                interactionAction_Txt.text = "";

                if (!currentInteractionInterface.hasAdditionalDescription)
                {

                    hintAnimator.SetBool("OpenHintDetail", false);
                    interactionDetail_Txt.text = "";
                }
                hintInputInteraction_Detail.SetActive(false);
                hintInputInteraction.SetActive(false);
            }
            //bandeDiscussion.SetBool("NearNPC", false);
            Selection_Feedback selection = currentInteractibleObject.GetComponent<Selection_Feedback>();
            if (selection != null) { selection.ChangeLayerToDefault(); }
            currentInteractibleObject = null;
            m_socleTransform = null;
            //StartCoroutine(CloseUIWithDelay(2));
        }
    }

    #endregion

    #region Input Functions

    public void ActionInteraction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {

            if (!GameState.IsPlaying())
                return;

            if (currentInteractionInterface != null)
            {
                currentInteractionInterface.CallOpenInteraction(this.gameObject);

                if (currentInteractionInterface.hasClosePhase)
                {
                    saveInteractionInterface = currentInteractionInterface;
                }
                return;
            }

        }
        m_characterMouvement.SlideInput(ctx);

    }

    public void CancelInteraction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {

            if (currentInteractionInterface != null)
            {
                currentInteractionInterface.CallCloseInteraction(this.gameObject);
                return;
            }
            if (saveInteractionInterface != null)
            {
                saveInteractionInterface.CallCloseInteraction(this.gameObject);
                saveInteractionInterface = null;
                return;
            }

        }
    }

    public void GeneralInputInteraction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {


            if (currentInteractionInterface != null && !currentInteractionInterface.isOpen)
            {
                if (!GameState.IsPlaying())
                    return;
                currentInteractionInterface.CallOpenInteraction(this.gameObject);
                if (currentInteractionInterface.hasClosePhase)
                {
                    saveInteractionInterface = currentInteractionInterface;
                }
                return;
            }


            if (currentInteractionInterface != null)
            {
                currentInteractionInterface.CallCloseInteraction(this.gameObject);
                return;
            }
            if (saveInteractionInterface != null)
            {
                saveInteractionInterface.CallCloseInteraction(this.gameObject);
                saveInteractionInterface = null;
                return;
            }

        }
    }

    #endregion

    #region Near Functions
    public void NearTrader()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, radiusInteraction, m_gameLayer.traderLayerMask);
        colliderProche = col;
        if (col.Length > 0)
        {
            bandeDiscussion.SetBool("NearNPC", true);
            /*
            if (!hintInputInteraction.activeSelf)
            {
                hintInputInteraction.SetActive(true);
            }
            if (ui_HintInteractionObject != null)
            {
                //ui_HintInteractionObject.SetActive(true);
                m_hintInteractionManager.ActivatePnjData(true);

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
            */
        }
        else if (col.Length == 0)
        {
            bandeDiscussion.SetBool("NearNPC", false);
            /*
            if (hintInputInteraction.activeSelf)
            {
                hintInputInteraction.SetActive(false);
            }*/
        }
    }

    // TODO : Delete 

    public void NearArtefact()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, radiusInteraction, m_gameLayer.artefactLayerMask);
        colliderProche = col;
        if (col.Length > 0 && TerrainGenerator.s_currentRoomManager.isRoomHasBeenValidate)
        {
            for (int i = 0; i < col.Length; i++)
            {
                if (lastArtefact != null)
                {
                    if (lastArtefact.gameObject != col[i].gameObject)
                    {

                        NewArtefact(col[i].gameObject.GetComponent<ArtefactHolder>());
                        //m_lastHintAnimator.SetBool("InteractionOn", true);
                        txt_ObjectifDescriptionPnj.text = lastArtefact.m_artefactsInfos.descriptionResult;
                        m_lastArtefactAnimator.SetBool("PlayerProxi", true);

                    }
                }
                else
                {
                    NewArtefact(col[i].gameObject.GetComponent<ArtefactHolder>());
                    //m_lastHintAnimator.SetBool("InteractionOn", true);
                    //txt_ObjectifDescriptionPnj.text = lastArtefact.m_artefactsInfos.description;
                    m_lastArtefactAnimator.SetBool("PlayerProxi", true);
                }

            }
        }
        else if (col.Length == 0 /*&& m_lastHintAnimator.GetBool("InteractionOn") */&& lastTrader == null && currentInteractibleObject == null)
        {
            lastArtefact = null;
            if (m_lastArtefactAnimator != null) { m_lastArtefactAnimator.SetBool("PlayerProxi", false); }

            m_lastArtefactAnimator = null;
            //txt_ObjectifDescription.text = "";
            //txt_ObjectifDescriptionPnj.text = "";

            //StartCoroutine(CloseUIWithDelay(2));
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
        m_lastArtefactAnimator = artefact.GetComponent<Animator>();
    }
    #endregion

    #region UI Functions
    public IEnumerator CloseUIWithDelay(float time)
    {
        ;
        if (m_lastArtefactAnimator != null) { m_lastArtefactAnimator.SetBool("PlayerProxi", false); }

        txt_ObjectifDescriptionPnj.text = "";
        yield return new WaitForSeconds(time);

        m_hintInteractionManager.ActivateAutelData(false);
        m_hintInteractionManager.ActivatePnjData(false);
        m_hintInteractionManager.ActiveArtefactData(false);
    }

    public void CalculateWorldPosition(GameObject objectToDisplayInfo)
    {
        Vector2 ViewportPosition = mainCamera.WorldToViewportPoint(objectToDisplayInfo.transform.position);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));


    }


    #endregion

}
