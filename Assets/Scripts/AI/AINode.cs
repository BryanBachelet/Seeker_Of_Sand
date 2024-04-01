using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames.AI
{
    /// <summary>
    /// This class is use in the behavior tree system as a base class for condition to validate
    /// </summary>
    public class Decorator
    {
        public AINode parent;
        public bool isSkipable = false;

        public virtual bool Evaluate()
        {
            return true;
        }

    }

    /// <summary>
    /// AINode is the base class for most of the behavior tree elements
    /// </summary>
    public class AINode : ScriptableObject
    {
        public BehaviorTree tree;
        public AINode parent;

        private List<AINode> m_children;
        public List<AINode> children
        {
            get { return m_children; }
        }

        private List<Decorator> m_decorator;
        public List<Decorator> decoratorsList
        {
            get { return m_decorator; }
        }

        public virtual void Init(BehaviorTree aiTree)
        {
            tree = aiTree;
        }

        public virtual void Evaluate()
        {
        }

        /// <summary>
        /// This function is testing all decorators conditions present on the node
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValid()
        {
            foreach (Decorator decorator in decoratorsList)
            {
                if (!decorator.Evaluate()) return false;
            }
            return true;
        }

        #region Node Management Functions
        public void AddNode(AINode node)
        {
            m_children.Add(node);
            node.parent = this;
        }

        public void RemoveNode(AINode node)
        {
            node.parent = null;
            m_children.Remove(node);
        }

        public void ClearAllNode()
        {
            foreach (AINode node in m_children)
            {
                RemoveNode(node);
            }
        }
        #endregion

        #region Decorator Management Functions
        public void AddDecortor(Decorator decorator)
        {
            m_decorator.Add(decorator);
            decorator.parent = this;
        }

        public void RemoveDecorator(Decorator decorator)
        {
            decorator.parent = null;
            m_decorator.Remove(decorator);
        }

        public void ClearAllDecorator()
        {
            foreach (Decorator decorator in m_decorator)
            {
                RemoveDecorator(decorator);
            }
        }

        #endregion

    }

    /// <summary>
    /// The Task class is an intermediate class with basic functions for AI game tasks
    /// </summary>
    public class Task : AINode
    {
        public override void Evaluate()
        {
        }

        public virtual void StartTask()
        {

        }

        public virtual void EndTask()
        {

        }
    }

    /// <summary>
    /// Sequence class allow the AI to do multiple tasks in a row.
    /// </summary>
    [CreateAssetMenu(fileName = "Sequence", menuName = "BehaviorTree/Sequence", order = 2)]
    public class Sequence : AINode
    {
        public bool IsBlockingSequence = false;
        public override void Evaluate()
        {

            foreach (AINode child in children)
            {
                if (!child.IsValid()) break;
                child.Evaluate();
            }
        }
    }

    /// <summary>
    ///  Selector class allow the AI to choose between possible depending if they condition are valid or not
    /// </summary>
    [CreateAssetMenu(fileName = "Selector", menuName = "BehaviorTree/Selector", order = 3)]
    public class Selector : AINode
    {
        public override void Evaluate()
        {
            foreach (AINode child in children)
            {
                if (child.IsValid())
                {
                    child.Evaluate();
                    break;
                }
            }
        }

    }

    /// <summary>
    /// AITreeRoot is the first element of the behavior Tree
    /// </summary>
    [CreateAssetMenu(fileName ="BehaviorTreeRoot",menuName ="BehaviorTree/Root",order = 1)]
    public class AITreeRoot : AINode
    {
        public AINode firstChildren;

        public void AddFirstChild(AINode node)
        {
            firstChildren = node;
            node.parent = this;
        }

        public void RemoveFirstChild()
        {
            firstChildren.parent = null;
            firstChildren = null;
        }

        public override void Evaluate()
        {
            if (!firstChildren.IsValid()) return;
            firstChildren.Evaluate();
        }
    }

}
