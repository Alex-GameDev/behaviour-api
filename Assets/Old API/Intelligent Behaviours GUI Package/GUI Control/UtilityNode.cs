using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

public enum utilityType
{
    Variable,
    Fusion,
    Action,
    Curve
}

public enum fusionType
{
    Weighted,
    GetMax,
    GetMin
}

public enum curveType
{
    Linear,
    Exponential,
    LinearParts
}

public class UtilityNode : BaseNode
{
    /// <summary>
    /// The type of <see cref="UtilityNode"/>
    /// </summary>
    public utilityType type;

    /// <summary>
    /// The type of fusion if this <see cref="UtilityNode"/> is of type Fusion
    /// </summary>
    public fusionType fusionType;

    /// <summary>
    /// The type of curve if this <see cref="UtilityNode"/> is of type Curve
    /// </summary>
    public curveType curveType;

    private NodeEditor privateEditor;

    /// <summary>
    /// Reference to the editor
    /// </summary>
    public NodeEditor editor
    {
        get
        {
            if (!privateEditor)
                privateEditor = EditorWindow.GetWindow<NodeEditor>();
            return privateEditor;
        }
    }

    /// <summary>
    /// Base height that will change depending on the number of Points when it's a curve node
    /// </summary>
    public float baseHeight;

    /// <summary>
    /// Min value for the Variable nodes
    /// </summary>
    public float variableMin;

    /// <summary>
    /// Max value for the Variable nodes
    /// </summary>
    public float variableMax;

    /// <summary>
    /// Slope value for the Curve nodes
    /// </summary>
    public float slope = 1;

    /// <summary>
    /// Exponent value for the Curve nodes
    /// </summary>
    public float exp = 1;

    /// <summary>
    /// Displacement on X value for the Curve nodes
    /// </summary>
    public float displX;

    /// <summary>
    /// Displacement on Y value for the Curve nodes
    /// </summary>
    public float displY;

    /// <summary>
    /// List of points for the Linear Parts Curve Node
    /// </summary>
    public List<Vector2> points;

    /// <summary>
    /// Boolean for keeping in memory if the curve visualizer foldout is open
    /// </summary>
    public bool openFoldout;

    /// <summary>
    /// Boolean for keeping in memory if the Points area foldout is open
    /// </summary>
    public bool openPointsFoldout;

    /// <summary>
    /// Value that determinates the max coordinate in the 4 directions equally
    /// </summary>
    private float regularSize = 10;

    /// <summary>
    /// Returns the <see cref="utilityType"/> properly written
    /// </summary>
    /// <returns></returns>
    public override string GetTypeString()
    {
        if (subElem is null)
            return type.ToString();
        else
            return subElem.GetTypeString();
    }

    /// <summary>
    /// The Initializer for the <seealso cref="UtilityNode"/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="typeNumber"></param>
    /// <param name="posx"></param>
    /// <param name="posy"></param>
    /// <param name="subElem"></param>
    public void InitUtilityNode(ClickableElement parent, utilityType type, float posx, float posy, ClickableElement subElem = null, string id = null)
    {
        InitBaseNode(parent, id);

        var foo = editor;

        this.type = type;

        if (subElem != null)
        {
            this.subElem = subElem;
            nodeName = this.subElem.elementName;
            windowRect = new Rect(posx, posy, ClickableElement.width, ClickableElement.height);
        }
        else
        {
            string nameToAdd = "New " + type;
            if (type != utilityType.Variable)
                nameToAdd += " Node ";
            nodeName = parent.elementNamer.AddName(identificator, nameToAdd);

            if (type == utilityType.Fusion)
            {
                windowRect = new Rect(posx, posy, width, height * 1.7f);
            }
            else if (type == utilityType.Curve)
            {
                baseHeight = height * 1.5f;
                windowRect = new Rect(posx, posy, width, baseHeight);
            }
            else
            {
                windowRect = new Rect(posx, posy, width, height);
            }
        }

        points = new List<Vector2>() { new Vector2(0, 0) };
    }

