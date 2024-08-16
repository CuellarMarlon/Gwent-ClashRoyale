using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Reflection;

namespace GwentPlus
{
public class Parser
{
    private Context context = new Context(null);
    private List<Token> _tokens;
    private int _position;

    public static readonly Dictionary<string,(int precedence, bool rightAssociative)> Operators = new()
    {
        { "+", (4, false) },
        { "-", (4, false) },
        { "*", (5, false) },
        { "/", (5, false) },
        { "&&", (1, false)},
        { "||", (2, false)},
        { "!", (1, true)},
        { "==", (3, false) }, 
        { "!=", (3, false) }, 
        { "<", (3, false) },  
        { ">", (3, false) },  
        { ">=", (3, false) }, 
        { "<=", (3, false) }

    };

    public Parser(List<Token> tokens)
    {
        context = new Context();
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

        Context effectContext = new Context(context);

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

                context.DeffineEffect(effect.Name, effect);
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
                    
                    if (effectContext.Variables.ContainsKey(paramName))
                    {
                        effectContext.SetVariable(paramName, paramType);
                    }
                    else if (paramType == "Number")
                    {
                        effectContext.DefineVariable(paramName, 0);
                    }
                    else if (paramType == "Bool")
                    {
                        effectContext.DefineVariable(paramName, false);
                    }
                    else
                    {
                        throw new Exception($"El tipo de dato no esta permitido para {paramName}.");
                    }
                    
                    if (CurrentToken.Value == ",")
                    {
                        Expect("SEPARATOR");
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
                    effect.Actions.Children = ParseActionBody(effectContext); 
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

    private List<ASTNode> ParseActionBody(Context currentContext)
    {
        List<ASTNode> statements = new List<ASTNode>();

        Expect("DELIMITER");

        while (CurrentToken.Value != "}")
        {
            if (CurrentToken.Type == "IDENTIFIER" && PeekNextToken().Value == ".")
            {
                statements.Add(ParseMemberAccess(currentContext));
            }
            else if (CurrentToken.Type == "IDENTIFIER" && PeekNextToken().Type == "ASSIGNMENTOPERATOR")
            {
                statements.Add(ParseAssignment(null, currentContext));
            }
            else if (CurrentToken.Value == "while")
            {
                statements.Add(ParseWhile(currentContext));
            }
            else if (CurrentToken.Value == "if")
            {
                statements.Add(ParseIf(currentContext));
            }
            else if (CurrentToken.Value == "for")
            {
                statements.Add(ParseFor(currentContext));
            }
            else
            {
                throw new Exception("Expresión no reconocida en el cuerpo de Action: " + CurrentToken.Value + " " + _tokens.IndexOf(CurrentToken));
            }
        }

        return statements;
    }

    private ASTNode ParseMemberAccess(Context currentContext)
    {
        string objectName = CurrentToken.Value;
        List<string> accessChain = new List<string>{objectName};

        //Procesar los accesos anidados 
        while (PeekNextToken().Value == ".")
        {
            Expect("IDENTIFIER"); //Se espera el siguiente identificador 
            Expect("ACCESS"); //Se espera el punto
            string memberName = CurrentToken.Value;
            accessChain.Add(memberName);
        }

        //Verifica si es una propiedad o un metodo
        bool isProperty = false;
        List<ExpressionNode> arguments = new List<ExpressionNode>();
        if (PeekNextToken().Value == ";")
        {
            isProperty = true;
            Expect("IDENTIFIER");
            Expect("SEMICOLON");
        }
        else if (PeekNextToken().Value == "(")
        {
            isProperty = false;
            Expect("IDENTIFIER");
            Expect("DELIMITER");
            arguments = ParseArguments();
            Expect("DELIMITER");
            Expect("SEMICOLON");
        }
        else if (PeekNextToken().Type == "ASSIGNMENTOPERATOR")
        {
            var assignmentNode = ParseAssignment(accessChain, currentContext);
            return assignmentNode ;
        }

        return new MemberAccessNode { AccessChain = accessChain, Arguments = arguments , IsProperty = isProperty };
    }

    private AssignmentNode ParseAssignment(List<string> accessChain, Context currentContext)
    {
        // Manejo de asignaciones
        string variableName = CurrentToken.Value;
        Expect("IDENTIFIER");
        string _operator = CurrentToken.Value;
        Expect("ASSIGNMENTOPERATOR");

        if (PeekNextToken().Value == ".")
        {
            var valueMemberAcces = ParseMemberAccess(currentContext);
            return new AssignmentNode { VariableName = variableName, ValueExpression = valueMemberAcces, AccessChain = accessChain, Operator = _operator };
        } 
        else 
        {
            var valueExpression = ParseExpression(ParseExpressionTokens(), false, currentContext);
            Expect("SEMICOLON"); //Check
            AssignmentNode assignmentNode = new AssignmentNode { VariableName = variableName, ValueExpression = valueExpression, AccessChain = accessChain, Operator = _operator };
            if (currentContext.Variables.ContainsKey(variableName))
            {
                currentContext.SetVariable(variableName, valueExpression.Evaluate(currentContext));
            }
            else
            {
                currentContext.DefineVariable(variableName, valueExpression.Evaluate(currentContext));
            }

            return assignmentNode;
        }
    }

    private List<ExpressionNode> ParseArguments()
    {
        List<ExpressionNode> arguments = new List<ExpressionNode>(); // Lista para almacenar los argumentos
    
        while (CurrentToken.Value != ")")
        {
            // Parsear cada argumento
            if (CurrentToken.Type == "SEPARATOR")
            {
                NextToken();
            }
            else if (CurrentToken.Type == "IDENTIFIER")
            {
                // Agregar el argumento 
                arguments.Add(new VariableReferenceNode { Name = CurrentToken.Value});
                NextToken(); // Avanzar al siguiente token después de agregar el argumento
            }
            else if (CurrentToken.Type == "NUMBER")
            {
                // Agregar el argumento 
                arguments.Add(new NumberLiteralNode { Value = int.Parse(CurrentToken.Value)});
                NextToken(); // Avanzar al siguiente token después de agregar el argumento
            }
            else if (CurrentToken.Type == "BOOLEAN")
            {
                bool currentTokenValue = false;
                if (CurrentToken.Value == "true") currentTokenValue = true;
                else if (CurrentToken.Value == "false") currentTokenValue = false;
                // Agregar el argumento 
                arguments.Add(new BooleanLiteralNode { Value = currentTokenValue});
                NextToken(); // Avanzar al siguiente token después de agregar el argumento
            }
            else
            {
                throw new Exception("Tipo de argumento no reconocido.");
            }
        }

        return arguments;
    }

    private List<Token> ParseExpressionTokens()
    {
        //Crear una lista para capturar los tokens de la expresion 
        List<Token> expressionTokens = new List<Token>();

        //Parsear la expresion a la derecha del "="
        while (CurrentToken.Value != ";" && CurrentToken.Value != "}" && CurrentToken.Value != ")")
        {
            expressionTokens.Add(CurrentToken);
            NextToken();
        }

        return expressionTokens;
    }

    private ExpressionNode ParseExpression(List<Token> expressionTokens, bool isCondition, Context currentContext)
    {
        var postfixTokens = ConvertToPostfix(expressionTokens); // Convierta la entrada infija a postfija.
        var ast = ParsePostfixExpression(postfixTokens, currentContext);
        
        if (isCondition && ast.Evaluate(currentContext).GetType() != typeof(bool))
        {   
           throw new Exception("La expresion de condicion debe evaluar a un booleano.");
        }
        
        return ast;
        
    }

    private ExpressionNode ParsePostfixExpression(List<Token> postfixTokens, Context currentContext)
    {
        Stack<ExpressionNode> stack = new Stack<ExpressionNode>();

        foreach (var token in postfixTokens)
        {
            if (token.Type == "NUMBER")
            {
                stack.Push(new NumberLiteralNode { Value = int.Parse(token.Value) });
            }
            else if (token.Type == "BOOLEAN")
            {
                stack.Push(new BooleanLiteralNode { Value = bool.Parse(token.Value)});
            }
            else if (token.Type == "IDENTIFIER")
            {
                if (currentContext.Variables.ContainsKey(token.Value))
                {
                    var variableValue = currentContext.GetVariable(token.Value);
                    stack.Push(new VariableReferenceNode { Name = token.Value, Value = variableValue });
                }
                else 
                {
                    throw new Exception($"La varible '{token.Value}' no esta definida");
                }
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

    private List<Token> ConvertToPostfix(List<Token> infixTokens)
    {
        Stack<Token> operatorStack = new Stack<Token>();
        List<Token> output = new List<Token>();

        foreach (var token in infixTokens)
        {
            if (token.Type == "NUMBER" || token.Type == "IDENTIFIER" || token.Type == "BOOLEAN")
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

    private WhileNode ParseWhile(Context currentContext)
    {
        Expect("KEYWORD"); // while
        Expect("DELIMITER");

        var whileNode = new WhileNode();

        // Parsear la condición
        whileNode.Condition = ParseExpression(ParseExpressionTokens(), true, currentContext);

        Expect("DELIMITER"); // Expect the delimiter after the condition "("

        // Parsear el cuerpo del ciclo
        Expect("DELIMITER"); //"{"
        while (CurrentToken.Value != "}")
        {
            if (CurrentToken.Type == "IDENTIFIER" && PeekNextToken().Value == ".")
            {
                whileNode.Body.Add(ParseMemberAccess(currentContext));
            }
            else if (CurrentToken.Type == "IDENTIFIER" && PeekNextToken().Type == "ASSIGNMENTOPERATOR")
            {
                whileNode.Body.Add(ParseAssignment(null, currentContext));
            }
            else if (CurrentToken.Value == "while" && PeekNextToken().Value == "(")
            {
                throw new Exception($"Mijito implemnta el while :)");
            }
            else if (CurrentToken.Value == "while")
            {
                whileNode.Body.Add(ParseWhile(currentContext));
            }
            else if (CurrentToken.Value == "if")
            {
                whileNode.Body.Add(ParseIf(currentContext));
            }
            else if (CurrentToken.Value == "for")
            {
                whileNode.Body.Add(ParseFor(currentContext));
            }
            else
            {
                // Aquí puedes agregar más tipos de nodos que quieras manejar en el cuerpo
                throw new Exception("Expresión no reconocida en el cuerpo del while: " + CurrentToken.Value);
            }
        }

        Expect("DELIMITER"); // Cerrar el ciclo con el delimitador
        return whileNode;
    }

    private IfNode ParseIf(Context currentContext)
    {
        Expect("KEYWORD"); // if
        Expect("DELIMITER");

        var ifNode = new IfNode();

        // Parsear la condición
        ifNode.Condition = ParseExpression(ParseExpressionTokens(), true, currentContext);

        Expect("DELIMITER"); // Expect the delimiter after the condition ")"

        // Parsear el cuerpo del if
        Expect("DELIMITER"); //"{"
        while (CurrentToken.Value != "else" && CurrentToken.Value != "}")
        {
            // Aquí puedes agregar más tipos de nodos que quieras manejar en el cuerpo
            if (CurrentToken.Type == "IDENTIFIER" && PeekNextToken().Value == ".")
            {
                ifNode.Body.Add(ParseMemberAccess(currentContext));
            }
            else if (CurrentToken.Type == "IDENTIFIER" && PeekNextToken().Type == "ASSIGNMENTOPERATOR")
            {
                ifNode.Body.Add(ParseAssignment(null, currentContext));
            }
            else if (CurrentToken.Value == "while")
            {
                ifNode.Body.Add(ParseWhile(currentContext));
            }
            else if (CurrentToken.Value == "if")
            {
                ifNode.Body.Add(ParseIf(currentContext));
            }
            else if (CurrentToken.Value == "for")
            {
                ifNode.Body.Add(ParseFor(currentContext));
            }
            else
            {
                throw new Exception("Expresión no reconocida en el cuerpo del if: " + CurrentToken.Value);
            }
        }

        Expect("DELIMITER");
        if (CurrentToken.Value == "else")
        {
            Expect("KEYWORD"); // 'else'
            Expect("DELIMITER"); // Expect the delimiter after 'else'

            // Parsear el cuerpo del else
            while (CurrentToken.Value != "}")
            {
                if (CurrentToken.Type == "IDENTIFIER" && PeekNextToken().Value == ".")
                {
                    ifNode.ElseBody.Add(ParseMemberAccess(currentContext));
                }
                else if (CurrentToken.Type == "IDENTIFIER" && PeekNextToken().Type == "ASSIGNMENTOPERATOR")
                {
                    ifNode.ElseBody.Add(ParseAssignment(null, currentContext));
                }
                else if (CurrentToken.Value == "while")
                {
                    ifNode.Body.Add(ParseWhile(currentContext));
                }
                else if (CurrentToken.Value == "if")
                {
                    ifNode.Body.Add(ParseIf(currentContext));
                }
                else if (CurrentToken.Value == "for")
                {
                    ifNode.Body.Add(ParseFor(currentContext));
                }
                else
                {
                    throw new Exception("Expresión no reconocida en el cuerpo del else: " + CurrentToken.Value);
                }
            }
        }

        Expect("DELIMITER"); // Cerrar el if con el delimitador
        return ifNode;
    }

    private ForNode ParseFor(Context currentContext)
    {
        Expect("KEYWORD"); // for
        Expect("DELIMITER"); // Expect '('

        var forNode = new ForNode();

        // Parsear el target
        forNode.Item = CurrentToken.Value;
        Expect("IDENTIFIER"); // target

        Expect("KEYWORD"); // in
        forNode.Collection = new VariableReferenceNode{ Name = CurrentToken.Value };
        Expect("IDENTIFIER"); // Expect 'targets'

        // Parsear la colección (targets)
        // forEachNode.Collection.Name = CurrentToken.Value;

        Expect("DELIMITER"); // Expect ')'

        // Parsear el cuerpo del ciclo
        Expect("DELIMITER"); //'{'
        while (CurrentToken.Value != "}")
        {
            if (CurrentToken.Type == "IDENTIFIER" && PeekNextToken().Value == ".")
            {
                forNode.Body.Add(ParseMemberAccess(currentContext));
            }
            else if (CurrentToken.Type == "IDENTIFIER" && PeekNextToken().Type == "ASSIGNMENTOPERATOR")
            {
                forNode.Body.Add(ParseAssignment(null, currentContext));
            }
            else if (CurrentToken.Value == "while")
            {
                forNode.Body.Add(ParseWhile(currentContext));
            }
            else if (CurrentToken.Value == "if")
            {
                forNode.Body.Add(ParseIf(currentContext));
            }
            else if (CurrentToken.Value == "for")
            {
                forNode.Body.Add(ParseFor(currentContext));
            }
            else
            {
                throw new Exception("Expresión no reconocida en el cuerpo del for: " + CurrentToken.Value);
            }
        }

        Expect("DELIMITER"); // Cerrar el for con el delimitador
        return forNode;
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

                context.DeffineCard(card.Name, card);
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
                
                bool isFirst = true;
                while (CurrentToken.Value != "]")
                {
                    card.OnActivation.Add(ParseActivation(isFirst));
                    isFirst = false;
                    if (CurrentToken.Value == "}")
                    {
                        Expect("DELIMITER");
                    }
                    if (CurrentToken.Value == ",")
                    {
                        Expect("SEPARATOR");
                    }
                    if(CurrentToken.Value == "}")
                    {
                        Expect("DELIMITER");
                    }
                    if (CurrentToken.Value == ",")
                    {
                        Expect("SEPARATOR");
                    }
                    if (CurrentToken.Value == "}")
                    {
                        Expect("DELIMITER");
                    }
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

    private ActivationNode ParseActivation(bool isFirst)
    {
        if (!isFirst) 
        {
            Expect("IDENTIFIER");
            Expect("ASSIGNMENTOPERATOR");
        }
        Expect("DELIMITER");
        
        ActivationNode activation = new ActivationNode();

        while (CurrentToken.Value != "}" && CurrentToken.Value != "PostAction")
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

            if (CurrentToken.Value == ",")
            {
                Expect("SEPARATOR");
            }

        }

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
                if (!context.Effects.ContainsKey(CurrentToken.Value))
                {
                    throw new Exception($"El effecto {CurrentToken.Value} no ha sido construido anteriormente.");
                }
                CardEffect.Name = CurrentToken.Value;
                Expect("STRING");
                
            }
            else if (CurrentToken.Type == "IDENTIFIER")
            {
                while(CurrentToken.Value != "}")
                {   
                    Expect("IDENTIFIER"); 
                    Expect("ASSIGNMENTOPERATOR");
                    CardEffect.Params.Add(CurrentToken.Value);
                    if(CurrentToken.Type == "NUMBER")
                    {
                        Expect("NUMBER");
                    }
                    else if (CurrentToken.Type == "BOOLEAN")
                    {
                        Expect("BOOLEAN");
                    }
                    else if (CurrentToken.Type == "STRING")
                    {
                        Expect("STRING");
                    }
                    Expect("SEPARATOR");
                }
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
                Expect("BOOLEAN");
            }
            else if (CurrentToken.Value == "Predicate")
            {
                PredicateNode predicateNode = new PredicateNode();
                Expect("IDENTIFIER"); // Predicate
                Expect("ASSIGNMENTOPERATOR");
                Expect("DELIMITER");
                Expect("IDENTIFIER");
                Expect("DELIMITER");
                Expect("LAMBDAOPERATOR");
                Expect("IDENTIFIER");
                Expect("ACCESS");

                predicateNode.LeftMember = CurrentToken.Value;
                Expect("IDENTIFIER");
                predicateNode.Operator = CurrentToken.Value;
                Expect("RELATIONALOPERATOR");
                if(CurrentToken.Type == "NUMBER")
                {
                    predicateNode.RightMember = int.Parse(CurrentToken.Value);
                    Expect("NUMBER");
                }
                else if(CurrentToken.Type == "STRING")
                {
                    predicateNode.RightMember = CurrentToken.Value.Substring(1, CurrentToken.Value.Length - 2);
                    Expect("STRING");
                }
                else
                {
                    throw new Exception($"Tipo de valor no permito para 'Predicate' se esperaba 'number' o 'string' pero se obtuvo {CurrentToken.Type}");
                }               
                
                selector.Predicate = predicateNode;

            }

            if (CurrentToken.Value == ",")
            {
                Expect("SEPARATOR");
            }
        }

        Expect("DELIMITER");
        return selector;
    }
}
}