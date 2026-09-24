using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{

    private GameManager gameManager;
    private MenuSceneAssets menuSceneAssets;
    [SerializeField] private InputField factionName;

    // Start is called before the first frame update
    void Start()
    {

        menuSceneAssets = MenuSceneAssets.Instance;
        gameManager = GameManager.gameManagerInstance;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Menu Scene Buttons
    public void PlayButton() {
        menuSceneAssets.factionName = factionName;
        SceneManager.LoadScene(1);
    }

    public void RestartButton() {
        gameManager.OnSceneRestartClears();
        SceneManager.LoadScene(1);
    }

    public void MenuButton() {
        gameManager.OnSceneRestartClears();
        SceneManager.LoadScene(0);
    }

    public void QuitGame() {
        Application.Quit();
    }

}
