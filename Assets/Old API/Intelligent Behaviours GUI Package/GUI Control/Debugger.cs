using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    public static bool isDebug
    {
        get
        {
            return FindObjectOfType<Debugger>();
        }
    }
}
