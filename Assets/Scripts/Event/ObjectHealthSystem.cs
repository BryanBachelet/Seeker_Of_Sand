using GuerhoubaGames;
using GuerhoubaGames.Character;
using UnityEngine;
using UnityEngine.UI;

public enum EventObjectState
{
    Active,
    Deactive,
    Death,
}

public class ObjectHealthSystem :MonoBehaviour, IDamageReceiver
{

    public string nameObject;
    public int maxLife;
    public HealthSystem healthSystem;
    public HealthManager healthManager;
    public float ratioLife;

    [SerializeField] private float m_invicibleDuration;
    [SerializeField] private bool m_isInvicible;

    private float m_invicibleTimer;
    private ObjectState state = new ObjectState();
    public EventObjectState eventState = EventObjectState.Deactive;
    public Image m_eventLifeUIFeedback;
    public GameObject m_eventLifeUIFeedbackObj;
    public TMPro.TMP_Text m_eventProgressUIFeedback;
    public Image m_eventProgressionSlider;

    public LayerMask enemyLayer;
    public float rangeDegatAugmente;
    public AnimationCurve evolutionDegatAugment;
    public int indexUIEvent;

    public AnimationCurve maxHealthEvolution;

    public Animator animatorAssociated;

    public Material _matAssociated;
    public MeshRenderer m_meshRender;

    //public Vector3 offsetDamage = new Vector3(0, 5, 0);
    public Vector3 offset_DisplayDamage = new Vector3(0, 5, 0);

    private SpawnerBehavior spawnerbehavior; //A RETIRER A TERME, N'A RIEN A FAIRE LA
    public GameObject miniMap_Icon;
    private SpriteRenderer spriteRenderer;


    private AfflictionManager m_afflictionManager;
    [SerializeField] private bool m_isObjectifEvent;
    private EntityModifier m_entityModifier;

    private void Start()
    {

      
        GameState.AddObject(state);
        healthManager = GameState.m_enemyManager.GetComponent<HealthManager>();
        healthSystem = new HealthSystem();
        maxLife = (int)maxHealthEvolution.Evaluate(healthManager.characterShoot.GetComponent<CharacterUpgrade>().avatarUpgradeList.Count);
        healthSystem.Setup(maxLife);
        m_afflictionManager = GetComponent<AfflictionManager>();
        m_entityModifier = GetComponent<EntityModifier>();
        if (miniMap_Icon) miniMap_Icon.SetActive(true);
        if(miniMap_Icon) spriteRenderer = miniMap_Icon.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = Color.red;
        if (m_meshRender != null) _matAssociated = m_meshRender.material;
    }

    public void Update()
    {
        InvicibleCountdown();
    }

    public void ReceiveDamage(string nameDamage, DamageStatData damageStat, Vector3 direction, float power, int element, int additionnalDamage)
    {
        float allDamage = (damageStat.damage + additionnalDamage) * m_entityModifier.GetDamageIncreasePercent();

        damageStat.damage = Mathf.RoundToInt(allDamage);
        healthSystem.ChangeCurrentHealth(-allDamage);
        GameStats.instance.AddDamageSource(nameDamage, damageStat);
        animatorAssociated.SetTrigger("TakeHit");
        m_invicibleTimer = 0;
        GlobalSoundManager.PlayOneShot(32, transform.position);
        ratioLife = healthSystem.percentHealth;
        // VfX feedback
        Vector3 positionOnScreen = transform.position + offset_DisplayDamage;
        healthManager.CallDamageEvent(positionOnScreen, allDamage, element);

        m_isInvicible = true;
        m_invicibleTimer = 0.0f;
        if (this.GetComponent<SpawnerBehavior>() != null)
        {
            spawnerbehavior = this.GetComponent<SpawnerBehavior>();
            spawnerbehavior.UpdatePulse(ratioLife);
        }
        if (healthSystem.health > 0) return;

        eventState = EventObjectState.Death;
        if (miniMap_Icon) miniMap_Icon.SetActive(false);
        spriteRenderer.color = new Color(0.35f,0.26f,0.26f);
        animatorAssociated.SetBool("ActiveEvent", false);
        if (this.GetComponent<SpawnerBehavior>() != null) 
        {
            this.GetComponent<SpawnerBehavior>().SendSpawnerDesactivation();
        }

        GlobalSoundManager.PlayOneShot(33, transform.position);

    }


    public void ResetUIHealthBar()
    {
        if (m_eventLifeUIFeedback == null && m_eventLifeUIFeedbackObj == null) return;

        m_eventLifeUIFeedback.fillAmount = 1;
        m_eventLifeUIFeedbackObj.gameObject.SetActive(false);
        m_eventLifeUIFeedbackObj = null;
        m_eventLifeUIFeedback = null;

    }

    private void InvicibleCountdown()
    {
        if (m_invicibleTimer > m_invicibleDuration)
        {
            m_isInvicible = false;
            animatorAssociated.ResetTrigger("TakeHit");
        }
        else
        {
            m_invicibleTimer += Time.deltaTime;
            if(_matAssociated)
            {
                _matAssociated.SetColor("_MainColor", Color.Lerp(new Color(0.55f, 0.17f, 0.17f), new Color(0.077f, 0.077f, 0.077f), m_invicibleTimer / m_invicibleDuration));
            }
           
        }
    }

    public void ChangeState(EventObjectState newState)
    {
        eventState = newState;
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        healthSystem.SetMaxHealth(newMaxHealth);
    }

    public void Setup(int maxHealth)
    {
        healthSystem.Setup(maxHealth);
    }
    public void ResetCurrentHealth()
    {
        healthSystem.ChangeCurrentHealth(healthSystem.maxHealth);
    }

    public bool IsEventActive()
    {
        return eventState == EventObjectState.Active;
    }

    public void CheckEnemyArround()
    {
        Collider[] colProch = Physics.OverlapSphere(transform.position, rangeDegatAugmente, enemyLayer);
        m_invicibleDuration = evolutionDegatAugment.Evaluate(colProch.Length / 250);
    }

    public float GetLifeRatio()
    {
        return healthSystem.percentHealth;
    }

    public string GetName()
    {
       return nameObject;
    }

    public AfflictionManager GetAfflictionManager()
    {
        return m_afflictionManager;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public bool IsDead()
    {
        return eventState != EventObjectState.Active;
    }
    public int GetLastingLife()
    {
        return (int)healthSystem.health;
    }

    public bool IsObjectifTarget()
    {
        return m_isObjectifEvent;
    }
}
