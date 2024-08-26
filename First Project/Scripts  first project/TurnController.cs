using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TurnController : MonoBehaviour
{
    // public static TurnController turnController { get; private set; }
    public BoxCollider2D rowM_P1, rowR_P1, rowS_P1; // Referencia a las filas del jugador 1
    public BoxCollider2D weatherM_P1, weatherR_P1, weatherS_P1; // Referencia a las zonas de clima del jugador 1
    public BoxCollider2D rowM_P2, rowR_P2, rowS_P2; // Referencia a las filas del jugador 2
    public BoxCollider2D weatherM_P2, weatherR_P2, weatherS_P2; // Referencia a las zonas de clima del jugador 2
    
    public GameObject hand_P1, hand_P2;
    public GameObject passTurnButtonGameObject; // Referencia al botón de pasar turno
    public Button passTurnButton;

    public GameObject rowMP1, rowRP1, rowSP1, rowMP2, rowRP2, rowSP2;
    public GameObject weatherMP1, weatherRP1, weatherSP1, weatherMP2, weatherRP2, weatherSP2;

    public GameObject graveyard_P1;
    public GameObject graveyard_P2;


    public static int currentPlayer = DragDrop.currentPlayer; // 1 para el jugador 1, 2 para el jugador 2
    public static bool hasPlayerPlayed = false; // Añade esta línea en la clase TurnController
    public static int turnPassCounter = 0; // Añade esta línea en la clase TurnController
    public static bool endRound = false;

    void Start()
    {
        passTurnButton = passTurnButtonGameObject.GetComponent<Button>();
        ToggleTurns();
    }

    public void OnPassTurnButtonClicked()
    {
        if (!hasPlayerPlayed) 
        {
            turnPassCounter++;
            endRound = true;
            if(turnPassCounter >= 2)
            {
                hasPlayerPlayed = false;
                turnPassCounter = 0;
                endRound = false;
                currentPlayer = (currentPlayer % 2) + 1;

                SendCardToGraveyard(rowMP1, graveyard_P1);
                SendCardToGraveyard(rowMP2, graveyard_P2);
                SendCardToGraveyard(rowRP1, graveyard_P2);
                SendCardToGraveyard(rowRP2, graveyard_P2);
                SendCardToGraveyard(rowSP1, graveyard_P2);
                SendCardToGraveyard(rowSP2, graveyard_P2);
                
                SendCardToGraveyard(weatherMP1, graveyard_P2);
                SendCardToGraveyard(weatherMP2, graveyard_P2);
                SendCardToGraveyard(weatherRP1, graveyard_P2);
                SendCardToGraveyard(weatherRP2, graveyard_P2);
                SendCardToGraveyard(weatherSP1, graveyard_P2);
                SendCardToGraveyard(weatherSP2, graveyard_P2);

                GameManager.Instance.CheckForWinCondition(); // Llama a la lógica para verificar las condiciones de ganador
                
            }
            else 
            {
                ToggleTurns();

            }
        }
        else
        {
            ToggleTurns();
            hasPlayerPlayed = false;
        }

    }
    
    public void ToggleTurns()
    {
        // Desactivar las filas y zonas de clima del jugador actual

        rowM_P1.enabled = currentPlayer != 1;
        rowR_P1.enabled = currentPlayer != 1;
        rowS_P1.enabled = currentPlayer != 1;
    
        rowM_P2.enabled = currentPlayer != 2;
        rowR_P2.enabled = currentPlayer != 2;
        rowS_P2.enabled = currentPlayer != 2;

        weatherM_P1.enabled = currentPlayer != 2;
        weatherR_P1.enabled = currentPlayer != 2;
        weatherS_P1.enabled = currentPlayer != 2;

        weatherM_P2.enabled = currentPlayer != 1;
        weatherR_P2.enabled = currentPlayer != 1;
        weatherS_P2.enabled = currentPlayer != 1;

        if (!endRound)
        {   
            currentPlayer = (currentPlayer % 2) + 1;        
        }
        else
        {
            if(currentPlayer == 2)
            {
                hand_P2.SetActive(false);
                hand_P1.SetActive(true);

            }
            else if (currentPlayer == 1)
            {
                hand_P1.SetActive(false);
                hand_P2.SetActive(true);

            }
        }
        
        if(currentPlayer == 2 && !endRound)
        {
            hand_P1.SetActive(false);
            hand_P2.SetActive(true);
        }
        else if (currentPlayer == 1 && !endRound)
        {
            hand_P1.SetActive(true);
            hand_P2.SetActive(false);

        }

        // Desactivar el botón de pasar turno después de cambiar el turno
        passTurnButton.interactable = true;
    }


    public void SendCardToGraveyard(GameObject position, GameObject graveyard)
    {
        // Obtener todas las cartas con el componente CardDisplay que son hijas del objeto padre
        CardDisplay[] cards = CardsInBoard(position);

        // Suponiendo que tienes un GameObject asignado para el cementerio
        foreach (CardDisplay card in cards)
        {
            // Mover la carta al cementerio
            card.transform.SetParent(graveyard.transform);
        }
    }


    public CardDisplay[] CardsInBoard(GameObject position)
    {
        CardDisplay[] cards = position.GetComponentsInChildren<CardDisplay>();
        return cards;
    }
}