    /// <summary>
    /// The Initializer for the <seealso cref="UtilityNode"/> when it is being loaded from an XML
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="typeNumber"></param>
    /// <param name="posx"></param>
    /// <param name="posy"></param>
    /// <param name="subElem"></param>
    public void InitUtilityNodeFromXML(ClickableElement parent, utilityType type, fusionType fusionType, curveType curveType,
        float posx, float posy, string id, string name, float variableMax, float variableMin, float slope, float exp, float displX, float displY, List<Vector2> points, ClickableElement subElem = null)
    {
        InitBaseNode(parent, id);

        var foo = editor;

        this.type = type;
        this.fusionType = fusionType;
        this.curveType = curveType;

        if (subElem != null)
        {
            this.subElem = subElem;
            nodeName = this.subElem.elementName;
            windowRect = new Rect(posx, posy, ClickableElement.width, ClickableElement.height);
        }
        else
        {
            nodeName = parent.elementNamer.AddName(id, name);

            if (type == utilityType.Fusion)
            {
                windowRect = new Rect(posx, posy, width, height * 1.7f);
            }
            else if (type == utilityType.Curve)
            {
                baseHeight = height * 1.5f;
                windowRect = new Rect(posx, posy, width, baseHeight);
            }
            else
            {
                windowRect = new Rect(posx, posy, width, height);
            }
        }

        this.variableMax = variableMax;
        this.variableMin = variableMin;
        this.slope = slope;
        this.exp = exp;
        this.displX = displX;
        this.displY = displY;
        this.points = points;
    }

