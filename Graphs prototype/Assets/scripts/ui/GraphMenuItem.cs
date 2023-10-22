using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GraphMenuOption", menuName = "ScriptableObjects/GraphMenuOption", order = 1)]
public class GraphMenuItem : ScriptableObject
{
    public string title;
    public Sprite image;
    public string[] scenes;

    [TextArea]
    public string graphToLoad;
    public graphMode startGraphMode;
    public slideSet slides;

}
