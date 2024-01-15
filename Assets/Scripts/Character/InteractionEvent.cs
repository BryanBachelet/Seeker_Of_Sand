using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class InteractionEvent : MonoBehaviour
{
    [SerializeField] private float radiusInteraction;
    [SerializeField] private LayerMask InteractibleObject;

    private float lastInteractionCheck;
    public float intervalCheckInteraction;
    public GameObject currentInteractibleObject;

    public GameObject ui_HintInteractionObject;
    public Animator m_lastHintAnimator;

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
    public Collider[] collider;

    public HintInteractionManager m_hintInteractionManager;
    // Start is called before the first frame update
    void Start()
    {
        lastInteractionCheck = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > lastInteractionCheck + intervalCheckInteraction)
        {
            NearPossibleInteraction();
            NearTrader();
            lastInteractionCheck = Time.time;
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
                    //Trouver methode pour récupérer le sprite du sort obtenu
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
                        nearest = newDistance;
                    }
                }
            }

        }
        else if (col.Length == 0 && currentInteractibleObject != null)
        {
            currentInteractibleObject = null;
            StartCoroutine(CloseUIWithDelay(2));
        }
    }

    public void ActionInteraction()
    {
        if (currentInteractibleObject != null) { currentInteractibleObject.GetComponent<AltarBehaviorComponent>().ActiveEvent(); }

    }

    public void NearTrader()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, rangeTrader, traderLayer);
        collider = col;
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
            lastTrader = null;
            StartCoroutine(CloseUIWithDelay(2));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.localPosition, traderLayer);
    }

    public void NewTrader(Animator trader)
    {
        lastTrader = trader;
        GlobalSoundManager.PlayOneShot(39, transform.position);
        lastTrader.SetBool("StandUp", true);
    }

    public IEnumerator CloseUIWithDelay(float time)
    {
        m_lastHintAnimator.SetBool("InteractionOn", false);
        yield return new WaitForSeconds(time);
        //ui_HintInteractionObject.SetActive(false);
        m_hintInteractionManager.activateAutelData(false);
        m_hintInteractionManager.activatePnjData(false);
    }


}
