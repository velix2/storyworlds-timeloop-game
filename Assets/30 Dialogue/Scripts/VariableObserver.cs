using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine;
using static Ink.Runtime.Story;

/// <summary>
/// This class manages the global variables used in the ink stories
/// It also detects changes in the variables so that the player state can be tracked
/// </summary>
public class VariableObserver
{
    // Create a dictionary from globals.ink
    public Dictionary<string, Ink.Runtime.Object> variables { get; private set; }

    private Story globalVariablesStory;

    public VariableObserver(TextAsset loadGlobalsJSON)
    {
        globalVariablesStory = new Story(loadGlobalsJSON.text);

        // initialize the dictionary
        variables = new Dictionary<string, Ink.Runtime.Object>();
        foreach (string name in globalVariablesStory.variablesState)
        {
            Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
        }
    }

    public void StartListening(Story story)
    {
        VariablesToStory(story);
        story.variablesState.variableChangedEvent += VariableChanged;
    }

    public void StopListening(Story story)
    {
        story.variablesState.variableChangedEvent -= VariableChanged;
    }

    private void VariableChanged(string name, Ink.Runtime.Object value)
    {
        Debug.Log("Variable Changed: " + name + " - " + value);
        if (variables.ContainsKey(name))
        {
            variables.Remove(name);
            variables.Add(name, value);
        }
    }

    public void VariablesToStory(Story story)
    {
        foreach (KeyValuePair<string, Ink.Runtime.Object> variable in variables)
        {
            story.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }

    /// <summary>
    /// Function to set a variable in globals.ink
    /// </summary>
    /// <param name="variableName">name of the variable</param>
    /// <param name="value"></param>
    public void SetVariable(string variableName, object value)
    {
        Ink.Runtime.Object inkValue;

        switch (value)
        {
            case int i:
                inkValue = new Ink.Runtime.IntValue(i);
                globalVariablesStory.variablesState[variableName] = i;
                break;

            case float f:
                inkValue = new Ink.Runtime.FloatValue(f);
                globalVariablesStory.variablesState[variableName] = f;
                break;

            case bool b:
                inkValue = new Ink.Runtime.BoolValue(b);
                globalVariablesStory.variablesState[variableName] = b;
                break;

            case string s:
                inkValue = new Ink.Runtime.StringValue(s);
                globalVariablesStory.variablesState[variableName] = s;
                break;

            case Ink.Runtime.InkList l:
                inkValue = new Ink.Runtime.ListValue(l);
                globalVariablesStory.variablesState[variableName] = l;
                break;

            default:
                Debug.LogError($"Unsupported value type: {value?.GetType()}");
                return;
        }

        if (variables.ContainsKey(variableName))
        {
            variables[variableName] = inkValue;
        }
        else
        {
            variables.Add(variableName, inkValue);
        }

        Debug.Log(variableName + " - " + value);
    }

    /// <summary>
    /// Get value according to variableName
    /// </summary>
    /// <param name="variableName"></param>
    /// <returns></returns>
    public object GetVariable(string variableName) 
    {
        variables.TryGetValue(variableName, out Ink.Runtime.Object variable);
        if (variable == null)
        {
            Debug.LogError("Variable with name " + variableName + " could not be found!");
            return null;
        }

        switch (variable)
        {
            case Ink.Runtime.IntValue intValue:
                return intValue.value;

            case Ink.Runtime.FloatValue floatValue:
                return floatValue.value;

            case Ink.Runtime.StringValue stringValue:
                return stringValue.value;

            case Ink.Runtime.BoolValue boolValue:
                return boolValue.value;

            case Ink.Runtime.ListValue listValue:
                return listValue.value; // InkList

            default:
                Debug.LogError(
                    $"Variable '{variableName}' has unsupported type {variable.GetType()}"
                );
                return null;
        }

    }
}