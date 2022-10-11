using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourAPI.Editor
{
    using Runtime.Core;

    /// <summary>
    /// Custom <see cref="Edge"/> type that represents a graph <see cref="Runtime.Core.Connection"/>
    /// </summary>
    public class ConnectionView : Edge
    {
        public Connection connection;
        ElementInspector m_inspector;

        public ConnectionView() : base()
        {
        }

        public override void OnSelected()
        {
            base.OnSelected();
            m_inspector?.UpdateInspector(connection);
        }

        public void SetInspector(ElementInspector inspector)
        {
            m_inspector = inspector;
        }
    }
}