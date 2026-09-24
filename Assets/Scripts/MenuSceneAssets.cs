using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuSceneAssets : MonoBehaviour
{
    public static MenuSceneAssets Instance;

    private GameManager gameManager;

    public InputField factionName;

    public bool SceneChange = true;

    private void Awake()
    {
        // Ensure there's only one instance of the class
        if (Instance == null)
        {
            // Set the instance to this object if it's the first one
            Instance = this;
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
        gameManager = GameManager.gameManagerInstance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public String FactionName() {
        if (factionName.text == null) {
            factionName.text = "";
        }
        return factionName.text;
    }
}
