namespace GwentPlus
{
public class Parser
{
    private List<Token> _tokens;
    private int _position;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
        _position = 0;
    }

    private Token CurrentToken => _position < _tokens.Count ? _tokens[_position] : null;

    private void NextToken() => _position++;

    private void Expect(string type)
    {
        if (CurrentToken?.Type != type)
        {
            throw new Exception($"Expected {type}, but got {CurrentToken?.Type}");
        }
        NextToken();
    }

    public List<ASTNode> Parse()
    {
        List<ASTNode> nodes = new List<ASTNode>();

        while (CurrentToken != null)
        {
            // Console.WriteLine(CurrentToken.Value);
            if (CurrentToken.Value == "effect")
            {
                nodes.Add(ParseEffect());
            }
            else if (CurrentToken.Value == "card")
            {
                nodes.Add(ParseCard());
            }
            // Console.WriteLine("abajo");
        }
            // Console.WriteLine("mas abajo");

        return nodes;
    }

    private EffectNode ParseEffect()
    {
        Expect("KEYWORD"); // effect
        Expect("DELIMITER");

        EffectNode effect = new EffectNode();
        // Console.WriteLine("abajosddfda");

        while (CurrentToken.Value != "}")
        {
            // Console.WriteLine(CurrentToken.Value);

            if (CurrentToken.Value == "Name")
            {
                Expect("IDENTIFIER"); // Name
                Expect("ASSIGNMENTOPERATOR");
                effect.Name = CurrentToken.Value;
                Expect("STRING");
            }
            else if (CurrentToken.Value == "Params")
            {
                Expect("IDENTIFIER"); // Params
                Expect("ASSIGNMENTOPERATOR");
                Expect("DELIMITER");

                while (CurrentToken.Value != "}")
                {
                    var paramName = CurrentToken.Value;
                    Expect("IDENTIFIER"); // params name
                    Expect("ASSIGNMENTOPERATOR");
                    var paramType = CurrentToken.Value;
                    Expect("IDENTIFIER"); // param type
                    effect.Params[paramName] = paramType;
                    if (CurrentToken.Value == ",")
                    {
                    }
                }

                Expect("DELIMITER");
                Expect("SEPARATOR");
            }
            else if (CurrentToken.Value == "Action")
            {
                Expect("IDENTIFIER"); // Action
                Expect("ASSIGNMENTOPERATOR");
                effect.Action = CurrentToken.Value;
                while (CurrentToken.Value != "}")
                {
                    NextToken();
                }
                Expect("DELIMITER");


            }

            if (CurrentToken.Value == ",")
            {
                Expect("SEPARATOR");
            }
        }

        Expect("DELIMITER");
        return effect;
    }

    private CardNode ParseCard()
    {
        Expect("KEYWORD"); // card
        Expect("DELIMITER");

        CardNode card = new CardNode();

while (CurrentToken.Value != "}")
        {
            if (CurrentToken.Value == "Type")
            {
                Expect("IDENTIFIER"); // Type
                Expect("ASSIGNMENTOPERATOR");
                card.Type = CurrentToken.Value;
                Expect("STRING");
            }
            else if (CurrentToken.Value == "Name")
            {
                Expect("IDENTIFIER"); // Name
                Expect("ASSIGNMENTOPERATOR");
                card.Name = CurrentToken.Value;
                Expect("STRING");
            }
            else if (CurrentToken.Value == "Faction")
            {
                Expect("IDENTIFIER"); // Faction
                Expect("ASSIGNMENTOPERATOR");
                card.Faction = CurrentToken.Value;
                Expect("STRING");
            }
            else if (CurrentToken.Value == "Power")
            {
                Expect("IDENTIFIER"); // Power
                Expect("ASSIGNMENTOPERATOR");
                card.Power = int.Parse(CurrentToken.Value);
                Expect("STRING");
            }
            else if (CurrentToken.Value == "Range")
            {
                Expect("IDENTIFIER"); // Range
                Expect("ASSIGNMENTOPERATOR");
                Expect("DELIMITER");
                while (CurrentToken.Value != "]")
                {
                    card.Range.Add(CurrentToken.Value);
                    Expect("STRING");
                    if (CurrentToken.Value == ",")
                    {
                        Expect("SEPARATOR");
                    }
                }
                Expect("DELIMITER");
            }
            else if (CurrentToken.Value == "OnActivation")
            {
                Expect("IDENTIFIER"); // OnActivation
                Expect("ASSIGNMENTOPERATOR");
                Expect("DELIMITER");
                
                while (CurrentToken.Value != "]")
                {
                    card.OnActivation.Add(ParseActivation());
                    if (CurrentToken.Value == ",")
                    {
                        Expect("SEPARATOR");
                    }
                }
                
                Expect("DELIMITER");
            }

            if (CurrentToken.Value == ",")
            {
                Expect("SEPARATOR");
            }
        }

        Expect("DELIMITER");
        return card;
    }

