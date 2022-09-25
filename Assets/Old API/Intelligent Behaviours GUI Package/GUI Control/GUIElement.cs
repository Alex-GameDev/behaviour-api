using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GUIElement : ScriptableObject
{
    /// <summary>
    /// The <see cref="Rect"/> for visualizing the <see cref="GUIElement"/>
    /// </summary>
    public Rect windowRect;

    /// <summary>
    /// Unique <see cref="string"/> to differentiate this <see cref="GUIElement"/> from others
    /// </summary>
    public string identificator;

    /// <summary>
    /// The <see cref="ClickableElement"/> that this <see cref="GUIElement"/> was created in, if it was. Null if it wasn't
    /// </summary>
    public ClickableElement parent;

    /// <summary>
    /// Returns a unique <see cref="string"/>
    /// </summary>
    /// <returns></returns>
    public static string UniqueID()
    {
        return Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Gets the type of the <see cref="GUIElement"/> properly written
    /// </summary>
    /// <returns></returns>
    public abstract string GetTypeString();

    /// <summary>
    /// Compares this <see cref="GUIElement"/> with <paramref name="other"/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool Equals(object other)
    {
        if (this.identificator != ((GUIElement)other).identificator)
            return false;

        return true;
    }

    /// <summary>
    /// Draws all elements inside the <see cref="GUIElement"/>
    /// </summary>
    public abstract void DrawWindow();

    /// <summary>
    /// Creates and returns an <see cref="XMLElement"/> that corresponds to this <see cref="GUIElement"/>
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public abstract XMLElement ToXMLElement(params object[] args);

    /// <summary>
    /// Returns a copy of this <see cref="GUIElement"/> with a new identificator, and proper references to other <see cref="GUIElement"/>s
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public abstract GUIElement CopyElement(params object[] args);

    /// <summary>
    /// Modifies <paramref name="name"/> to remove unnecesary spaces and newlines
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string CleanName(string name)
    {
        string result;
        var numberChars = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        var spacesAndNewlines = new[] { ' ', '\n' };

        result = name.Trim(spacesAndNewlines);
        result = result.TrimStart(numberChars);

        return result;
    }
}
