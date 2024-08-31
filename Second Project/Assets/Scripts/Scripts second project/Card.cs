using System.Collections.Generic;
using System;
using UnityEngine;

namespace GwentPlus
{
    public enum CardType { Oro, Plata, Clima, Aumento, Despeje, Senuelo, Lider }
    public enum Effect { Oro, Plata, Clima, Aumento, Despeje, Senuelo, Dark, Celestial, NoEffect }
    public enum Faction { Dark, Celestial, Neutral } 
    public enum Range { Melee, Ranged, Siege }

    [CreateAssetMenu(fileName = "NewCard", menuName = "Card/Card")]
    public class Card : ScriptableObject
    {
        public  int Owner;
        public bool IsCreated;
        public Sprite Photo;
        public CardType Type;
        public Effect Effect;
        public string Name;
        public Faction Faction;
        public int Power;
        public Range[] Range;
        public List<Effects> OnActivation;
        public EffectCreated EffectCreated;

        public void ActivateEffects()
        {
            if (IsCreated)
            {
                foreach (var effect in OnActivation)
                {
                    Debug.Log("Owner: " + Owner);
                    ActivateSpecificEffect(effect, effect.Params);
                }
            }
            else 
            {
                if (Effect == Effect.Oro)
                {
                    // EffectOro();
                }
                else if (Effect == Effect.Plata)
                {
                    // EffectPlata();
                }
                else if (Effect == Effect.Clima)
                {
                    EffectClima();
                }
                else if (Effect == Effect.Aumento)
                {
                    EffectAumento();
                }
                else if (Effect == Effect.Senuelo)
                {
                    EffectSenuelo();
                }
                else if (Effect == Effect.Despeje)
                {
                    EffectDespeje();
                }
                else if (Effect == Effect.Dark)
                {
                    EffectDark();
                }
                else if (Effect == Effect.Celestial)
                {
                    EffectCelestial();
                }
                else if (Effect == Effect.NoEffect)
                {
                    return;
                }
            }
        }
        private void ActivateSpecificEffect(Effects effect, List<object> prms)
        {
            var effectMethod = typeof(EffectCreated).GetMethod(effect.Name.Substring(1, effect.Name.Length - 2) + "Effect");
            if (effectMethod != null)
            {
                if(prms.Count == 0 || prms == null)
                {
                    var targetList = effect.Targets ; 
                    effectMethod.Invoke(EffectCreated, new object[] { targetList, GameContext.Instance }); 
                }
                else if(prms.Count == 1)
                {
                    var targetList = effect.Targets; 
                    effectMethod.Invoke(EffectCreated, new object[] { targetList, GameContext.Instance, int.Parse(prms[0].ToString()!) }); 
                }
                else if(prms.Count == 2)
                {
                    var targetList = effect.Targets; 
                    effectMethod.Invoke(EffectCreated, new object[] { targetList, GameContext.Instance, int.Parse(prms[0].ToString()!), int.Parse(prms[1].ToString()!) }); 
                }
                else if(prms.Count == 3)
                {
                    var targetList = effect.Targets; 
                    effectMethod.Invoke(EffectCreated, new object[] { targetList, GameContext.Instance, int.Parse(prms[0].ToString()!), int.Parse(prms[1].ToString()!), int.Parse(prms[2].ToString()!) }); 
                }
                else
                {
                    var targetList = effect.Targets; 
                    effectMethod.Invoke(EffectCreated, new object[] { targetList, GameContext.Instance, int.Parse(prms[0].ToString()!), int.Parse(prms[1].ToString()!), int.Parse(prms[2].ToString()!), int.Parse(prms[3].ToString()!) }); 
                }
            }
            else
            {
                Console.WriteLine($"Efecto no encontrado: {effect.Name}");
            }
        }

        public void EffectSenuelo()
        {
            // Encontrar el lado del campo del jugador con la tarjeta de mayor poder
            CardList cards = GameContext.Instance.Fields[Owner];
            Card maxPowerCard = null;
            int maxPower = 0;
            
            foreach (Card card in cards)
            {
                if (card.Power > maxPower)
                {
                    maxPower = card.Power;
                    maxPowerCard = card;
                }
            }
        
            // Si no se encuentra una tarjeta de alto poder, hacer nada
            if (maxPowerCard == null)
            {
                return;
            }
        
            // Mover el Senuelo a la misma fila que la tarjeta de mayor poder
            Card senuelo = this;
            GameContext.Instance.Fields[Owner].Remove(senuelo);
            GameContext.Instance.Fields[Owner].Add(senuelo);
        
            // Mover la tarjeta de mayor poder decsde el campo a la mano del jugador
            GameContext.Instance.Fields[Owner].Remove(maxPowerCard);
            GameContext.Instance.Hands[Owner].Add(maxPowerCard);

            GameContext.Instance.RemoveCard(maxPowerCard);

        }
        
