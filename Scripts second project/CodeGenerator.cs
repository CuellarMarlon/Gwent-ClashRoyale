using System.Text;

namespace GwentPlus
{
    public class CodeGenerator
    {
        private StringBuilder _code;
        private List<string> _effectMethods; // Lista para almacenar métodos generados
        private bool  _inAssignment;

        public CodeGenerator()
        {
            _code = new StringBuilder();
            _effectMethods = new List<string>();
            _inAssignment = false;
        }

        public List<string> GenerateCode(List<ASTNode> nodes)
        {
            foreach (var node in nodes)
            {
                GenerateNodeCode(node);
            }
            return _effectMethods; // Retornar la lista de métodos generados
        }

        private void GenerateNodeCode(ASTNode node)
        {
            switch (node)
            {
                case EffectNode effectNode:
                    GenerateEffectCode(effectNode);
                    break;
                case CardNode cardNode:
                    GenerateCardCode(cardNode);
                    break;
                case ActivationNode activationNode:
                    GenerateActivationCode(activationNode);
                    break;
                case SelectorNode selectorNode:
                    GenerateSelectorCode(selectorNode);
                    break;
                case PostActionNode postActionNode:
                    GeneratePostActionCode(postActionNode);
                    break;
                case ActionNode actionNode:
                    GenerateActionCode(actionNode);
                    break;
                case NumberLiteralNode numberLiteralNode:
                    GenerateNumberLiteralCode(numberLiteralNode);
                    break;
                case BooleanLiteralNode booleanLiteralNode:
                    GenerateBooleanLiteralCode(booleanLiteralNode);
                    break;
                case VariableReferenceNode variableReferenceNode:
                    GenerateVariableReferenceCode(variableReferenceNode);
                    break;
                case BinaryOperationNode binaryOperationNode:
                    GenerateBinaryOperationCode(binaryOperationNode);
                    break;
                case AssignmentNode assignmentNode:
                    GenerateAssignmentCode(assignmentNode);
                    break;
                case MemberAccessNode memberAccessNode:
                    GenerateMemberAccessCode(memberAccessNode);
                    break;
                case WhileNode whileNode:
                    GenerateWhileCode(whileNode);
                    break;
                case IfNode ifNode:
                    GenerateIfCode(ifNode);
                    break;
                case ForNode forNode:
                    GenerateForCode(forNode);
                    break;
                default:
                    throw new NotSupportedException($"Tipo de nodo no soportado: {node.GetType()}");
            }
        }

        private void GenerateEffectCode(EffectNode effect)
        {
            var methodName = effect.Name.Substring(1, effect.Name.Length - 2);
            _code.Clear();
            _code.AppendLine($"public void {methodName}(Context context, List<Card> targets) {{");
            foreach (var action in effect.Actions)
            {
                GenerateNodeCode(action);
            }
            _code.AppendLine("}");
            _effectMethods.Add(_code.ToString());
        }

        private void GenerateCardCode(CardNode card)
        {
            _code.AppendLine($"public class {card.Name.Substring(1, card.Name.Length - 2)}Card {{");
            _code.AppendLine($"    public string Type {{ get; set; }} = \"{card.Type}\";");
            _code.AppendLine($"    public string Faction {{ get; set; }} = \"{card.Faction}\";");
            _code.AppendLine($"    public int Power {{ get; set; }} = {card.Power};");
            _code.AppendLine($"    public List<string> Range {{ get; set; }} = new List<string> {{ {string.Join(", ", card.Range)} }};");

            foreach (var activation in card.OnActivation)
            {
                GenerateNodeCode(activation);
            }
            _code.AppendLine("}");
        }

        private void GenerateActivationCode(ActivationNode activation)
        {
            _code.AppendLine("public void OnActivate(Context context) {");
            if (activation.Selector != null)
            {
                GenerateNodeCode(activation.Selector);
            }
            if (activation.PostAction != null)
            {
                GenerateNodeCode(activation.PostAction);
            }
            _code.AppendLine("}");
        }

        private void GenerateSelectorCode(SelectorNode selector)
        {
            _code.AppendLine($"// Selector: Source = {selector.Source}, Single = {selector.Single}, Predicate = {selector.Predicate}");
        }

        private void GeneratePostActionCode(PostActionNode postAction)
        {
            _code.AppendLine($"// PostAction: Type = {postAction.Type}");
            if (postAction.Selector != null)
            {
                GenerateNodeCode(postAction.Selector);
            }
        }

        private void GenerateActionCode(ActionNode action)
        {
            foreach (var child in action.Children)
            {
                GenerateNodeCode(child);
            }
        }

        private void GenerateNumberLiteralCode(NumberLiteralNode number)
        {
            _code.Append($"{number.Value}");
        }

        private void GenerateBooleanLiteralCode(BooleanLiteralNode boolean)
        {
            _code.AppendLine($"{boolean.Value.ToString().ToLower()}");
        }

        private void GenerateVariableReferenceCode(VariableReferenceNode variable)
        {
            _code.Append($"{variable.Name}");
        }

        private void GenerateBinaryOperationCode(BinaryOperationNode binary)
        {
            _code.Append("(");
            GenerateNodeCode(binary.Left);
            _code.Append($" {binary.Operator} ");
            GenerateNodeCode(binary.Right);
            _code.Append(")");
        }

        private void GenerateAssignmentCode(AssignmentNode assignment)
        {
            _inAssignment = true;
            string access = "";
            if (assignment.AccessChain != null)
            {
                for (int i = 0; i < assignment.AccessChain.Count - 1; i++)
                {
                    access += assignment.AccessChain[i] + ".";
                }
            }
            _code.Append($"{assignment.VariableName} {assignment.Operator} ");
            GenerateNodeCode(assignment.ValueExpression);
            _code.AppendLine(";");
            _inAssignment = false;
        }

        private void GenerateMemberAccessCode(MemberAccessNode memberAccess)
        {
            var member = string.Join(".", memberAccess.AccessChain);
            if (memberAccess.IsProperty)
            {
                if (_inAssignment) _code.Append($"{member}");
                else _code.AppendLine($"{member};");
            }
            else
            {
                var args = string.Join(", ", memberAccess.Arguments);
                if (_inAssignment) _code.Append($"{member}({args})");
                else _code.AppendLine($"{member}({args});");
            }
        }

        private void GenerateWhileCode(WhileNode whileNode)
        {
            _code.Append($"while (");
            GenerateNodeCode(whileNode.Condition);
            _code.AppendLine($") {{");
            foreach (var statement in whileNode.Body)
            {
                GenerateNodeCode(statement);
            }
            _code.AppendLine("}");
        }

        private void GenerateIfCode(IfNode ifNode)
        {
            _code.Append($"if (");
            GenerateNodeCode(ifNode.Condition);
            _code.AppendLine($") {{");
            foreach (var statement in ifNode.Body)
            {
                GenerateNodeCode(statement);
            }
            _code.AppendLine("}");
            if (ifNode.ElseBody.Count > 0)
            {
                _code.AppendLine("else {");
                foreach (var statement in ifNode.ElseBody)
                {
                    GenerateNodeCode(statement);
                }
                _code.AppendLine("}");
            }
        }

        private void GenerateForCode(ForNode forNode)
        {
            _code.AppendLine($"foreach (var {forNode.Item} in {forNode.Collection.Name}) {{");
            foreach (var statement in forNode.Body)
            {
                GenerateNodeCode(statement);
            }
            _code.AppendLine("}");
        }
    }
}