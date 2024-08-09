using System.Collections.Generic;

namespace GwentPlus
{
    public class Context
    {
        public Dictionary<string, object> Variables { get; } = new Dictionary<string, object>();

        public void DefineVariable(string name, object value)
        { 
            if (Variables.ContainsKey(name))
            {
                throw new Exception($"Variable '{name}' ya esta definida.");
            }
            Variables[name] = value;
        }
        
        public object GetVariable(string name)
        {
            if (Variables.TryGetValue(name, out var value))
            {
                return value;
            }
            throw new Exception($"Variable '{name}' no definida.");
        }    

        public void SetVariable(string name, object value)
        {
            if (!Variables.ContainsKey(name))
            {
                throw new Exception($"Variable '{name}' no definida.");
            }

            var currentValue = Variables[name];

            if (currentValue.GetType() != value.GetType())
            {
                throw new Exception($"No se puede asignar un valor de tipo '{value.GetType().Name}' a la variable '{name}' de tipo '{currentValue.GetType().Name}'.");
            }

            Variables[name] = value;
        }
    }
}