using System.Security.Cryptography;
//using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Net.Sockets;
using System.Threading;
using Unity.VisualScripting;



//using System.Numerics;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VisualScripting.FullSerializer;

public class Enemy1 : MonoBehaviour
{
    
    public int ID;

    public System.Guid guid => throw new System.NotImplementedException();

    private Color thisColor;
    [SerializeField] private GameObject CloneAsset;
    private GameObject bulletsParent;
    private GameObject bulletsParentPlayer;
    private Enemy1 enemy1;
    private DivineCultist divine;
    private PoliceGuard pguard;
    private PoliceScript police;
    private MonsterScript monster;
    private CivillianScript civillian;
    private EnemyBullet bulletScript;
    private DivineCultist divineCultistScript;

    [SerializeField] private GameObject Bullet;
    [SerializeField] private float duration = 0.5f;
    private float timer = 0f;


    [SerializeField] private float health = 10;


    [SerializeField] private float speed = 1f;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private LayerMask detectionLayerOfObsticles;
    [SerializeField] private float rotationSpeed = 5f; // Adjust the rotation speed as needed

    
    [SerializeField] private float changeDirectionInterval = 3f;
    private float timerTimer = 0f;
    private float randomRotationAngle;

    private Vector2 moveDirection;
    private float changeDirectionTimer = 0f;

    private int LeadOrFollow;
    public string Rank;
    private Enemy1 closestlAllyScript;
    private Enemy1 leaderHitScript;
    [SerializeField] private bool LeaderDetected = false;
    [SerializeField] GameObject closestLeader = null;
    private float closestLeaderDistance;
    private SpriteRenderer thisObjectsSpriteRenderer;
    [SerializeField] private Sprite LeaderUniform;
    [SerializeField] private Sprite defaultSprite;

    [SerializeField] private Transform GunPoint;
    [SerializeField] private ParticleSystem ShootParticles;
    [SerializeField] private ParticleSystem GotHitParticles;
    [SerializeField] private ParticleSystem WalkingEffect;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Player playerScript;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Text factionNameText;

    private float KillersID = 0;

    public bool CapturedCivilian = false;
    private GameObject CapturedCivilianObject;

    public GameObject bloodPond;
    private GameObject[] blood_ponds;
    public bool changedUniform = false;


    public string FactionName;

    [SerializeField] private bool isClearLineOfSight = true;


    // OnSpawn and OnDeath things
    [SerializeField] private ParticleSystem OnSPawn;
    [SerializeField] private ParticleSystem OnDeath;
    [SerializeField] private GameObject blood;
    [SerializeField] private GameObject deadCultist; 

    // THE CHANGE TAG SYSTEM INTO ID SYSTEM

    
    private GameManager gameManager;
    [SerializeField] private Enemy1 ThisScript;

    // awake is called when your mom wakes up from her delusions
    void Awake()
    {
       
    }

    // Start is called before the first frame update
    void Start()
    {

        blood_ponds = GameObject.FindGameObjectsWithTag("Blood_Ponds");
        
        gameManager = GameManager.gameManagerInstance;
        gameManager.AddEnemiesAndScripts(gameObject, ThisScript);
        if (ID > -1 && ID < gameManager.CultistFactionLimit+1) {
            gameManager.CountBotsPerIDPlus(ID);
        }

        thisObjectsSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        thisColor = thisObjectsSpriteRenderer.color;

        // Set the initial move direction to a random direction
        moveDirection = Random.insideUnitCircle.normalized;

        randomRotationAngle = Random.Range(0, 360);

        LeadOrFollow = Random.Range(0, 100);

        if (LeadOrFollow > 80) {
            Rank = "Leader";
            thisObjectsSpriteRenderer.sprite = LeaderUniform;
        }else {
            Rank = "Follower";
            thisObjectsSpriteRenderer.sprite = defaultSprite;
        }

        playerScript = gameManager.playerScript;
        factionNameText.text = FactionName;

    }

    // Update is called once per frame
    void Update()
    {

        if (playerPrefab == null || playerScript == null) {
            GameObject.FindGameObjectWithTag("Player");
        }
        
        healthBar.value = health/10;

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, detectionRadius, Vector2.zero, Mathf.Infinity, detectionLayer);

        
        GameObject closestAllyObject = null;
        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity;
        float closestAllyDistance = Mathf.Infinity;

        float closestBloodPondDistance = Mathf.Infinity;
        
        foreach(GameObject pond in blood_ponds) {

            // Calculate the distance between the detected object and the detecting object
            float distance = Vector2.Distance(transform.position, pond.transform.position);

             // Check if the detected object is closer than the current closest object
            if (distance < closestBloodPondDistance)
            {
                bloodPond = pond;
                closestBloodPondDistance = distance;
            }
        }

