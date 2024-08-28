using System.Collections.Generic;
using UnityEngine;

namespace GwentPlus
{  
    public class GameContext
    {
        public  static GameContext Instance = new GameContext(); //instancia estatica de la clase para poder usarla un unity
        public int TriggerPlayer { get; set; } 
        public Dictionary<int, CardList> Boards { get; set; } = new Dictionary<int, CardList>(); // Representa el tablero completo, con IDs de jugadores como clave
        public Dictionary<int, CardList> Hands { get; set; } = new Dictionary<int, CardList>();
        public Dictionary<int, CardList> Fields { get; set; } = new Dictionary<int, CardList>();
        public Dictionary<int, CardList> Graveyards { get; set; } = new Dictionary<int, CardList>();
        public Dictionary<int, CardList> Decks { get; set; } = new Dictionary<int, CardList>();

        public CardList hand => Hands[TriggerPlayer];
        public CardList field => Fields[TriggerPlayer];
        public CardList graveyard => Graveyards[TriggerPlayer];
        public CardList deck => Decks[TriggerPlayer];


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

        
        public CardList Hand(int playerId)
        {
            return Hands[playerId];
        }

        public CardList Field(int playerId)
        {
            return Fields[playerId];
        }

        public CardList Graveyard(int playerId)
        {
            return Graveyards[playerId];
        }

        public CardList Deck(int playerId)
        {
            return Decks[playerId];
        }

        private void ActualiceVisual()
        {
            
        }

        private void ActualiceContext(int owner, GameObject hand, GameObject deck, GameObject cementery, GameObject rowMelee, GameObject rowRange, GameObject rowSiege, GameObject weatherMelee, GameObject weatherRange, GameObject weatherSiege)
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
                    Card card = child.GetComponent<Card>();
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