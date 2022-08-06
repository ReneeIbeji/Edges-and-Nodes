using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toolBoxItem :MonoBehaviour
{
    public string itemName;
    public GraphUIManager graphUIManager;
    public bool selected = false;

    public void onClick()
    {
        if (!selected) { 
            graphUIManager.OnToolBoxItemClicked(itemName,gameObject); 
            selected = true;
        }
        
        
    }
}
