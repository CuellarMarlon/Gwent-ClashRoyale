using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GwentPlus
{
    //Clase para representar un token
    public class Token
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public Token(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}