        public void EffectDespeje()
        {
            // Encontrar la posición de la tarjeta que activó este efecto
            CardList cards = GameContext.Instance.Board;
            Card cardToRemove = null;
            Card weatherCardToRemove = null;
            
            foreach (Card card in cards)
            {
                if (card == this)
                {
                    cardToRemove = card;
                    break;
                }
            }
        
            // Si no se encontró la tarjeta, hacer nada
            if (cardToRemove == null)
            {
                return;
            }
        
            // Mover la tarjeta al cementerio
            GameContext.Instance.Board.Remove(cardToRemove);
            GameContext.Instance.Graveyards[Owner % 2 + 1].Add(cardToRemove);


            // Mover la tarjeta de clima al cementerio
            foreach (Card card in cards)
            {
                if (card.Type.ToString() == "Clima") 
                {
                    weatherCardToRemove = card;
                    break;
                }
            }
        
            if (weatherCardToRemove != null)
            {
                GameContext.Instance.Board.Remove(weatherCardToRemove);
                GameContext.Instance.Graveyards[Owner].Add(weatherCardToRemove);
            }

            GameContext.Instance.RemoveCard(cardToRemove);
            GameContext.Instance.RemoveCard(weatherCardToRemove);


        }

        public void EffectAumento()
        {
            // Buscar la carta activadora en todas las filas
            CardList filaActivadora = null;
            if (GameContext.Instance.rowMeleeP1.Contains(this))
                filaActivadora = GameContext.Instance.rowMeleeP1;
            else if (GameContext.Instance.rowRangeP1.Contains(this))
                filaActivadora = GameContext.Instance.rowRangeP1;
            else if (GameContext.Instance.rowSiegeP1.Contains(this))
                filaActivadora = GameContext.Instance.rowSiegeP1;
            else if (GameContext.Instance.rowMeleeP2.Contains(this))
                filaActivadora = GameContext.Instance.rowMeleeP2;
            else if (GameContext.Instance.rowRangeP2.Contains(this))
                filaActivadora = GameContext.Instance.rowRangeP2;
            else if (GameContext.Instance.rowSiegeP2.Contains(this))
                filaActivadora = GameContext.Instance.rowSiegeP2;

            // Si se encontró la carta activadora en alguna fila
            if (filaActivadora != null)
            {
                // Aumentar el poder de todas las cartas en esa fila
                foreach (Card carta in filaActivadora)
                {
                    carta.Power += 1;
                }
            }
        }

        public void EffectClima()
        {
            // Buscar la carta activadora en todas las filas
            CardList filaActivadora = null;
            if (GameContext.Instance.weatherMeleeP1.Contains(this))
                filaActivadora = GameContext.Instance.rowMeleeP1;
            else if (GameContext.Instance.weatherRangeP1.Contains(this))
                filaActivadora = GameContext.Instance.rowRangeP1;
            else if (GameContext.Instance.weatherSiegeP1.Contains(this))
                filaActivadora = GameContext.Instance.rowSiegeP1;
            else if (GameContext.Instance.weatherMeleeP2.Contains(this))
                filaActivadora = GameContext.Instance.rowMeleeP2;
            else if (GameContext.Instance.weatherRangeP2.Contains(this))
                filaActivadora = GameContext.Instance.rowRangeP2;
            else if (GameContext.Instance.weatherSiegeP2.Contains(this))
                filaActivadora = GameContext.Instance.rowSiegeP2;

            // Si se encontró la carta activadora en alguna fila
            if (filaActivadora != null)
            {
                // Aumentar el poder de todas las cartas en esa fila
                foreach (Card carta in filaActivadora)
                {
                    carta.Power -= 1;
                }
            }
        }

        public void EffectDark()
        {
            foreach (Card card in GameContext.Instance.Fields[Owner])
            {
                card.Power -= 2;
            }
        }

        public void EffectCelestial()
        {
            CardList cards = GameContext.Instance.Fields[Owner % 2 + 1];

            Card firstMaxPowerCard = null;
            Card secondMaxPowerCard = null;
            int maxPower = 0;
            int secondMaxPower = 0;

            foreach (Card card in cards)
            {
                if (card.Power > maxPower)
                {
                    secondMaxPower = maxPower;
                    secondMaxPowerCard = firstMaxPowerCard;
                    maxPower = card.Power;
                    firstMaxPowerCard = card;
                }
                else if (card.Power > secondMaxPower && card.Power <= maxPower)
                {
                    secondMaxPower = card.Power;
                    secondMaxPowerCard = card;
                }
            }

            if (firstMaxPowerCard == null || secondMaxPowerCard == null)
            {
                return;
            }

            GameContext.Instance.Graveyards[Owner % 2 + 1].Add(firstMaxPowerCard);
            GameContext.Instance.Graveyards[Owner % 2 + 1].Add(secondMaxPowerCard);

            GameContext.Instance.Fields[Owner % 2 + 1].Remove(firstMaxPowerCard);
            GameContext.Instance.Fields[Owner % 2 + 1].Remove(secondMaxPowerCard);

            GameContext.Instance.RemoveCard(firstMaxPowerCard);
            GameContext.Instance.RemoveCard(secondMaxPowerCard);
        }
    }
        
}