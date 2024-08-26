using UnityEngine;
using UnityEngine.EventSystems;
using GwentPlus;

public class DiscardCard : MonoBehaviour, IPointerClickHandler
{
    // Incrementa el contador global y alterna el turno entre los jugadores.
    public void OkButton()
    {
        GameManager.Instance.counter += 1;
        GameManager.Instance.ToggleCurrentPlayer();

        // Verifica qué jugador es el actual y mueve todas las cartas del panel de mano al mazo correspondiente.
        if (GameManager.Instance.currentPlayer == 2)
        {
            // Obtiene el número de cartas en el panel de mano.
            int childCount = GameManager.Instance.handPanel.transform.childCount;

            // Itera sobre todas las cartas en el panel de mano.
            for (int i = 0; i < childCount; i++)
            {
                // Accede a cada carta por índice.
                Transform child = GameManager.Instance.handPanel.transform.GetChild(0);

                // Mueve cada carta al mazo del jugador 1.
                child.SetParent(GameManager.Instance.p1Hand.transform, false);
                // Debug.Log("Carta añadida a la mano del jugador 1");
            }
            
            // Llama a la función para redibujar cartas.
            GameManager.Instance.DrawnCard(GameManager.Instance.counterDrawn);
            GameManager.Instance.counterDrawn = 0;
            // Baraja las cartas en el mazo del jugador 1.
            GameManager.Instance.ShuffleChildren(GameManager.Instance.p1Deck.transform);
            // Reinicia el contador de clics.
            GameManager.Instance.counterClick = 0;
            // Llama al método Discard para el jugador 2.
            GameManager.Instance.Discard(GameManager.Instance.p2Hand);
        }
        else if (GameManager.Instance.currentPlayer == 1)
        {
            // Obtiene el número de cartas en el panel de mano.
            int childCount = GameManager.Instance.handPanel.transform.childCount;

            // Itera sobre todas las cartas en el panel de mano.
            for (int i = 0; i < childCount; i++)
            {
                // Accede a cada carta por índice.
                Transform child = GameManager.Instance.handPanel.transform.GetChild(0);

                // Mueve cada carta al mazo del jugador 2.
                child.SetParent(GameManager.Instance.p2Hand.transform, false);
                // Debug.Log("Carta añadida a la mano del jugador 2");
            }

            // Llama a la función para redibujar cartas.
            GameManager.Instance.DrawnCard(GameManager.Instance.counterDrawn);
            // Baraja las cartas en el mazo del jugador 2.
            GameManager.Instance.ShuffleChildren(GameManager.Instance.p2Deck.transform);
        }
        // Debug.Log("Counter: " + GameManager.Instance.counter);

        // Desactiva el panel de descarte cuando se han realizado 2 acciones.
        if(GameManager.Instance.counter == 2)
        {
            GameManager.Instance.discardPanel.SetActive(false);
            GameManager.Instance.StartTurn();
        }
    }

    // Maneja el evento de clic en la carta. Aumenta el contador de clics y mueve la carta al mazo correspondiente.
    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.counterClick += 1;

        if(GameManager.Instance.counterClick <= 2)
        {
            GameManager.Instance.counterDrawn++;
            // Debug.Log("currentplayer" + GameManager.Instance.currentPlayer);
            if (GameManager.Instance.currentPlayer == 1)
            {
                // Obtiene el transform de la carta.
                Transform cardTransform = GetComponent<Transform>();

                // Cambia el padre de la carta al mazo del jugador 1.
                cardTransform.SetParent(GameManager.Instance.p1Deck.transform, false);
                // Debug.Log("Carta movida al mazo del jugador 1");
            }
            else if (GameManager.Instance.currentPlayer == 2)
            {
                // Obtiene el transform de la carta.
                Transform cardTransform = GetComponent<Transform>();

                // Cambia el padre de la carta al mazo del jugador 2.
                cardTransform.SetParent(GameManager.Instance.p2Deck.transform, false);
                // Debug.Log("Carta movida al mazo del jugador 2");
            }
        }
    }
}