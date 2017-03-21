using System.Collections.Generic;
using System.Collections;
using Scripting;
using System.Linq;
using Datum;

public class ScriptRuntime 
{
    Dictionary<string, List<ScriptObject>> ScriptObjects
    {
        get
        {
            return scriptObjects;
        }
    }

    private readonly Dictionary<string, List<ScriptObject>> scriptObjects = new Dictionary<string, List<ScriptObject>>();    
    private readonly HashSet<Variable> variables = new HashSet<Variable>();

    public ScriptObject GetScriptObject(IRuntimeResolvable resolvable)
    {
        return resolvable.GetResolvedValue(this);
    }

    public ScriptObject[] GetScriptObjects(string tableId)
    {
        return scriptObjects[tableId].ToArray();
    }

    public ScriptObject GetScriptObject(string id)
    {
        foreach (var list in scriptObjects.Values)
        {
            ScriptObject scriptObject = list.FirstOrDefault((obj) => obj.Id == id);

            if (scriptObject != null)
                return scriptObject;
        }

        return null;
    }
 
    public void AddVariable(Variable variable)
    {
        if (!Variable.IsValidIdentifier(variable.Identifier))
        {
            throw new InvalidIdentifierException(variable.Identifier);
        }

        variables.Add(variable);
    }

    public void AddScriptObject(string contentId, ScriptObject scriptObject)
    {
        if (!scriptObjects.ContainsKey(contentId))
        {
            scriptObjects[contentId] = new List<ScriptObject>();
        }
        
        Diagnostics.Log("Adding script object {0}", scriptObject.Id);
        scriptObject.RegisterRuntime(this);
        scriptObjects[contentId].Add(scriptObject);
    }

    public Variable GetVariableWithIdentifier(string identifier)
    {
        Diagnostics.Log("Trying to get variable with identifier: {0}", identifier);
        if (!Variable.IsValidIdentifier(identifier))
        {
            throw new InvalidIdentifierException(identifier);
        }

        return variables.FirstOrDefault((variable) => variable.Identifier == identifier);
    }
}