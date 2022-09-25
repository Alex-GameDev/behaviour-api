using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

public class PopupWindow : EditorWindow
{
    enum typeOfPopup
    {
        Delete,
        FailedExport,
        Warning
    }

    /// <summary>
    /// Type of popup
    /// </summary>
    static typeOfPopup PopupType;

    static ClickableElement exportingElem;

    /// <summary>
    /// Type of the <see cref="ClickableElement"/> that will be deleted
    /// </summary>
    static string typeOfElem;

    /// <summary>
    /// The <see cref="NodeEditor"/> window that is calling for this <see cref="PopupWindow"/> to be shown
    /// </summary>
    private static NodeEditor privateEditor;

    static NodeEditor senderEditor
    {
        get
        {
            if (!privateEditor)
                privateEditor = EditorWindow.GetWindow<NodeEditor>();
            return privateEditor;
        }
    }

    /// <summary>
    /// The List of <see cref="GUIElement"/> that will be deleted
    /// </summary>
    static List<GUIElement> elems;

    static string warningText = "";

    /// <summary>
    /// Width of the <see cref="PopupWindow"/>
    /// </summary>
    static int width = 300;

    /// <summary>
    /// Height of the <see cref="PopupWindow"/>
    /// </summary>
    static int height = 150;

    /// <summary>
    /// Initializer for the <see cref="PopupWindow"/> when deleting a <see cref="GUIElement"/>
    /// </summary>
    /// <param name="focusedElems"></param>
    /// <param name="type"></param>
    public static void InitDelete(params GUIElement[] focusedElems)
    {
        var foo = senderEditor;

        PopupType = typeOfPopup.Delete;

        elems = new List<GUIElement>(focusedElems);
        typeOfElem = elems[0]?.GetTypeString();

        PopupWindow window = ScriptableObject.CreateInstance<PopupWindow>();
        window.ShowModalUtility();

        window.titleContent = new GUIContent("Delete");
        window.position = new Rect(senderEditor.position.center.x - width / 2, senderEditor.position.center.y - height / 2, width, height);
    }

    /// <summary>
    /// Initializer for the <see cref="PopupWindow"/> when failed at exporting a <see cref="ClickableElement"/>
    /// </summary>
    /// <param name="focusElem"></param>
    /// <param name="type"></param>
    public static void InitExport(ClickableElement elem)
    {
        var foo = senderEditor;

        PopupType = typeOfPopup.FailedExport;

        exportingElem = elem;

        PopupWindow window = ScriptableObject.CreateInstance<PopupWindow>();

        window.titleContent = new GUIContent("Export");
        window.position = new Rect(senderEditor.position.center.x - width / 2, senderEditor.position.center.y - height / 2, width, height);

        window.ShowModalUtility();
    }

    /// <summary>
    /// Initializer for the <see cref="PopupWindow"/> when we want to show a warning to the user
    /// </summary>
    public static void InitWarningPopup(string text)
    {
        var foo = senderEditor;

        warningText = text;

        PopupType = typeOfPopup.Warning;

        PopupWindow window = ScriptableObject.CreateInstance<PopupWindow>();

        window.titleContent = new GUIContent("Warning");
        window.position = new Rect(senderEditor.position.center.x - width / 2, senderEditor.position.center.y - height / 2, width, height);

        window.ShowModalUtility();
    }

    /// <summary>
    /// Called once every frame
    /// </summary>
    void OnGUI()
    {
        switch (PopupType)
        {
            case typeOfPopup.Delete:
                ShowDeletePopup();
                break;
            case typeOfPopup.FailedExport:
                ShowExportPopup();
                break;
            case typeOfPopup.Warning:
                ShowWarningPopup();
                break;
        }

        if (Event.current.isKey && Event.current.type == EventType.KeyUp)
        {
            switch (Event.current.keyCode)
            {
                case KeyCode.Escape:
                    this.Close();
                    break;
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    foreach (GUIElement elem in elems)
                        senderEditor.Delete(elem);
                    this.Close();
                    break;
            }
        }
    }

    /// <summary>
    /// Shows the <see cref="PopupWindow"/> asking if you're sure you wanna delete the <see cref="elems"/>
    /// </summary>
    private void ShowDeletePopup()
    {
        string text = elems.Count == 1 ? "Do you want to delete this " + typeOfElem + "?" : "Do you want to delete these elements?";

        EditorGUILayout.LabelField(text, EditorStyles.boldLabel, GUILayout.Width(this.position.width - 10), GUILayout.ExpandHeight(true));
        if (senderEditor.currentElem is BehaviourTree)
        {
            int numberOfSons = 0;

            foreach (GUIElement elem in elems.Where(e => e is BehaviourNode))
            {
                numberOfSons += ((BehaviourTree)senderEditor.currentElem).ChildrenCount((BehaviourNode)elem, true);
            }

            if (numberOfSons > 0)
            {
                EditorGUILayout.LabelField(numberOfSons + " child node/s will also be deleted", Styles.WarningLabel, GUILayout.Width(this.position.width - 10), GUILayout.ExpandHeight(true));
                GUILayout.Space(20);
            }
        }
        else
        {
            GUILayout.Space(30);
        }

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Delete"))
        {
            foreach (GUIElement elem in elems)
                senderEditor.Delete(elem);
            this.Close();
        }
        GUI.backgroundColor = Color.grey;
        if (GUILayout.Button("Cancel"))
        {
            this.Close();
        }
    }

    /// <summary>
    /// Shows the <see cref="PopupWindow"/> telling you that you should fix all errors before exporting the <see cref="ClickableElement"/>
    /// </summary>
    private void ShowExportPopup()
    {
        EditorGUILayout.LabelField("Fix all the errors before exporting code", Styles.WarningLabel, GUILayout.Width(this.position.width - 10), GUILayout.ExpandHeight(true));

        GUILayout.Space(30);

        //GUI.backgroundColor = Color.grey;
        if (GUILayout.Button("Ok"))
        {
            this.Close();
        }

        GUI.backgroundColor = Color.grey;
        if (GUILayout.Button("Export anyway"))
        {
            this.Close();
            NodeEditorUtilities.GenerateElemScript(exportingElem);
        }
    }

    /// <summary>
    /// Shows the <see cref="PopupWindow"/> warning you about something
    /// </summary>
    private void ShowWarningPopup()
    {
        EditorGUILayout.LabelField(warningText, EditorStyles.boldLabel, GUILayout.Width(this.position.width - 10), GUILayout.ExpandHeight(true));

        GUILayout.Space(30);

        if (GUILayout.Button("Ok"))
        {
            this.Close();
        }
    }

    private void OnLostFocus()
    {
        this.Close();
    }
}