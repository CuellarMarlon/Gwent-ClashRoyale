using System.Collections;
using System.Collections.Generic;
using GwentPlus;
using UnityEngine;

public class LeaderButton : MonoBehaviour
{
    public GameObject button1;
    public GameObject button2;

    public GameObject leader1Pos;
    public GameObject leader2Pos;

    

    public void OnButtonCliked()
    {
        if (GameManager.Instance.currentPlayer == 1)
        {
            CardDisplay cardDisplay = leader1Pos.transform.GetChild(0).GetComponent<CardDisplay>();
            Card card = cardDisplay.card;
            card.ActivateEffects();
            GameManager.Instance.ActualiceVisual();
            button2.SetActive(false);
            GameManager.Instance.StartTurn();
        }
        else
        {
            CardDisplay cardDisplay = leader2Pos.transform.GetChild(0).GetComponent<CardDisplay>();
            Card card = cardDisplay.card;
            card.ActivateEffects();
            GameManager.Instance.ActualiceVisual();
            button1.SetActive(false);
            GameManager.Instance.StartTurn();
        }

    }


    

}