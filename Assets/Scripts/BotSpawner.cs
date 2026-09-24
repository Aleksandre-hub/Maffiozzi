using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSpawner : MonoBehaviour
{

    [SerializeField] private GameObject bot;
    [SerializeField] private GameObject civil;
    [SerializeField] private GameObject Police;
    private float CultistspawnTimer = 0f;
    private float CivillianSpawnTimer = 0f;
    private float PoliceSpawnTimer = 0f;
    [SerializeField] private float CultistspawnInterval = 2f;
    [SerializeField] private float CivillianspawnInterval = 2f;
    [SerializeField] private float PolicespawnInterval = 2f;
    private Vector2 spawnArea;

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

    // Dictionary to store cult names with incrementing keys
    private Dictionary<int, string> cultDictionary = new Dictionary<int, string>();

    private int tagNumber = 0;
    List<string> tagList = new List<string>();

    [SerializeField] private float spawnDiameter;
    public int CultistFactionLimit;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {

        gameManager = GameManager.gameManagerInstance;
        gameManager.GetBotSpawnerScript(gameObject);

        //Debug.Log("we have: " + cultNameWords.Length + " number of cult names in reserve Sir Lord nigga watermelon");

        // Assign unique cult names to each key starting from 1
        for (int i = 0; i < cultNameWords.Length; i++) // Generating 150 names
        {
            
            cultDictionary.Add(i, cultNameWords[i]); 
        
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        CultistspawnTimer += Time.deltaTime;
        CivillianSpawnTimer += Time.deltaTime;
        PoliceSpawnTimer += Time.deltaTime;

        if (tagNumber >= CultistFactionLimit) {
            tagNumber = 0;
        }

        if (CultistspawnTimer >= CultistspawnInterval && tagNumber <= CultistFactionLimit) { // Cultist Spawner
            
            if (gameManager.CheckIfIDIsAvailable(tagNumber)) {
            

                gameManager.ListUsedIDS(tagNumber);

                spawnArea = new Vector2(Random.Range(-spawnDiameter, spawnDiameter), Random.Range(-spawnDiameter, spawnDiameter));
                CultistspawnTimer = 0;

                var bott = Instantiate(bot, spawnArea, Quaternion.identity);
                bott.GetComponent<Enemy1>().ID = tagNumber;
                bott.GetComponent<Enemy1>().FactionName = cultDictionary[tagNumber];
                bott.GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value);

                tagNumber++;    
            }else {
                tagNumber++;
            }
            

        }

        if (CivillianSpawnTimer >= CivillianspawnInterval) { // Civilian Spawner
            spawnArea = new Vector2(Random.Range(-spawnDiameter, spawnDiameter), Random.Range(-spawnDiameter, spawnDiameter));
            CivillianSpawnTimer = 0;

            var civillian = Instantiate(civil, spawnArea, Quaternion.identity);
            
        }

        if (PoliceSpawnTimer >= PolicespawnInterval) { // Civilian Spawner
            spawnArea = new Vector2(Random.Range(-spawnDiameter, spawnDiameter), Random.Range(-spawnDiameter, spawnDiameter));
            PoliceSpawnTimer = 0;

            var police = Instantiate(Police, spawnArea, Quaternion.identity);
            
        }
    }
}
