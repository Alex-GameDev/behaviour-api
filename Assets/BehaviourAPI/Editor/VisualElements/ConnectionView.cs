using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourAPI.Editor
{
    using Runtime.Core;
    public class ConnectionView : Edge
    {
        public Connection Connection { get; set; }
        public ConnectionView() : base()
        {
        }


    }
}