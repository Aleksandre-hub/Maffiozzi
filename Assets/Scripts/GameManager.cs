using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{

    public static GameManager gameManagerInstance;

    public Dictionary<GameObject, Enemy1> ObjectsWithEnemyScript = new Dictionary<GameObject, Enemy1>();
    public Dictionary<GameObject, DivineCultist> ObjectsWithDivineCultistScript = new Dictionary<GameObject, DivineCultist>();
    public Dictionary<GameObject, CivillianScript> ObjectsWithCivillianScript = new Dictionary<GameObject, CivillianScript>();
    public Dictionary<int, bool> IDAvalaiability = new Dictionary<int, bool>();
    public Dictionary<int, int> BotsPerID = new Dictionary<int, int>();
    public Dictionary<int, int> SacraficisPerID = new Dictionary<int, int>();

    public Dictionary<int, int> KillsPerID = new Dictionary<int, int>();
    private int ScorePerKill = 20;

    public Dictionary<int, int> ScorePerID = new Dictionary<int, int>();
    private int ScorePerSacrifice = 50;

    private string[] cultNameWords = { "Black Sun Sect", "Crimson Dagger Clan", "Ebon Veil Brotherhood", "Obsidian Serpent Circle", "Shadow Hand Order",
            "Void Walker Assembly", "Midnight Raven Syndicate", "Dark Flame Coven", "Abyssal Dawn Cult", "Bleak Moon Fellowship",
            "Forest Whisperers", "Sacred Grove Circle", "Earthbound Enlightenment", "Sunfire Brotherhood", "Crystal Lake Congregation",
            "Stone Circle Society", "Mountain Spirit Tribe", "Windcaller Clan", "Oceanic Oracle Order", "Lunar Tide Assembly",
            "Silver Wolf Pack", "Mystic Star Guild", "Eclipse Eye Brotherhood", "Phoenix Flame Order", "Emerald Dragon Society",
            "Golden Serpent Clan", "Silent Shadow Fellowship", "Iron Claw Tribe", "Blue Flame Sect", "Twilight Thorn Circle",
            "Scarlet Raven Coven", "Amber Moon Brotherhood", "Nightfall Assembly", "Whispering Wind Clan", "Eternal Flame Sect",
            "Celestial Dawn Cult", "Crimson Moon Brotherhood", "Silver Flame Order", "Emerald Dawn Fellowship", "Shadow Moon Sect",
            "Dark Star Assembly", "Twilight Flame Order", "Silent Serpent Circle", "Obsidian Flame Cult", "Eclipse Flame Order",
            "Golden Flame Brotherhood", "Lunar Flame Assembly", "Mystic Flame Clan", "Phoenix Dawn Cult", "Scarlet Flame Order",
            "Silver Dawn Brotherhood", "Whispering Flame Sect", "Amber Dawn Fellowship", "Crystal Flame Cult", "Eternal Dawn Order",
            "Twilight Dawn Brotherhood", "Silent Dawn Assembly", "Iron Dawn Clan", "Blue Dawn Sect", "Dark Dawn Circle",
            "Nightfall Dawn Order", "Whispering Dawn Clan", "Celestial Dawn Brotherhood", "Golden Dawn Cult", "Lunar Dawn Sect",
            "Obsidian Dawn Circle", "Eclipse Dawn Order", "Mystic Dawn Fellowship", "Phoenix Dawn Sect", "Scarlet Dawn Brotherhood",
            "Silver Dawn Cult", "Whispering Dawn Order", "Amber Dawn Circle", "Crystal Dawn Sect", "Eternal Dawn Brotherhood",
            "Twilight Dawn Cult", "Silent Dawn Order", "Iron Dawn Sect", "Blue Dawn Fellowship", "Dark Dawn Brotherhood",
            "Nightfall Dawn Assembly", "Whispering Dawn Order", "Celestial Dawn Fellowship", "Golden Dawn Sect", "Lunar Dawn Order",
            "Obsidian Dawn Fellowship", "Eclipse Dawn Sect", "Mystic Dawn Order", "Phoenix Dawn Brotherhood", "Scarlet Dawn Cult",
            "Silver Dawn Sect", "Whispering Dawn Fellowship", "Amber Dawn Order", "Crystal Dawn Brotherhood", "Eternal Dawn Sect",
            "Twilight Dawn Fellowship", "Silent Dawn Sect", "Iron Dawn Fellowship", "Blue Dawn Order", "Dark Dawn Sect",
            "Nightfall Dawn Fellowship", "Whispering Dawn Brotherhood", "Celestial Dawn Order", "Golden Dawn Fellowship",
            "Lunar Dawn Sect", "Obsidian Dawn Brotherhood", "Eclipse Dawn Fellowship", "Mystic Dawn Sect", "Phoenix Dawn Order",
            "Scarlet Dawn Fellowship", "Silver Dawn Brotherhood", "Whispering Dawn Sect", "Amber Dawn Fellowship", "Crystal Dawn Order",
            "Eternal Dawn Brotherhood", "Twilight Dawn Sect", "Silent Dawn Fellowship", "Iron Dawn Order", "Blue Dawn Brotherhood",
            "Dark Dawn Sect", "Nightfall Dawn Fellowship", "Whispering Dawn Order", "Celestial Dawn Sect", "Golden Dawn Brotherhood",
            "Lunar Dawn Fellowship", "Obsidian Dawn Sect", "Eclipse Dawn Brotherhood", "Mystic Dawn Fellowship", "Phoenix Dawn Sect",
            "Scarlet Dawn Order", "Silver Dawn Fellowship", "Whispering Dawn Brotherhood", "Amber Dawn Sect", "Crystal Dawn Fellowship",
            "Eternal Dawn Order", "Twilight Dawn Brotherhood", "Silent Dawn Sect", "Iron Dawn Fellowship", "Blue Dawn Order",
            "Dark Dawn Brotherhood", "Nightfall Dawn Sect", "Whispering Dawn Fellowship", "Celestial Dawn Order", "Golden Dawn Sect",
            "Lunar Dawn Brotherhood", "Obsidian Dawn Fellowship", "Eclipse Dawn Sect", "Mystic Dawn Brotherhood", "Phoenix Dawn Fellowship",
            "Scarlet Dawn Sect", "Silver Dawn Order", "Whispering Dawn Fellowship", "Amber Dawn Brotherhood", "Crystal Dawn Sect",
            "Eternal Dawn Fellowship", "Twilight Dawn Order", "Silent Dawn Brotherhood", "Iron Dawn Sect", "Blue Dawn Fellowship",
            "Dark Dawn Order", "Nightfall Dawn Brotherhood", "Whispering Dawn Sect", "Celestial Dawn Fellowship", "Golden Dawn Order",
            "Lunar Dawn Sect", "Obsidian Dawn Brotherhood", "Eclipse Dawn Fellowship", "Mystic Dawn Sect", "Phoenix Dawn Brotherhood",
            "Scarlet Dawn Fellowship", "Silver Dawn Sect", "Whispering Dawn Order", "Amber Dawn Fellowship", "Crystal Dawn Brotherhood",
            "Eternal Dawn Sect", "Twilight Dawn Fellowship", "Silent Dawn Order", "Iron Dawn Brotherhood", "Blue Dawn Sect",
            "Dark Dawn Fellowship", "Nightfall Dawn Order", "Whispering Dawn Brotherhood", "Celestial Dawn Sect", "Golden Dawn Fellowship",
            "Lunar Dawn Order", "Obsidian Dawn Sect", "Eclipse Dawn Brotherhood", "Mystic Dawn Fellowship", "Phoenix Dawn Sect",
            "Scarlet Dawn Order", "Silver Dawn Fellowship", "Whispering Dawn Brotherhood", "Amber Dawn Sect", "Crystal Dawn Fellowship",
            "Eternal Dawn Order", "Twilight Dawn Brotherhood", "Silent Dawn Sect", "Iron Dawn Fellowship", "Blue Dawn Order",
            "Dark Dawn Brotherhood", "Nightfall Dawn Sect", "Whispering Dawn Fellowship", "Celestial Dawn Order", "Golden Dawn Sect",
            "Lunar Dawn Brotherhood", "Obsidian Dawn Fellowship", "Eclipse Dawn Sect", "Mystic Dawn Brotherhood", "Phoenix Dawn Fellowship",
            "Scarlet Dawn Sect", "Silver Dawn Order", "Whispering Dawn Fellowship", "Amber Dawn Brotherhood", "Crystal Dawn Sect",
            "Eternal Dawn Fellowship", "Twilight Dawn Order", "Silent Dawn Brotherhood", "Iron Dawn Sect", "Blue Dawn Fellowship",
            "Dark Dawn Order", "Nightfall Dawn Brotherhood", "Whispering Dawn Sect", "Celestial Dawn Fellowship", "Golden Dawn Order",
            "Lunar Dawn Sect", "Obsidian Dawn Brotherhood", "Eclipse Dawn Fellowship", "Mystic Dawn Sect", "Phoenix Dawn Brotherhood",
            "Scarlet Dawn Fellowship", "Silver Dawn Sect", "Whispering Dawn Order", "Amber Dawn Fellowship", "Crystal Dawn Brotherhood",
            "Eternal Dawn Sect", "Twilight Dawn Fellowship", "Silent Dawn Order", "Iron Dawn Brotherhood", "Blue Dawn Sect",
            "Dark Dawn Fellowship", "Nightfall Dawn Order", "Whispering Dawn Brotherhood", "Celestial Dawn Sect", "Golden Dawn Fellowship",
            "Lunar Dawn Order", "Obsidian Dawn Sect", "Eclipse Dawn Brotherhood", "Mystic Dawn Fellowship", "Phoenix Dawn Sect",
            "Scarlet Dawn Order", "Silver Dawn Fellowship", "Whispering Dawn Brotherhood", "Amber Dawn Sect", "Crystal Dawn Fellowship",
            "Eternal Dawn Order", "Twilight Dawn Brotherhood", "Silent Dawn Sect", "Iron Dawn Fellowship", "Blue Dawn Order",
            "Dark Dawn Brotherhood", "Nightfall Dawn Sect", "Whispering Dawn Fellowship", "Celestial Dawn Order", "Golden Dawn Sect",
            "Lunar Dawn Brotherhood", "Obsidian Dawn Fellowship", "Eclipse Dawn Sect", "Mystic Dawn Brotherhood", "Phoenix Dawn Fellowship",
            "Scarlet Dawn Sect", "Silver Dawn Order", "Whispering Dawn Fellowship", "Amber Dawn Brotherhood", "Crystal Dawn Sect",
            "Eternal Dawn Fellowship", "Twilight Dawn Order", "Silent Dawn Brotherhood", "Iron Dawn Sect", "Blue Dawn Fellowship",
            "Dark Dawn Order", "Nightfall Dawn Brotherhood", "Whispering Dawn Sect", "Celestial Dawn Fellowship", "Golden Dawn Order",
            "Lunar Dawn Sect", "Obsidian Dawn Brotherhood", "Eclipse Dawn Fellowship", "Mystic Dawn Sect", "Phoenix Dawn Brotherhood",
            "Scarlet Dawn Fellowship", "Silver Dawn Sect", "Whispering Dawn Order", "Amber Dawn Fellowship", "Crystal Dawn Brotherhood",
            "Eternal Dawn Sect", "Twilight Dawn Fellowship", "Silent Dawn Order", "Iron Dawn Brotherhood", "Blue Dawn Sect",
            "Dark Dawn Fellowship", "Nightfall Dawn Order", "Whispering Dawn Brotherhood", "Celestial Dawn Sect", "Golden Dawn Fellowship",
            "Lunar Dawn Order", "Obsidian Dawn Sect", "Eclipse Dawn Brotherhood", "Mystic Dawn Fellowship", "Phoenix Dawn Sect",
            "Scarlet Dawn Order", "Silver Dawn Fellowship", "Whispering Dawn Brotherhood", "Amber Dawn Sect", "Crystal Dawn Fellowship",
            "Eternal Dawn Order", "Twilight Dawn Brotherhood", "Silent Dawn Sect", "Iron Dawn Fellowship", "Blue Dawn Order",
            "Dark Dawn Brotherhood", "Nightfall Dawn Sect", "Whispering Dawn Fellowship", "Celestial Dawn Order", "Golden Dawn Sect",
            "Lunar Dawn Brotherhood", "Obsidian Dawn Fellowship", "Eclipse Dawn Sect", "Mystic Dawn Brotherhood", "Phoenix Dawn Fellowship",
            "Scarlet Dawn Sect", "Silver Dawn Order", "Whispering Dawn Fellowship", "Amber Dawn Brotherhood", "Crystal Dawn Sect",
            "Eternal Dawn Fellowship", "Twilight Dawn Order", "Silent Dawn Brotherhood", "Iron Dawn Sect", "Blue Dawn Fellowship" };
    public Dictionary<int, string> cultDictionary = new Dictionary<int, string>();

    [SerializeField] public GameObject RetryPanel;
    public Player playerScript;
    [SerializeField] private GameObject playerPrefab;


    private int playerID = 1111111;
    private int policeID = 77777;

    public int CultistFactionLimit;
    [SerializeField] private BotSpawner botSpawnerSCript;
    private int IdChecker;

    [SerializeField] private MenuSceneAssets menuSceneAssets; 

    [SerializeField] GameObject NeededThings;
    [SerializeField] NeededObjectsScript neededObjectsScript;
    public String playerFactionName = "PlayerFactionName";
    [SerializeField] public Text playerScore;
    [SerializeField] private Text firstPlace;
    [SerializeField] private Text secondPlace;
    [SerializeField] private Text thirdPlace;
    [SerializeField] private Text fourthPlace;
    [SerializeField] private Text fifthPlace;
    [SerializeField] private Text shedegiText;
    [SerializeField] private Text timeBoard;
    [SerializeField] Text MessageText;
    private float setTime;
    private float time = 180.0f;
    private int minutes;
    private int seconds;

    public string testtext = "";
    public bool SceneRestarted = false;
    public bool timeLagging = false;

    private void Awake()
    {
        // Ensure there's only one instance of the class
        if (gameManagerInstance == null)
        {
            // Set the instance to this object if it's the first one
            gameManagerInstance = this;
            // Keep the object alive throughout scenes
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists, destroy this one
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        //playerScript = playerPrefab.GetComponent<Player>();
        //botSpawnerSCript = GameObject.FindObjectOfType("BotSpawner").GetComponent<BotSpawner>();
        menuSceneAssets = MenuSceneAssets.Instance;
        if (menuSceneAssets != null && menuSceneAssets.FactionName() != "") {
            playerFactionName = menuSceneAssets.FactionName();
        }else {
            Debug.Log("either menuSceneAssets is null or menuSceneAssets.FactionName() != '' ");
            if (menuSceneAssets == null) {
                Debug.Log("menuSceneAssets is null");
            }else {
                Debug.Log("menuSceneAssets.FactionName() != '' ");
            }
        }
        setTime = time;

        StartCoroutine(Timer());
        
        CultistFactionLimit = botSpawnerSCript.CultistFactionLimit;

        Debug.Log("Starting Engine");

        for (int i = 0; i < CultistFactionLimit; i++) {
            IDAvalaiability.Add(i, true);
        }

        for (int i = 0; i < CultistFactionLimit; i++) {
            BotsPerID.Add(i, 0);
        }

        SacraficisPerID.Add(1111111, 0);
        for (int i = 0; i < CultistFactionLimit; i++) {
            SacraficisPerID.Add(i, 0);
        }

        cultDictionary.Add(1111111, playerFactionName);
        for (int i = 0; i < CultistFactionLimit; i++) {
            cultDictionary.Add(i, cultNameWords[i]);
        }

        KillsPerID.Add(1111111, 0);
        for (int i = 0; i < CultistFactionLimit; i++) {
            KillsPerID.Add(i, 0);
        }

        ScorePerID.Add(1111111, 0);
        for (int i = 0; i < CultistFactionLimit; i++) {
            ScorePerID.Add(i, 0);
        }

        Debug.Log("CultistFactionLimit: " + CultistFactionLimit);
        
    }

    // Update is called once per frame
    void Update()
    {
        //           KEY     VALUE                           KEY     VALUE
        // BotsPerID[ID = 0, BotCount = 1] || IDAvailability[ID = 0, False]
        // BotsPerID[ID = 1, BotCount = 0] || IDAvailability[ID = 1, False]

        foreach(var botCount in BotsPerID) {
            
            //Debug.Log("the ID[" + botCount.Key + "] " + "has [" + botCount.Value + "] number of bots");

            if (botCount.Value <= 0) {
                IDAvalaiability[botCount.Key] = true;
            }else if (botCount.Value > 0) {
                IDAvalaiability[botCount.Key] = false;
            }

        }

        

        for (int i = 0; i < CultistFactionLimit; i++) {
            ScorePerID[i] = SacraficisPerID[i] * ScorePerSacrifice + KillsPerID[i] * ScorePerKill;
        }

        ScorePerID[1111111] = SacraficisPerID[1111111] * ScorePerSacrifice + KillsPerID[1111111] * ScorePerKill;

        var top5Keys = SacraficisPerID.OrderByDescending(kvp => kvp.Value)
                                .Take(5)
                                .Select(kvp => kvp.Key)
                                .ToList();

        var top5Values = SacraficisPerID.OrderByDescending(kvp => kvp.Value)
                                .Take(5)
                                .Select(kvp => kvp.Value)
                                .ToList();

        var top5Scores = ScorePerID.OrderByDescending(kvp => kvp.Value)
                                .Take(5)
                                .Select(kvp => kvp.Key)
                                .ToList();


        minutes = Mathf.FloorToInt(time / 60); 
        seconds = Mathf.FloorToInt(time % 60);

        timeBoard.text = string.Format("Time-{0:00}:{1:00}", minutes, seconds);

        if (menuSceneAssets == null) {
            menuSceneAssets = MenuSceneAssets.Instance;
        }

        if (menuSceneAssets != null && playerFactionName != menuSceneAssets.FactionName()) {
            if (menuSceneAssets.FactionName() != null && menuSceneAssets.FactionName().Length > 0) {
                playerFactionName = menuSceneAssets.FactionName();
            }else if (playerFactionName.Length <= 0){
                playerFactionName = "PlayerFaction";
            }
        }

        if (SceneManager.GetActiveScene().name == "GameScene" || SceneManager.GetActiveScene().buildIndex == 1) {
            

            testtext = $"ttSCORE = {cultDictionary[1111111]}: {ScorePerID[1111111]} || {playerScore.name} || playerScore length: {playerScore.text.Length} || retry panel is active: {RetryPanel.activeSelf} || {playerScore.text} || time: {time} || health: {playerScript.health}";

            cultDictionary[1111111] = playerFactionName;
            playerScore.text = cultDictionary[1111111]+":"+ScorePerID[1111111].ToString();
            firstPlace.text = $"{cultDictionary[top5Scores[0]]}: {ScorePerID[top5Scores[0]]}";
            secondPlace.text = $"{cultDictionary[top5Scores[1]]}: {ScorePerID[top5Scores[1]]}";
            thirdPlace.text = $"{cultDictionary[top5Scores[2]]}: {ScorePerID[top5Scores[2]]}";
            fourthPlace.text = $"{cultDictionary[top5Scores[3]]}: {ScorePerID[top5Scores[3]]}";
            fifthPlace.text = $"{cultDictionary[top5Scores[4]]}: {ScorePerID[top5Scores[4]]}";


            if (Input.GetKeyDown(KeyCode.Escape) && playerScript.health >= 1 && time > 0) {
                if (RetryPanel.activeSelf == true) {
                    RetryPanel.SetActive(false);
                    Time.timeScale = 1;
                }else {
                    RetryPanel.SetActive(true);
                    Time.timeScale = 0;
                }
            }

            //MessageText.text = "THIS must BE the GAME SCENE";//$"player score is = {neededObjectsScript.playerscore.text}, lost panel is = {neededObjectsScript.lostPanel.name}";
            if (neededObjectsScript == null) {
                MessageText.text = $"needed objects script is null, player score is = {neededObjectsScript.playerscore.text}, lost panel is = {neededObjectsScript.lostPanel.name}";
            }else if (menuSceneAssets == null) {
                MessageText.text = $"menu scene assets is null";
            }

            
        }else {

            testtext = "HOOOOW?????";
            
        }

        if (time <= 0 && RetryPanel != null) {
            Time.timeScale = 0;
            RetryPanel.SetActive(true);
            if (top5Scores[0] == 1111111) {
                shedegiText.text = "Victory";
                shedegiText.color = Color.green;
            }else {
                shedegiText.text = "You Lost";
                shedegiText.color = Color.red;
            }
        }

        if (RetryPanel != null && playerScript.health < 1) {
            RetryPanel.SetActive(true);
            Time.timeScale = 0;
        }

    }

    public void GetNeededThings(NeededObjectsScript neededThingsScript) {
        neededObjectsScript = neededThingsScript;
        botSpawnerSCript = neededThingsScript.botSpawner;
        playerScript = neededThingsScript.playerScript;
        playerScore = neededThingsScript.playerscore;
        firstPlace = neededThingsScript.leaderboards[0];
        secondPlace = neededThingsScript.leaderboards[1];
        thirdPlace = neededThingsScript.leaderboards[2];
        fourthPlace = neededThingsScript.leaderboards[3];
        fifthPlace = neededThingsScript.leaderboards[4];
        timeBoard = neededThingsScript.timeboard;
        shedegiText = neededThingsScript.shedegi;
        RetryPanel = neededThingsScript.lostPanel;
        MessageText = neededThingsScript.messageText;
    }

    public IEnumerator Timer() {

        yield return new WaitForSeconds(0.1f);
        while (time > 0) {
            time -= 1;
            yield return new WaitForSeconds(1);
        }
    }

    public void GetPlayerScript(Player playerScript) {
        playerScript = this.playerScript;
    }

    public void GetPlayerPrefab(GameObject player) {
        playerPrefab = player;
        playerScript = playerPrefab.GetComponent<Player>();
    }

    public void GetBotSpawnerScript(GameObject botSpawner) {
        botSpawnerSCript = botSpawner.GetComponent<BotSpawner>();
    }

    public void OnSceneRestartClears() {
        IDAvalaiability.Clear();
        BotsPerID.Clear();
        SacraficisPerID.Clear();
        cultDictionary.Clear();
        ScorePerID.Clear();
        KillsPerID.Clear();

        time = setTime;

        for (int i = 0; i < CultistFactionLimit; i++) {
            IDAvalaiability.Add(i, true);
        }

        for (int i = 0; i < CultistFactionLimit; i++) {
            BotsPerID.Add(i, 0);
        }

        SacraficisPerID.Add(1111111, 0);
        for (int i = 0; i < CultistFactionLimit; i++) {
            SacraficisPerID.Add(i, 0);
        }

        cultDictionary.Add(1111111, "PlayerFaction");
        for (int i = 0; i < CultistFactionLimit; i++) {
            cultDictionary.Add(i, cultNameWords[i]);
        }

        KillsPerID.Add(1111111, 0);
        for (int i = 0; i < CultistFactionLimit; i++) {
            KillsPerID.Add(i, 0);
        }

        ScorePerID.Add(1111111, 0);
        for (int i = 0; i < CultistFactionLimit; i++) {
            ScorePerID.Add(i, 0);
        }

        SceneRestarted = true;

    }

    public float GetGameObjectsID(GameObject gameObject) {

        if (gameObject.name.Contains("Enemy")) {
            if (ObjectsWithEnemyScript.ContainsKey(gameObject)) {
                return GiveScriptAccordingToGameObject(gameObject).ID;
            }
        }else if (gameObject.name.Contains("Divine")) {
            return GiveDivineScriptAccordingToGameObject(gameObject).ID;
        }else if (gameObject.name.Contains("Civillian")) {
            return GiveCivillianScriptAccordingToGameObject(gameObject).CivilianID;
        }else if (gameObject.CompareTag("Police")) {
            return policeID;
        }else if (gameObject.tag.Contains("Player")) {
            return playerID;
        }else if (gameObject.tag.Contains("Bullet")) {
            var nameAsInt = int.Parse(gameObject.name);
            return nameAsInt;
        }

        return -1;

    }

    public int WhatID(string str) {
        if (str == "Player") {
            return playerID;
        }else if (str == "Police") {
            return policeID;
        }

        return -1;

    }


    public void CountBotsPerIDPlus(int id) {

        BotsPerID[id] = BotsPerID[id] + 1;
        //Debug.Log("The id of [" + id + "] contains " + BotsPerID[id] + " bots");
    }

    public void CountBotsPerIDMinus(int id) {

        BotsPerID[id] = BotsPerID[id] - 1;
        //Debug.Log("The id of [" + id + "] contains " + BotsPerID[id] + " bots");
    }

    public void CountSacrificesPerID(int id) {

        // counts how much civilians are sacrificed by the given id cultist faction
        SacraficisPerID[id] += 1;

    }

    public void CountKills(float id) {
        var intid = (int)id;
        if (intid != 77777) {
            KillsPerID[intid] += 1;
        }
    }

    public void ListUsedIDS(int id) {
        
        IDAvalaiability[id] = false;

    }

    public bool CheckIfIDIsAvailable(int id) {

        return IDAvalaiability[id];

    }

    public void AddCivillianScripts(GameObject gameObject, CivillianScript script) {

        ObjectsWithCivillianScript.Add(gameObject, script);
        //Debug.Log("Civillian Is Added to the dictionary");
    }

    public CivillianScript GiveCivillianScriptAccordingToGameObject(GameObject gameObject) {

        return ObjectsWithCivillianScript[gameObject];

    }

    public void AddEnemiesAndScripts(GameObject enemy, Enemy1 script) {

        ObjectsWithEnemyScript.Add(enemy, script);

    }

    public void RemoveFromEnemyDictionary(GameObject gameObject) {

        ObjectsWithEnemyScript.Remove(gameObject);

    }

    public void AddDivineCultsistScripts(GameObject divineCultist, DivineCultist script) {

        ObjectsWithDivineCultistScript.Add(divineCultist, script);

    }

    public Enemy1 GiveScriptAccordingToGameObject(GameObject gameObject) {

        return ObjectsWithEnemyScript[gameObject];
        
    }

    public DivineCultist GiveDivineScriptAccordingToGameObject(GameObject gameObject) {

        return ObjectsWithDivineCultistScript[gameObject];
        
    }

    public GameObject GiveGameObjectAccordingToScript(Enemy1 script) {

        return ObjectsWithEnemyScript.FirstOrDefault(x => x.Value == script).Key;

    }

    public void FooFunction() {
        Debug.Log("FOOOOO NIGGA!");
    }

}
