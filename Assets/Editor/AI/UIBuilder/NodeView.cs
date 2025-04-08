using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.AI;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

namespace GuerhoubaTools.Gameplay
{


    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Action<NodeView> OnNodeSelected;
        public GuerhoubaGames.AI.Node node;
        public Port input;
        public Port output;

        public NodeView(GuerhoubaGames.AI.Node node) : base("Assets/Editor/AI/UIBuilder/NodeView.uxml")
        {

            this.node = node;
            this.title = node.name;
            this.viewDataKey = node.guid;
            style.left = node.position.x;
            style.top = node.position.y;


            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();

            Label descriptionLabel = this.Q<Label>("description");
            descriptionLabel.bindingPath = "description";
            descriptionLabel.Bind(new SerializedObject(node));

        }

        private void SetupClasses()
        {
            if (node is ActionNode)
            {
                AddToClassList("action");
            }
            else if (node is CompositeNode)
            {
                AddToClassList("composite");
            }
            else if (node is DecoratorNode)
            {
                AddToClassList("decorator");
            }
            else if (node is RootNode)
            {
                AddToClassList("root");
            }
        }

        private void CreateInputPorts()
        {
            if (node is ActionNode)
            {
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            else if (node is CompositeNode)
            {
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            else if (node is DecoratorNode)
            {
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            else if (node is RootNode)
            {

            }

            if (input != null)
            {
                input.portName = "";
                input.style.flexDirection = FlexDirection.Column;
                inputContainer.Add(input);
            }

        }

        private void CreateOutputPorts()
        {
            if (node is ActionNode)
            {

            }
            else if (node is CompositeNode)
            {
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));

            }
            else if (node is DecoratorNode)
            {
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
            }
            else if (node is RootNode)
            {
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
            }

            if (output != null)
            {
                output.portName = "";
                output.style.flexDirection = FlexDirection.ColumnReverse;
                outputContainer.Add(output);
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(node, "Behaviour Tree (Set Position)");
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
            EditorUtility.SetDirty(node);

        }

        public override void OnSelected()
        {
            base.OnSelected();
            if (OnNodeSelected != null)
            {
                OnNodeSelected.Invoke(this);
            }
        }

        public void SortChildren()
        {
            CompositeNode composite = node as CompositeNode;
            if (composite)
            {
                composite.children.Sort(SortByHorizontalPosition);
            }
        }

        private int SortByHorizontalPosition(GuerhoubaGames.AI.Node left, GuerhoubaGames.AI.Node right)
        {
            return left.position.x < right.position.x ? -1 : 1;
        }

        public void UpdateState()
        {
            RemoveFromClassList("running");
            RemoveFromClassList("failure");
            RemoveFromClassList("success");
            if (Application.isPlaying)
            {

                switch (node.state)
                {
                    case GuerhoubaGames.AI.Node.State.RUNNING:
                        if (node.started)
                        {
                            AddToClassList("running");
                        }
                        break;
                    case GuerhoubaGames.AI.Node.State.FAILURE:
                        AddToClassList("failure");
                        break;
                    case GuerhoubaGames.AI.Node.State.SUCCESS:
                        AddToClassList("success");
                        break;
                    default:
                        break;
                }
            }
        }

    }
}