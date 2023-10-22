using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemManager : MonoBehaviour
{
    [TextArea]
    public string graphToLoad;
    [TextArea]
    public string defultGraph;
    public slideSet slidesToLoad;

    public string[] sceneNames;
    public int currentSceneNum;

    public graphMode startGraphMode;

    public static SystemManager instance;



    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }





    public void setValues(GraphMenuItem item)
    {
        graphToLoad = item.graphToLoad;
        sceneNames = item.scenes;
        startGraphMode = item.startGraphMode;
        slidesToLoad = item.slides;
        nextScene();
    }


    public void nextScene()
    {
        SceneManager.LoadScene(sceneNames[currentSceneNum]);
        currentSceneNum++;
    }

    public void openEditor()
    {
        startGraphMode = graphMode.edit;
        SceneManager.LoadScene("Editor");
    }
    public void openMainMenu()
    {
        SceneManager.LoadScene("Menu");
        currentSceneNum = 0;
    }
}
