using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphOptionPanel : MonoBehaviour
{
    public GraphMenuItem item;
    public TMP_Text titleLable;
    public Image image;
    public SystemManager manager;




    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("SystemManager").GetComponent<SystemManager>();
        updateVisuals();
    }

    public void updateVisuals()
    {
        titleLable.text = item.title;
        image.sprite = item.image;
    }

    public void onClick()
    {
        manager.setValues(item);
    }


}
