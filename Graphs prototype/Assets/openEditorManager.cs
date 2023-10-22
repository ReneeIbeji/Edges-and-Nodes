using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openEditorManager : MonoBehaviour
{
    SystemManager manager;
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("SystemManager").GetComponent<SystemManager>();
    }

    public void openEditor()
    {
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().Play("pressButton");
        manager.openEditor();
    }
}
