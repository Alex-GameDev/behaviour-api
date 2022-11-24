using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

namespace BehaviourAPI.Core
{
    /// <summary>
    /// Represents a link between two nodes.
    /// </summary>
    public abstract class Connection : GraphElement
    {
        /// <summary>
        /// The Node where this connection starts
        /// </summary>
        public Node? SourceNode;

        /// <summary>
        /// The node where this connection ends
        /// </summary>
        public Node? TargetNode;

        public override void SerializeToJSON(Utf8JsonWriter writer)
        {
            base.SerializeToJSON(writer);
            if(BehaviourGraph != null)
            {
                if(SourceNode != null)
                {
                    writer.WriteStartObject("src");
                    writer.WriteNumber("id", BehaviourGraph.Nodes.IndexOf(SourceNode));
                    writer.WriteNumber("index", SourceNode.OutputConnections.IndexOf(this));
                    writer.WriteEndObject();
                }
                if(TargetNode != null)
                {
                    writer.WriteStartObject("target");
                    writer.WriteNumber("id", BehaviourGraph.Nodes.IndexOf(TargetNode));
                    writer.WriteNumber("index", TargetNode.InputConnections.IndexOf(this));
                    writer.WriteEndObject();
                }              
            }            
        }
    }
}