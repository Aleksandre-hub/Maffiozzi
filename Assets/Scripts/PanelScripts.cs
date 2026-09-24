using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelScripts : MonoBehaviour
{

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.gameManagerInstance;

        gameObject.SetActive(false);
        //Debug.Log("panel nigga lord sir niggarion niggacus LORD, water MelonIA");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
