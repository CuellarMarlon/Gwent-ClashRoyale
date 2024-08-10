using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GwentPlus
{
    public abstract class ASTNode
    {
        public abstract void Print(int indent = 0);
        public abstract object Evaluate(Context context);

    }

    public class EffectNode : ASTNode
    {
        public string Name { get; set; }
        public Dictionary<string, string> Params { get; set; }
        public List<ASTNode> Actions { get; set; }

        public EffectNode()
        {
            Params = new Dictionary<string, string>();
            Actions = new List<ASTNode>();
        }

        public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}Effect: {Name}");
            foreach (var param in Params)
            {
                Console.WriteLine($"{indentation}  Param: {param.Key} = {param.Value}");
            }
            Console.WriteLine($"{indentation}  Action:");
            foreach (var action in Actions)
            {
                action.Print(indent + 3); 
            }
        }

        public override object Evaluate(Context context)
        {
            return 0;
        }

    }

    public class CardNode : ASTNode
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Faction { get; set; }
        public int Power { get; set; }
        public List<string> Range { get; set; }
        public List<ActivationNode> OnActivation { get; set; }

        public CardNode()
        {
            Range = new List<string>();
            OnActivation = new List<ActivationNode>();
        }

        public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}Card: {Name}");
            Console.WriteLine($"{indentation}  Type: {Type}");
            Console.WriteLine($"{indentation}  Faction: {Faction}");
            Console.WriteLine($"{indentation}  Power: {Power}");
            Console.WriteLine($"{indentation}  Range: [{string.Join(", ", Range)}]");
            Console.WriteLine($"{indentation}  OnActivation:");
            foreach (var activation in OnActivation)
            {
                activation.Print(indent + 2);
            }
        }

        public override object Evaluate(Context context)
        {
            return 0;
        }
    }

    public class ActivationNode : ASTNode
    {
        public CardEffectNode Effect { get; set; }
        public SelectorNode Selector { get; set; }
        public PostActionNode PostAction { get; set; }

        public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}Activation:");
            Effect.Print(indent + 2);
            Selector?.Print(indent + 2);
            PostAction?.Print(indent + 2);
        }

        public override object Evaluate(Context context)
        {
            return 0;
        }
    }

    public class SelectorNode : ASTNode
    {
        public string Source { get; set; }
        public bool Single { get; set; }
        public string Predicate { get; set; }

        public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}Selector:");
            Console.WriteLine($"{indentation}  Source: {Source}");
            Console.WriteLine($"{indentation}  Single: {Single}");
            Console.WriteLine($"{indentation}  Predicate: {Predicate}");
        }

        public override object Evaluate(Context context)
        {
            return 0;
        }
    }

    public class PostActionNode : ASTNode
    {
        public string Type { get; set; }
        public SelectorNode Selector { get; set; }

        public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}PostAction:");
            Console.WriteLine($"{indentation}  Type: {Type}");
            Selector?.Print(indent + 2);
        }

        public override object Evaluate(Context context)
        {
            return 0;
        }
    }

    public class CardEffectNode : ASTNode
    {
        public string Name { get; set; }
        public string Amount { get; set; }

        public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}Effect:");
            Console.WriteLine($"{indentation}  Name: {Name}");
            Console.WriteLine($"{indentation}  Amount: {Amount}");
        }

        public override object Evaluate(Context context)
        {
            return 0;
        }
    }

    public class ActionNode : ASTNode
    {
        public List<ASTNode> Children { get; set; } = new List<ASTNode>();

        public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}Action:");
            foreach (var child in Children)
            {
                child.Print(indent + 2);
            }
        }

        public override object Evaluate(Context context)
        {
            return 0;
        }
    }

    public abstract class ExpressionNode : ASTNode 
    { 
        
    }
    
    public class NumberLiteralNode : ExpressionNode
    {
        public int Value { get; set; }

        public override void Print(int indent = 0)
        {
            Console.WriteLine($"{new string(' ', indent)}NumberLiteral: {Value}");
        }

        public override object Evaluate(Context context)
        {
            return Value;
        }
    }

    public class BooleanLiteralNode : ExpressionNode
    {
        public bool Value { get; set; }

        public override void Print(int indent = 0)
        {
            Console.WriteLine($"{new string(' ', indent)}BooleanLiteral: {Value}");
        }

        public override object Evaluate(Context context)
        {
            return Value;
        }
    }

    public class VariableReferenceNode : ExpressionNode
    {
        public string Name { get; set; }

        public override void Print(int indent = 0)
        {
            Console.WriteLine($"{new string(' ', indent)}VariableReference: {Name}");
        }

        public override object Evaluate(Context context)
        {
            return context.GetVariable(Name);
        }
    }

    public class ConditionNode : ExpressionNode
    {
        public Func<Context, bool> Condition { get; set; }

        public override void Print(int indent = 0)
        {
            
        }
        public override object Evaluate(Context context)
        {
            return Condition(context);
        }
    }

    public class BinaryOperationNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public string Operator { get; set; }
        public ExpressionNode Right { get; set; }

        public override void Print(int indent = 0)
        {
            Console.WriteLine($"{new string(' ', indent)}Binary Operation: {Operator}");
            Left.Print(indent + 2);
            Right.Print(indent + 2);
        }

        public override object Evaluate(Context context)
        {
            var leftValue = Left.Evaluate(context);
            var rightValue = Right.Evaluate(context);

            switch (Operator)
            {
                case "+":
                    return (int)leftValue + (int)rightValue;
                case "-":
                    return (int)leftValue - (int)rightValue;
                case "*":
                    return (int)leftValue * (int)rightValue;
                case "/":
                    return (int)leftValue / (int)rightValue;
                case "&&":
                    return (bool)leftValue && (bool)rightValue;
                case "||":
                    return (bool)leftValue || (bool)rightValue;
                case "!":
                    return !(bool)leftValue;
                case "==":
                    return (int)leftValue == (int)rightValue;
                case "!=":
                    return (int)leftValue != (int)rightValue;
                case ">":
                    return (int)leftValue > (int)rightValue;
                case "<":
                    return (int)leftValue < (int)rightValue;
                case ">=":
                    return (int)leftValue >= (int)rightValue;
                case "<=":
                    return (int)leftValue <= (int)rightValue;
                
                
                default:
                    throw new InvalidOperationException($"Operador desconocido: {Operator}");
            }

        }
    }

    public class AssignmentNode : ASTNode
    {
        public string VariableName { get; set; }
        public ASTNode ValueExpression { get; set; }
        public List<string> AccessChain { get; set; } = new List<string>();
        public string Operator { get; set; }

        public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);

            // Imprimir la cadena de accesos anidados si existe
            if (AccessChain != null && AccessChain.Any())
            {
                Console.WriteLine($"{indentation}Access: {string.Join(".", AccessChain)}");
            }

            // Imprimir el nombre de la variable, el operador y el valor de la expresión
            Console.WriteLine($"{indentation}Assignment: {VariableName} {Operator}");

            // Imprimir el valor de la expresión en una nueva línea
            Console.Write($"{indentation}");
            ValueExpression.Print(indent + 2); // Aumentar la indentación para el valor
        }

        public override object Evaluate(Context context)
        {
            var value = ValueExpression.Evaluate(context);

            if (context.Variables.ContainsKey(VariableName))
            {
                context.SetVariable(VariableName, value);
            }
            else 
            {
                context.DefineVariable(VariableName, value);
            }

            return value;
        }
    }

    public class MemberAccessNode : ASTNode
    {
        public List<string> AccessChain { get; set; } = new List<string>();
        public List<ExpressionNode> Arguments { get; set; } =  new List<ExpressionNode>();
        public bool IsProperty { get; set; }

        public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            string memberType = IsProperty ? "PropertyAccess" : "MethodCall";
            Console.WriteLine($"{indentation}{memberType}: {string.Join(".", AccessChain)}");

            if (!IsProperty && Arguments.Count > 0)
            {
                Console.WriteLine($"{indentation}Arguments:");
                foreach (var arg in Arguments)
                {
                    arg.Print(indent + 4); 
                }
            }
        }

        public override object Evaluate(Context context)
        {
            var obj = context.GetVariable(AccessChain[0]); // El primer objeto en la cadena
            for (int i = 1; i < AccessChain.Count; i++)
            {
                if (IsProperty)
                {
                    var propertyInfo = obj.GetType().GetProperty(AccessChain[i]);
                    if (propertyInfo == null)
                    {
                        throw new Exception($"Propiedad '{AccessChain[i]}' no encontrada en '{obj.GetType().Name}'");
                    }
                    obj = propertyInfo.GetValue(obj);
                }
                else
                {
                    var methodInfo = obj.GetType().GetMethod(AccessChain[i]);
                    if (methodInfo == null)
                    {
                        throw new Exception($"Método '{AccessChain[i]}' no encontrado en '{obj.GetType().Name}'");
                    }
                    obj = methodInfo.Invoke(obj, Arguments.ToArray()); 
                }
            }
            return obj; // Devuelve el valor final
        }
    }
    
    public class WhileNode : ASTNode
    {
        public ExpressionNode Condition { get; set; }
        public List<ASTNode> Body { get; set; } = new List<ASTNode>();

        public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}While:");
            Console.WriteLine($"{indentation}  Condition:");
            Condition.Print(indent + 2);
            Console.WriteLine($"{indentation}  Body:");
            foreach (var statement in Body)
            {
                statement.Print(indent + 2);
            }
        }

        public override object Evaluate(Context context)
        {
            while ((bool)Condition.Evaluate(context))
            {
                foreach (var statement in Body)
                {
                    statement.Evaluate(context);
                }
            }
            return null;
        }
    }

    public class IfNode : ASTNode
    {
        public ExpressionNode Condition { get; set; }
        public List<ASTNode> Body { get; set; } = new List<ASTNode>();
        public List<ASTNode> ElseBody { get; set; } = new List<ASTNode>();
    
        public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}If:");
            Console.WriteLine($"{indentation}  Condition:");
            Condition.Print(indent + 2);
            Console.WriteLine($"{indentation}  Body:");
            foreach (var statement in Body)
            {
                statement.Print(indent + 2);
            }
            if (ElseBody.Any())
            {
                Console.WriteLine($"{indentation}Else:");
                foreach (var statement in ElseBody)
                {
                    statement.Print(indent + 2);
                }
            }
        }
    
        public override object Evaluate(Context context)
        {
            if ((bool)Condition.Evaluate(context))
            {
                foreach (var statement in Body)
                {
                    statement.Evaluate(context);
                }
            }
            else
            {
                foreach (var statement in ElseBody)
                {
                    statement.Evaluate(context);
                }
            }
            return null;
        }
    }

    public class ForNode : ASTNode
    {
        public string Item { get; set; }
        public VariableReferenceNode Collection { get; set; }
        public List<ASTNode> Body { get; set; } = new List<ASTNode>();

        public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}For:");
            Console.WriteLine($"{indentation}  Item: {Item}");
            Console.WriteLine($"{indentation}  Collection:");
            Collection.Print(indent + 2);
            Console.WriteLine($"{indentation}  Body:");
            foreach (var statement in Body)
            {
                statement.Print(indent + 2);
            }
        }

        public override object Evaluate(Context context)
        {
            var collection = Collection.Evaluate(context) as IEnumerable<object>;
            foreach (var item in collection)
            {
                context.DefineVariable(Item, item);
                foreach (var statement in Body)
                {
                    statement.Evaluate(context);
                }
            }
            return null;
        }
    }
}