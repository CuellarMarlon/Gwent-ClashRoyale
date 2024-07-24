using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<Card> allLeaders = new List<Card>();
    public GameObject leaderSquareP1;
    public GameObject leaderSquareP2;
    public GameObject cardPrefab;

    public List<Card> allAvailableCards = new List<Card>();
    public GameObject deckSquareP1;
    public GameObject deckSquareP2;

    public GameObject handP1;
    public GameObject handP2;

     public GameObject rowM_P1;
    public GameObject rowR_P1;
    public GameObject rowS_P1;
    public GameObject rowM_P2;
    public GameObject rowR_P2;
    public GameObject rowS_P2;

    public GameObject weatherSquareM_P1;
    public GameObject weatherSquareR_P1;
    public GameObject weatherSquareS_P1;
    public GameObject weatherSquareM_P2;
    public GameObject weatherSquareR_P2;
    public GameObject weatherSquareS_P2;

    private int currentPlayer = TurnController.currentPlayer;
    private bool canPlay = DragDrop.canPlay;

    public bool hasPlayer1Played = false;
    public bool hasPlayer2Played = false;

    public TextMeshProUGUI pointsP1;
    public TextMeshProUGUI pointsP2;
    private int roundWin_P1 = 0;
    private int roundWin_P2 = 0;

    private int currentRound = 1;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {   
        PositioningLeader();
        AddCardToDeck();
        ShuffleDecks();
        StealCards(10);

    }

    private void PositioningLeader()
    {
        GameObject leaderObject1 = Instantiate(cardPrefab, leaderSquareP1.transform);
        GameObject leaderObject2 = Instantiate(cardPrefab, leaderSquareP2.transform);
        
        leaderObject1.GetComponent<CardDisplay>().card = allLeaders[0];
        leaderObject2.GetComponent<CardDisplay>().card = allLeaders[1];

        EventTrigger eventTrigger1 = leaderObject1.GetComponent<EventTrigger>();
        EventTrigger eventTrigger2 = leaderObject2.GetComponent<EventTrigger>();

        if (eventTrigger1 != null && eventTrigger2 != null)
        {
            eventTrigger1.enabled = false;
            eventTrigger2.enabled = false;

        }       
    }

    private void AddCardToDeck()
    {
        foreach (Card leader in allLeaders)
        {
            foreach (Card card in allAvailableCards)
            {   
                if (card.faction == Card.Faction.Neutral)
                {   
                    GameObject cardInstance1 = Instantiate(cardPrefab, deckSquareP1.transform);
                    GameObject cardInstance2 = Instantiate(cardPrefab, deckSquareP2.transform);
                    CardDisplay cardDisplay1 = cardInstance1.GetComponent<CardDisplay>();
                    CardDisplay cardDisplay2 = cardInstance2.GetComponent<CardDisplay>();
                    
                    if (cardDisplay1 != null && cardDisplay2 != null)
                    {
                        cardDisplay1.card = card;
                        cardDisplay1.InitializeCard();
                        
                        cardDisplay2.card = card;
                        cardDisplay2.InitializeCard();
                    }

                    EventTrigger eventTrigger1 = cardInstance1.GetComponent<EventTrigger>();
                    EventTrigger eventTrigger2 = cardInstance2.GetComponent<EventTrigger>();

                    if (eventTrigger1 != null && eventTrigger2 != null)
                    {
                        eventTrigger1.enabled = false;
                        eventTrigger2.enabled = false;
                    }        
                }
                else if (card.faction == leader.faction)
                {
                    GameObject cardInstance = Instantiate(cardPrefab, deckSquareP1.transform);
                    CardDisplay cardDisplay = cardInstance.GetComponent<CardDisplay>();
                    if (cardDisplay != null)
                    {
                        cardDisplay.card = card;
                        cardDisplay.InitializeCard();
                    }

                    EventTrigger eventTrigger = cardInstance.GetComponent<EventTrigger>();

                    if (eventTrigger != null)
                    {
                        eventTrigger.enabled = false;
                    }     
                }
                else
                {
                    GameObject cardInstance = Instantiate(cardPrefab, deckSquareP2.transform);
                    CardDisplay cardDisplay = cardInstance.GetComponent<CardDisplay>();
                    if (cardDisplay != null)
                    {
                        cardDisplay.card = card;
                        cardDisplay.InitializeCard();
                    }

                    EventTrigger eventTrigger = cardInstance.GetComponent<EventTrigger>();

                    if (eventTrigger != null)
                    {
                        eventTrigger.enabled = false;
                    }     
                }
            }
            break;    
        }    
    }

    private void ShuffleDecks()
    {
        // Recorrer todos los hijos del primer deck
        foreach (Transform child in deckSquareP1.transform)
        {
            // Guardar la referencia al primer hijo
            Transform firstChild = child;

            // Recorrer todos los hijos del segundo deck
            foreach (Transform child2 in deckSquareP2.transform)
            {
                // Cambiar la posición aleatoriamente
                if (Random.value < 0.5f)
                {
                    // Si el valor aleatorio es menor que 0.5, mover el primer hijo al segundo deck
                    firstChild.SetAsFirstSibling();
                    child2.SetAsLastSibling();
                }
                else
                {
                    // Si el valor aleatorio es mayor o igual a 0.5, mover el segundo hijo al primer deck
                    child2.SetAsFirstSibling();
                    firstChild.SetAsLastSibling();
                }
            }
        }
    }

    private void StealCards(int n)
    {
        // Función para mover una carta de un deck a una mano
        void MoveCard(GameObject deck, GameObject hand)
        {
            // Recorrer los hijos del deck para seleccionar las primeras 10 cartas
            for (int i = 0; i < n; i++)
            {
                Transform cardTransform = deck.transform.GetChild(i);
                cardTransform.SetParent(hand.transform, false);
                
                EventTrigger eventTrigger = cardTransform.GetComponent<EventTrigger>();
                if (eventTrigger!= null)
                {
                    eventTrigger.enabled = true;
                }
            }

        }

        // Mover las primeras 10 cartas del primer deck a la mano del primer jugador
        MoveCard(deckSquareP1, handP1);

        // Mover las primeras 10 cartas del segundo deck a la mano del segundo jugador
        MoveCard(deckSquareP2, handP2);
    }
  
    public void CheckForWinCondition()
    {
        currentRound++;

        int points_P1 = PointsCounter.totalPoints_P1;
        int points_P2 = PointsCounter.totalPoints_P2;

        if (points_P1 > points_P2)
        {   
            roundWin_P1++;
            pointsP1.text = "Round: " + roundWin_P1.ToString(); 
            StealCards(2);

        }
        if (points_P2 > points_P1)
        {
            roundWin_P2++;
            pointsP2.text = "Round: " + roundWin_P2.ToString(); 
            StealCards(2);

        }
        if (points_P1 == points_P2)
        {
            roundWin_P1++;
            pointsP1.text = "Round: " + roundWin_P1.ToString(); 

            roundWin_P2++;
            pointsP2.text = "Round: " + roundWin_P2.ToString(); 
            StealCards(2);

        }

       
        
        if(roundWin_P1 > roundWin_P2 && roundWin_P1 == 2)
        {
            Debug.Log("Player 1 gano el juego!!");
            SceneManager.LoadScene(3);
        } 
        if (roundWin_P2 > roundWin_P1 && roundWin_P2 == 2)
        {
            Debug.Log("Player 2 gano el juego!!");
            SceneManager.LoadScene(4);
        }
        if (roundWin_P1 == 2 && roundWin_P2 == roundWin_P1)
        {
            Debug.Log("Empate!! ");
            SceneManager.LoadScene(5);
        }

    }

    public void ActiveEventTrigger(CardDisplay draggableObject)
    {
        EventTrigger eventTrigger = draggableObject.GetComponent<EventTrigger>();
        if (eventTrigger!= null && draggableObject)
        {
            eventTrigger.enabled = true;
        }
    }

    public void Update()
    {

    }
}