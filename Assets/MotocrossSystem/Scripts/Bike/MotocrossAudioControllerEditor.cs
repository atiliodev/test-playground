using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MotocrossAudioController))]
public class MotocrossAudioControllerEditor : Editor
{
    private SerializedProperty idleClip, lowRPMClip, midRPMClip, highRPMClip, limiterClip;
    private SerializedProperty idleVolumeCurve, lowVolumeCurve, stateVolumeCurve, midVolumeCurve, highVolumeCurve, limiterVolumeCurve;
    private SerializedProperty idlePitchCurve, lowPitchCurve, midPitchCurve, highPitchCurve, limiterPitchCurve;
    private SerializedProperty currentRPM, maxRPM;
    private SerializedProperty dopplerLevel, minDistance, maxDistance;
    private SerializedProperty mixer;

    private bool showAudioClips = true;
    private bool showVolumeCurves = true;
    private bool showPitchCurves = true;
    private bool showRPM = true;
    private bool show3DSound = true;

    void OnEnable()
    {
        idleClip = serializedObject.FindProperty("idleClip");
        lowRPMClip = serializedObject.FindProperty("lowRPMClip");
        midRPMClip = serializedObject.FindProperty("midRPMClip");
        highRPMClip = serializedObject.FindProperty("highRPMClip");
        limiterClip = serializedObject.FindProperty("limiterClip");
        limiterClip = serializedObject.FindProperty("limiterClip");
        mixer = serializedObject.FindProperty("mixer");

        idleVolumeCurve = serializedObject.FindProperty("idleVolumeCurve");
        lowVolumeCurve = serializedObject.FindProperty("lowVolumeCurve");
        midVolumeCurve = serializedObject.FindProperty("midVolumeCurve");
        stateVolumeCurve = serializedObject.FindProperty("stateVolumeCurve");
        highVolumeCurve = serializedObject.FindProperty("highVolumeCurve");
        limiterVolumeCurve = serializedObject.FindProperty("limiterVolumeCurve");

        idlePitchCurve = serializedObject.FindProperty("idlePitchCurve");
        lowPitchCurve = serializedObject.FindProperty("lowPitchCurve");
        midPitchCurve = serializedObject.FindProperty("midPitchCurve");
        highPitchCurve = serializedObject.FindProperty("highPitchCurve");
        limiterPitchCurve = serializedObject.FindProperty("limiterPitchCurve");

        currentRPM = serializedObject.FindProperty("currentRPM");
        maxRPM = serializedObject.FindProperty("maxRPM");

        dopplerLevel = serializedObject.FindProperty("dopplerLevel");
        minDistance = serializedObject.FindProperty("minDistance");
        maxDistance = serializedObject.FindProperty("maxDistance");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField(" Motocross Audio Controller", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Clips
        showAudioClips = EditorGUILayout.Foldout(showAudioClips, "Clips");
        if (showAudioClips)
        {
            EditorGUILayout.PropertyField(idleClip, new GUIContent("Idle"));
            EditorGUILayout.PropertyField(lowRPMClip, new GUIContent("Low RPM"));
            EditorGUILayout.PropertyField(midRPMClip, new GUIContent("Mid RPM"));
            EditorGUILayout.PropertyField(highRPMClip, new GUIContent("High RPM"));
            EditorGUILayout.PropertyField(limiterClip, new GUIContent("Limiter Clip"));
        }
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(mixer, new GUIContent("Mixer"));
        

        // Volume Curves
        showVolumeCurves = EditorGUILayout.Foldout(showVolumeCurves, "Volume Curves");
        if (showVolumeCurves)
        {
            EditorGUILayout.PropertyField(idleVolumeCurve, new GUIContent("Idle"));
            EditorGUILayout.PropertyField(lowVolumeCurve, new GUIContent("Low RPM"));
            EditorGUILayout.PropertyField(midVolumeCurve, new GUIContent("Mid RPM"));
            EditorGUILayout.PropertyField(stateVolumeCurve, new GUIContent("State RPM"));
            EditorGUILayout.PropertyField(highVolumeCurve, new GUIContent("High RPM"));
            EditorGUILayout.PropertyField(limiterVolumeCurve, new GUIContent("Limiter"));
        }

        // Pitch Curves
        showPitchCurves = EditorGUILayout.Foldout(showPitchCurves, "Pitch Curves");
        if (showPitchCurves)
        {
            EditorGUILayout.PropertyField(idlePitchCurve, new GUIContent("Idle"));
            EditorGUILayout.PropertyField(lowPitchCurve, new GUIContent("Low RPM"));
            EditorGUILayout.PropertyField(midPitchCurve, new GUIContent("Mid RPM"));
            EditorGUILayout.PropertyField(highPitchCurve, new GUIContent("High RPM"));
            EditorGUILayout.PropertyField(limiterPitchCurve, new GUIContent("Limiter"));
        }

        // RPM Parameters
       // showRPM = EditorGUILayout.Foldout(showRPM, "RPM Parameters");
      
       // EditorGUILayout.PropertyField(currentRPM);
        //EditorGUILayout.PropertyField(maxRPM);
        

        // 3D Sound and Doppler
        show3DSound = EditorGUILayout.Foldout(show3DSound, "Space 3D & Doppler");
        if (show3DSound)
        {
            EditorGUILayout.PropertyField(dopplerLevel);
            EditorGUILayout.PropertyField(minDistance);
            EditorGUILayout.PropertyField(maxDistance);
        }

        serializedObject.ApplyModifiedProperties();
    }
}