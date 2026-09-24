using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PoliceScript : MonoBehaviour
{

    [SerializeField] private GameObject Bullet;
    [SerializeField] private float duration = 0.5f;
    private float timer = 0f;


    public float health = 10;


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
    [SerializeField] private string Rank;
    private PoliceScript closestlAllyScript;
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

    public bool CapturedCivilian = false;
    private GameObject CapturedCivilianObject;

    public GameObject RescuePlace;
    private GameObject[] Rescue_Places;

    private GameObject RaidSpot;
    private GameObject[] RaidSpots;

    public bool Raid;

    [SerializeField] private Slider healthBar;

    private Color thisColor;

    // OnSpawn and OnDeath things
    [SerializeField] private ParticleSystem OnSPawn;
    [SerializeField] private ParticleSystem OnDeath;
    [SerializeField] private GameObject blood;
    [SerializeField] private GameObject deadCultist; 

    private GameObject closestAllyObject;
    private GameObject closestObject;
    private float closestDistance;
    private float closestAllyDistance;
    private float closestRescuePlaceDistance;
    private float closestRaidSpot;
    private int PoliceID = 77777;

    // Start is called before the first frame update
    void Start()
    {

        Rescue_Places = GameObject.FindGameObjectsWithTag("Rescue_Place");
        RaidSpots = GameObject.FindGameObjectsWithTag("Blood_Ponds");

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


    }

    // Update is called once per frame
    void Update()
    {

        healthBar.value = health/10;

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, detectionRadius, Vector2.zero, Mathf.Infinity, detectionLayer);

        
        closestAllyObject = null;
        closestObject = null;

        closestDistance = Mathf.Infinity;
        closestAllyDistance = Mathf.Infinity;

        closestRescuePlaceDistance = Mathf.Infinity;

        closestRaidSpot = Mathf.Infinity;
        
        foreach(GameObject place in Rescue_Places) {

            // Calculate the distance between the detected object and the detecting object
            float distance = Vector2.Distance(transform.position, place.transform.position);

             // Check if the detected object is closer than the current closest object
            if (distance < closestRescuePlaceDistance)
            {
                RescuePlace = place;
                closestRescuePlaceDistance = distance;
            }
        }

        foreach (GameObject spot in RaidSpots) {

            // Calculate the distance between the detected object and the detecting object
            float distance = Vector2.Distance(transform.position, spot.transform.position);

            // Check if the detected object is closer than the current closest object
            if (distance < closestRaidSpot)
            {
                RaidSpot = spot;
                closestRaidSpot = distance;
            }

        }

        if (hits.Length == 1) {
            LeaderDetected = false;
        }

        if (Rank == "Follower") {
            thisObjectsSpriteRenderer.sprite = defaultSprite;
        }

        foreach(RaycastHit2D hitL in hits) {

            if (Rank == "Leader") {
                break;
            }else if (hitL.collider.gameObject == gameObject) {
                continue;
            }

            if (closestLeader == null){
                break;
            }else if (closestLeader == hitL.collider.gameObject) {
                LeaderDetected = true;
                break;
            }else {
                LeaderDetected = false;
            }
        } 

        // Iterate through all hits and find the closest object
        foreach (RaycastHit2D hit in hits)
        {

            if (hit.collider.gameObject == gameObject) { // Detecting Enemy
                continue;
            }else if (hit.collider.tag == gameObject.tag) { // Detecting Ally

                

                GameObject detectedAllyObject = hit.collider.gameObject;

                // Calculate the distance between the detected object and the detecting object
                float distanceAlly = Vector2.Distance(transform.position, detectedAllyObject.transform.position);

                // Check if the detected object is closer than the current closest object
                if (distanceAlly < closestAllyDistance)
                {
                    closestAllyObject = detectedAllyObject;
                    closestAllyDistance = distanceAlly;
                    closestlAllyScript = closestAllyObject.GetComponent<PoliceScript>();
                }

                if (closestAllyObject != null && closestAllyObject.GetComponent<DivineCultist>() == null && !closestAllyObject.name.Contains("Guard") && closestAllyObject.name != "Player" && !closestAllyObject.name.Contains("Civillian") && closestlAllyScript.Rank == "Leader") {
                    closestLeader = closestAllyObject;
                    closestLeaderDistance = closestAllyDistance;
                    LeaderDetected = true;
                }

                continue;
            }if ((hit.collider.tag.Contains("Enemy") || hit.collider.tag.Contains("Player")) && hit.collider.name.Contains("Civillian")) {
                continue;
            }

            GameObject detectedObject = hit.collider.gameObject;

            // Calculate the distance between the detected object and the detecting object
            float distance = Vector2.Distance(transform.position, detectedObject.transform.position);

            // Perform a linear raycast to check if there's no object between the detecting object and the detected object
            RaycastHit2D[] linearHits = Physics2D.RaycastAll(transform.position, (detectedObject.transform.position - transform.position).normalized, distance, detectionLayerOfObsticles);

            bool isClearLineOfSight = true;

            foreach (RaycastHit2D linearHit in linearHits)
            {
                    // detected object is not the target                 detected object is not itself                  ally is no on the way of detecting target
                if (linearHit.collider.gameObject != detectedObject && linearHit.collider.gameObject != gameObject && linearHit.collider.gameObject.tag != gameObject.tag)
                {
                    // If any object other than the detected object or detecting object is hit, there's an obstruction
                    isClearLineOfSight = false;
                    break;
                }
            }

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
        

        if (closestObject != null) { // Follow and shoot the Enemy

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

            if (timer >= duration && !closestObject.name.Contains("Civillian"))
            {
                // Shoot the bullet
                var bullet = Instantiate(Bullet, GunPoint.position, transform.rotation);

                // Control Bullet Somewhat
                ShootParticles.Play();
                bullet.GetComponent<EnemyBullet>().Owner(gameObject);
                bullet.name = PoliceID.ToString();
                bullet.GetComponent<SpriteRenderer>().color = Color.blue;

                // Reset the timer if needed
                timer = 0f;
            }

        }else if (CapturedCivilian) { // Transport Captive to the blood pond and sacrifice him


            // Calculate the direction vector from the detecting object to the closest object
            Vector2 direction = (RescuePlace.transform.position - transform.position).normalized;

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

        
        if (Raid && closestObject == null && !CapturedCivilian && (!LeaderDetected || Rank == "Leader")) {

            // test feature
            //transform.position = Vector2.MoveTowards(transform.position, RaidSpot.transform.position, 1f);
            
            // 2nd way
            // Calculate the direction vector from the detecting object to the closest object
            Vector2 direction = (RaidSpot.transform.position - transform.position).normalized;

            // Calculate the angle between the direction vector and the current forward direction of the detecting object
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Smoothly rotate the detecting object towards the closest object
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle-90), rotationSpeed * Time.deltaTime);
            
            transform.Translate(Vector2.up * Time.deltaTime * speed);

            if (closestRaidSpot <= 15f) {
                Raid = false;
                closestRaidSpot = 50000f;
            }

        }
        

        if (health <= 0) {
            Instantiate(OnDeath, transform.position, Quaternion.identity);
            Instantiate(blood, gameObject.transform.position, gameObject.transform.rotation);
            var deadman = Instantiate(deadCultist, gameObject.transform.position, Quaternion.Euler(0, 0,transform.rotation.eulerAngles.z - 180f));
            deadman.GetComponent<SpriteRenderer>().color = Color.blue;
            Destroy(gameObject);
        }

        if (CapturedCivilianObject == null) {
            CapturedCivilian = false;
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Contains("Bullet") && PoliceID != int.Parse(other.name)) {
            
            GotHitParticles.Play();
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
