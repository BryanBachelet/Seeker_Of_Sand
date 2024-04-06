using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GuerhoubaGames.AI
{
    [CreateAssetMenu()]
    public class BehaviorTree : ScriptableObject
    {
        public Node rootNode;
        public Node.State treeState = Node.State.RUNNING;
        public List<Node> nodes = new List<Node>();
        public Blackboard blackboard = new Blackboard();


        public Node.State Update()
        {
            if (rootNode.state == Node.State.RUNNING)
            {
                treeState = rootNode.Evaluate();
            }
            return treeState;
        }
#if UNITY_EDITOR
        public Node CreateNode(System.Type type)
        {
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
            nodes.Add(node);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }
        
            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");
            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(Node node)
        {
            Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
            nodes.Remove(node);
            
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(Node parent, Node child)
        {
            RootNode rootNode = parent as RootNode;
            if (rootNode)
            {
                Undo.RecordObject(rootNode, "Behaviour Tree (AddChild)");
                rootNode.child = child;
                EditorUtility.SetDirty(rootNode);
            }

            DecoratorNode decoratorNode = parent as DecoratorNode;
            if (decoratorNode)
            {
                Undo.RecordObject(decoratorNode, "Behaviour Tree (AddChild)");
                decoratorNode.child = child;
                EditorUtility.SetDirty(decoratorNode);
            }

            CompositeNode composite = parent as CompositeNode;
            if (composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (AddChild)");
                composite.children.Add(child);
                EditorUtility.SetDirty(composite);
            }
        }

        public void RemoveChild(Node parent, Node child)
        {

            RootNode rootNode = parent as RootNode;
            if (rootNode)
            {
                Undo.RecordObject(rootNode, "Behaviour Tree (RemoveChild)");
                rootNode.child = null;
                EditorUtility.SetDirty(rootNode);
            }

            DecoratorNode decoratorNode = parent as DecoratorNode;
            if (decoratorNode)
            {
                Undo.RecordObject(decoratorNode, "Behaviour Tree (RemoveChild)");
                decoratorNode.child = null;
                EditorUtility.SetDirty(decoratorNode);
            }

            CompositeNode composite = parent as CompositeNode;
            if (composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (RemoveChild)");
                composite.children.Remove(child);
                EditorUtility.SetDirty(composite);
            }
        }

        public List<Node> GetChildren(Node parent)
        {
            List<Node> children = new List<Node>();
            DecoratorNode decoratorNode = parent as DecoratorNode;
            if (decoratorNode && decoratorNode.child != null)
            {
                children.Add(decoratorNode.child);
            }

            RootNode rootNode = parent as RootNode;
            if (rootNode && rootNode.child != null)
            {
                children.Add(rootNode.child);
            }

            CompositeNode composite = parent as CompositeNode;
            if (composite)
            {
                return composite.children;
            }
            return children;
        }

        public void  Traverse(Node node, System.Action<Node> visiter)
        {
            if(node)
            {
                visiter.Invoke(node);
                var children = GetChildren(node);
                children.ForEach((n) => Traverse(n, visiter));
            }
        }
        public BehaviorTree Clone()
        {
            BehaviorTree Tree = Instantiate(this);
            Tree.rootNode = Tree.rootNode.Clone();
            Tree.nodes = new List<Node>();
            Traverse(Tree.rootNode, (n) =>
            {
                Tree.nodes.Add(n);
            });
            return Tree;
        }

        public void Bind(NPCAgent agent)
        {
            Traverse(rootNode, node => {
                node.agent = agent;
                node.blackboard = blackboard;
            });

        }
#endif
    }
}
