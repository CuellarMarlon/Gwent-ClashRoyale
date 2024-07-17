using System;
using System.Collections.Generic;

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
        public string Action { get; set; }

        public EffectNode()
        {
            Params = new Dictionary<string, string>();
        }

        public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}Effect: {Name}");
            foreach (var param in Params)
            {
                Console.WriteLine($"{indentation}  Param: {param.Key} = {param.Value}");
            }
            Console.WriteLine($"{indentation}  Action: {Action}");
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
        public CardEffectNode Effect { get; set; }//!!!!!!
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
        public string Amount { get; set; }//!!!

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

    public class ForLoopNode : ActionNode
    {
        public ASTNode Condition { get; set; }
        public ASTNode Increment { get; set; }

        public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}ForLoop:");
            Console.WriteLine($"{indentation}  Condition: {Condition}");
            Console.WriteLine($"{indentation}  Increment: {Increment}");
            base.Print(indent + 2); // Llama al método Print de la clase base para imprimir los hijos
        }
    }

    public class WhileLoopNode : ActionNode
    {
        public ASTNode Condition { get; set; }

        public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}WhileLoop:");
            Console.WriteLine($"{indentation}  Condition: {Condition}");
            base.Print(indent + 2); // Llama al método Print de la clase base para imprimir los hijos
        }
    }

    public class IfStatementNode : ActionNode
    {
        public ASTNode Condition { get; set; }
        public ActionNode TrueBranch { get; set; }
        public ActionNode FalseBranch { get; set; }

        public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}IfStatement:");
            Console.WriteLine($"{indentation}  Condition: {Condition}");
            Console.WriteLine($"{indentation}TrueBranch:");
            TrueBranch?.Print(indent + 2);
            Console.WriteLine($"{indentation}FalseBranch:");
            FalseBranch?.Print(indent + 2);
        }
    }

    public class AssignmentNode : ActionNode
    {
        public string Variable { get; set; }
        public ASTNode Value { get; set; }

        public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}Assignment:");
            Console.WriteLine($"{indentation}Variable: {Variable}");
            Console.WriteLine($"{indentation}Value: {Value}");
        }
    }

    public class MethodCallNode : ActionNode
    {
        public string MethodName { get; set; }
        public List<ASTNode> Arguments { get; set; } = new List<ASTNode>();
    
        public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}MethodCall:");
            Console.WriteLine($"{indentation}MethodName: {MethodName}");
            // Console.WriteLine($"{indentation}Arguments: "[{string.Join(Arguments.Where(a => a is VariableAccessNode).Select(a => ((VariableAccessNode)a).VariableName), "", "")}]"");
        }
    }

    public class VariableAccessNode : ActionNode
    {
        public string VariableName { get; set; }

        public override void Print(int indent = 0)
        {
            string indentation = new string(' ', indent);
            Console.WriteLine($"{indentation}VariableAccess:");
            Console.WriteLine($"{indentation}VariableName: {VariableName}");
        }
    }
}