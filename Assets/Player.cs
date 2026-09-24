using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{

    Vector3 sideSpeedVector; 
    Vector3 verticalSpeedVector;
    Vector3 mousePosition;
    Vector2 direction;
    
    private GameObject BulletController;
    private GameObject AllyController;
    
    [SerializeField] private float speed = 1;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject bulletSpawnPosition;

    [SerializeField] private Image[] GotHitImage;

    private float previousSpeed;

    [SerializeField] GameObject AllyBot;

    [SerializeField] private Transform GunPoint;
    [SerializeField] private ParticleSystem ShootParticles;
    [SerializeField] private ParticleSystem GotHit;
    [SerializeField] private Sprite LeaderUniform;
    [SerializeField] private Player thisScript;
    public float maxHealth = 10;
    public float health = 10;
    [SerializeField] private Slider healthBar;

    public string Rank = "Follower";


    private Vector2 spawnArea;
    private float spawnDiameter = 120;


    public Color thisColor;

    public int PlayerID = 1111111;
    public int playerPoliceID = 77777;
    GameManager gameManager;

    MenuSceneAssets menuInstance;
    [SerializeField] private GameObject botSpawner;
    [SerializeField] Text MessageText;
    

    // Start is called before the first frame update
    void Start()
    {

        spawnArea = new Vector2(Random.Range(-spawnDiameter, spawnDiameter), Random.Range(-spawnDiameter, spawnDiameter));
        transform.position = spawnArea;
        gameManager = GameManager.gameManagerInstance;
        menuInstance = MenuSceneAssets.Instance;

        gameManager.GetPlayerPrefab(gameObject);

        Time.timeScale = 1;

        previousSpeed = speed;
        sideSpeedVector = new Vector3(speed, 0, 0);
        verticalSpeedVector = new Vector3(0, speed, 0);

        thisColor = gameObject.GetComponent<SpriteRenderer>().color;

        gameManager.GetPlayerScript(thisScript);
    }

    // Update is called once per frame
    void Update()
    {

        

        if (gameManager == null) {
            MessageText.text = "gameManager is null";
        }else if (menuInstance == null) {
            MessageText.text = "menu assets is null";
        }else if (botSpawner == null) {
            MessageText.text = "bot spawner is null";
        }else if (gameManager.playerScore == null) {
            MessageText.text = "player score is null";
        }else {
            MessageText.text = gameManager.name + " - " + SceneManager.GetActiveScene().name + " - " + gameManager.RetryPanel.name + " - player score is: " + gameManager.playerScore.text + " || " + gameManager.ScorePerID[1111111] + " - " + gameManager.testtext + " || time scale: " + Time.deltaTime;
        }

        // CONTROLLING HEALTH BAR ******************************************

        healthBar.value = health/maxHealth;

        if (health < 1) {
            //health = maxHealth;
            Time.timeScale = 0;
        }

        // --------------------------------------------------------------------------------------------------------------


        
        // PLAYER ROTATION TO MOUSE POSITION *********************

        mousePosition = Input.mousePosition;
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);

        transform.up = direction;

        // --------------------------------------------------------------------------------------------------------------


        // PLAYER MOVEMENT ******************************************

        if(speed != previousSpeed) {
            sideSpeedVector = new Vector3(speed, 0, 0);
            verticalSpeedVector = new Vector3(0, speed, 0);
            previousSpeed = speed;
            Debug.Log(sideSpeedVector);
        } 

        if (Input.GetKey(KeyCode.A)) 
        {
            
            transform.position -= sideSpeedVector*Time.deltaTime;

        }else if (Input.GetKey(KeyCode.D)) 
        {

            transform.position += sideSpeedVector*Time.deltaTime;

        }
        
        if (Input.GetKey(KeyCode.W)) 
        {

            transform.position += verticalSpeedVector*Time.deltaTime;

        }else if (Input.GetKey(KeyCode.S)) 
        {

            transform.position -= verticalSpeedVector*Time.deltaTime;

        }

        // -----------------------------------------------------------------------------------


        // PLAYER SHOOTING ***********************************************

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            ShootParticles.Play();
            BulletController = Instantiate(bullet, GunPoint.position, transform.rotation);
            BulletController.name = PlayerID.ToString();
            BulletController.GetComponent<Bullet>().Owner(gameObject);
        }

    }

    public void BecomeLeader() {
        if (Rank != "Leader") {
            gameObject.GetComponent<SpriteRenderer>().sprite = LeaderUniform;
            Rank = "Leader";
        }
    }


    public void InstantiateAlly() {
        Debug.Log("instantiates ally");
        AllyController = Instantiate(AllyBot, transform.position - new Vector3(0.5f, 0, 0), transform.rotation);
        AllyController.tag = gameObject.tag;
        Debug.Log("Searching for allys ID");
        AllyController.GetComponent<Enemy1>().ID = PlayerID;
        AllyController.GetComponent<SpriteRenderer>().color = thisColor;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.name.Contains(PlayerID.ToString()) && other.tag.Contains("Bullet")) {
            health -= 1;
            GotHit.Play();
            StartCoroutine(GotHitE());
            StartCoroutine(UIgotHit());
        }
    }

    private IEnumerator GotHitE() {

        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        gameObject.GetComponent<SpriteRenderer>().color = thisColor;
    
    }

    private IEnumerator UIgotHit() {
        foreach(Image img in GotHitImage) {
            Color currentColor = img.color;
            currentColor.a = 0.5f;    
            img.color = currentColor;
        }
        yield return new WaitForSeconds(0.3f);
        foreach(Image img in GotHitImage) {
            Color currentColor = img.color;
            currentColor.a = 0f;    
            img.color = currentColor;
        }
    }

}
