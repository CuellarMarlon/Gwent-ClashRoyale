using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GwentPlus
{
    public abstract class ASTNode
    {
        public abstract void Print(int indent = 0);

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
    }

    public class MethodCallNode : ActionNode
    {
        public string MethodName { get; set; }
        public List<string> Arguments { get; set; } = new List<string>();
    
       public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}MethodCall: {MethodName}");
            Console.Write($"{indentation + " "}Arguments: ");
            foreach (var argument in Arguments)
            {
                Console.Write(argument + ", "); 
            }
            Console.Write("\n");
        }

    }

    public abstract class ExpressionNode : ASTNode 
    { 
        public abstract object Evaluate();
    }
    
    public class NumberLiteralNode : ExpressionNode
    {
        public int Value { get; set; }

        public override void Print(int indent = 0)
        {
            Console.WriteLine($"{new string(' ', indent)}NumberLiteral: {Value}");
        }

        public override object Evaluate()
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

        public override object Evaluate()
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

        public override object Evaluate()
        {
            return Name;
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

        public override object Evaluate()
        {
            var leftValue = Left.Evaluate();
            var rightValue = Right.Evaluate();

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

}