        if (hits.Length == 1) {
            LeaderDetected = false;
        }

        if (Rank == "Follower" && !changedUniform) {
            gameObject.GetComponent<SpriteRenderer>().sprite = defaultSprite;
            changedUniform = true;
        }else if (Rank == "Leader" && !changedUniform) {
            gameObject.GetComponent<SpriteRenderer>().sprite = LeaderUniform;
            changedUniform = true;
        }

        if (closestLeader == null) {
            LeaderDetected = false;
        }

        foreach(RaycastHit2D hitL in hits) {

            if (Rank == "Leader" || closestLeader == null) {
                LeaderDetected = false;
                break;
            }else if (hitL.collider.gameObject == gameObject) {
                continue;
            }

            if (closestLeader == hitL.collider.gameObject) {
                LeaderDetected = true;
                break;
            }
        } 

        // Iterate through all hits and find the closest object
        foreach (RaycastHit2D hit in hits)
        {

            if (hit.collider.gameObject == gameObject) { // Detecting Enemy
                continue;
            }else if (hit.collider.tag == gameObject.tag && (hit.collider.name.Contains("Enemy") || hit.collider.name.Contains("Player")) ) { // Detecting Ally

                if (gameManager.ObjectsWithEnemyScript.ContainsKey(hit.collider.gameObject) && gameManager.GiveScriptAccordingToGameObject(hit.collider.gameObject).ID == ID) {
                    

                    GameObject detectedAllyObject = hit.collider.gameObject;

                    // Calculate the distance between the detected object and the detecting object
                    float distanceAlly = Vector2.Distance(transform.position, detectedAllyObject.transform.position);

                    // Check if the detected object is closer than the current closest object
                    if (distanceAlly < closestAllyDistance)
                    {
                        closestAllyObject = detectedAllyObject;
                        closestAllyDistance = distanceAlly;
                        closestlAllyScript = closestAllyObject.GetComponent<Enemy1>();
                    }

                    if (closestAllyObject.GetComponent<Enemy1>() != null && closestlAllyScript.Rank == "Leader") {
                        closestLeader = closestAllyObject;
                        closestLeaderDistance = closestAllyDistance;
                        LeaderDetected = true;
                    }

                    continue;

                }else if (hit.collider.name.Contains("Player")) {
                    
                    GameObject detectedAllyObject = hit.collider.gameObject;

                    // Calculate the distance between the detected object and the detecting object
                    float distanceAlly = Vector2.Distance(transform.position, detectedAllyObject.transform.position);

                    // Check if the detected object is closer than the current closest object
                    if (distanceAlly < closestAllyDistance)
                    {
                        closestAllyObject = detectedAllyObject;
                        closestAllyDistance = distanceAlly;
                    }

                    if (playerScript.Rank == "Leader") {
                        closestLeader = detectedAllyObject;
                        closestLeaderDistance = distanceAlly;
                        LeaderDetected = true;
                    }

                    continue;

                }

                
            }else if (hit.collider.tag == gameObject.tag && hit.collider.name.Contains("Divine")) {
                
                if (gameManager.GiveDivineScriptAccordingToGameObject(hit.collider.gameObject).ID == ID) {

                    GameObject detectedAllyObject = hit.collider.gameObject;

                    // Calculate the distance between the detected object and the detecting object
                    float distanceAlly = Vector2.Distance(transform.position, detectedAllyObject.transform.position);

                    // Check if the detected object is closer than the current closest object
                    if (distanceAlly < closestAllyDistance)
                    {
                        closestAllyObject = detectedAllyObject;
                        closestAllyDistance = distanceAlly;
                    }

                    continue;
                }

            }else if (hit.collider.name.Contains("Civillian") && gameManager.ObjectsWithCivillianScript.ContainsKey(hit.collider.gameObject) && gameManager.GiveCivillianScriptAccordingToGameObject(hit.collider.gameObject).CivilianID == ID) {
                continue;
            }else if (hit.collider.name.Contains("Monster") && ID == hit.collider.GetComponent<MonsterScript>().ID) {
                continue;
            }

            GameObject detectedObject = hit.collider.gameObject;

            // Calculate the distance between the detected object and the detecting object
            float distance = Vector2.Distance(transform.position, detectedObject.transform.position);

            // Perform a linear raycast to check if there's no object between the detecting object and the detected object
            RaycastHit2D[] linearHits = Physics2D.RaycastAll(transform.position, (detectedObject.transform.position - transform.position).normalized, distance, detectionLayerOfObsticles);

            isClearLineOfSight = true;


            foreach (RaycastHit2D linearHit in linearHits)
            {
                    // detected object is not the target                 detected object is not itself                  ally is not on the way of detecting target
                if (linearHit.collider.gameObject != detectedObject && linearHit.collider.gameObject != gameObject && gameManager.GetGameObjectsID(linearHit.collider.gameObject) != gameManager.GetGameObjectsID(gameObject))
                {
                    // If any object other than the detected object or detecting object is hit, there's an obstruction
                    isClearLineOfSight = false;
                    break;
                }
            }


            // foreach (RaycastHit2D linearHit in linearHits)
            // {
            //     if (linearHit.collider.gameObject == gameObject) {
            //         continue;
            //     }
            //     else if (linearHit.collider.gameObject == detectedObject) {
            //         isClearLineOfSight = true;
            //     }else {
            //         isClearLineOfSight = false;
            //         Debug.Log(gameObject.name + " from " + FactionName + " is saying that LinearHit collided with " + linearHit.collider.gameObject.name + " ID=" + gameManager.GetGameObjectsID(linearHit.collider.gameObject) + " but detected Object was: " + (detectedObject.name) + " ID=" + gameManager.GetGameObjectsID(detectedObject));
            //         //continue;
            //     }
            // }

            // Check if the detected object is closer than the current closest object
            if (distance < closestDistance && isClearLineOfSight)
            {
                closestObject = detectedObject;
                closestDistance = distance;
            }

        }

