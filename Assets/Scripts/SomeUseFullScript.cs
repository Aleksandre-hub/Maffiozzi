using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SomeUseFullScript : MonoBehaviour
{

    [SerializeField] private SpriteRenderer ThisRenderer;
    private Color vitomColor;

    private float interval = 3;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        vitomColor = ThisRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (ThisRenderer.color.a <= 0) {
            Destroy(gameObject);
        }

        timer += Time.deltaTime;
        if (timer >= interval) {
            timer = 0;
            vitomColor.a -= 0.1f;
            ThisRenderer.color = vitomColor;
        }

    }


}
