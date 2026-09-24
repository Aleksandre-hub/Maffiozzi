using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meleehit : MonoBehaviour
{

    [SerializeField] private GameObject MonsterParent;
    [SerializeField] private MonsterScript MonsterParenScrtipt;
    public Color FactionColor;
    public int ID = 55555555; 

    // Start is called before the first frame update
    void Start()
    {
        name = ID.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetOwner(GameObject obj) {
        MonsterParent = obj;
    }

    public void GetID(MonsterScript parentsScript) {
        ID = parentsScript.ID;
        name = ID.ToString();
        MonsterParenScrtipt = parentsScript;
        FactionColor = parentsScript.Brand.color;
    }

    public GameObject TellOwner() {
        return MonsterParent;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Hit the enemy");
        Destroy(gameObject);
    }

}
