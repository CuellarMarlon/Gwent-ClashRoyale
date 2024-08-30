using System.Collections.Generic;
using UnityEngine;

namespace GwentPlus
{  
    public class GameContext
    {
        public  static GameContext Instance = new GameContext(); //instancia estatica de la clase para poder usarla un unity
        public int TriggerPlayer { get; set; } = 1;
        public Dictionary<int, CardList> Boards { get; set; } = new Dictionary<int, CardList>(); // Representa el tablero completo, con IDs de jugadores como clave
        public Dictionary<int, CardList> Hands { get; set; } = new Dictionary<int, CardList>();
        public Dictionary<int, CardList> Fields { get; set; } = new Dictionary<int, CardList>();
        public Dictionary<int, CardList> Graveyards { get; set; } = new Dictionary<int, CardList>();
        public Dictionary<int, CardList> Decks { get; set; } = new Dictionary<int, CardList>();

        public CardList Hand => Hands[TriggerPlayer];
        public CardList Board => GetBoard();
        public CardList Graveyard => Graveyards[TriggerPlayer];
        public CardList Deck => Decks[TriggerPlayer];


        //Filas y casillas de clima del tablero
        public CardList rowMeleeP1 = new CardList();
        public CardList rowRangeP1 = new CardList();
        public CardList rowSiegeP1 = new CardList();

        public CardList rowMeleeP2 = new CardList();
        public CardList rowRangeP2 = new CardList();
        public CardList rowSiegeP2 = new CardList();
        
        public CardList weatherMeleeP1 =  new CardList();
        public CardList weatherRangeP1 =  new CardList();
        public CardList weatherSiegeP1 =  new CardList();

        public CardList weatherMeleeP2 =  new CardList();
        public CardList weatherRangeP2 =  new CardList();
        public CardList weatherSiegeP2 =  new CardList();

        public GameContext()
        {
            Hands[1] = new CardList();
            Hands[2] = new CardList();
            Fields[1] = new CardList();
            Fields[2] = new CardList();
            Graveyards[1] = new CardList();
            Graveyards[2] = new CardList();
            Decks[1] = new CardList();
            Decks[2] = new CardList();
        }

        public CardList GetBoard()
        {
            CardList board = new CardList();

            board.AddRange(Fields[1]);
            board.AddRange(Fields[2]);

            return board;
        }
        
        public CardList HandOfPlayer(int playerId)
        {
            return Hands[playerId];
        }

        public CardList FieldOfPlayer(int playerId)
        {
            return Fields[playerId];
        }

        public CardList GraveyardOfPlayer(int playerId)
        {
            return Graveyards[playerId];
        }

        public CardList DeckOfPlayer(int playerId)
        {
            return Decks[playerId];
        }

