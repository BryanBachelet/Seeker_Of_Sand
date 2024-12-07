using GuerhoubaGames.GameEnum;
using SeekerOfSand.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingArea : MonoBehaviour
{
    [SerializeField] public GameElement element;
    [SerializeField] private GameObject[] capacityGameObject = new GameObject[5]; //Correspond à la zone et sa capacité. N'implique que la partie "offensive" de l'event
    private GameObject instantiatedArea;

    [SerializeField] private GameObject[] objectInstantiate; //Correspond à l'objet spawn pour la difficulté (Laser, Eclaire, bulle, mur)

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

    private bool test = true;
    // Start is called before the first frame update
    void Start()
    {
        SelectElement(element);
    }

    // Update is called once per frame
    void Update()
    {


        

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

    public void SelectElement(GameElement element)
    {
        instantiatedArea = Instantiate(capacityGameObject[GeneralTools.GetElementalArrayIndex(element)], this.transform);
        Debug.Log("Active area : [" + element.ToString() + "]");
    }
}
