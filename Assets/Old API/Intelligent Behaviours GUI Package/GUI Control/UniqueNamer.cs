using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UniqueNamer : ScriptableObject
{
    /// <summary>
    /// List of keys to identify all <see cref="names"/> in this <see cref="UniqueNamer"/>
    /// </summary>
    private List<string> keys = new List<string>();

    /// <summary>
    /// List of all unique names in this <see cref="UniqueNamer"/>
    /// </summary>
    private List<string> names = new List<string>();

    /// <summary>
    /// Adds and returns a <paramref name="name"/>, and makes sure it's not repeated by adding a number at the end if necessary
    /// </summary>
    /// <param name="key"></param>
    /// <param name="name"></param>
    /// <param name="count"></param>
    /// <returns>The added name</returns>
    public string AddName(string key, string name, int count = 0, bool rename = false)
    {
        string nameToAdd = count > 0 ? name + count : name;

        if (keys.Contains(key))
        {
            if (rename)
                names[keys.IndexOf(key)] = nameToAdd;

            return names[keys.IndexOf(key)];
        }
        else if (names.Contains(nameToAdd))
        {
            return AddName(key, name, ++count);
        }
        else
        {
            keys.Add(key);
            names.Add(nameToAdd);

            return nameToAdd;
        }
    }

    /// <summary>
    /// Returns the name corresponding to the <paramref name="key"/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetName(string key)
    {
        int keyIndex = keys.IndexOf(key);

        if (keyIndex >= 0)
        {
            return names[keyIndex];
        }
        else
        {
            if (Debugger.isDebug)
                Debug.LogError("[OnGetName] Key not found");
            return null;
        }
    }

    /// <summary>
    /// Removes the name corresponding to the <paramref name="key"/>
    /// </summary>
    /// <param name="key"></param>
    public void RemoveName(string key)
    {
        int index = keys.IndexOf(key);

        if (index >= 0)
        {
            keys.RemoveAt(index);
            names.RemoveAt(index);
        }
        else
        {
            if (Debugger.isDebug)
                Debug.LogError("[OnDelete] Key not found");
        }
    }
}