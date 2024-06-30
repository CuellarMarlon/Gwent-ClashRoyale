using System;
using System.Text.RegularExpressions;

namespace GwentPlus
{
    public class Lexer
    {
        public void Tokenize (string input)
        {
            //Definir las expresiones regulares
            string numberPattern = @"-?\b\d+\b";
            string keywordPattern = @"\b()\b";

            //Crear expresiones regulares
            Regex numberRegex = new Regex(numberPattern);
            Regex keywordRegex = new Regex(keywordPattern);

            //Obtener los matches
            MatchCollection numberMatches = numberRegex.Matches(input);
            MatchCollection keywordMatches = keywordRegex.Matches(input);

            //Imprimir los matches
            Console.WriteLine("Numeros encontrados:");
            foreach (Match match in numberMatches)
            {
                Console.WriteLine(match.Value);
            }
            
            Console.WriteLine("Palabras encontradas");
            foreach (Match match in keywordMatches)
            {
                Console.WriteLine(match.Value);
            }

        }
    }
}