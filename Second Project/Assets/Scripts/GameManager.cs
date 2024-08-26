using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using GwentPlus;

public class GameManager : MonoBehaviour
{
    // Crear una propiedad estática pública para acceder a la instancia del GameManager desde cualquier parte del código
    public static GameManager Instance { get; private set; }

    public GameObject cardPrefab; //Prefab de cartas
    public GameObject p1Deck, p2Deck, p1Hand, p2Hand; //Obejetos del tablero
    public GameObject p1R_M, p1R_R, p1R_S, p2R_M, p2R_R, p2R_S; // Filas del  tablero
    public GameObject p1W_M, p1W_R, p1W_S, p2W_M, p2W_R, p2W_S; // Climas del  tablero
    public GameObject cementeryP1, cementeryP2; 
    public List<Card> p1Cards = new List<Card>(); //Cartas seleccionadas por el jugador uno
    public List<Card> p2Cards = new List<Card>(); //Cartas seleccionadas por el jugador dos
    public GameObject discardPanel; //Panel para descartar las dos cartas
    public GameObject handPanel; //Objeto que  contiene las cartas de la mano del jugador en el panel 
    
    public int currentPlayer = 1; 
    public int counter = 0; //Contador para llevar a cabo la desactivacion del panel 
    public int counterDrawn = 0; //Contador para saber cuantas cartas robar 
    public int counterClick = 0; //Contador de los click sobre las  cartas 

    public bool isFirst = true;
    private int otherCounter = 0; //contador para verificar si es cierto que es la primera vez que se roba

    void Awake()
    {
        // Comprueba si ya existe una instancia de GameManager
        if (Instance == null)
        {
            // Si no existe, establece esta instancia como la instancia principal
            Instance = this;
            // Hace que el objeto no sea destruido automáticamente al cargar una nueva escena
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Si ya existe una instancia, destruye el objeto actual para evitar duplicados
            Destroy(gameObject);
        }
    }


    void Start()
    {
        discardPanel.SetActive(false);
        PrepareGame(p1Cards, p1Deck);
        PrepareGame(p2Cards, p2Deck);
        ShuffleChildren(p1Deck.transform);
        ShuffleChildren(p2Deck.transform);
        DrawnCard(10);
    }

    public void PrepareGame(List<Card> playerCards, GameObject playerDeck)
    {
        // Instanciar las cartas del primer jugador
        foreach (var card in playerCards) 
        {
            // Instanciar la carta
            GameObject cardInstance = GameObject.Instantiate(cardPrefab, playerDeck.transform);
            
            // Obtener el componente CardDisplay
            CardDisplay cardDisplay = cardInstance.GetComponent<CardDisplay>();

            // Asignar las propiedades del scriptableOject
            cardDisplay.card = card;
            cardDisplay.InitializeCard();

            // // Agrerar la carta a la lista deck
            // deckP1.AddLast(card);
        }
    }

