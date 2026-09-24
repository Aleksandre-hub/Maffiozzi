using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class DivineCultist : MonoBehaviour
{

    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private LayerMask detectionLayerOfObsticles;
    [SerializeField] private float detectionRadius = 5;
    [SerializeField] private float rotationSpeed = 7f;

    private float timer = 0f;
    private float duration = 0.2f;

    [SerializeField] GameObject Bullet;

    [SerializeField] private Color thisColor;
    [SerializeField] private SpriteRenderer myRenderer;

    [SerializeField] private float health = 15;
    [SerializeField] private Slider healthBar;

    private Bullet playerBulletScript;
    
    private Enemy1 enemy1;

    [SerializeField] Transform GunPoint;
    [SerializeField] ParticleSystem ShootParticle;
    [SerializeField] ParticleSystem GotHitParticle;

    public int ID = 9999;
    private GameManager gameManager;
    [SerializeField] DivineCultist ThisScript;

    [SerializeField] private GameObject Player;
    [SerializeField] private SpriteRenderer PlayersSpriteRenderer;
    private Color PlayersColor;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.gameManagerInstance;

        gameManager.AddDivineCultsistScripts(gameObject, ThisScript);

        PlayersColor = PlayersSpriteRenderer.color;

        thisColor = gameObject.GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {

        healthBar.value = health/15;
        
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, detectionRadius, Vector2.zero, Mathf.Infinity, detectionLayer);

        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach (RaycastHit2D hit in hits)
        {

            if (hit.collider.gameObject == gameObject) { // Detecting Itself
                continue;
            }else if (hit.collider.GetComponent<Enemy1>() != null) { // Detecting Ally
                enemy1 = hit.collider.GetComponent<Enemy1>();
                if (enemy1.ID == ID) {
                    continue;
                }

            }else if (hit.collider.GetComponent<DivineCultist>() != null) {

                if (hit.collider.GetComponent<DivineCultist>().ID == ID) {
                    continue;
                }

            }else if (gameObject.tag.Contains("Player") && gameObject.tag == hit.collider.tag) {
                continue;
            }else if (hit.collider.name.Contains("Civillian") && gameManager.ObjectsWithCivillianScript.ContainsKey(hit.collider.gameObject) && gameManager.GiveCivillianScriptAccordingToGameObject(hit.collider.gameObject)) {
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
    


        if (closestObject != null) { // Follow and shoot the Enemy

            // Calculate the direction vector from the detecting object to the closest object
            Vector2 direction = (closestObject.transform.position - transform.position).normalized;

            // Calculate the angle between the direction vector and the current forward direction of the detecting object
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Smoothly rotate the detecting object towards the closest object
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle-90), rotationSpeed * Time.deltaTime);

            // Check if the timer has reached the duration and shoot the bullet
            timer += Time.deltaTime;

            if (timer >= duration)
            {
                // Shoot the bullet
                var bullet = Instantiate(Bullet, GunPoint.position, transform.rotation);
                var enemyBulletScript = bullet.GetComponent<EnemyBullet>();
                // Control Bullet Somewhat
                ShootParticle.Play();
                enemyBulletScript.Owner(gameObject);
                enemyBulletScript.getID(ID);
                bullet.name = ID.ToString();
                bullet.GetComponent<SpriteRenderer>().color = thisColor;
                enemyBulletScript.thisColor = thisColor;

                // Reset the timer if needed
                timer = 0f;
            }

        }

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Contains("Bullet") && ID != int.Parse(other.name)) {
            
            GotHitParticle.Play();

            if (health <= 1 && other.name.Contains("1111111")) {

                ID = int.Parse(other.name);

                thisColor = PlayersColor;
                myRenderer.color = PlayersColor;
                gameObject.tag = Player.tag;
                health = 15;


            }else if (health <= 1 && !other.name.Contains("1111111") && !other.name.Contains("77777")) {
                
                if (other.GetComponent<EnemyBullet>() != null) {
                    var enemyBulletScript = other.GetComponent<EnemyBullet>();

                    if (enemyBulletScript.TellOwner() != null) {

                        if (enemyBulletScript.TellOwner().name.Contains("Divine")) {
                            gameManager.GiveDivineScriptAccordingToGameObject(enemyBulletScript.TellOwner()).health = 15;
                        }

                        gameObject.tag = enemyBulletScript.TellOwner().tag;
                        ID = int.Parse(other.name);
                        thisColor = enemyBulletScript.thisColor;
                        myRenderer.color = thisColor;
                    }else {
                        Debug.Log(gameObject.name + " has some problems on line 125");
                    }
                    
                    health = 15;

                }else {
                    var meleehitScript = other.GetComponent<Meleehit>();

                    gameObject.tag = meleehitScript.TellOwner().tag;
                    ID = meleehitScript.ID;
                    thisColor = meleehitScript.FactionColor;
                    myRenderer.color = thisColor;
                    health = 15;
                }


            }

            if (thisColor == Color.red) {
                Debug.Log("thisColor became red for some reason: " + gameObject.name);
            }

            StartCoroutine(GotHit());
            health -= 1;
        }else if (other.tag.Contains("Bullet") && other.name == "77777") {
            health -= 0.5f;

            if (health <= 1f) {
                Destroy(gameObject);
            }

        }

    }

    private IEnumerator GotHit() {

        myRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        myRenderer.color = thisColor;

    }

}
