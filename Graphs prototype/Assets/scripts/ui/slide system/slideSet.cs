using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "slideSet", menuName = "ScriptableObjects/slideSet", order = 2)]
public class slideSet : ScriptableObject
{
    public string slideName;
    public slide[] slides;
}


[System.Serializable]
public struct slide
{
    public Sprite img;
    [TextArea]
    public string text;
}