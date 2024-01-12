using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GloomyGlow.Common;

namespace ShadowDetect
{
    [CustomEditor(typeof(ShadowDetect))]
    public class ShadowDetectEditor : Editor
    {
        SerializedProperty shadowtargets, layers, lights, onChangeState, onEnterShadow, onExitShadow, onShadow, outShadow;
        ShadowDetect sd;

        bool ShowHideGeneral = true;
        bool ShowHideTargets = true;
        bool ShowHideCommons = true;

        void OnEnable()
        {
            sd = (ShadowDetect)target;
            shadowtargets = serializedObject.FindProperty("_targets");
            layers = serializedObject.FindProperty("_layers");
            lights = serializedObject.FindProperty("_lights");
            onChangeState = serializedObject.FindProperty("_onChangeState");
            onEnterShadow = serializedObject.FindProperty("_onEnterShadow");
            onExitShadow = serializedObject.FindProperty("_onExitShadow");
            onShadow = serializedObject.FindProperty("_onShadow");
            outShadow = serializedObject.FindProperty("_outShadow");
        }

        public override void OnInspectorGUI()
        {
            //Undo.RecordObject(sd, "Undo ShadowDetect settings");
            //serializedObject.Update();

            Rect bgRect = EditorGUILayout.GetControlRect();
            bgRect = new Rect(bgRect.x - 10, bgRect.y, bgRect.width + 10, bgRect.height);

            //=======================================================================================================
            //-----------------------------------------------//HEADER\\----------------------------------------------
            GloomyGlowSkin.DrawHeader(bgRect, "Shadow Detect");

            //=======================================================================================================
            //---------------------------------------------//OPTIONS GENERAL\\-------------------------------------------
            GloomyGlowSkin.OnCollapsibleVisible generalCollapsibleVisible = delegate ()
            {
                sd._detectMode = (DETECT_MODE)EditorGUILayout.EnumPopup("Shadow Detect Mode:", sd._detectMode);
                sd._raycastinRate = EditorGUILayout.Slider(new GUIContent("Raycasting Rate:", "Rate to test if your targets are in or out a shadow"), sd._raycastinRate, 10.0f, 60.0f);
                EditorGUILayout.PropertyField(layers, new GUIContent("Layers:", "Layer that is used to selectively ignore Colliders when casting a ray"), true);
                GUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("Automatic lights research?", "Defined if you want to research all lights of the scene or if you set them"));
                GUILayout.FlexibleSpace();
                sd.IsAuto = EditorGUILayout.Toggle(sd.IsAuto);
                GUILayout.EndHorizontal();
               
                if (!sd.IsAuto)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(lights, new GUIContent("Lights"), true);
                    EditorGUI.indentLevel--;
                }
                    
            };
            GloomyGlowSkin.DrawCollapsible(bgRect, "PARAMETERS", ref ShowHideGeneral, generalCollapsibleVisible);

            //=======================================================================================================
            //---------------------------------------------//OPTIONS GENERAL\\-------------------------------------------
            GloomyGlowSkin.OnCollapsibleVisible targetsCollapsibleVisible = delegate ()
            {
                EditorGUILayout.PropertyField(shadowtargets, new GUIContent("Targets", "Transforms of GameObjects to detect on shadow"), true);
            };
            GloomyGlowSkin.DrawCollapsible(bgRect, "TARGETS", ref ShowHideTargets, targetsCollapsibleVisible);

            //=======================================================================================================
            //---------------------------------------------//OPTIONS COMMON\\-------------------------------------------
            GloomyGlowSkin.OnCollapsibleVisible commonCollapsibleVisible = delegate ()
            {
                EditorGUILayout.PropertyField(onChangeState, new GUIContent("On Change State", "Event call on change of state, enter or exit shadow"), true);
                EditorGUILayout.PropertyField(onEnterShadow, new GUIContent("On Enter Shadow", "Event call if the GameObject enter in a shadow"), true);
                EditorGUILayout.PropertyField(onExitShadow, new GUIContent("On Exit Shadow", "Event call if the GameObject exit a shadow"), true);
                EditorGUILayout.PropertyField(onShadow, new GUIContent("On Shadow", "Event call if the GameObject is on a shadow and stay in"), true);
                EditorGUILayout.PropertyField(outShadow, new GUIContent("Out Shadow", "Event call if the GameObject is out of a shadow and stay out"), true);
            };
            GloomyGlowSkin.DrawCollapsible(bgRect, "COMMON EVENTS", ref ShowHideCommons, commonCollapsibleVisible);

            //=======================================================================================================
            //----------------------------------------------//UPDATE\\-----------------------------------------------
            serializedObject.ApplyModifiedProperties();
        }
    }
}