    /// <summary>
    /// Draws all the elements inside the <see cref="UtilityNode"/>
    /// </summary>
    public override void DrawWindow()
    {
        switch (type)
        {
            case utilityType.Variable:
                nodeName = CleanName(EditorGUILayout.TextArea(nodeName, Styles.TitleText, GUILayout.ExpandWidth(true), GUILayout.Height(25)));
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Min:", Styles.NonEditable, GUILayout.Width(40), GUILayout.Height(25));
                float.TryParse(EditorGUILayout.TextArea(variableMin.ToString(), Styles.TitleText, GUILayout.ExpandWidth(false), GUILayout.Height(20)), out variableMin);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Max:", Styles.NonEditable, GUILayout.Width(40), GUILayout.Height(25));
                float.TryParse(EditorGUILayout.TextArea(variableMax.ToString(), Styles.TitleText, GUILayout.ExpandWidth(false), GUILayout.Height(20)), out variableMax);
                GUILayout.EndHorizontal();

                GUILayout.EndHorizontal();
                break;
            case utilityType.Fusion:
                nodeName = CleanName(EditorGUILayout.TextArea(nodeName, Styles.TitleText, GUILayout.ExpandWidth(true), GUILayout.Height(25)));
                GUILayout.BeginHorizontal();
                GUILayout.Space(windowRect.width * 0.2f);
                GUILayout.BeginVertical(GUILayout.Width(20));
                if (GUILayout.Toggle(fusionType == fusionType.Weighted, fusionType.Weighted.ToString(), EditorStyles.radioButton))
                    fusionType = fusionType.Weighted;
                if (GUILayout.Toggle(fusionType == fusionType.GetMax, fusionType.GetMax.ToString(), EditorStyles.radioButton))
                    fusionType = fusionType.GetMax;
                if (GUILayout.Toggle(fusionType == fusionType.GetMin, fusionType.GetMin.ToString(), EditorStyles.radioButton))
                    fusionType = fusionType.GetMin;
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                break;
            case utilityType.Action:
                nodeName = CleanName(EditorGUILayout.TextArea(nodeName, Styles.TitleText, GUILayout.ExpandWidth(true), GUILayout.Height(25)));
                break;
            case utilityType.Curve:
                nodeName = CleanName(EditorGUILayout.TextArea(nodeName, Styles.TitleText, GUILayout.ExpandWidth(true), GUILayout.Height(25)));

                // Type of curve selector
                GUILayout.BeginHorizontal();
                GUILayout.Space(windowRect.width * 0.2f);
                if (GUILayout.Button(curveType.ToString(), EditorStyles.toolbarDropDown))
                {
                    GenericMenu toolsMenu = new GenericMenu();

                    foreach (string name in Enum.GetNames(typeof(curveType)).ToArray())
                    {
                        toolsMenu.AddItem(new GUIContent(name), false, () =>
                        {
                            curveType = (curveType)Enum.Parse(typeof(curveType), name);
                        });
                    }

                    toolsMenu.DropDown(new Rect(0, Event.current.mousePosition.y, 0, 0));
                    EditorGUIUtility.ExitGUI();
                }
                GUILayout.Space(windowRect.width * 0.2f);
                GUILayout.EndHorizontal();

                // Parameters
                switch (curveType)
                {
                    case curveType.Linear:
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(windowRect.width * 0.2f);
                        GUILayout.BeginHorizontal();

                        // Linear curve formula
                        GUILayout.Label("y = ", Styles.NonEditable, GUILayout.Width(20), GUILayout.Height(25));
                        float.TryParse(EditorGUILayout.TextArea(slope.ToString(), Styles.TitleText, GUILayout.ExpandWidth(false), GUILayout.Height(20)), out slope);
                        GUILayout.Space(2);
                        GUILayout.Label("x + ", Styles.NonEditable, GUILayout.Width(20), GUILayout.Height(25));
                        float.TryParse(EditorGUILayout.TextArea(displY.ToString(), Styles.TitleText, GUILayout.ExpandWidth(false), GUILayout.Height(20)), out displY);

                        GUILayout.EndHorizontal();
                        GUILayout.Space(windowRect.width * 0.2f);
                        GUILayout.EndHorizontal();

                        // We reset the baseHeight in case the curveType was linearParts and then it came here
                        baseHeight = height * 1.5f;
                        break;
                    case curveType.Exponential:
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(windowRect.width * 0.2f);
                        GUILayout.BeginHorizontal();

                        // Exponential curve formula
                        GUILayout.Label("y = (x - ", Styles.NonEditable, GUILayout.Width(20), GUILayout.Height(25));
                        GUILayout.Space(15);
                        float.TryParse(EditorGUILayout.TextArea(displX.ToString(), Styles.TitleText, GUILayout.ExpandWidth(false), GUILayout.Height(20)), out displX);
                        GUILayout.Label(")", Styles.NonEditable, GUILayout.Width(10), GUILayout.Height(25));
                        float.TryParse(EditorGUILayout.TextArea(exp.ToString(), Styles.Exponent, GUILayout.ExpandWidth(false), GUILayout.Height(20)), out exp);
                        GUILayout.Space(2);
                        GUILayout.Label(" + ", Styles.NonEditable, GUILayout.Width(20), GUILayout.Height(25));
                        float.TryParse(EditorGUILayout.TextArea(displY.ToString(), Styles.TitleText, GUILayout.ExpandWidth(false), GUILayout.Height(20)), out displY);

                        GUILayout.EndHorizontal();
                        GUILayout.Space(windowRect.width * 0.2f);
                        GUILayout.EndHorizontal();

                        // We reset the baseHeight in case the curveType was linearParts and then it came here
                        baseHeight = height * 1.5f;
                        break;
                    case curveType.LinearParts:
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(windowRect.width * 0.2f);
                        GUILayout.BeginVertical();

                        openPointsFoldout = EditorGUILayout.Foldout(openPointsFoldout, "Points");

                        if (openPointsFoldout)
                        {
                            List<Vector2> auxList = new List<Vector2>();

                            // 2D points in the current list
                            foreach (Vector2 point in points)
                            {
                                GUILayout.BeginHorizontal();

                                // The point
                                GUILayout.Label("• (", Styles.NonEditable, GUILayout.Width(20), GUILayout.Height(25));
                                float.TryParse(EditorGUILayout.TextArea(point.x.ToString(), Styles.TitleText, GUILayout.ExpandWidth(false), GUILayout.Height(20)), out float varX);
                                GUILayout.Label(", ", Styles.NonEditable, GUILayout.Width(10), GUILayout.Height(25));
                                float.TryParse(EditorGUILayout.TextArea(point.y.ToString(), Styles.TitleText, GUILayout.ExpandWidth(false), GUILayout.Height(20)), out float varY);
                                GUILayout.Label(")", Styles.NonEditable, GUILayout.Width(10), GUILayout.Height(25));

                                // The X button for deleting the point
                                // If you don't press it it adds the element to the updated list, else it doesn't hence it will appear as it has been deleted
                                if (!GUILayout.Button("-", new GUIStyle(GUI.skin.button)
                                {
                                    padding = new RectOffset(2, 0, 0, 2),
                                    alignment = TextAnchor.MiddleCenter,
                                    fontSize = 15
                                },
                                GUILayout.Width(20), GUILayout.Height(20)))
                                {
                                    auxList.Add(new Vector2(varX, varY));
                                }

                                GUILayout.EndHorizontal();
                            }

                            points = auxList;

                            // The + button for adding a point
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("+", new GUIStyle(GUI.skin.button)
                            {
                                padding = new RectOffset(0, 0, 0, 3),
                                alignment = TextAnchor.MiddleCenter,
                                fontSize = 15
                            },
                            GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                points.Add(new Vector2(0, 0));
                            }
                            GUILayout.EndHorizontal();

                            baseHeight = height * 1.5f + (points.Count + 1) * 25;
                        }
                        else
                        {
                            baseHeight = height * 1.5f;
                        }

                        GUILayout.EndVertical();
                        GUILayout.Space(windowRect.width * 0.2f);
                        GUILayout.EndHorizontal();
                        break;
                }

                // Create the curve visualization foldout arrow
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical();
                try
                {
                    openFoldout = EditorGUILayout.Foldout(openFoldout, "Visualize curve");

                    if (openFoldout)
                    {
                        windowRect.height = baseHeight + 110;

                        Rect rect = new Rect(windowRect.width * 0.15f, windowRect.height - 110, windowRect.width * 0.7f, windowRect.width * 0.7f);
                        EditorGUI.DrawRect(rect, new Color(0, 0, 1, 0.25f));

                        Event e = Event.current;
                        if (e.type == EventType.ScrollWheel)
                        {
                            regularSize += e.delta.y * 0.02f * regularSize;
                        }

                        float yMin = -regularSize;
                        float yMax = regularSize;
                        float xMin = -regularSize;
                        float xMax = regularSize;

                        float step = 1 / editor.position.width;

                        Handles.color = new Color(0.6f, 0.6f, 0.6f);
                        DrawAxis(rect);

                        Handles.color = Color.white;

                        Vector3 prevPos = new Vector3(0, CurveFunc(xMin), 0);
                        for (float t = xMin + step; t < xMax; t += step * regularSize * 5)
                        {
                            Vector3 pos = new Vector3((t + xMax) / (xMax - xMin), CurveFunc(t), 0);

                            if (pos.y < yMax && pos.y > yMin)
                            {
                                Handles.DrawLine(
                                    new Vector3(rect.xMin + prevPos.x * rect.width, rect.yMax - ((prevPos.y - yMin) / (yMax - yMin)) * rect.height, 0),
                                    new Vector3(rect.xMin + pos.x * rect.width, rect.yMax - ((pos.y - yMin) / (yMax - yMin)) * rect.height, 0));
                            }

                            prevPos = pos;
                        }
                    }
                    else
                    {
                        windowRect.height = baseHeight;
                    }
                }
                finally
                {
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }

                break;
        }
    }