    private ActivationNode ParseActivation()
    {
        Expect("DELIMITER");

        ActivationNode activation = new ActivationNode();

        while (CurrentToken.Value != "}")
        {
            if (CurrentToken.Value == "Effect")
            {
                Expect("IDENTIFIER"); // Effect
                Expect("ASSIGNMENTOPERATOR");
                activation.Effect = ParseCardEffect();
            }
            else if (CurrentToken.Value == "Selector")
            {
                Expect("IDENTIFIER"); // Selector
                Expect("ASSIGNMENTOPERATOR");
                activation.Selector = ParseSelector();
            }
            else if (CurrentToken.Value == "PostAction")
            {
                Expect("IDENTIFIER"); // PostAction
                Expect("ASSIGNMENTOPERATOR");
                activation.PostAction = ParsePostAction();
            }

            if (CurrentToken.Value == ",")
            {
                Expect("SEPARATOR");
            }
        }

        Expect("DELIMITER");
        return activation;
    }

    private CardEffectNode ParseCardEffect()
    {
        
        Expect("DELIMITER");


        CardEffectNode CardEffect = new CardEffectNode();

while (CurrentToken.Value != "}")
        {
            if (CurrentToken.Value == "Name")
            {
                Expect("IDENTIFIER"); // Name
                Expect("ASSIGNMENTOPERATOR");
                CardEffect.Name = CurrentToken.Value;
                Expect("STRING");
            }
            else if (CurrentToken.Value == "Amount")
            {
                Expect("IDENTIFIER"); // Amount
                Expect("ASSIGNMENTOPERATOR");
                CardEffect.Amount = CurrentToken.Value;
                Expect("STRING");
            }
            if (CurrentToken.Value == ",")
            {
                Expect("SEPARATOR");
            }
        }

        Expect("DELIMITER");
        return CardEffect;
    }

    private SelectorNode ParseSelector()
    {
        Expect("DELIMITER");

        SelectorNode selector = new SelectorNode();

while (CurrentToken.Value != "}")
        {
            if (CurrentToken.Value == "Source")
            {
                Expect("IDENTIFIER"); // Source
                Expect("ASSIGNMENTOPERATOR");
                selector.Source = CurrentToken.Value;
                Expect("STRING");
            }
            else if (CurrentToken.Value == "Single")
            {
                Expect("IDENTIFIER"); // Single
                Expect("ASSIGNMENTOPERATOR");
                selector.Single = bool.Parse(CurrentToken.Value);
                Expect("IDENTIFIER");
            }
            else if (CurrentToken.Value == "Predicate")
            {
                Expect("IDENTIFIER"); // Predicate
                Expect("ASSIGNMENTOPERATOR");
                selector.Predicate = CurrentToken.Value;
                Expect("STRING");
            }

            if (CurrentToken.Value == ",")
            {
                Expect("SEPARATOR");
            }
        }

        Expect("DELIMITER");
        return selector;
    }

    private PostActionNode ParsePostAction()
    {
        Expect("DELIMITER");

        PostActionNode postAction = new PostActionNode();

        while (CurrentToken.Value != "}")
        {
            if (CurrentToken.Value == "Type")
            {
                Expect("IDENTIFIER"); // Type
                Expect("ASSIGNMENTOPERATOR");
                postAction.Type = CurrentToken.Value;
                Expect("STRING");
            }
            else if (CurrentToken.Value == "Selector")
            {
                Expect("IDENTIFIER"); // Selector
                Expect("ASSIGNMENTOPERATOR");
                postAction.Selector = ParseSelector();
            }

            if (CurrentToken.Value == ",")
            {
                Expect("SEPARATOR");
            }
        }

        Expect("DELIMITER");
        return postAction;
    }
}
}