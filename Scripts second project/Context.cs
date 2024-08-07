using System.Collections.Generic;

namespace GwentPlus
{
    public class Context
    {
        public Dictionary<string, object> Variables { get; } = new Dictionary<string, object>();

        public object GetVariable(string name)
        {
            if (Variables.TryGetValue(name, out var value))
            {
                return value;
            }
            throw new Exception($"Variable '{name}' no definida.");
        }
    }
}