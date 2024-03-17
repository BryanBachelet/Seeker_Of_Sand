using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CapsuleProfil))]
[CanEditMultipleObjects()]
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
    SerializedProperty trajectoryTimerProperty;
    SerializedProperty shootNumberProperty;
    SerializedProperty TimeIntervalProperty;
    SerializedProperty sizeProperty;
    SerializedProperty sizeMultiplicatorFactorProperty;
    SerializedProperty piercingMaxProperty;
    SerializedProperty descriptionProperty;
    SerializedProperty formSpellProperty;
    SerializedProperty stackDurationProperty;
    SerializedProperty stackPerGainProperty;


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
        piercingMaxProperty = serializedObject.FindProperty("stats.piercingMax");
        descriptionProperty = serializedObject.FindProperty("stats.description");
        trajectoryTimerProperty = serializedObject.FindProperty("stats.trajectoryTimer");
        formSpellProperty = serializedObject.FindProperty("stats.formType");
        stackDurationProperty = serializedObject.FindProperty("stats.stackDuration");
        stackPerGainProperty = serializedObject.FindProperty("stats.stackPerGain");

        

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
    
        CapsuleProfil m_capsuleProfile = (CapsuleProfil)target;
        m_capsuleProfile.stats.lifetime = EditorGUILayout.FloatField(lifetimeProperty.displayName, lifetimeProperty.floatValue);
       // useTravelTimeProperty.boolValue = EditorGUILayout.Toggle(useTravelTimeProperty.displayName, useTravelTimeProperty.boolValue);
        EditorGUILayout.PropertyField(useTravelTimeProperty);
        if (m_capsuleProfile.stats.useTravelTime)
        {
            travelTimeProperty.floatValue = EditorGUILayout.FloatField(travelTimeProperty.displayName, travelTimeProperty.floatValue);
        }
        speedProperty.floatValue = EditorGUILayout.FloatField(speedProperty.displayName, speedProperty.floatValue);
        rangeProperty.floatValue = EditorGUILayout.FloatField(rangeProperty.displayName, rangeProperty.floatValue);
        damageProperty.floatValue = EditorGUILayout.FloatField(damageProperty.displayName, damageProperty.floatValue);
        projectileNumberProperty.intValue = EditorGUILayout.IntField(projectileNumberProperty.displayName, projectileNumberProperty.intValue);
        totalShotTimeProperty.floatValue = EditorGUILayout.FloatField(totalShotTimeProperty.displayName, totalShotTimeProperty.floatValue);
        shootAngleProperty.floatValue = EditorGUILayout.FloatField(shootAngleProperty.displayName, shootAngleProperty.floatValue);
        EditorGUILayout.PropertyField(trajectoryProperty);
        if (m_capsuleProfile.stats.trajectory == TrajectoryType.CURVE)
        {
            angleTrajectoryProperty.floatValue = EditorGUILayout.FloatField(angleTrajectoryProperty.displayName, angleTrajectoryProperty.floatValue);
            trajectoryTimerProperty.floatValue = EditorGUILayout.FloatField(trajectoryTimerProperty.displayName, trajectoryTimerProperty.floatValue);
        }
        shootNumberProperty.floatValue = EditorGUILayout.FloatField(shootNumberProperty.displayName, shootNumberProperty.floatValue);
        TimeIntervalProperty.floatValue = EditorGUILayout.FloatField(TimeIntervalProperty.displayName, TimeIntervalProperty.floatValue);
        sizeProperty.floatValue = EditorGUILayout.FloatField(sizeProperty.displayName, sizeProperty.floatValue);
        sizeMultiplicatorFactorProperty.floatValue = EditorGUILayout.FloatField(sizeMultiplicatorFactorProperty.displayName, sizeMultiplicatorFactorProperty.floatValue);
        piercingMaxProperty.intValue = EditorGUILayout.IntField(piercingMaxProperty.displayName, piercingMaxProperty.intValue);
        descriptionProperty.stringValue = EditorGUILayout.TextField(descriptionProperty.displayName, descriptionProperty.stringValue);
        EditorGUILayout.PropertyField(formSpellProperty);
        stackDurationProperty.floatValue = EditorGUILayout.FloatField(stackDurationProperty.displayName, stackDurationProperty.floatValue);
        stackPerGainProperty.intValue = EditorGUILayout.IntField(stackPerGainProperty.displayName, stackPerGainProperty.intValue);

        serializedObject.ApplyModifiedProperties();
    }
}
