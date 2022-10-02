using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourAPI.Editor
{
    public class PortView : Port
    {
        public PortView(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
            this.AddManipulator(new EdgeConnector<ConnectionView>(new DefaultEdgeConnectorListener()));
        }
    }

    public class DefaultEdgeConnectorListener : IEdgeConnectorListener
    {

        public void OnDrop(GraphView graphView, Edge edge)
        {
            List<GraphElement> edgesToDelete = new List<GraphElement>();
            List<Edge> edgesToCreate = new List<Edge>() { edge };

            BehaviourGraphView behaviourGraphView = graphView as BehaviourGraphView;

            GraphViewChange change = new GraphViewChange();
            change.edgesToCreate = edgesToCreate;

            if (edge.input.capacity == Port.Capacity.Single)
            {
                edgesToDelete.AddRange(GetEdgesToRemove(edge.input, edge));
            }
            if (edge.output.capacity == Port.Capacity.Single)
            {
                edgesToDelete.AddRange(GetEdgesToRemove(edge.output, edge));
            }

            if (edgesToDelete.Count > 0) graphView.DeleteElements(edgesToDelete);

            if (graphView.graphViewChanged != null)
            {
                edgesToCreate = graphView.graphViewChanged(change).edgesToCreate;
            }

            foreach (Edge e in edgesToCreate)
            {
                graphView.AddElement(e);
                edge.input.Connect(e);
                edge.output.Connect(e);
            }
        }

        private List<GraphElement> GetEdgesToRemove(Port port, Edge createdEdge)
        {
            List<GraphElement> itemsToRemove = new List<GraphElement>();
            foreach (var portEdge in port.connections)
            {
                if (portEdge != createdEdge) itemsToRemove.Add(portEdge);
            }
            return itemsToRemove;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {

        }
    }

}