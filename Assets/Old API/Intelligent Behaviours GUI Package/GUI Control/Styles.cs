using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Styles
{
    public static GUIStyle TitleText = new GUIStyle()
    {
        alignment = TextAnchor.LowerCenter,
        fontSize = 13
    };

    public static GUIStyle CenteredTitleText = new GUIStyle()
    {
        alignment = TextAnchor.MiddleCenter,
        fontSize = 13
    };

    public static GUIStyle Exponent = new GUIStyle()
    {
        alignment = TextAnchor.UpperCenter,
        fontSize = 13
    };

    public static GUIStyle SubTitleText = new GUIStyle()
    {
        alignment = TextAnchor.MiddleCenter,
        fontSize = 10
    };

    public static GUIStyle NonEditable = new GUIStyle()
    {
        alignment = TextAnchor.MiddleCenter,
        fontSize = 13,
        normal = new GUIStyleState()
        {
            textColor = new Color(0, 0, 0, 0.5f)
        }
    };

    public static GUIStyle ErrorPrompt = new GUIStyle()
    {
        fontStyle = FontStyle.Bold
    };

    public static GUIStyle OptionsButton = new GUIStyle()
    {
        hover = new GUIStyleState()
        {
            textColor = Color.grey
        },
        alignment = TextAnchor.UpperRight,
        fontSize = 15,
        fontStyle = FontStyle.Bold
    };

    public static GUIStyle TopBarButton = new GUIStyle(GUI.skin.button)
    {
        alignment = TextAnchor.MiddleLeft
    };

    public static GUIStyle WarningLabel = new GUIStyle(GUI.skin.label)
    {
        fontStyle = FontStyle.Bold,
        normal = new GUIStyleState()
        {
            textColor = Color.red
        }
    };
}
