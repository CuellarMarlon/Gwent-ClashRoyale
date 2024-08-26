
using System.Collections;

namespace GwentPlus
{
    public class CardList : IEnumerable<Card>
    {
        protected List<Card> cards = new List<Card>(); //Lista para almecenar las cartas  

        public IEnumerator<Card> GetEnumerator()
        {
            return cards.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //Devuelve todas las cartas que cumplen con un predicado
        public Card Find(Func<Card, bool> predicate)
        {
            return cards.FirstOrDefault(predicate);
        }

        //Agrega una carta al tope de la lista 
        public void Push(Card card)
        {
            cards.Add(card);
        }

        //Agrega una carta al fondo de la lista 
        public void SendBottom(Card card)
        {
            cards.Insert(0, card);
        }

        //Quita la carta que esta al tope y la devuelve 
        public Card Pop()
        {
            if (cards.Count == 0) return null;
            Card topCard = cards[cards.Count - 1];
            cards.RemoveAt(cards.Count - 1);
            return topCard;
        }

        //Remueve una carta de la lista 
        public void Remove(Card card)
        {
            cards.Remove(card);
        }

        //Mezcla la lista
        public void Shuffle()
        {
            Random rng = new Random();
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = cards[k];
                cards[k] = cards[n];
                cards[n] = value;
            }
        }

        //Agregar una card a la lista
        public void Add(Card card)
        {
            cards.Add(card);
        }

        
    }
}