using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public abstract class BaseNode : GUIElement
{
    /// <summary>
    /// Name of the <see cref="BaseNode"/>
    /// </summary>
    public string nodeName = "";

    /// <summary>
    /// The <see cref="ClickableElement"/> that is contained in this node
    /// </summary>
    public ClickableElement subElem;

    /// <summary>
    /// Width of the <see cref="GUIElement.windowRect"/>
    /// </summary>
    public static int width = 140;

    /// <summary>
    /// Height of the <see cref="GUIElement.windowRect"/>
    /// </summary>
    public static int height = 63;

    /// <summary>
    /// The Initializer for the <seealso cref="BaseNode"/>
    /// </summary>
    public void InitBaseNode(ClickableElement parent, string id = null)
    {
        this.parent = parent;
        if(id == null)
        {
            identificator = UniqueID();
        }
        else
        {
            identificator = id;
        }
    }

    /// <summary>
    /// Draws all the elements inside the <see cref="BaseNode"/>
    /// </summary>
    public override abstract void DrawWindow();
}
