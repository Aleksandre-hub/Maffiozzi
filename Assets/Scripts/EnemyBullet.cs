using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{

    [SerializeField] private float enemyBulletSpeed;
    private GameObject owner;

    public Color thisColor;

    public int bulletID;

    [SerializeField] ParticleSystem OnHitParticles;

    private GameManager gameManager;

    [SerializeField] private ParticleSystem DestroyEffect;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.gameManagerInstance;
        thisColor = gameObject.GetComponent<SpriteRenderer>().color;

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * enemyBulletSpeed);
        Destroy(gameObject, 1f);


        if (owner != null && (gameManager.ObjectsWithEnemyScript.ContainsKey(owner) || gameManager.ObjectsWithDivineCultistScript.ContainsKey(owner))) {
            if (owner.name.Contains("Divine")) {
                bulletID = gameManager.GiveDivineScriptAccordingToGameObject(owner).ID;
            }else if (owner.name.Contains("Enemy")) {
                bulletID = gameManager.GiveScriptAccordingToGameObject(owner).ID;
            }
        }
        
        

    }

    public void getID(int id) {

        this.bulletID = id;

    }

    public int TellID() {
        
        return bulletID;
         
    }

    public void Owner(GameObject owner) {

        this.owner = owner;
        
    }

    public GameObject TellOwner() {

        return owner;
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if(!other.tag.Contains("Bullet") && gameObject.name != gameManager.GetGameObjectsID(other.gameObject).ToString() && !other.CompareTag("Blood_Ponds") && !other.CompareTag("Rescue_Place") && !other.CompareTag("Leader_ritual")) {
            

            if (!other.tag.Contains("Player") && !other.tag.Contains("Enemy") && !other.tag.Contains("Police") && !other.tag.Contains("Civillian")) {
                Instantiate(DestroyEffect, transform.position, Quaternion.Euler(transform.rotation.eulerAngles.x - 90f, 0, 0));
                Destroy(gameObject);
            }else {
                Destroy(gameObject);
            }

        }
        
    }

   
}
