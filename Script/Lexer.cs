using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GwentPlus
{
    //Clase para analizar y tokenizar la entrada 
    public class Lexer
    {
        private List<Token> tokens; // Lista de tokens identificados
        private string input; // Entrada a analizar 

        public Lexer(string input)
        {
            this.input = input;
            this.tokens = new List<Token>();
        }

        public List<Token> Tokenize()
        {
            // Ignorar espacios al principio y al final de la entrada
            input = input.Trim();

            // Definir expresiones regulares
            string keywordPattern =@"\b(effect|card|for|while|if)\b";
            string identifiersPattern = @"^[a-zA-Z_][a-zA-Z0-9_]*";
            string lambdaOperetorPattern =@"=>\b"; 
            string arithmeticOperatorPattern = @"(?:[+\-*/%])";
            string relationalOperatorPattern = @"(?:==|!=|>=|<=|>|<)";
            string logicOperatorPattern = @"(?:&&|\|\||!)";
            string assignmentOperatorPattern = @"(?:=|\+=|-=|\*=|/=|%=|:)";
            string unaryOperatorPattern = @"(?:\+\+|--)";
            string numberPattern = @"\d+";
            string delimiterPattern = @"[\(\)\{\}\[\]]";
            string separatorPattern = @",";
            string semicolonPattern = @";";
            string accessPattern = @"\.";
            string stringPattern = @"""(([^""\\]|\.)*?)""";





            // Crear un diccionario para mapear patrones de tokens a sus tipos
            var patterns = new Dictionary<string, string>
            {
                {stringPattern, "String" },
                { keywordPattern, "KeyWord" },
                { identifiersPattern, "Identifier" },
                { numberPattern, "Number" },
                { lambdaOperetorPattern, "LambdaOperator" },
                { arithmeticOperatorPattern, "ArithmeticOperator" },
                { relationalOperatorPattern, "RelationalOperator" },
                { logicOperatorPattern, "LogicOperator" },
                { assignmentOperatorPattern, "AssignmentOperator" },
                { unaryOperatorPattern, "UnaryOperator" },
                { delimiterPattern, "Delimiter" },
                { separatorPattern, "Separator" },
                { semicolonPattern, "Semicolon" },
                { accessPattern, "Access" }
            };

            // Mientras haya entrada por analizar
            while (!string.IsNullOrEmpty(input))
            {
                // Intentar encontrar el mejor token que coincida al principio de la entrada
                string bestMatch = null;
                string bestTokenType = null;
                int bestLength = 0;

                foreach (var pair in patterns)
                {
                    var match = Regex.Match(input, pair.Key);
                    if (match.Success && match.Index == 0 && match.Length > bestLength)
                    {
                        bestMatch = match.Value;
                        bestTokenType = pair.Value;
                        bestLength = match.Length;
                    }
                }

                // Si no se encontró ninguna coincidencia, avanzar en la entrada
                if (bestMatch == null)
                {
                    input = input.Substring(1).Trim();
                    continue;
                }

                // Crear y agregar el token encontrado
                tokens.Add(new Token(bestTokenType, bestMatch));

                // Avanzar en la cadena de entrada para continuar con el análisis
                input = input.Substring(bestLength).Trim();

                // Si el token encontrado es un operador unario y hay otro operador unario justo después, combinarlos
                if (bestTokenType == "UnaryOperator" && !string.IsNullOrEmpty(input))
                {
                    var nextMatch = Regex.Match(input, unaryOperatorPattern);
                    if (nextMatch.Success && nextMatch.Index == 0)
                    {
                        tokens[tokens.Count - 1].Value += nextMatch.Value;
                        input = input.Substring(nextMatch.Length).Trim();
                    }
                }
            }

            return tokens;
        }
    }
}