    private float CurveFunc(float t)
    {
        switch (curveType)
        {
            case curveType.Linear:
                return slope * t + displY;
            case curveType.Exponential:
                return Mathf.Pow(t - displX, exp) + displY;
            case curveType.LinearParts:
                float returnValue = 0.0f;

                for (int i = 0; i < points.Count; i++)
                {
                    float xPoint = points[i].x;
                    if (i == 0 && t < xPoint) { returnValue = points[i].y; break; };
                    if ((i == points.Count - 1) && t > xPoint) { returnValue = points[i].y; break; };
                    if (t == xPoint) { returnValue = points[i].y; break; }

                    if (t > xPoint && t < points[i + 1].x)
                    {
                        returnValue = ((t - xPoint) / (points[i + 1].x - xPoint)) * (points[i + 1].y - points[i].y) + points[i].y;
                        break;
                    }
                }

                return returnValue;
            default:
                return 0;
        }
    }

    /// <summary>
    /// Draws the axis in the center of the rect
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="yMin"></param>
    /// <param name="yMax"></param>
    private void DrawAxis(Rect rect)
    {
        Handles.DrawLine(
            new Vector3(rect.xMin, rect.yMax - 0.5f * rect.height, 0),
            new Vector3(rect.xMin + rect.width, rect.yMax - 0.5f * rect.height, 0));
        Handles.DrawLine(
            new Vector3(rect.xMin + 0.5f * rect.width, rect.yMax, 0),
            new Vector3(rect.xMin + 0.5f * rect.width, rect.yMax - rect.height, 0));
    }

