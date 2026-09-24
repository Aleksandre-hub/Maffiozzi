using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MonsterScript : MonoBehaviour
{

    [SerializeField] private float detectionRadius = 6;
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private LayerMask detectionLayerOfObsticles;

    [SerializeField] private float health = 20;
    [SerializeField] private float lifesteal = 1;
    [SerializeField] private ParticleSystem GotHit;

    [SerializeField] private GameObject hitObject;
    GameManager gameManager;
    public int ID;
    private GameObject Master;
    private GameObject previousMaster;
    public SpriteRenderer Brand;
    private Color thisColor;
    [SerializeField] private MonsterScript thisScript;

    [SerializeField] private float speed = 2;
    private float timer = 0f;
    [SerializeField] private float duration = 0.5f;
    private float rotationSpeed = 5f;

    private float DistanceBetweenMaster;

    private Quaternion TowardsPlayer;

    [SerializeField] private Slider healthBar;

    // OnSpawn and OnDeath things
    [SerializeField] private ParticleSystem OnSPawn;
    [SerializeField] private ParticleSystem OnDeath;
    [SerializeField] private GameObject blood;
    [SerializeField] private GameObject deadCultist; 

    private bool masterOnTheWay = false;
    private float DirectionChanger5 = 80;
    private float DireCtionChanger7 = 70;
    private bool focusOnPrey = false;

    // Start is called before the first frame update
    void Start()
    {
        
        gameManager = GameManager.gameManagerInstance;
        Brand.color = Master.GetComponent<SpriteRenderer>().color;

        thisColor = gameObject.GetComponent<SpriteRenderer>().color;

    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = health/20;

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, detectionRadius, Vector2.zero, Mathf.Infinity, detectionLayer);
        
        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach(RaycastHit2D hit in hits) {

            if (hit.collider.name.Contains("Civillian") || ( ( hit.collider.CompareTag("Enemy1") || hit.collider.gameObject.CompareTag("PlayerCultist") ) && gameManager.ObjectsWithEnemyScript.ContainsKey(hit.collider.gameObject) && gameManager.GiveScriptAccordingToGameObject(hit.collider.gameObject).ID == ID) ) {
                continue;
            }else if (hit.collider.gameObject == Master) {
                continue;
            }else if (hit.collider.gameObject == gameObject) {
                continue;
            }else if (hit.collider.name.Contains("Monster")) {
                continue;
            }if (ID == 1111111 && hit.collider.CompareTag("PlayerCultist")) {
                continue;
            }if (hit.collider.gameObject.name.Contains("Divine") && gameManager.ObjectsWithDivineCultistScript.ContainsKey(hit.collider.gameObject) && gameManager.GiveDivineScriptAccordingToGameObject(hit.collider.gameObject).ID == ID) {
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

                    // detected object is not the target                 detected object is not itself                  ally is not on the way of detecting target
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

        if (Master == null) {
            foreach(RaycastHit2D hit in hits) {
                if (hit.collider.name.Contains("Enemy") && gameManager.GiveScriptAccordingToGameObject(hit.collider.gameObject).ID == ID) {
                    Master = hit.collider.gameObject;
                }else if (hit.collider.name.Contains("Player") && hit.collider.GetComponent<Player>().PlayerID == ID) {
                    Master = hit.collider.gameObject;
                }
            }

            if (Master == null) {
                Destroy(gameObject);
            }

        }

        if (Master != null) {
            DistanceBetweenMaster = Vector2.Distance(transform.position, Master.transform.position);
            // Calculate the direction vector from the detecting object to the closest object
            Vector2 direction = (Master.transform.position - transform.position).normalized;

            // Calculate the angle between the direction vector and the current forward direction of the detecting object
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            TowardsPlayer = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle-90), rotationSpeed * Time.deltaTime);

        }

        if (closestObject != null && DistanceBetweenMaster <= 5f) {

            // Calculate the direction vector from the detecting object to the closest object
            Vector2 direction = (closestObject.transform.position - transform.position).normalized;

            // Calculate the angle between the direction vector and the current forward direction of the detecting object
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Smoothly rotate the detecting object towards the closest object
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle-90), rotationSpeed * Time.deltaTime);



            if (DistanceBetweenMaster <= 0.7f && DistanceBetweenMaster >= 0.5f) {
               
                 // Perform a linear raycast to check if there's no object between the detecting object and the detected object
                RaycastHit2D[] linearHits = Physics2D.RaycastAll(transform.position, (closestObject.transform.position - transform.position).normalized, closestDistance, detectionLayerOfObsticles);

                foreach (RaycastHit2D linearHit in linearHits)
                {

                    if (linearHit.collider.gameObject == Master) {
                        masterOnTheWay = true;
                    }
                }

                if (focusOnPrey) {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle-90), rotationSpeed * Time.deltaTime);

                }
                else if (masterOnTheWay) {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle-90-DireCtionChanger7), rotationSpeed * Time.deltaTime);
                }else {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle-20), rotationSpeed * Time.deltaTime);
                }

            }else if (DistanceBetweenMaster < 0.3f) {
                
                 // Perform a linear raycast to check if there's no object between the detecting object and the detected object
                RaycastHit2D[] linearHits = Physics2D.RaycastAll(transform.position, (closestObject.transform.position - transform.position).normalized, closestDistance, detectionLayerOfObsticles);

                foreach (RaycastHit2D linearHit in linearHits)
                {

                    if (linearHit.collider.gameObject == Master) {
                        Debug.Log("Master On The Way");
                        masterOnTheWay = true;
                    }
                }

                if (focusOnPrey) {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle-90), rotationSpeed * Time.deltaTime);
                    Debug.Log("Focusing on prey");
                }
                else if (masterOnTheWay) {
                    Debug.Log("Change Direction <= 0.5");
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle-90-DirectionChanger5), rotationSpeed * Time.deltaTime);
                }else {
                    Debug.Log("Dont change direction <= 0.5");
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle-10), rotationSpeed * Time.deltaTime);
                }
            }


            if (DistanceBetweenMaster >= 1f) {

                // Perform a linear raycast to check if there's no object between the detecting object and the detected object
                RaycastHit2D[] linearHits = Physics2D.RaycastAll(transform.position, (closestObject.transform.position - transform.position).normalized, closestDistance, detectionLayerOfObsticles);

                foreach (RaycastHit2D linearHit in linearHits)
                {

                    if (linearHit.collider.gameObject != Master) {
                        masterOnTheWay = false;
                        focusOnPrey = false;
                    }
                }

            }

           
            transform.Translate(Vector2.up * Time.deltaTime * speed); 
            
                       
            // Check if the timer has reached the duration and shoot the bullet
            timer += Time.deltaTime;
            

            // Check if the timer has reached the duration and shoot the bullet
            timer += Time.deltaTime;

            if (timer >= duration && closestDistance <= 0.5f)
            {
               
                var hitObj = Instantiate(hitObject, transform.position, Quaternion.identity);
                hitObj.GetComponent<Meleehit>().GetOwner(gameObject);
                hitObj.GetComponent<Meleehit>().GetID(thisScript);
                hitObj.transform.position = Vector2.MoveTowards(transform.position, closestObject.transform.position, 0.5f);

                health += lifesteal;

                // Reset the timer if needed
                timer = 0f;
            }
        }else if (Master != null){

            // Calculate the direction vector from the detecting object to the closest object
            Vector2 direction = (Master.transform.position - transform.position).normalized;

            // Calculate the angle between the direction vector and the current forward direction of the detecting object
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            TowardsPlayer = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle-90), rotationSpeed * Time.deltaTime);

            // Smoothly rotate the detecting object towards the closest object
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle-90), rotationSpeed * Time.deltaTime);
            
            // Move towards the enemy but leave some distance between
            if (DistanceBetweenMaster >= 1f) {
                transform.Translate(Vector2.up * Time.deltaTime * speed);
            }else if (DistanceBetweenMaster <= 0.9f) {
                transform.Translate(-Vector2.up * Time.deltaTime * speed);
            }

        }

        if (health <= 0) {
            Destroy(gameObject);
        }

        

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!masterOnTheWay) {
            masterOnTheWay = true;
        }else if (masterOnTheWay) {
            focusOnPrey = true;
        }
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Contains("Bullet") && ID != int.Parse(other.name)) {
            GotHit.Play();
            StartCoroutine(GotHitc());
            health -= 1;

            if (health <= 1) {
                Instantiate(OnDeath, transform.position, Quaternion.identity);
                Instantiate(blood, gameObject.transform.position, gameObject.transform.rotation);
                var deadman = Instantiate(deadCultist, gameObject.transform.position, Quaternion.Euler(0, 0,transform.rotation.eulerAngles.z - 180f));
                deadman.GetComponent<SpriteRenderer>().color = Color.green;
            }

        }

    }

    private IEnumerator GotHitc() {

        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        gameObject.GetComponent<SpriteRenderer>().color = thisColor;

    }

    public void SetMaster(GameObject master) {
        Master = master;
        previousMaster = master;
        tag = master.tag;
    }

    public SpriteRenderer GetBrand() {

        return Brand;

    }
}
