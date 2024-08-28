using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


namespace GwentPlus
{
    public class CardList : IEnumerable<Card>
    {
        protected List<Card> cards = new List<Card>();

        public IEnumerator GetEnumerator()
        {
            return cards.GetEnumerator();
        }

        IEnumerator<Card> IEnumerable<Card>.GetEnumerator() // This line is redundant and can be removed
        {
            return cards.GetEnumerator();
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
            System.Random rng = new System.Random();
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

        public void Clear()
        {
            cards.Clear();
        } 

        public void AddRange(List<Card> cards)
        {
            cards.AddRange();
        }

        
    }
}