    /// <summary>
    /// Creates and returns an <see cref="XMLElement"/> that corresponds to this <see cref="UtilityNode"/>
    /// </summary>
    /// <param name="args"></param>
    /// <returns>The <see cref="XMLElement"/> corresponding to this <see cref="BehaviourNode"/></returns>
    public override XMLElement ToXMLElement(params object[] args)
    {
        XMLElement result;

        if (this.subElem)
        {
            result = this.subElem.ToXMLElement();
        }
        else
        {
            result = new XMLElement
            {
                name = CleanName(this.nodeName),
                elemType = this.GetType().ToString(),
                windowPosX = this.windowRect.x,
                windowPosY = this.windowRect.y,
                thirdType = this.fusionType.ToString(),
                fourthType = this.curveType.ToString(),
                variableMax = this.variableMax,
                variableMin = this.variableMin,
                slope = this.slope,
                exp = this.exp,
                displX = this.displX,
                displY = this.displY,
                points = this.points
            };
        }

        result.Id = this.identificator;
        result.secondType = this.type.ToString();

        return result;
    }

    /// <summary>
    /// Creates a copy of this <see cref="UtilityNode"/>
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public override GUIElement CopyElement(params object[] args)
    {
        UtilitySystem parent = (UtilitySystem)args[0];

        UtilityNode result = CreateInstance<UtilityNode>();
        result.identificator = this.identificator;
        result.nodeName = this.nodeName;
        result.parent = parent;
        result.windowRect = new Rect(this.windowRect);
        result.type = this.type;
        result.fusionType = this.fusionType;
        result.curveType = this.curveType;
        result.variableMax = this.variableMax;
        result.variableMin = this.variableMin;
        result.slope = this.slope;
        result.exp = this.exp;
        result.displX = this.displX;
        result.displY = this.displY;
        result.points = this.points;

        if (this.subElem)
        {
            result.subElem = (ClickableElement)this.subElem.CopyElement(parent);
        }

        return result;
    }

    /// <summary>
    /// Returns a list of all the weights associated with this Fusion node
    /// </summary>
    /// <returns></returns>
    public List<KeyValuePair<float, UtilityNode>> GetWeightsAndFactors()
    {
        List<KeyValuePair<float, UtilityNode>> weightsFactorPair = new List<KeyValuePair<float, UtilityNode>>();
        List<TransitionGUI> transitions = parent.transitions.Where(t => !t.isExit && t.toNode.Equals(this)).ToList();

        foreach (TransitionGUI t in transitions)
        {
            weightsFactorPair.Add(new KeyValuePair<float, UtilityNode>(t.weight, (UtilityNode)t.fromNode));
        }

        return weightsFactorPair;
    }
}