        public void ActualiceVisual(int owner, GameObject hand, GameObject deck, GameObject cementery, GameObject rowMelee, GameObject rowRange, GameObject rowSiege, GameObject weatherMelee, GameObject weatherRange, GameObject weatherSiege, GameObject cardPrefab)
        {
            // Limpiar los hijos de cada GameObject
            void ClearChildren(GameObject parent)
            {
                foreach (Transform child in parent.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }

            ClearChildren(hand);
            ClearChildren(deck);
            ClearChildren(cementery);
            ClearChildren(rowMelee);
            ClearChildren(rowRange);
            ClearChildren(rowSiege);
            ClearChildren(weatherMelee);
            ClearChildren(weatherRange);
            ClearChildren(weatherSiege);

            // Función auxiliar para añadir cartas a un GameObject
            void AddCardsToGameObject(CardList cardList, GameObject parent, GameObject prefab)
            {
                foreach (Card card in cardList)
                {
                    GameObject cardObject = GameObject.Instantiate(prefab, parent.transform);
                    CardDisplay cardDisplay = cardObject.GetComponent<CardDisplay>();
                    cardDisplay.card = card;
                    cardDisplay.InitializeCard();
                
                }
            }

            // Añadir las cartas de las listas a los GameObjects correspondientes
            AddCardsToGameObject(Hands[owner], hand, cardPrefab);
            AddCardsToGameObject(Decks[owner], deck, cardPrefab);
            AddCardsToGameObject(Graveyards[owner], cementery, cardPrefab);
            AddCardsToGameObject(owner == 1 ? rowMeleeP1 : rowMeleeP2, rowMelee, cardPrefab);
            AddCardsToGameObject(owner == 1 ? rowRangeP1 : rowRangeP2, rowRange, cardPrefab);
            AddCardsToGameObject(owner == 1 ? rowSiegeP1 : rowSiegeP2, rowSiege, cardPrefab);
            AddCardsToGameObject(owner == 1 ? weatherMeleeP1 : weatherMeleeP2, weatherMelee, cardPrefab);
            AddCardsToGameObject(owner == 1 ? weatherRangeP1 : weatherRangeP2, weatherRange, cardPrefab);
            AddCardsToGameObject(owner == 1 ? weatherSiegeP1 : weatherSiegeP2, weatherSiege, cardPrefab);
        }


        public void ActualiceContext(int owner, GameObject hand, GameObject deck, GameObject cementery, GameObject rowMelee, GameObject rowRange, GameObject rowSiege, GameObject weatherMelee, GameObject weatherRange, GameObject weatherSiege)
        {
            // Limpiar las listas actuales
            Hands[owner].Clear();
            Decks[owner].Clear();
            Graveyards[owner].Clear();
            Fields[owner].Clear();
            if (owner == 1)
            {
                rowMeleeP1.Clear();
                rowRangeP1.Clear();
                rowSiegeP1.Clear();
                weatherMeleeP1.Clear();
                weatherRangeP1.Clear();
                weatherSiegeP1.Clear();
            }
            else
            {
                rowMeleeP2.Clear();
                rowRangeP2.Clear();
                rowSiegeP2.Clear();
                weatherMeleeP2.Clear();
                weatherRangeP2.Clear();
                weatherSiegeP2.Clear();
            }
        
            // Función auxiliar para extraer cartas de un GameObject
            void ExtractCards(GameObject parent, CardList cardList)
            {
                foreach (Transform child in parent.transform)
                {
                    CardDisplay cardDisplay = child.GetComponent<CardDisplay>();
                    Card card = cardDisplay.card;
                    if (card != null)
                    {
                        cardList.Add(card);
                    }
                }
            }
        
            // Actualizar las listas con las cartas extraídas
            ExtractCards(hand, Hands[owner]);
            ExtractCards(deck, Decks[owner]);
            ExtractCards(cementery, Graveyards[owner]);
            ExtractCards(rowMelee, owner == 1 ? rowMeleeP1 : rowMeleeP2);
            ExtractCards(rowRange, owner == 1 ? rowRangeP1 : rowRangeP2);
            ExtractCards(rowSiege, owner == 1 ? rowSiegeP1 : rowSiegeP2);
            ExtractCards(weatherMelee, owner == 1 ? weatherMeleeP1 : weatherMeleeP2);
            ExtractCards(weatherRange, owner == 1 ? weatherRangeP1 : weatherRangeP2);
            ExtractCards(weatherSiege, owner == 1 ? weatherSiegeP1 : weatherSiegeP2);
        
            // Actualizar los Fields con las cartas de las filas y los climas
            if (owner == 1)
            {
                Fields[owner].AddRange(rowMeleeP1);
                Fields[owner].AddRange(rowRangeP1);
                Fields[owner].AddRange(rowSiegeP1);
                Fields[owner].AddRange(weatherMeleeP1);
                Fields[owner].AddRange(weatherRangeP1);
                Fields[owner].AddRange(weatherSiegeP1);
            }
            else
            {
                Fields[owner].AddRange(rowMeleeP2);
                Fields[owner].AddRange(rowRangeP2);
                Fields[owner].AddRange(rowSiegeP2);
                Fields[owner].AddRange(weatherMeleeP2);
                Fields[owner].AddRange(weatherRangeP2);
                Fields[owner].AddRange(weatherSiegeP2);
            }
            
        }

    }
}