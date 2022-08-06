using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class robotControl : MonoBehaviour
{

    SpriteRenderer sprite;

    private void Start()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        hide();
    }
    public void hide()
    {
        sprite.enabled = false;
    }

    public void show()
    {
        sprite.enabled = true;
    }
}
