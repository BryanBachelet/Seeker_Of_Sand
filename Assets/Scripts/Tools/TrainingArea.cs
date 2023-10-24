using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingArea : MonoBehaviour
{
    public GameObject[] attack;
    public Texture[] AreaFeedback;
    public float[] cdAttack;
    public float[] tempsEcouleLastAttack;
    [Range(0.01f, 1)]
    public float[] predictionPercent;
    public float[] tempsRealese;
    public float[] RangeAttack;

    public float densityAttack;
    private float attackCount;

    private float tempsEcoule;

    public List<GameObject> attackEnCour = new List<GameObject>();

    public bool playerHere = false;
    public Transform playerPosition;
    private Vector3 imprecision;
    public Vector3 playerRigidBodyVelocity;
    private Rigidbody playerRb;

    public float imprecisionLevel;

    [Range(1,5)]
    public int difficulty;
    [Range(1,10)]
    public int numberByDifficulty;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!playerHere) { return; }
        tempsEcoule = Time.time;
        attackCount = attackEnCour.Count;
        if (attackCount >= densityAttack) { return; }

        for(int i = 0; i < cdAttack.Length; i++)
        {
            if(tempsEcoule > tempsEcouleLastAttack[i] + cdAttack[i])
            {
                LaunchAttack(i);
                tempsEcouleLastAttack[i] = tempsEcoule;
            }
        }
    }

    public void LaunchAttack(int indexAttack)
    {
        //Debug.Log( "spawn this prefab : " + attack[indexAttack]);
        int variationQuantity = Random.Range(-difficulty, difficulty);
        for(int i = 0; i < numberByDifficulty + variationQuantity; i++)
        {
            GameObject attackInstiate = Instantiate(attack[indexAttack], foundNewPosition() + (playerRigidBodyVelocity * predictionPercent[indexAttack]), transform.rotation, transform);
            //Debug.Log("Attack spawned : " + attackInstiate.name);
            AttackTrainingArea dataLife = attackInstiate.GetComponent<AttackTrainingArea>();
            dataLife.lifeTimeVFX = tempsRealese[indexAttack];
            dataLife.playerTarget = playerPosition;
            dataLife.rangeHit = RangeAttack[indexAttack];
            attackEnCour.Add(attackInstiate);
            StartCoroutine(DestroyAfterDelay(6, attackInstiate));
        }

    }

    public IEnumerator DestroyAfterDelay(float time, GameObject attackCreated)
    {
        yield return new WaitForSeconds(time);
        attackEnCour.Remove(attackCreated);
    }

    public Vector3 foundNewPosition()
    {
        float posX = Random.Range(-imprecisionLevel, imprecisionLevel);
        float posZ = Random.Range(-imprecisionLevel, imprecisionLevel);
        imprecision = playerPosition.position + new Vector3(posX, 0, posZ);
        return imprecision;
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
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
        if(other.tag == "Player")
        {
            if(playerRb == null)
            {
                playerRb = other.GetComponent<Rigidbody>();
            }
        }
    }

}
