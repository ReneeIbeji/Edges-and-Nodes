using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphItemScript : MonoBehaviour
{
    public GraphItemType type;
    public node nodeItem;
    public vertex vertexItem;

    SpriteRenderer sprite;


    private void Awake()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }


    public void hightlightPrimary()
    {
        sprite.color = Color.blue;
    }

    public void highlightSecondary()
    {
        sprite.color = Color.green;
    }

    public void resetColour()
    {
        sprite.color = Color.white;
    }
}
