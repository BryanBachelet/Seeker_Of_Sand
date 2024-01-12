using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GloomyGlow.Common
{
    public static class GloomyGlowSkin
    {
        public delegate void OnCollapsibleVisible();

        private static string _companyName = "GloomyGlow";

        // Common config
        private static Color _backgroundColor = Color.black;
        
        private static float _logoSize = 64.0f;

        public static void DrawHeader(Rect rect, string name)
        {
            Texture2D logo = AssetDatabase.LoadAssetAtPath("Assets/ShadowDetect/Textures/mini_logo_official.png", typeof(Texture2D)) as Texture2D;
            GUISkin skin = AssetDatabase.LoadAssetAtPath("Assets/ShadowDetect/Skins/Editor.guiskin", typeof(GUISkin)) as GUISkin;
            GUIStyle headerStyle_project = new GUIStyle(skin.customStyles[0]);
            GUIStyle headerStyle_title = new GUIStyle(skin.customStyles[1]);

            
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 64), _backgroundColor);
            
            GUI.DrawTexture(new Rect(rect.x, rect.y, _logoSize, _logoSize), logo, ScaleMode.ScaleToFit);

            EditorGUI.LabelField(new Rect(rect.x + _logoSize, rect.y, rect.width - _logoSize, _logoSize), _companyName, headerStyle_project);
            EditorGUI.LabelField(new Rect(rect.x + _logoSize, rect.y, rect.width - _logoSize, _logoSize), name, headerStyle_title);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        public static void DrawCollapsible(Rect rect, string name, ref bool collapsibleContentVisible, OnCollapsibleVisible collapsibleContent)
        {
            Texture2D hideCollapsibleTexture = AssetDatabase.LoadAssetAtPath("Assets/ShadowDetect/Textures/arrow-point-to-bottom.png", typeof(Texture2D)) as Texture2D;
            Texture2D showCollapsibleTexture = AssetDatabase.LoadAssetAtPath("Assets/ShadowDetect/Textures/arrow-point-to-right.png", typeof(Texture2D)) as Texture2D;
            GUISkin skin = AssetDatabase.LoadAssetAtPath("Assets/ShadowDetect/Skins/Editor.guiskin", typeof(GUISkin)) as GUISkin;
            GUIStyle collapseStyle = new GUIStyle(skin.customStyles[2]);

            EditorGUI.DrawRect(new Rect(rect.x, GUILayoutUtility.GetRect(rect.width, 2).y, rect.width, 24), _backgroundColor);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(name.ToUpper(), collapseStyle);

            collapseStyle.alignment = TextAnchor.MiddleRight;
            collapsibleContentVisible = EditorGUILayout.Toggle(collapsibleContentVisible, collapseStyle, GUILayout.Width(24), GUILayout.Height(24));
            EditorGUILayout.EndHorizontal();

            GUI.DrawTexture(new Rect(rect.width - 14, GUILayoutUtility.GetRect(rect.width, -2).y - 26, 16, 16), ((collapsibleContentVisible) ? hideCollapsibleTexture : showCollapsibleTexture));

            if (collapsibleContentVisible)
                collapsibleContent();
        }


    }
}