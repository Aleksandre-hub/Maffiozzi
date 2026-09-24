using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CivillianScript : MonoBehaviour
{

    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private float detectionRadius = 5.5f;
    [SerializeField] private float rotationSpeed = 7f;

    [SerializeField] private float changeDirectionInterval = 3f;
    private float timerTimer = 0f;
    private float randomRotationAngle;

    private Vector2 moveDirection;
    private float changeDirectionTimer = 0f;
    public float speed = 1f;

    private bool Captured = false;
    private GameObject Captor;

    [SerializeField] private float health = 10;

    private float regainSpeedTimer = 0;
    [SerializeField] private float regainSpeedInterval = 1.5f;

    [SerializeField] private SpriteRenderer mySpriteRenderer;
    private Color thisColor;

    [SerializeField] private Slider healthBar;

    // [SerializeField] private Animator movement;
    // private Vector3 previousPosition;


    // OnSpawn and OnDeath things
    [SerializeField] private ParticleSystem OnSPawn;
    [SerializeField] private ParticleSystem OnDeath;
    [SerializeField] private GameObject blood;
    [SerializeField] private GameObject deadCultist; 


    public int CivilianID = 33333;
    [SerializeField] private CivillianScript civillianScript;
    GameManager gameManager;

    [SerializeField] private GameObject playerPrefab;

    [SerializeField] GameObject MonsterMinion;
    private MonsterScript monsterScript;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.gameManagerInstance;
        gameManager.AddCivillianScripts(gameObject, civillianScript);

        thisColor = mySpriteRenderer.color;
        
    }

    // Update is called once per frame
    void Update()
    {

        healthBar.value = health/10;

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, detectionRadius, Vector2.zero, Mathf.Infinity, detectionLayer);


        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach (RaycastHit2D hit in hits)
        {

            if (hit.collider.gameObject == gameObject) { // Detecting Itself
                continue;
            }else if (hit.collider.tag == gameObject.tag) { // Detecting Ally
                continue;
            }else if (hit.collider.name.Contains("Guard")) { // ignore police guardsmen
                continue;
            }

            GameObject detectedObject = hit.collider.gameObject;

            // Calculate the distance between the detected object and the detecting object
            float distance = Vector2.Distance(transform.position, detectedObject.transform.position);

            // Check if the detected object is closer than the current closest object
            if (distance < closestDistance)
            {
                closestObject = detectedObject;
                closestDistance = distance;
            }
        }


        if (closestObject != null && !Captured) { // Run from Enemy

            // Calculate the direction vector from the detecting object to the closest object
            Vector2 direction = (closestObject.transform.position - transform.position).normalized;

            // Calculate the angle between the direction vector and the current forward direction of the detecting object
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Run away from enemy
            if (closestDistance <= detectionRadius && !closestObject.CompareTag("Police")) {
                
                // Smoothly rotate the detecting object towards the closest object
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle-270), rotationSpeed * Time.deltaTime);

                transform.Translate(Vector2.up * Time.deltaTime * speed);
            }else if (closestDistance <= detectionRadius && closestObject.CompareTag("Police")) {
                
                // Smoothly rotate the detecting object towards the closest object
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle-90), rotationSpeed * Time.deltaTime);

                transform.Translate(Vector2.up * Time.deltaTime * speed);
            }

        }else if (closestObject == null && !Captured) {
            

            // Update the change direction timer
            changeDirectionTimer += Time.deltaTime;
            
            // Check if it's time to change direction
            if (changeDirectionTimer >= changeDirectionInterval)
            {
            
                float timerTimerEnd = 2f;

                timerTimer += Time.deltaTime;
                if (timerTimer >= timerTimerEnd) {
                    randomRotationAngle = Random.Range(0, 360);
                    timerTimer = 0f;
                    changeDirectionTimer = 0f;
                }

                // Rotate the bot to face the direction it is moving
                RotateToFaceDirection();
            }else {
                transform.Translate(Vector2.up * Time.deltaTime * speed);
            }
        
        }

        if (Captured && Captor != null) {

            // Calculate the direction vector from the detecting object to the closest object
            Vector2 direction = (Captor.transform.position - transform.position).normalized;

            // Calculate the angle between the direction vector and the current forward direction of the detecting object
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Smoothly rotate the detecting object towards the closest object
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle-90), rotationSpeed * Time.deltaTime);

            float Distance = Vector2.Distance(transform.position, Captor.transform.position);

            // Follow the captor
            if (Distance >= 1.5f) {
                transform.Translate(Vector2.up * Time.deltaTime * speed);
            }else if (Distance <= 1f) {
                transform.Translate(Vector2.up * Time.deltaTime * -speed);
            }

        }else if (Captor == null || Captor.tag != gameObject.tag) {

            if (Captured) {
                foreach(RaycastHit2D hit in hits) {

                    var collider = hit.collider.gameObject;

                    if (hit.collider.name.Contains("Enemy") && gameManager.GiveScriptAccordingToGameObject(hit.collider.gameObject).ID == CivilianID) {
                        Captor = collider;

                        if (collider.name.Contains("Enemy")) {
                            gameManager.GiveScriptAccordingToGameObject(collider).CaptiveInfo(gameObject);
                        }else if (collider.CompareTag("Police") && !collider.name.Contains("Guard")) {
                            collider.GetComponent<PoliceScript>().CaptiveInfo(gameObject);
                        }

                    }else if (hit.collider.name.Contains("Player") && gameManager.WhatID("Player") == CivilianID) {
                        Captor = hit.collider.gameObject;
                    }
                }

                if (Captor == null) {
                    CivilianID = 33333;
                    Captured = false;
                    Captor = null;
                    gameObject.tag = "Civillian";
                }

            }else {
                CivilianID = 33333;
                Captured = false;
                Captor = null;
                gameObject.tag = "Civillian";
            }

        }

        if (health <= 1) {
            Instantiate(OnDeath, transform.position, Quaternion.identity);
            Instantiate(blood, gameObject.transform.position, gameObject.transform.rotation);
            var deadman = Instantiate(deadCultist, gameObject.transform.position, Quaternion.Euler(0, 0,transform.rotation.eulerAngles.z - 180f));
            deadman.GetComponent<SpriteRenderer>().color = Color.cyan;
            Destroy(gameObject);
        }

        if (speed <= 0.1) {
            speed = 0;

            regainSpeedTimer += Time.deltaTime;
            if (regainSpeedTimer >= regainSpeedInterval) {
                speed = 2;
                regainSpeedTimer = 0;
            }

        }


    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void RotateToFaceDirection()
    {

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, randomRotationAngle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!Captured && !other.gameObject.name.Contains("Divine") && !other.gameObject.name.Contains("Civillian") && !other.gameObject.name.Contains("Guard") && (other.gameObject.tag.Contains("Enemy") || other.gameObject.tag.Contains("Cultist") || other.gameObject.CompareTag("Police"))) {
            gameObject.tag = other.gameObject.tag;
            Captor = other.gameObject;
            health = 10;
            speed = 2;
            Captured = true;

            if (other.gameObject.name.Contains("Enemy")) {
                CivilianID = gameManager.GiveScriptAccordingToGameObject(other.gameObject).ID;
                other.gameObject.GetComponent<Enemy1>().CaptiveInfo(gameObject);
            }else if (other.gameObject.CompareTag("Police") && !other.gameObject.name.Contains("Guard")) {
                other.gameObject.GetComponent<PoliceScript>().CaptiveInfo(gameObject);
            }else if (other.gameObject.name.Contains("Player")) {
                CivilianID = gameManager.WhatID("Player");
            }

        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Contains("Bullet") && !other.tag.Contains("Police") && !other.name.Contains(CivilianID.ToString())) {
            
            if (gameObject.tag == "Civillian") {
                speed -= 0.4f;
            }
            
            StartCoroutine(GotHit());

            if (speed <= 0.5f) {
                health -= 0.5f;
            }else {
                health -= 1;
            }

        }

        if (other.CompareTag("Blood_Ponds") || other.CompareTag("Rescue_Place")) {

            if (!gameObject.CompareTag("Police") && !gameObject.CompareTag("Civillian") && other.CompareTag("Blood_Ponds")) {
                var Monster = Instantiate(MonsterMinion, transform.position, Quaternion.identity);
                Monster.tag = Captor.tag;
                monsterScript = Monster.GetComponent<MonsterScript>();
                monsterScript.SetMaster(Captor);
                monsterScript.GetBrand().color = mySpriteRenderer.color;

                gameManager.CountSacrificesPerID(CivilianID);

                if (Captor.CompareTag("Enemy1")) {
                    monsterScript.ID = gameManager.GiveScriptAccordingToGameObject(Captor).ID;

                }else {
                    monsterScript.ID = 1111111;
                }
                Destroy(gameObject);
            }else if (Captor != null && other.CompareTag("Rescue_Place") && Captor.CompareTag("Police")) {
                Captor.GetComponent<PoliceScript>().Raid = true;
            }

            Destroy(gameObject);
        }else if (other.CompareTag("Leader_ritual") && !gameObject.CompareTag("Police") && !gameObject.CompareTag("Civillian")) {

            if (gameObject.CompareTag("PlayerCultist") && Captor.name.Contains("Player")) {
                Captor.GetComponent<Player>().BecomeLeader();
                Destroy(gameObject);
            }else {
                var ownerScript = gameManager.GiveScriptAccordingToGameObject(Captor);
                ownerScript.Rank = "Leader";
                ownerScript.changedUniform = false;
                Destroy(gameObject);
            }

        }

    }

    private IEnumerator GotHit() {

        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        gameObject.GetComponent<SpriteRenderer>().color = thisColor;

    }

}
