using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectLeader : MonoBehaviour
{   
    public static EffectLeader effectLeader { get; set; }
    public GameObject board_P1;
    public Button buttonLeader1;
    public Button buttonLeader2;
    public GameObject gameObject1;
    public GameObject gameObject2;

    void Start()
    {
        gameObject1.SetActive(false);
        gameObject2.SetActive(false);

    }

    public void OnEffectLeader2ButtonClicked()
    {

        gameObject2.SetActive(true);

        if (board_P1 != null)
        {
            // Obtener todos los objetos hijos de otherZone que tengan el componente CardDisplay
            CardDisplay[] cardDisplays = board_P1.GetComponentsInChildren<CardDisplay>();
            foreach (CardDisplay cardDisplay in cardDisplays)
            {
                // Acceder a la tarjeta de cada CardDisplay
                Card card = cardDisplay.card;
                card.power -= 1;
                cardDisplay.power.text = card.power.ToString();
            }

        }

    }
    public void OnEffectLeader1ButtonClicked()
    {

        gameObject1.SetActive(true); 

        if (board_P1 != null)
        {
            // Obtener todos los objetos hijos de otherZone que tengan el componente CardDisplay
            CardDisplay[] cardDisplays = board_P1.GetComponentsInChildren<CardDisplay>();
            foreach (CardDisplay cardDisplay in cardDisplays)
            {
                // Acceder a la tarjeta de cada CardDisplay
                Card card = cardDisplay.card;
                card.power += 1;
                cardDisplay.power.text = card.power.ToString();
            }

           
        }
    }

    
}

