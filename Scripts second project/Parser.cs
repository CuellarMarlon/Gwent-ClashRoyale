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
    private Token PeekNextToken() => _position + 1 < _tokens.Count ? _tokens[_position + 1] : null;

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
            // Console.WriteLine("effect");
            // Console.WriteLine(CurrentToken.Value);

            if (CurrentToken.Value == "Name")
            {

                Expect("IDENTIFIER"); // Name
                Expect("ASSIGNMENTOPERATOR");
                effect.Name = CurrentToken.Value;
                Expect("STRING");
                Expect("SEPARATOR");
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
                    // Console.WriteLine(CurrentToken.Value + " " + _tokens.IndexOf(CurrentToken) + " " + CurrentToken.Type);

                Expect("IDENTIFIER"); // Action
                Expect("ASSIGNMENTOPERATOR");
                // effect.Name = CurrentToken.Value;
                while (CurrentToken.Value != "}")
                {
                    Expect("DELIMITER");
                    Expect("IDENTIFIER");
                    Expect("SEPARATOR");
                    Expect("IDENTIFIER");
                    Expect("DELIMITER");
                    Expect("LAMBDAOPERATOR");

                    // Aquí es donde llamamos a ParseActionBody para manejar el cuerpo de Action
                    effect.Actions = ParseActionBody(); 
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

    private List<ASTNode> ParseActionBody()
    {
        List<ASTNode> statements = new List<ASTNode>();

        Expect("DELIMITER");

        while (CurrentToken.Value != "}")
        {
            Console.WriteLine(CurrentToken.Value + " " + _tokens.IndexOf(CurrentToken));
            if (CurrentToken.Type == "IDENTIFIER" && PeekNextToken().Value == "(")
            {
                statements.Add(ParseMethodCall());
            }
            else if (CurrentToken.Type == "NUMBER" && PeekNextToken().Type == "ARITHMETICOPERATOR")
            {
                statements.Add(ParseExpression());
            }
            else
            {
                throw new Exception("Expresión no reconocida en el cuerpo de Action");
            }

            NextToken();
        }

        return statements;
    }

    private ASTNode ParseMethodCall()
    {
        // Asumiendo que una llamada a método comienza con el nombre y termina con un paréntesis de cierre
        string methodName = CurrentToken.Value;
        Expect("IDENTIFIER"); // El nombre del método
        Expect("DELIMITER"); // Paréntesis de apertura
    
        List<string> arguments = new List<string>(); // Lista para almacenar los argumentos
    
        while (CurrentToken.Value != ")")
        {
            // Parsear cada argumento
            if (CurrentToken.Type == "SEPARATOR")
            {
                NextToken();
            }
            else if (CurrentToken.Type == "IDENTIFIER")
            {
                // Agregar el argumento como un ASTNode
                arguments.Add(CurrentToken.Value);
                NextToken(); // Avanzar al siguiente token después de agregar el argumento
            }
            else
            {
                throw new Exception("Tipo de argumento no reconocido.");
            }
        }
    
        Expect("DELIMITER"); // Paréntesis de cierre
    
        // Crear y devolver el nodo de llamada al método con el nombre del método y la lista de argumentos
        return new MethodCallNode
        {
            MethodName = methodName,
            Arguments = arguments // Asignar la lista de argumentos al nodo de llamada al método
        };
    }

    private ExpressionNode ParseExpression()
    {
        return ParseBinaryOperation();
    }

    private ExpressionNode ParseBinaryOperation(int precedence = 0)
    {
        var left = ParsePrimaryExpression();

        while (true)
        {
            var operatorToken = CurrentToken;
            // Aquí asumimos que solo hay operaciones de suma y resta por ahora
            if (operatorToken.Value == "+" || operatorToken.Value == "-")
            {
                Expect(operatorToken.Type); // Avanza el token del operador
                var right = ParsePrimaryExpression(); // Recursivamente parsea la expresión a la derecha del operador
                left = new BinaryOperationNode
                {
                    Left = left,
                    Operator = operatorToken.Value,
                    Right = right
                };
            }
            else
            {
                break;
            }
        }
        
        return left;
    }

    private ExpressionNode ParsePrimaryExpression()
    {
        switch (CurrentToken.Type)
        {
            case "NUMBER":
                string tmp = CurrentToken.Value;
                Expect("NUMBER"); // ojo 
                return new NumberLiteralNode { Value = int.Parse(tmp) };
            case "IDENTIFIER":
                string tmp1 = CurrentToken.Value;
                Expect("IDENTIFIER"); // ojo 
                return new VariableReferenceNode { Name = tmp1 };
            default:
                throw new Exception($"Token no esperado: {CurrentToken.Type}  {_tokens.IndexOf(CurrentToken)}");
        }
    }

    private CardNode ParseCard()
    {
        Expect("KEYWORD"); // card
        Expect("DELIMITER");

        CardNode card = new CardNode();

        while (CurrentToken.Value != "}")
        {
            // Console.WriteLine("card");
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
                Expect("NUMBER");
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
            // Console.WriteLine("ParseActivation");

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
            // Console.WriteLine("ParseCardEffect");

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
                Expect("NUMBER");
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
            // Console.WriteLine("ParseSelector");

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
                Expect("NUMBER");
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
            // Console.WriteLine("ParsePostAction");

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