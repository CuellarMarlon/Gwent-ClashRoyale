using System.Collections.Generic;
using System;
using UnityEngine;

namespace GwentPlus
{
    public enum CardType { Oro, Plata, Clima, Aumento, Despeje, Senuelo, Lider }
    public enum Faction { Faction1, Faction2, Faction3, Faction4 } 
    public enum Range { Melee, Ranged, Siege }
    

    [CreateAssetMenu(fileName = "NewCard", menuName = "Card/Card")]
    public class Card : ScriptableObject
    {
        public  int Owner;
        public Sprite Photo;
        public CardType Type;
        public string Name;
        public Faction Faction;
        public int Power;
        public Range[] Range;
        public List<Effects> OnActivation;
        public EffectCreated EffectCreated;

        public void ActivateEffects()
        {
            foreach (var effect in OnActivation)
            {
                ActivateSpecificEffect(effect, effect.Params);
            }
        }
        private void ActivateSpecificEffect(Effects effect, List<object> prms)
        {
            var effectMethod = typeof(EffectCreated).GetMethod(effect.Name.Substring(1, effect.Name.Length - 2) + "Effect");
            if (effectMethod != null)
            {
                if(prms.Count == 0 || prms == null)
                {
                    var targetList = new CardList { this }; 
                    effectMethod.Invoke(EffectCreated, new object[] { targetList, GameContext.Instance }); 
                }
                else if(prms.Count == 1)
                {
                    var targetList = new CardList { this }; 
                    effectMethod.Invoke(EffectCreated, new object[] { targetList, GameContext.Instance, int.Parse(prms[0].ToString()!) }); 
                }
                else if(prms.Count == 2)
                {
                    var targetList = new CardList { this }; 
                    effectMethod.Invoke(EffectCreated, new object[] { targetList, GameContext.Instance, int.Parse(prms[0].ToString()!), int.Parse(prms[1].ToString()!) }); 
                }
                else if(prms.Count == 3)
                {
                    var targetList = new CardList { this }; 
                    effectMethod.Invoke(EffectCreated, new object[] { targetList, GameContext.Instance, int.Parse(prms[0].ToString()!), int.Parse(prms[1].ToString()!), int.Parse(prms[2].ToString()!) }); 
                }
                else
                {
                    var targetList = new CardList { this }; 
                    effectMethod.Invoke(EffectCreated, new object[] { targetList, GameContext.Instance, int.Parse(prms[0].ToString()!), int.Parse(prms[1].ToString()!), int.Parse(prms[2].ToString()!), int.Parse(prms[3].ToString()!) }); 
                }
            }
            else
            {
                Console.WriteLine($"Efecto no encontrado: {effect.Name}");
            }
        }
        

        
    }
}