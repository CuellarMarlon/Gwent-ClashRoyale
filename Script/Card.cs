using System.Collections.Generic;
using UnityEngine;
using System;


[CreateAssetMenu(fileName = "NewCard", menuName = "Card/Card")]
public class Card : ScriptableObject
{
    public string cardName;
    public int power;
    public Sprite cardPhoto;
    public Faction faction;
    public CardType cardType;
    public UnitType unitType;
    public SpecialType specialType;
    public AttackType attackType; 
    public string effect;
    public List<string> validZonesTag;
    public Effect effectlLogic;
    public int initialPower;
    
    public enum AttackType { M, R, S, MR, MS, RS, MRS }
    public enum Faction { Azul, Rojo, Neutral }
    public enum CardType { UnitCard, SpecialCard, LeaderCard }
    public enum UnitType { Gold, Silver }
    public enum SpecialType { Weather, Bait, Clear, Boost }
    public enum Effect { IncreaseRowPower, ReduceRowPower, RemoveWeather, PickUpUnit, NotEffect }

    public void ActivateEffect(CardDisplay cardDropped, GameObject drop_Zone)
    {
        switch (cardDropped.card.effectlLogic)
        {
            case Effect.IncreaseRowPower:
            IncreaseRowPower(drop_Zone);
            break;
            case Effect.ReduceRowPower:
            ReduceRowPower(drop_Zone);
            break;
            case Effect.RemoveWeather:
            RemoveWeather(cardDropped);
            break;
            case Effect.PickUpUnit:
            PickUpUnit(cardDropped);
            break;
            case Effect.NotEffect:
            break;
        }
    }

    public void IncreaseRowPower(GameObject drop_Zone) //Aumento
    {   
        if(drop_Zone.tag == "M")
        {
            // Obtener todos los objetos hijos de otherZone que tengan el componente CardDisplay
            CardDisplay[] cardDisplays = drop_Zone.GetComponentsInChildren<CardDisplay>();
            
            foreach (CardDisplay cardDisplay in cardDisplays)
            {
                // Acceder a la tarjeta de cada CardDisplay
                Card card = cardDisplay.card;
                card.power += 1;
                cardDisplay.power.text = card.power.ToString();
            }       

        }
        else if (drop_Zone.tag == "R")
        {
            // Obtener todos los objetos hijos de otherZone que tengan el componente CardDisplay
            CardDisplay[] cardDisplays = drop_Zone.GetComponentsInChildren<CardDisplay>();
            
            foreach (CardDisplay cardDisplay in cardDisplays)
            {
                // Acceder a la tarjeta de cada CardDisplay
                Card card = cardDisplay.card;
                card.power += 1;
                cardDisplay.power.text = card.power.ToString();
            }       

        }
        else if (drop_Zone.tag == "S")
        {
            // Obtener todos los objetos hijos de otherZone que tengan el componente CardDisplay
            CardDisplay[] cardDisplays = drop_Zone.GetComponentsInChildren<CardDisplay>();
            
            foreach (CardDisplay cardDisplay in cardDisplays)
            {
                // Acceder a la tarjeta de cada CardDisplay
                Card card = cardDisplay.card;
                card.power += 1;
                cardDisplay.power.text = card.power.ToString();
            }       

        }


    }

