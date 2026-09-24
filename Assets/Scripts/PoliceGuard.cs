using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoliceGuard : MonoBehaviour
{
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private LayerMask detectionLayerOfObsticles;
    [SerializeField] private float detectionRadius = 5;
    [SerializeField] private float rotationSpeed = 7f;

    private float timer = 0f;
    private float duration = 0.2f;

    [SerializeField] GameObject Bullet;

    private Color thisColor;

    public float health = 15;
    [SerializeField] private Slider healthBar;

    private Bullet playerBulletScript;
    private EnemyBullet enemyBulletScript;

    [SerializeField] Transform GunPoint;
    [SerializeField] ParticleSystem ShootParticle;
    [SerializeField] ParticleSystem GotHitParticle;

    private int PoliceID = 77777;

    // Start is called before the first frame update
    void Start()
    {
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
            }else if (hit.collider.tag == gameObject.tag && gameObject.tag != null) { // Detecting Ally
                continue;
            }else if (hit.collider.name.Contains("Civillian")) {
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

                // Control Bullet Somewhat
                ShootParticle.Play();
                bullet.GetComponent<EnemyBullet>().Owner(gameObject);
                bullet.name = PoliceID.ToString();
                bullet.GetComponent<SpriteRenderer>().color = Color.blue;

                // Reset the timer if needed
                timer = 0f;
            }

        }

        if (health <= 0) {
            Destroy(gameObject);
        }

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Contains("Bullet") && PoliceID != int.Parse(other.name)) {
            
            GotHitParticle.Play();

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
