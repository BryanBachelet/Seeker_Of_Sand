using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GuerhoubaGames.AI;

namespace GuerhoubaTools.Gameplay
{
    public class Test : EditorWindow
    {

        BehaviorTreeView treeView;
        InspectorView inspectorView;
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        [MenuItem("Window/UI Toolkit/Test")]
        public static void OpenWindow()
        {
            Test wnd = GetWindow<Test>();
            wnd.titleContent = new GUIContent("Test");
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

            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            BehaviorTree tree = Selection.activeObject as BehaviorTree;
            if (tree)
            {
                treeView.PopulateView(tree);
            }
        }
    }
}
