using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toolBoxItem :MonoBehaviour
{
    public string itemName;
    public GraphUIManager graphUIManager;
    public bool selected = false;
    public Sprite itemImg;

    public void onClick()
    {
        if (!selected) {
            graphUIManager.graphManager.audioManager.Play("SelectTool");
            graphUIManager.OnToolBoxItemClicked(itemName,gameObject); 
            selected = true;
        }
        
        
    }
}
