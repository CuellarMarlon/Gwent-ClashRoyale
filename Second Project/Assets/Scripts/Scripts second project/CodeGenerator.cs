using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace GwentPlus
{
    public class CodeGenerator
    {
        private List<ASTNode> _nodes;
        public List<Card> _cards = new List<Card>(); // Almacena las cartas creadas
        public Context context = new Context(null!); //para llevar las variables

        public CodeGenerator(List<ASTNode> nodes)
        {
            _nodes = nodes;
        }

        public void GenerateCode(string outputPath)
        {
            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                writer.WriteLine("namespace GwentPlus");
                writer.WriteLine("{");


                // Escribir la definición de la clase EffectCreated
                writer.WriteLine("public class EffectCreated");
                writer.WriteLine("{");

                foreach (var node in _nodes)
                {
                    if (node is EffectNode effectNode)
                    {
                        GenerateEffectMethod(writer, effectNode);
                    }
                    else if (node is CardNode cardNode)
                    {
                        CreateCardInstance(cardNode);
                    }
                }

                writer.WriteLine("}");
                writer.WriteLine("}");

            }
        }

        private void GenerateEffectMethod(StreamWriter writer, EffectNode effectNode)
        {
            string parametersString;
            // Genera una lista de parámetros basada en el diccionario Params del EffectNode
            if(effectNode.Params.Count != 0)
            {
                var parameters = new List<string>();
                foreach (var param in effectNode.Params)
                {
                    var type = param.Value;
                    if (type == "Number")
                    {
                        parameters.Add($"int {param.Key}");
                    }
                    else if (type == "String")
                    {
                        parameters.Add($"string {param.Key}");
                    }
                    else if (type == "Bool")
                    {
                        parameters.Add($"bool {param.Key}");
                    }

                }

                parametersString = ", " + string.Join(", ", parameters);
            }
            else
            {
                parametersString = "";
            }

            writer.WriteLine($"    public void {effectNode.Name.Substring(1, effectNode.Name.Length - 2)}Effect(CardList targets, GameContext context {parametersString})");
            writer.WriteLine("    {");
            writer.WriteLine("         UnityEngine.Debug.Log(\"EffectoEjecutado\");");
            writer.WriteLine("         UnityEngine.Debug.Log(\"Current:\" + GameManager.Instance.currentPlayer);");



            foreach (var action in effectNode.Actions.Children)
            {
                GenerateActionCode(writer, action);
            }

            writer.WriteLine("    }");
            writer.WriteLine();
        }

        private void GenerateActionCode(StreamWriter writer, ASTNode action)
        {
            if (action is AssignmentNode assignmentNode)
            {
                string variableDeclaration = context.Variables.ContainsKey(assignmentNode.VariableName) ? "" : "var";
                string access = "";
                if (assignmentNode.AccessChain != null)
                {
                    for (int i = 0; i < assignmentNode.AccessChain.Count; i++)
                    {
                        if (i < assignmentNode.AccessChain.Count - 1)
                        access += assignmentNode.AccessChain[i] + ".";
                        else
                        access += assignmentNode.AccessChain[i];
                    }
                    variableDeclaration = "";
                }
                else access = assignmentNode.VariableName;

                writer.WriteLine($"        {variableDeclaration} {access} {assignmentNode.Operator} {GenerateValueExpressionCode(assignmentNode.ValueExpression)};");
                if (!context.Variables.ContainsKey(assignmentNode.VariableName))
                {
                    context.DefineVariable(assignmentNode.VariableName, null); // Asumiendo que el valor se asignará más adelante o es irrelevante en este contexto
                }
            }
            else if (action is IfNode ifNode)
            {
                writer.WriteLine($"        if ({GenerateValueExpressionCode(ifNode.Condition)})");
                writer.WriteLine("        {");
                foreach (var statement in ifNode.Body)
                {
                    GenerateActionCode(writer, statement);
                }
                writer.WriteLine("        }");
            }
            else if (action is WhileNode whileNode)
            {
                writer.WriteLine($"        while ({GenerateValueExpressionCode(whileNode.Condition)})");
                writer.WriteLine("        {");
                foreach (var statement in whileNode.Body)
                {
                    GenerateActionCode(writer, statement);
                }
                writer.WriteLine("        }");
            }
            else if (action is ForNode forNode)
            {
                writer.WriteLine($"        foreach (Card {forNode.Item} in {GenerateValueExpressionCode(forNode.Collection)})");
                writer.WriteLine("        {");
                foreach (var statement in forNode.Body)
                {
                    GenerateActionCode(writer, statement);
                }
                writer.WriteLine("        }");
            }
            else if (action is MemberAccessNode memberAccessNode)
            {
                // Aquí puedes manejar el acceso a miembros y llamadas a métodos
                if (memberAccessNode.IsProperty)
                {
                    // Si es una propiedad, simplemente accede a la propiedad
                    writer.WriteLine($"        {string.Join(".", memberAccessNode.AccessChain)}");
                }
                else
                {
                    /// Si no es una propiedad y tiene argumentos, es una llamada a método
                    string arguments = string.Join(", ", memberAccessNode.Arguments.Select(arg => GenerateValueExpressionCode(arg)));
                    writer.WriteLine($"        {string.Join(".", memberAccessNode.AccessChain)}({arguments});");
                }
            }
            
        }

        private string GenerateValueExpressionCode(ASTNode valueExpression)
        {
            if (valueExpression is NumberLiteralNode numberLiteral)
            {
                return numberLiteral.Value.ToString(); // Asumiendo que Value es un número
            }
            else if (valueExpression is BooleanLiteralNode booleanLiteral)
            {
                return booleanLiteral.Value.ToString().ToLower(); // Asumiendo que Value es un bool
            }
            else if (valueExpression is VariableReferenceNode variableReferenceNode)
            {
                return variableReferenceNode.Name;
            }
            else if (valueExpression is BinaryOperationNode binaryOperationNode)
            {
                return $"{GenerateValueExpressionCode(binaryOperationNode.Left)} {binaryOperationNode.Operator} {GenerateValueExpressionCode(binaryOperationNode.Right)}";
            }
            else if (valueExpression is MemberAccessNode memberAccessNode)
            {
                // Aquí puedes manejar el acceso a miembros y llamadas a métodos
                if (memberAccessNode.IsProperty)
                {
                    // Si es una propiedad, simplemente accede a la propiedad
                    return $"{string.Join(".", memberAccessNode.AccessChain)}";
                }
                else
                {
                    // Si no es una propiedad y tiene argumentos, es una llamada a método
                    string arguments = string.Join(", ", memberAccessNode.Arguments.Select(arg => GenerateValueExpressionCode(arg)));
                    return $"{string.Join(".", memberAccessNode.AccessChain)}({arguments})";
                }
            }
            // Agrega más casos según sea necesario

            // Si no se reconoce el tipo, simplemente devuelve una cadena vacía o un valor predeterminado
            return "";
        }

        private void CreateCardInstance(CardNode cardNode)
        {
            // Crea una nueva instancia de CardData
            Card cardData = ScriptableObject.CreateInstance<Card>();

            // Asigna las propiedades
            cardData.IsCreated = true;
            cardData.Name = cardNode.Name.Substring(1, cardNode.Name.Length - 2);
            cardData.Type = (CardType)Enum.Parse(typeof(CardType), cardNode.Type.Substring(1, cardNode.Type.Length - 2));
            cardData.Faction = (Faction)Enum.Parse(typeof(Faction), cardNode.Faction.Substring(1, cardNode.Faction.Length - 2));
            cardData.Power = cardNode.Power;
            cardData.Range = Array.ConvertAll(cardNode.Range.ToArray(), r => (Range)Enum.Parse(typeof(Range), r.Substring(1, r.Length - 2)));
            cardData.OnActivation = new List<Effects>();
            cardData.EffectCreated = new EffectCreated();

            // Aquí puedes manejar los efectos de activación si es necesario
            foreach (var activation in cardNode.OnActivation)
            {
                cardData.OnActivation.Add(CreateEffect(activation));
            }

            _cards.Add(cardData);
        }

        private Effects CreateEffect(ActivationNode activation)
        {
            Effects effect = new Effects
            {
                Name = activation.Effect?.Name ?? "DefaultEffectName", // Provide a default value or handle null differently
                Params = activation.Effect != null ? activation.Effect.Params : new List<object>(),
                Source = activation.Selector?.Source ?? "DefaultSource",
                Single = activation.Selector?.Single ?? false,
                Predicate = new Predicate // Aquí se crea una nueva instancia de Predicate y se asignan sus propiedades
                {
                    LeftMember = activation.Selector?.Predicate?.LeftMember ?? "DefaultLeftMember",
                    Operator = activation.Selector?.Predicate?.Operator ?? "DefaultOperator",
                    RightMember = activation.Selector?.Predicate?.RightMember ?? "DefaultValue"
                },                    
            };
            
            return effect;
        }
    }
}