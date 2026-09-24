using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeededObjectsScript : MonoBehaviour
{

    //public GameObject[] hits;
    public NeededObjectsScript ownScript;
    public Text[] leaderboards;
    public Text playerscore;
    public Text timeboard;
    public Text shedegi;
    public GameObject lostPanel;
    public Text messageText;
    public GameObject player;
    public Player playerScript;
    public BotSpawner botSpawner;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.gameManagerInstance;
        gameManager.GetNeededThings(ownScript);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
