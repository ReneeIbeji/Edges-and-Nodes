using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontMenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject graphMenu;

    public void openGraphMenu()
    {
        graphMenu.SetActive(true);
    }
    public void closeGraphMenu()
    {
        graphMenu.SetActive(false);
    }
}