    public void ShuffleChildren(Transform deck)
    {
        // Obtiene todos los hijos del contenedor del mazo
        Transform[] children = new Transform[deck.childCount];
        for (int i = 0; i < deck.childCount; i++)
        {
            children[i] = deck.GetChild(i);
        }

        // Baraja los hijos
        System.Random rnd = new System.Random();
        for (int i = 0; i < children.Length; i++)
        {
            Transform temp = children[i];
            int randomIndex = rnd.Next(i, children.Length -1);
            children[i] = children[randomIndex];
            children[randomIndex] = temp;
        }

        // Reordena los hijos en el contenedor del mazo según la nueva secuencia aleatoria
        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetParent(null); // Temporalmente remueve el hijo del contenedor para evitar errores de indexación al reordenar
            children[i].SetParent(deck); // Vuelve a agregar el hijo en la nueva posición
        }
    }

    public void DrawnCard(int n)
    {
        if(isFirst)
        {   
            StartCoroutine(MoveCardAnimated(p1Deck, p1Hand, n));
            StartCoroutine(MoveCardAnimated(p2Deck, p2Hand, n));
        }
        else if(currentPlayer == 2)
        {
            StartCoroutine(MoveCardAnimated(p1Deck, p1Hand, n));
        }
        else if (currentPlayer == 1)
        {
            StartCoroutine(MoveCardAnimated(p2Deck, p2Hand, n));
        }

        float durationPerCard = 0.6f;
        float totalDuration = n * durationPerCard;
        
        if(isFirst && otherCounter == 0)
        {
            otherCounter += 1;

            Invoke("CallDiscard", totalDuration);
            isFirst = false;
        }

        // Función para mover una carta de un deck a una mano
        IEnumerator MoveCardAnimated(GameObject deck, GameObject hand, int count)
        {
            // Recorrer los hijos del deck para seleccionar las primeras 10 cartas
            for (int i = 0; i < count; i++)
            {
                Transform cardTransform = deck.transform.GetChild(0);
                Vector3 startPosition = deck.transform.position;
                Vector3 endPosition = hand.transform.position;
                float duration = 1f; // Duración de la animación en segundos

                //Comenzar la animacion para mover la carta 
                yield return MoveCard(cardTransform, startPosition, endPosition, duration);

                // Cambiar el padre de la carta al final de la animación
                cardTransform.SetParent(hand.transform, false);
            }
        }

        IEnumerator MoveCard(Transform cardTransform, Vector3 startPosition, Vector3 endPosition, float duration)
        {
            float elapsed = 0.5f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                cardTransform.position = Vector3.Lerp(startPosition, endPosition, t);
                yield return null;
            }
        }
    
    }

    public void CallDiscard()
    {
        discardPanel.SetActive(true);
        Discard(p1Hand);
    }

    public void Discard(GameObject hand)
    {
        //Mover las cartas de la mano del jugador al panel 
        for(int i = 0; i < 10; i++)
        {
            Transform cardTransform = hand.transform.GetChild(0);
            cardTransform.SetParent(handPanel.transform, false);
        }
    }

    // Método para alternar entre el jugador 1 y el jugador 2
    public void ToggleCurrentPlayer()
    {
        currentPlayer = currentPlayer % 2 + 1;
    }
    
    public void StartTurn()
    {
        if (currentPlayer == 1)
        {
            p1Hand.SetActive(false);
            p2Hand.SetActive(true);
        }
        else if (currentPlayer == 2)
        {            
            p1Hand.SetActive(true);
            p2Hand.SetActive(false);
        }

        currentPlayer = currentPlayer % 2 + 1;
    }

    public void SendCardsToCementery()
    {
        List<GameObject> boardObjectsP1 = new List<GameObject>(){ p1R_M, p1R_R, p1R_S, p1W_M, p1W_R, p1W_S };
        List<GameObject> boardObjectsP2 = new List<GameObject>(){ p2R_M, p2R_R, p2R_S, p2W_M, p2W_R, p2W_S };
    
        foreach (var boardObject in boardObjectsP1)
        {
            Transform transform = boardObject.transform;
            Debug.Log("P1 Object: " + boardObject.name + " has " + transform.childCount + " children.");
            while (transform.childCount > 0)
            {
                Transform child = transform.GetChild(0);
                if (child != null)
                {
                    child.SetParent(cementeryP1.transform);
                }
            }
        }
    
        foreach (var boardObject in boardObjectsP2)
        {
            Transform transform = boardObject.transform;
            Debug.Log("P2 Object: " + boardObject.name + " has " + transform.childCount + " children.");
            while (transform.childCount > 0)
            {
                Transform child = transform.GetChild(0);
                if (child != null)
                {
                    child.SetParent(cementeryP2.transform);
                }
            }
        }    
    }


    
    void Update()
    {
        ActivateDragsInHands();
        void ActivateDragsInHands()
        {
            ActivateDrags(p1Hand);
            ActivateDrags(p2Hand);
        }

        void ActivateDrags(GameObject hand)
        {
            foreach (Transform child in hand.transform)
            {
                DragDrop dragComponent = child.GetComponent<DragDrop>();
                if (dragComponent != null)
                {
                    dragComponent.enabled = true;
                }
            }
        }
    }


}