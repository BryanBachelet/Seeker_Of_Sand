using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GuerhoubaGames.AI;
using UnityEditor.Callbacks;
using System;

namespace GuerhoubaTools.Gameplay
{

    public class BehaviorTreeEditor : EditorWindow
    {

        BehaviorTreeView treeView;
        InspectorView inspectorView;
        IMGUIContainer blackboardView;

        SerializedObject treeObject;
        SerializedProperty blackboardProperty;

        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        [MenuItem("BehaviorTreeEditor/Editor ...")]
        public static void OpenWindow()
        {
            BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
            wnd.titleContent = new GUIContent("BehaviorTreeEditor");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is BehaviorTree)
            {
                OpenWindow();
                return true;
            }
            return false;
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            m_VisualTreeAsset.CloneTree(root);

            var styleSheets = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/AI/BehaviorTreeEditor.uss");
            root.styleSheets.Add(styleSheets);

            treeView = root.Q<BehaviorTreeView>();
            inspectorView = root.Q<InspectorView>();
            blackboardView = root.Q<IMGUIContainer>();
            blackboardView.onGUIHandler = () =>
            {
                if (treeObject != null)
                {
                    treeObject.Update();
                    EditorGUILayout.PropertyField(blackboardProperty);
                    treeObject.ApplyModifiedProperties();
                }
            };

            treeView.OnNodeSelected = OnNodeSelectionChanged;
            OnSelectionChange();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }
        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
        private void OnSelectionChange()
        {
            BehaviorTree tree = Selection.activeObject as BehaviorTree;
            if (!tree)
            {
                if (Selection.activeGameObject)
                {
                    BehaviorTreeComponent behaviorTreeComponent = Selection.activeGameObject.GetComponent<BehaviorTreeComponent>();
                    if (behaviorTreeComponent)
                    {
                        tree = behaviorTreeComponent.tree;
                    }

                }
            }

            if (Application.isPlaying)
            {
                if (tree != null  && treeView != null)
                {
                    treeView.PopulateView(tree);
                }
            }
            else
            {
                if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
                {
                    treeView.PopulateView(tree);
                }
            }

            if(tree != null)
            {
                treeObject = new SerializedObject(tree);
                blackboardProperty = treeObject.FindProperty("blackboard");
            }
        }

        void OnNodeSelectionChanged(NodeView node)
        {

            inspectorView.UpdateSelection(node);
        }

        private void OnInspectorUpdate()
        {
            treeView?.UpdateNodeStates();
        }
    }
}