    public void ReduceRowPower(GameObject drop_Zone) //Clima
    {
        if(drop_Zone.name == "WeatherSquareMP1")
        {
            // Buscar el GameObject por su nombre
            GameObject otherZone = GameObject.Find("RowMP2");

            // Verificar si el GameObject fue encontrado
            if (otherZone != null)
            {
                // Obtener todos los objetos hijos de otherZone que tengan el componente CardDisplay
                CardDisplay[] cardDisplays = otherZone.GetComponentsInChildren<CardDisplay>();

                foreach (CardDisplay cardDisplay in cardDisplays)
                {
                    // Acceder a la tarjeta de cada CardDisplay
                    Card card = cardDisplay.card;
                    card.power -= 1;
                    cardDisplay.power.text = card.power.ToString();
                }
            }

        }
        else if (drop_Zone.name == "WeatherSquareRP1")
        {
            // Buscar el GameObject por su nombre
            GameObject otherZone = GameObject.Find("RowRP2");

            // Verificar si el GameObject fue encontrado
            if (otherZone != null)
            {
                // Obtener todos los objetos hijos de otherZone que tengan el componente CardDisplay
                CardDisplay[] cardDisplays = otherZone.GetComponentsInChildren<CardDisplay>();

                foreach (CardDisplay cardDisplay in cardDisplays)
                {
                    // Acceder a la tarjeta de cada CardDisplay
                    Card card = cardDisplay.card;
                    card.power -= 1;
                    cardDisplay.power.text = card.power.ToString();
                }
            }

        }
        else if (drop_Zone.name == "WeatherSquareSP1")
        {
            // Buscar el GameObject por su nombre
            GameObject otherZone = GameObject.Find("RowSP2");

            // Verificar si el GameObject fue encontrado
            if (otherZone != null)
            {
                // Obtener todos los objetos hijos de otherZone que tengan el componente CardDisplay
                CardDisplay[] cardDisplays = otherZone.GetComponentsInChildren<CardDisplay>();

                foreach (CardDisplay cardDisplay in cardDisplays)
                {
                    // Acceder a la tarjeta de cada CardDisplay
                    Card card = cardDisplay.card;
                    card.power -= 1;
                    cardDisplay.power.text = card.power.ToString();
                }
            }

        }
        else if (drop_Zone.name == "WeatherSquareMP2")
        {
            // Buscar el GameObject por su nombre
            GameObject otherZone = GameObject.Find("RowMP1");

            // Verificar si el GameObject fue encontrado
            if (otherZone != null)
            {
                // Obtener todos los objetos hijos de otherZone que tengan el componente CardDisplay
                CardDisplay[] cardDisplays = otherZone.GetComponentsInChildren<CardDisplay>();

                foreach (CardDisplay cardDisplay in cardDisplays)
                {
                    // Acceder a la tarjeta de cada CardDisplay
                    Card card = cardDisplay.card;
                    card.power -= 1;
                    cardDisplay.power.text = card.power.ToString();
                }
            }

        }
        else if (drop_Zone.name == "WeatherSquareRP2")
        {
            // Buscar el GameObject por su nombre
            GameObject otherZone = GameObject.Find("RowRP1");

            // Verificar si el GameObject fue encontrado
            if (otherZone != null)
            {
                // Obtener todos los objetos hijos de otherZone que tengan el componente CardDisplay
                CardDisplay[] cardDisplays = otherZone.GetComponentsInChildren<CardDisplay>();

                foreach (CardDisplay cardDisplay in cardDisplays)
                {
                    // Acceder a la tarjeta de cada CardDisplay
                    Card card = cardDisplay.card;
                    card.power -= 1;
                    cardDisplay.power.text = card.power.ToString();
                }
            }

        }
        else if (drop_Zone.name == "WeatherSquareSP2")
        {
            // Buscar el GameObject por su nombre
            GameObject otherZone = GameObject.Find("RowSP1");

            // Verificar si el GameObject fue encontrado
            if (otherZone != null)
            {
                // Obtener todos los objetos hijos de otherZone que tengan el componente CardDisplay
                CardDisplay[] cardDisplays = otherZone.GetComponentsInChildren<CardDisplay>();

                foreach (CardDisplay cardDisplay in cardDisplays)
                {
                    // Acceder a la tarjeta de cada CardDisplay
                    Card card = cardDisplay.card;
                    card.power -= 1;
                    cardDisplay.power.text = card.power.ToString();
                }
            }

        }

    }

