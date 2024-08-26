
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

        public CardList Hand => Hands[TriggerPlayer];
        public CardList Board => Fields[TriggerPlayer];
        public CardList Graveyard => Graveyards[TriggerPlayer];
        public CardList Deck => Decks[TriggerPlayer];

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
    }
}