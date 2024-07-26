namespace GwentPlus
{
public class Parser
{
    private List<Token> _tokens;
    private int _position;

    private static readonly Dictionary<string,(int precedence, bool rightAssociative)> Operators = new()
    {
        { "+", (1, false) },
        { "-", (1, false) },
        { "*", (2, false) },
        { "/", (2, false) },
    };

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
            // Console.WriteLine(CurrentToken.Value + " " + _tokens.IndexOf(CurrentToken));
            if (CurrentToken.Type == "IDENTIFIER" && PeekNextToken().Value == "(")
            {
                statements.Add(ParseMethodCall());
            }
            else if ((CurrentToken.Type == "NUMBER" || CurrentToken.Type == "IDENTIFIER") && PeekNextToken().Type == "ARITHMETICOPERATOR")
            {
                // Inicia una nueva lista para capturar los tokens de la expresión matemática
                List<Token> expressionTokens = new List<Token>();
                // Agrega el primer token de la expresión
                expressionTokens.Add(CurrentToken);

                // Avanza hasta encontrar un punto y coma para terminar la expresión
                while (CurrentToken.Value != ";" && CurrentToken.Value != "}")
                {
                    NextToken(); // Avanza al siguiente token
                    if (CurrentToken.Value == ";") 
                    {   
                        Expect("SEMICOLON");
                        break; // Detiene la captura en el punto y coma
                    }
                    expressionTokens.Add(CurrentToken); // Agrega el token actual a la expresión
                }

                statements.Add(ParseExpression(expressionTokens)); // Parsea la expresión capturada

            }
            else
            {
                throw new Exception("Expresión no reconocida en el cuerpo de Action");
            }

            // NextToken();
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
        Expect("SEMICOLON");
    
        return new MethodCallNode
        {
            MethodName = methodName,
            Arguments = arguments // Asignar la lista de argumentos al nodo de llamada al método
        };
    }

    private ExpressionNode ParseExpression(List<Token> expressionTokens)
    {
        var postfixTokens = ConvertToPostfix(expressionTokens); // Convierta la entrada infija a postfija.
        return ParsePostfixExpression(postfixTokens);
    }

    private ExpressionNode ParsePostfixExpression(List<Token> postfixTokens)
    {
        Stack<ExpressionNode> stack = new Stack<ExpressionNode>();

        foreach (var token in postfixTokens)
        {
            if (token.Type == "NUMBER")
            {
                stack.Push(new NumberLiteralNode { Value = int.Parse(token.Value) });
            }
            else if (token.Type == "IDENTIFIER")
            {
                stack.Push(new VariableReferenceNode { Name = token.Value });
            }
            else if (Operators.ContainsKey(token.Value))
            {
                var right = stack.Pop();
                var left = stack.Pop();
                stack.Push(new BinaryOperationNode { Left = left, Operator = token.Value, Right = right });
            }
        }

        return stack.Pop();
    }

    public List<Token> ConvertToPostfix(List<Token> infixTokens)
    {
        Stack<Token> operatorStack = new Stack<Token>();
        List<Token> output = new List<Token>();

        foreach (var token in infixTokens)
        {
            if (token.Type == "NUMBER" || token.Type == "IDENTIFIER")
            {
                output.Add(token);
            }
            else if (Operators.ContainsKey(token.Value))
            {
                while (operatorStack.Any() && Operators.ContainsKey(operatorStack.Peek().Value) &&
                       ((Operators[token.Value].rightAssociative && Operators[token.Value].precedence < Operators[operatorStack.Peek().Value].precedence) ||
                        (!Operators[token.Value].rightAssociative && Operators[token.Value].precedence <= Operators[operatorStack.Peek().Value].precedence)))
                {
                    output.Add(operatorStack.Pop());
                }
                operatorStack.Push(token);
            }
            else if (token.Value == "(")
            {
                operatorStack.Push(token);
            }
            else if (token.Value == ")")
            {
                while (operatorStack.Peek().Value != "(")
                {
                    output.Add(operatorStack.Pop());
                }
                operatorStack.Pop();
            }
        }

        while (operatorStack.Any())
        {
            output.Add(operatorStack.Pop());
        }

        return output;
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