using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class slideManager : MonoBehaviour
{
    public slideSet slideSet;
    public int currentSlide;

    public TMP_Text text;
    public Image img;


    void Start()
    {
        displaySlide(slideSet.slides[currentSlide]);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            nextSlide();
        }
    }

    public void displaySlide(slide slide)
    {
        text.text = slide.text;
        img.sprite = slide.img;
    }

    public void nextSlide()
    {
        if(currentSlide +1 == slideSet.slides.Length)
        {
            GameObject.FindGameObjectWithTag("SystemManager").GetComponent<SystemManager>().nextScene();
            return;
        }

        currentSlide++;

        displaySlide(slideSet.slides[currentSlide]);
    }
}
