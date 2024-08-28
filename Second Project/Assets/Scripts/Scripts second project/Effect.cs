using System;
using System.Collections.Generic;

namespace GwentPlus
{
    public class Effects 
    {
        public string Name { get; set; } 
        public List<object> Params { get; set; } 
        public string Source { get; set; } 
        public bool Single { get; set; } 
        public Predicate Predicate { get; set; } 
        
        private CardList CreateTargets()
        {
            CardList cards = new CardList();

            if (Source == "\"board\"")
            {
                foreach (Card unit in GameContext.Instance.Board)
                {
                    switch (Predicate.Operator)
                    {
                        case "==":
                            if (Predicate.LeftMember == "Type" && unit.Type.ToString() == Predicate.RightMember.ToString())
                            {
                                cards.Add(unit); 
                            }
                            else if (Predicate.LeftMember == "Faction" && unit.Faction.ToString() == Predicate.RightMember.ToString())
                            {
                                cards.Add(unit);
                            }
                            else if (Predicate.LeftMember == "Power" && unit.Power.ToString() == Predicate.RightMember.ToString())
                            {
                                cards.Add(unit);
                            }
                            else if (Predicate.LeftMember == "Range")
                            {
                                foreach (var range in unit.Range)
                                {
                                    if(range.ToString() == Predicate.RightMember.ToString())
                                    {
                                        cards.Add(unit);
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception($"Se esperaba otro miembro izquierdo para predicate");
                            }
                            break;
                        case "!=":
                            if (Predicate.LeftMember == "Type" && unit.Type.ToString() != Predicate.RightMember.ToString())
                            {
                                cards.Add(unit); 
                            }
                            else if (Predicate.LeftMember == "Faction" && unit.Faction.ToString() != Predicate.RightMember.ToString())
                            {
                                cards.Add(unit);
                            }
                            else if (Predicate.LeftMember == "Power" && unit.Power.ToString() != Predicate.RightMember.ToString())
                            {
                                cards.Add(unit);
                            }
                            else if (Predicate.LeftMember == "Range")
                            {
                                foreach (var range in unit.Range)
                                {
                                    if(range.ToString() != Predicate.RightMember.ToString())
                                    {
                                        cards.Add(unit);
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception($"Se esperaba otro miembro izquierdo para predicate");
                            }
                            break;
                        case "<=":
                            if (Predicate.LeftMember == "Power" && unit.Power <= int.Parse(Predicate.RightMember.ToString()!))
                            {
                                cards.Add(unit); 
                            }
                            else
                            {
                                throw new Exception($"Se esperaba otro miembro izquierdo para predicate");
                            }
                            break;
                        case ">=":
                            if (Predicate.LeftMember == "Power" && unit.Power >= int.Parse(Predicate.RightMember.ToString()!))
                            {
                                cards.Add(unit); 
                            }
                            else
                            {
                                throw new Exception($"Se esperaba otro miembro izquierdo para predicate");
                            }
                            break;
                        case "<":
                            if (Predicate.LeftMember == "Power" && unit.Power < int.Parse(Predicate.RightMember.ToString()!))
                            {
                                cards.Add(unit); 
                            }
                            else
                            {
                                throw new Exception($"Se esperaba otro miembro izquierdo para predicate");
                            }
                            break;
                        case ">":
                            if (Predicate.LeftMember == "Power" && unit.Power > int.Parse(Predicate.RightMember.ToString()!))
                            {
                                cards.Add(unit); 
                            }
                            else
                            {
                                throw new Exception($"Se esperaba otro miembro izquierdo para predicate");
                            }
                            break;
                        
                    }

                    
                }
                
            }
            
            return cards;
        }

    }

    public class Predicate
    {
        public string LeftMember { get; set; }
        public string Operator { get; set; }
        public object RightMember { get; set; }


    }
}