using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(CapsuleProfil))]
public class CapsuleProfil_UI : Editor
{

    SerializedProperty lifetimeProperty;
    SerializedProperty travelTimeProperty;
    SerializedProperty useTravelTimeProperty;
    SerializedProperty speedProperty;
    SerializedProperty rangeProperty;
    SerializedProperty damageProperty;
    SerializedProperty projectileNumberProperty;
    SerializedProperty totalShotTimeProperty;
    SerializedProperty shootAngleProperty;
    SerializedProperty trajectoryProperty;
    SerializedProperty angleTrajectoryProperty;
    SerializedProperty shootNumberProperty;
    SerializedProperty TimeIntervalProperty;
    SerializedProperty sizeProperty;
    SerializedProperty sizeMultiplicatorFactorProperty;

    public void OnEnable()
    {
        rangeProperty = serializedObject.FindProperty("stats.range");
        lifetimeProperty = serializedObject.FindProperty("stats.lifetime");
        travelTimeProperty = serializedObject.FindProperty("stats.travelTime");
        useTravelTimeProperty = serializedObject.FindProperty("stats.useTravelTime");
        speedProperty = serializedObject.FindProperty("stats.speed");
        damageProperty = serializedObject.FindProperty("stats.damage");
        projectileNumberProperty = serializedObject.FindProperty("stats.projectileNumber");
        totalShotTimeProperty = serializedObject.FindProperty("stats.totalShotTime");
        shootAngleProperty = serializedObject.FindProperty("stats.shootAngle");
        trajectoryProperty = serializedObject.FindProperty("stats.trajectory");
        angleTrajectoryProperty = serializedObject.FindProperty("stats.angleTrajectory");
        shootNumberProperty = serializedObject.FindProperty("stats.shootNumber");
        TimeIntervalProperty = serializedObject.FindProperty("stats.timeInterval");
        sizeProperty = serializedObject.FindProperty("stats.size");
        sizeMultiplicatorFactorProperty = serializedObject.FindProperty("stats.sizeMultiplicatorFactor");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        CapsuleProfil m_capsuleProfile = (CapsuleProfil)target;
        lifetimeProperty.floatValue = EditorGUILayout.FloatField(lifetimeProperty.displayName, lifetimeProperty.floatValue);
       // useTravelTimeProperty.boolValue = EditorGUILayout.Toggle(useTravelTimeProperty.displayName, useTravelTimeProperty.boolValue);
        EditorGUILayout.PropertyField(useTravelTimeProperty);
        if (m_capsuleProfile.stats.useTravelTime)
        {
            travelTimeProperty.floatValue = EditorGUILayout.FloatField(travelTimeProperty.displayName, travelTimeProperty.floatValue);
        }
        speedProperty.floatValue = EditorGUILayout.FloatField(speedProperty.displayName, speedProperty.floatValue);
        rangeProperty.floatValue = EditorGUILayout.FloatField(rangeProperty.displayName, rangeProperty.floatValue);
        damageProperty.floatValue = EditorGUILayout.FloatField(damageProperty.displayName, damageProperty.floatValue);
        projectileNumberProperty.floatValue = EditorGUILayout.FloatField(projectileNumberProperty.displayName, projectileNumberProperty.floatValue);
        totalShotTimeProperty.floatValue = EditorGUILayout.FloatField(totalShotTimeProperty.displayName, totalShotTimeProperty.floatValue);
        shootAngleProperty.floatValue = EditorGUILayout.FloatField(shootAngleProperty.displayName, shootAngleProperty.floatValue);
        EditorGUILayout.PropertyField(trajectoryProperty);
        if (m_capsuleProfile.stats.trajectory == TrajectoryType.CURVE)
        {
            angleTrajectoryProperty.floatValue = EditorGUILayout.FloatField(angleTrajectoryProperty.displayName, angleTrajectoryProperty.floatValue);
        }
        shootNumberProperty.floatValue = EditorGUILayout.FloatField(shootNumberProperty.displayName, shootNumberProperty.floatValue);
        TimeIntervalProperty.floatValue = EditorGUILayout.FloatField(TimeIntervalProperty.displayName, TimeIntervalProperty.floatValue);
        sizeProperty.floatValue = EditorGUILayout.FloatField(sizeProperty.displayName, sizeProperty.floatValue);
        sizeMultiplicatorFactorProperty.floatValue = EditorGUILayout.FloatField(sizeMultiplicatorFactorProperty.displayName, sizeMultiplicatorFactorProperty.floatValue);

        serializedObject.ApplyModifiedProperties();
    }
}
