using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using GuerhoubaGames.AI;
using System;
using System.Linq;

namespace GuerhoubaTools.Gameplay
{
    public class BehaviorTreeView : GraphView
    {
        public Action<NodeView> OnNodeSelected;
        public new class UxmlFactory : UxmlFactory<BehaviorTreeView, GraphView.UxmlTraits> { }
        BehaviorTree tree;

        public BehaviorTreeView()
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/AI/BehaviorTreeEditor.uss");
            styleSheets.Add(styleSheet);


            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);


            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            Insert(0, new GridBackground());

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            PopulateView(tree);
            AssetDatabase.SaveAssets();
        }

        NodeView FindNodeView (GuerhoubaGames.AI.Node node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }

        internal void PopulateView(BehaviorTree tree)
        {
            this.tree = tree;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            if(tree.rootNode == null)
            {
                tree.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
                EditorUtility.SetDirty(tree);
                AssetDatabase.SaveAssets();
            }

            tree.nodes.ForEach(n => CreateNodeView(n));


            // Create edges
            tree.nodes.ForEach(n =>
            {
                var children = tree.GetChildren(n);
                children.ForEach(c =>
                {
                    NodeView parentView = FindNodeView(n);
                    NodeView childView = FindNodeView(c);

                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);

                });
                });
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    NodeView nodeView = elem as NodeView;
                    if (nodeView != null)
                    {
                        tree.DeleteNode(nodeView.node);
                    }

                    Edge edge = elem as Edge;
                    if (edge != null)
                    {
                        NodeView parentView = edge.output.node as NodeView;
                        NodeView childView = edge.input.node as NodeView;

                        tree.RemoveChild(parentView.node, childView.node);
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;

                    tree.AddChild(parentView.node, childView.node);

                });
            }

            if(graphViewChange.movedElements != null)
            {
                nodes.ForEach((n) =>
                {
                    NodeView view = n as NodeView;
                    view.SortChildren();
                });
            }

            return graphViewChange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // base.BuildContextualMenu(evt);
            {
                var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
                foreach (var type in types)
                {
                    Vector2 mousPos = evt.mousePosition;
                    mousPos= contentViewContainer.WorldToLocal(evt.mousePosition);
                    evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type, mousPos));
                }
            }

            {
                var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
                foreach (var type in types)
                {
                    Vector2 mousPos = evt.mousePosition;
                    evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type, mousPos));
                }
            }

            {
                var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
                foreach (var type in types)
                {
                    Vector2 mousPos = evt.mousePosition;
                    evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type, mousPos));
                }
            }
        }

        void CreateNode(System.Type type ,Vector2 pos)
        {
            GuerhoubaGames.AI.Node node = tree.CreateNode(type);
            node.position = pos;
            CreateNodeView(node);
        }


        void CreateNodeView(GuerhoubaGames.AI.Node node)
        {
            
            NodeView nodeView = new NodeView(node);
            Rect rectPos = new Rect();
            rectPos.position = node.position;
            nodeView.SetPosition(rectPos);
            nodeView.OnNodeSelected = OnNodeSelected;
            AddElement(nodeView);
        }

        public void UpdateNodeStates()
        {
            nodes.ForEach(n =>
            {
                NodeView view = n as NodeView;
                view.UpdateState();
            });
        }

    }
}