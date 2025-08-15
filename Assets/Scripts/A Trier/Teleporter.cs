using GuerhoubaGames.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public int TeleporterNumber;
    public bool usedTeleporter = false;

    [HideInInspector] public bool isReceiver = true;
    [HideInInspector] public bool isSpawn;
    [HideInInspector] public bool activation = false;
    [HideInInspector] public bool teleportorIsActive = false;
    [HideInInspector] public Material socleMaterial;

    static private TerrainGenerator terrainGen;
    [HideInInspector] public AltarBehaviorComponent altarBehavior;
    [HideInInspector] public EnemyManager enemyManager;

    [HideInInspector] public TeleporterFeebackController tpFeedbackController;
    [HideInInspector] public Animator animatorPortal;

    private Animator m_animator;
    static private CameraFadeFunction m_cameraFadeFonction;
    // Start is called before the first frame update
    void Awake()
    {
        if (socleMaterial == null) socleMaterial = this.GetComponentInChildren<MeshRenderer>().material;
        if (m_animator == null) m_animator = this.GetComponent<Animator>();
        if (terrainGen == null)
        {
            terrainGen = GameObject.Find("9-TerrainGenerator").GetComponent<TerrainGenerator>();
        }
        tpFeedbackController = gameObject.GetComponentInChildren<TeleporterFeebackController>();
        if (m_cameraFadeFonction == null) m_cameraFadeFonction = Camera.main.GetComponent<CameraFadeFunction>();
    }


    public void ActivationTeleportor()
    {
        teleportorIsActive = true;
        tpFeedbackController.activeChange = true;
        if (!tpFeedbackController.gameObject.activeSelf) 
            tpFeedbackController.gameObject.SetActive(true);
        m_animator.SetBool("Open", true);
        if (socleMaterial == null)
            socleMaterial = this.GetComponentInChildren<MeshRenderer>().material;

        socleMaterial.SetFloat("_TEXMCOLINT", 50f);

    }

    public void DesactivationTeleportor()
    {
        teleportorIsActive = false;
        m_animator.SetBool("Open", false);
        if (socleMaterial == null)
            socleMaterial = this.GetComponentInChildren<MeshRenderer>().material;
        socleMaterial.SetFloat("_TEXMCOLINT", -200f);

    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && teleportorIsActive && !isReceiver)
        {
            if (m_cameraFadeFonction && m_cameraFadeFonction.renderCam)
            {
                RenderTexture cuustomTexture = m_cameraFadeFonction.renderCam.targetTexture;
                terrainGen.GetTexturePreviousMap(cuustomTexture);
            }
            m_animator.SetTrigger("PlayerEnter");
            enemyManager.DestroyAllEnemy();
            terrainGen.SelectTerrain(TeleporterNumber);


            usedTeleporter = true;



        }
    }
}
