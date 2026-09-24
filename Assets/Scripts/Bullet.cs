using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    Vector3 mousePosition;
    Vector3 direction;
    GameObject target;
    Vector3 targetPosition;
    private GameObject owner;


    [SerializeField] private Vector3 bulletTraveDistance;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private ParticleSystem OnDestroyEffect;

    public Color thisColor;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {

        gameManager = GameManager.gameManagerInstance;

        thisColor = gameObject.GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * Time.deltaTime * bulletSpeed);

        
        Destroy(gameObject, 0.8f);
        
    }

    public void Owner(GameObject owner) {

        this.owner = owner;
        
    }

    public GameObject TellOwner() {

        return owner;

    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.gameObject.tag.Contains("Bullet") && !other.gameObject.tag.Contains("Player") && !other.CompareTag("Blood_Ponds") && !other.CompareTag("Rescue_Place") && !other.CompareTag("Leader_ritual")) {
            
            if (!other.tag.Contains("Enemy") && !other.tag.Contains("Police") && !other.tag.Contains("Civillian")) {
                Instantiate(OnDestroyEffect, transform.position, transform.rotation);
                Destroy(gameObject);
            }else {
                Destroy(gameObject);
            }

        }

    }

    
}
