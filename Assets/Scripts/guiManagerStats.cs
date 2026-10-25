using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class guiManagerStats : MonoBehaviour
{
    [SerializeField] GameObject[] healthPoints;

    [SerializeField] GameObject fillBar;
    double increments;

    [SerializeField] GameObject[] colorOrbs;

    private void OnEnable()
    {
        //CharcterController.onHurt += showHealthPoints;
        CharcterController.onJuiceChange += showPaintBar;
        Orb.onOrbFinish += updateOrb;
        for(int i = colorOrbs.Length-1;i>=0;i--)
        {
            colorOrbs[i].GetComponent<Image>().color = new Color(225, 225, 225);
        }
    }
    private void OnDisable()
    {
        //CharcterController.onHurt -= showHealthPoints;
        CharcterController.onJuiceChange -= showPaintBar;
        Orb.onOrbFinish -= updateOrb;
    }

    public void showHealthPoints(int playerHealth, int maxHealth, int damage)
    {
        increments = maxHealth / healthPoints.Length;
        int index = 0;
        double countUp = 0;
        while (countUp < maxHealth)
        {
            if(playerHealth > countUp)
            {
                healthPoints[index].GetComponent<Image>().enabled = true;
            } else
            {
                healthPoints[index].GetComponent<Image>().enabled = false;
            }
            index += 1;
            countUp += increments;
        }
        


    }
    public void updateOrb(string color)
    {
        switch(color)
        {
            case "Purple":
                colorOrbs[2].GetComponent<Image>().color = new Color(197, 0, 179);
                break;
            case "Blue":
                colorOrbs[2].GetComponent<Image>().color = new Color(65, 0, 197);
                break;
            case "Yellow":
                colorOrbs[2].GetComponent<Image>().color = new Color(255, 255, 117);
                break;
            case "Green":
                colorOrbs[2].GetComponent<Image>().color = new Color(0, 197, 29);
                break;
            case "Orange":
                colorOrbs[2].GetComponent<Image>().color = new Color(255, 316, 0);
                break;
            case "Red":
                colorOrbs[2].GetComponent<Image>().color = new Color(255, 8, 24);
                break;



        }
    }

    public void showPaintBar(int currentJuice, int maxJuice)
    {
       double percentage = (double) currentJuice / maxJuice;
       fillBar.GetComponent<Image>().fillAmount = (float) percentage;
    }
}