        // Bot moves away from ally when too close
        if (closestAllyObject != null && closestAllyDistance <= 0.7f) {
            Vector2 directionAlly = closestAllyObject.transform.position - transform.position;
            transform.Translate(-directionAlly * Time.deltaTime * speed);
        }

        if (!isClearLineOfSight) {
            //Debug.Log(gameObject.name + " from " + FactionName + " is saying that clearence of line of sight is: " + isClearLineOfSight);
        }
        

        if (closestObject != null && (closestBloodPondDistance >= 2 || (closestBloodPondDistance <= 2 && !CapturedCivilian)) && isClearLineOfSight) { // Follow and shoot the Enemy

            if (closestObject.CompareTag("Civillian")) {
                speed = 2.5f;
            }else {
                speed = 2f;
            }

            if (!isClearLineOfSight) {
                Debug.Log("Why come in??");
            }

            // Calculate the direction vector from the detecting object to the closest object
            Vector2 direction = (closestObject.transform.position - transform.position).normalized;

            // Calculate the angle between the direction vector and the current forward direction of the detecting object
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Smoothly rotate the detecting object towards the closest object
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle-90), rotationSpeed * Time.deltaTime);

            if (closestObject.tag == "Civillian") {

                transform.Translate(Vector2.up * Time.deltaTime * speed);


            }else {
                // Move towards the enemy but leave some distance between
                if (closestDistance >= 1.5f) {
                    transform.Translate(Vector2.up * Time.deltaTime * speed);
                }else if (closestDistance <= 1.4f) {
                    transform.Translate(-Vector2.up * Time.deltaTime * speed);
                }
            }
            

            // Check if the timer has reached the duration and shoot the bullet
            timer += Time.deltaTime;

            if (timer >= duration && (!closestObject.name.Contains("Civillian") || (closestObject.name.Contains("Civillian") && gameManager.ObjectsWithCivillianScript.ContainsKey(closestObject) && gameManager.GiveCivillianScriptAccordingToGameObject(closestObject).speed >= 0.5f)))
            {
                
                // Shoot the bullet
                var bullet = Instantiate(Bullet, GunPoint.position, transform.rotation);

                bulletScript = bullet.GetComponent<EnemyBullet>();

                // Control Bullet Somewhat
                ShootParticles.Play();
                bulletScript.Owner(gameObject);
                bulletScript.getID(ID);
                bullet.name = ID.ToString();
                bullet.GetComponent<SpriteRenderer>().color = thisColor;
                bulletScript.thisColor = thisColor;

                // Reset the timer if needed
                timer = 0f;
            }

        }else if (CapturedCivilian) { // Transport Captive to the blood pond and sacrifice him


            // Calculate the direction vector from the detecting object to the closest object
            Vector2 direction = (bloodPond.transform.position - transform.position).normalized;

            // Calculate the angle between the direction vector and the current forward direction of the detecting object
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Smoothly rotate the detecting object towards the closest object
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle-90), rotationSpeed * Time.deltaTime);
            
            transform.Translate(Vector2.up * Time.deltaTime * speed);

        }else if (Rank == "Follower" && LeaderDetected && closestLeader != null) { // Following Leader

            Vector2 directionAlly = closestLeader.transform.position - transform.position;
            float angle = Mathf.Atan2(directionAlly.y, directionAlly.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle-90), rotationSpeed * Time.deltaTime);

            // Move towards the ally leader but leave some distance between
            if (closestLeaderDistance >= 1.7f) {
                transform.Translate(Vector2.up * Time.deltaTime * speed);
            }else if (closestLeaderDistance <= 1.4f) {
                transform.Translate(-Vector2.up * Time.deltaTime * speed);
            }

        }else if (closestObject == null) {
            

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

        if (closestLeader != null && gameManager.ObjectsWithEnemyScript.ContainsKey(closestLeader) && gameManager.GiveScriptAccordingToGameObject(closestLeader).Rank != "Leader") {
            closestLeader = null;
        }
        

        if (health <= 0) {
            if (ID > -1 && ID < gameManager.CultistFactionLimit+1) {
                gameManager.CountBotsPerIDMinus(ID);
            }
            gameManager.RemoveFromEnemyDictionary(gameObject);
            gameManager.CountKills(KillersID);
            Destroy(gameObject);
        }

        if (CapturedCivilianObject == null) {
            CapturedCivilian = false;
        }

        if (CapturedCivilian && Rank != "Leader") {
            Rank = "Leader";
            changedUniform = true;
        }else if (Rank == "Leader" && !CapturedCivilian && GetComponent<SpriteRenderer>().sprite == defaultSprite) {
            Rank = "Follower";
        }

    }

    public void CaptiveInfo(GameObject civilian) {
        CapturedCivilianObject = civilian;
        CapturedCivilian = true;
    }

    private void RotateToFaceDirection()
    {

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, randomRotationAngle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        
    }

    public void Clone() {
        health = 10;
        var clone = Instantiate(CloneAsset, transform.position+new Vector3(1, 1, 0), Quaternion.identity, transform.parent);
        clone.tag = gameObject.tag;
        clone.GetComponent<SpriteRenderer>().color = thisColor;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Contains("Bullet") && ID != int.Parse(other.name)) {
            

            GotHitParticles.Play();

            if (health <= 1) {
                
                if (other.name.Contains("1111111")) {
                    
                    if (other.GetComponent<EnemyBullet>() != null && other.CompareTag("Bullet") && other.GetComponent<EnemyBullet>().TellOwner() != null && !other.GetComponent<EnemyBullet>().TellOwner().name.Contains("Divine") && other.gameObject.layer != 8) {
                        bulletsParent = other.gameObject.GetComponent<EnemyBullet>().TellOwner();
                        gameManager.GiveScriptAccordingToGameObject(bulletsParent).Clone();
                    }else if (other.CompareTag("PlayerBullet")){
                        bulletsParent = other.gameObject.GetComponent<Bullet>().TellOwner();
                        playerScript = bulletsParent.GetComponent<Player>();
                        playerScript.health = playerScript.maxHealth;
                        playerScript.InstantiateAlly();
                    }

                }else if (other.CompareTag("Bullet") && other.name != "77777") {

                    if (other.GetComponent<EnemyBullet>() != null && other.GetComponent<EnemyBullet>().TellOwner() != null && !other.GetComponent<EnemyBullet>().TellOwner().name.Contains("Divine") && other.gameObject.layer != 8) {
                        bulletsParent = other.gameObject.GetComponent<EnemyBullet>().TellOwner();
                        gameManager.GiveScriptAccordingToGameObject(bulletsParent).Clone();
                    }
                    
                }else if (other.name.Contains("77777")) {
                    
                    bulletsParent = other.GetComponent<EnemyBullet>().TellOwner();
                    if (bulletsParent != null) {

                        if (bulletsParent.name.Contains("Guard")) {
                        bulletsParent.GetComponent<PoliceGuard>().health = 15;
                        }else {
                            bulletsParent.GetComponent<PoliceScript>().health = 10;
                        }
                        
                    }
                    

                }

                
                Instantiate(OnDeath, transform.position, Quaternion.identity);
                Instantiate(blood, gameObject.transform.position, gameObject.transform.rotation);

                var deadman = Instantiate(deadCultist, gameObject.transform.position, Quaternion.Euler(0, 0,transform.rotation.eulerAngles.z - 180f));
                deadman.GetComponent<SpriteRenderer>().color = thisColor;
                
            }

            KillersID = gameManager.GetGameObjectsID(other.gameObject);
            StartCoroutine(GotHit());
            health -= 1;
        }
    }

    
    private IEnumerator GotHit() {

        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        gameObject.GetComponent<SpriteRenderer>().color = thisColor;

    }
    

    
}
