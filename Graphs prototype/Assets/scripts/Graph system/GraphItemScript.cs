using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphItemScript : MonoBehaviour
{
    public GraphItemType type;
    public node nodeItem;
    public vertex vertexItem;

    SpriteRenderer sprite;

    Color baseColor;
    Color blue;
    Color green;


    private void Awake()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        baseColor = sprite.color;
        blue = new Color(129f / 255f, 167f / 255f, 1f, 1f);
        green = new Color(146f / 255f, 217f/255f, 148f/255f, 1f);


    }

   
    public void hightlightPrimary()
    {
        sprite.color = blue;

    }

    public void highlightSecondary()
    {
        sprite.color = green;
    }

    public void resetColour()
    {
        sprite.color = baseColor;
    }
}