    public void RemoveWeather(CardDisplay cardDropped)
    {
        GameObject[] weatherObjectsP1 = GameObject.FindGameObjectsWithTag("WeatherP1");
        GameObject[] weatherObjectsP2 = GameObject.FindGameObjectsWithTag("WeatherP2");

        GameObject[] allWeatherObjects = new GameObject[weatherObjectsP1.Length + weatherObjectsP2.Length];
        Array.Copy(weatherObjectsP1, 0, allWeatherObjects, 0, weatherObjectsP1.Length);
        Array.Copy(weatherObjectsP2, 0, allWeatherObjects, weatherObjectsP1.Length, weatherObjectsP2.Length);

        foreach (GameObject weatherObject in allWeatherObjects)
        {
            // Buscar el componente Card dentro del objeto "clima"
            CardDisplay cardComponent = weatherObject.GetComponentInChildren<CardDisplay>();

            // Si se encuentra un componente Card, enviar la carta al cementerio
            if (cardComponent != null)
            {
                SendCardToCemetery(weatherObject, cardComponent);
               
            }
        }

        void SendCardToCemetery(GameObject weatherObject, CardDisplay cardComponent)
        {
            GameObject graveyard = GameObject.Find("GraveyardSquareP1");
            GameObject graveyard2 = GameObject.Find("GraveyardSquareP2");
            if (weatherObject.tag == "WeatherP1")
            {
                weatherObject.transform.SetParent(graveyard.transform, false);
                weatherObject.transform.position = graveyard.transform.position;
            }
            else if (weatherObject.tag == "WeatherP2")
            {
                weatherObject.transform.SetParent(graveyard2.transform, false);
                weatherObject.transform.position = graveyard2.transform.position;
            }

        
        }
    }

    public void PickUpUnit(CardDisplay cardDropped)
    {
        GameObject board_P1 = GameObject.Find("BoardPlayer1");
        GameObject board_P2 = GameObject.Find("BoardPlayer2");

        GameObject hand_P1 = GameObject.FindGameObjectWithTag("HandP1");
        GameObject hand_P2 = GameObject.FindGameObjectWithTag("HandP2");

        // Comprueba si hand_P1 o hand_P2 son null antes de continuar
        if (hand_P1 == null || hand_P2 == null)
        {
            Debug.LogError("Uno de los objetos de mano es null.");
            return; // Sal de la función si alguno de los objetos de mano es null
        }

        CardDisplay strongestCard = FindStrongestCard(board_P1, board_P2);

        if (strongestCard != null)
        {
            RemoveCardToHand(strongestCard);
        }

        CardDisplay FindStrongestCard(GameObject board_P1, GameObject board_P2)
        {
            CardDisplay strongestCard = null;
            int maxPower = 0;
            
            CardDisplay[] cardsP1 = board_P1.GetComponentsInChildren<CardDisplay>();
            foreach (CardDisplay card in cardsP1)
            {
                if (card.card.power > maxPower)
                {
                    maxPower = card.card.power;
                    strongestCard = card;
                }
            }
            CardDisplay[] cardsP2 = board_P2.GetComponentsInChildren<CardDisplay>();
            foreach (CardDisplay card in cardsP2)
            {
                if (card.card.power > maxPower)
                {
                    maxPower = card.card.power;
                    strongestCard = card;
                }
            }    

           
            return strongestCard;
        }

        void RemoveCardToHand(CardDisplay strongestCard)
        {
            if (strongestCard.card.faction == Faction.Azul)
            {
                strongestCard.transform.SetParent(hand_P1.transform, false);
                strongestCard.transform.position = hand_P1.transform.position;
                GameManager.Instance.ActiveEventTrigger(strongestCard);
                  
            }
            else if (strongestCard.card.faction == Faction.Rojo)
            {
                strongestCard.transform.SetParent(hand_P2.transform, false);
                strongestCard.transform.position = hand_P2.transform.position;
                GameManager.Instance.ActiveEventTrigger(strongestCard);
            }
        }
    }   
    
    public void Reset()
    {
        power = initialPower;
        
    }

    
}
