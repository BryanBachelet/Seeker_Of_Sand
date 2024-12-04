using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Capacity_Eclaire : MonoBehaviour
{
    [HideInInspector] public TrainingArea areaRef;

    public GameObject shotVfx;
    public GameObject[] attack;
    public float[] cdAttack;
    public float[] tempsEcouleLastAttack;
    [Range(0.01f, 1)]
    public float[] predictionPercent;
    public float[] tempsRealese;
    public float[] RangeAttack;
    private bool[] attackCheck = new bool[3];
    private int[] numberOfAttackByTypeToLaunch = new int[3];
    private int[] numberOfAttackByTypeCounter = new int[3];

    public float densityAttack;
    private float attackCount;

    private float tempsEcoule;

    public List<GameObject> attackEnCour = new List<GameObject>();
    public int[] attackEncourCount = new int[3];

    public bool playerHere = false;
    public Transform playerPosition;
    private Vector3 imprecision;
    public Vector3 playerRigidBodyVelocity;
    private Rigidbody playerRb;

    public float imprecisionLevel;
    public float delayBtwnSingleAttack = 0.2f;
    private float tempsEcouleLastSingleAttack = 0;

    public GameObject altarAssociated;
    public Vector3 offSetSign;
    // Start is called before the first frame update
    void Start()
    {
        areaRef = transform.parent.GetComponent<TrainingArea>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerHere) { return; }
        tempsEcoule = Time.time;
        attackCount = attackEnCour.Count;


        for (int i = 0; i < cdAttack.Length; i++)
        {
            if (attackEncourCount[i] >= densityAttack) { return; }
            if (tempsEcoule > tempsEcouleLastAttack[i] + cdAttack[i])
            {
                LaunchAttack(i);
                tempsEcouleLastAttack[i] = tempsEcoule;
            }
            if (attackCheck[i] == true)
            {
                if (numberOfAttackByTypeCounter[i] < numberOfAttackByTypeToLaunch[i])
                {
                    if (tempsEcouleLastSingleAttack > 0)
                    {
                        tempsEcouleLastSingleAttack -= Time.deltaTime;
                    }
                    else
                    {
                        Vector3 position = Vector3.zero;
                        Vector3 direction = playerPosition.position - transform.position;
                        direction.Normalize();
                        if (i == 0 && attackEncourCount[i] <= 1)
                        {
                            //transform.LookAt(-new Vector3(playerPosition.position.x, this.transform.position.y, playerPosition.position.z));
                        }
                        if (i == 0)
                        {
                            position = new Vector3(direction.x * 20 * attackEncourCount[i], direction.y * 0, direction.z * 0);
                        }
                        else if (i == 1)
                        {
                            position = new Vector3(direction.x * -20 * attackEncourCount[i], direction.y * 0, direction.z * 0);
                        }
                        else if(i == 2)
                        {
                            position = new Vector3(direction.x * 0, direction.y * 0, direction.z * 20 * attackEncourCount[i]);
                        }


                        GameObject attackInstiate = Instantiate(attack[i], foundNewPosition(areaRef.altarAssociated.transform.position + offSetSign) + position, transform.rotation, transform);
                        GameObject vfx = Instantiate(shotVfx, areaRef.altarAssociated.transform.position + offSetSign, transform.rotation);
                        float speed = Vector3.Distance(vfx.transform.position, attackInstiate.transform.position) / tempsRealese[i];
                        SignAttack signAttack_Projectil = vfx.GetComponent<SignAttack>();
                        signAttack_Projectil.positionToGo = attackInstiate.transform.position;
                        signAttack_Projectil.speedMovement = speed;
                        //Debug.Log("Attack spawned : " + attackInstiate.name + " Life time is : " + tempsRealese[i] + " distance with player is : " + distance + " Speed will be : " + (distance / tempsRealese[i]));
                        AttackTrainingArea dataLife = attackInstiate.GetComponent<AttackTrainingArea>();
                        dataLife.lifeTimeVFX = tempsRealese[i];
                        dataLife.playerTarget = playerPosition;
                        dataLife.rangeHit = RangeAttack[i];
                        attackEnCour.Add(attackInstiate);
                        attackEncourCount[i]++;
                        StartCoroutine(DestroyAfterDelay(tempsRealese[i] + 1, attackInstiate, i));
                        numberOfAttackByTypeCounter[i] += 1;
                        tempsEcouleLastSingleAttack = delayBtwnSingleAttack;
                    }
                }
                else
                {
                    attackCheck[i] = false;

                }

            }
        }


    }

    public void LaunchAttack(int indexAttack)
    {
        attackCheck[indexAttack] = true;
        numberOfAttackByTypeToLaunch[indexAttack] = (int)densityAttack;
        numberOfAttackByTypeCounter[indexAttack] = 0;

    }

    public IEnumerator DestroyAfterDelay(float time, GameObject attackCreated, int indexAttack)
    {
        yield return new WaitForSeconds(time);
        attackEnCour.Remove(attackCreated);
        attackEncourCount[indexAttack]--;
    }

    public Vector3 foundNewPosition(Vector3 positionRef)
    {
        float posX = Random.Range(-imprecisionLevel, imprecisionLevel);
        float posZ = Random.Range(-imprecisionLevel, imprecisionLevel);
        imprecision = positionRef + new Vector3(posX, 0, posZ);
        return imprecision;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            playerHere = true;

            playerPosition = other.transform;

            playerRigidBodyVelocity = playerRb.velocity;

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerHere = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (playerRb == null)
            {
                playerRb = other.GetComponent<Rigidbody>();
            }
        }
    }

}
