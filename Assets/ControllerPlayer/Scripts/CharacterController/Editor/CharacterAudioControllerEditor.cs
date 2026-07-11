using UnityEngine;
using UnityEditor;

namespace Invector.vCharacterController
{
    [CustomEditor(typeof(CharacterAudioController))]
    public class CharacterAudioControllerEditor : Editor
    {
        private SerializedProperty audioSource;
        private SerializedProperty jumpSound;
        private SerializedProperty jumpVolume;
        private SerializedProperty landingSound;
        private SerializedProperty landingVolume;
        private SerializedProperty walkFootsteps;
        private SerializedProperty walkVolume;
        private SerializedProperty runFootsteps;
        private SerializedProperty runVolume;
        private SerializedProperty sprintFootsteps;
        private SerializedProperty sprintVolume;
        private SerializedProperty walkStepInterval;
        private SerializedProperty runStepInterval;
        private SerializedProperty sprintStepInterval;
        private SerializedProperty minInputForFootsteps;
        private SerializedProperty useRandomPitch;
        private SerializedProperty pitchVariation;

        private bool showJumpSettings = true;
        private bool showFootstepSettings = true;
        private bool showAdvancedSettings = false;
        private bool showPresetSettings = false;
        
        private CharacterAudioPreset presetToApply;
        private CharacterAudioPreset presetToSave;

        void OnEnable()
        {
            audioSource = serializedObject.FindProperty("audioSource");
            jumpSound = serializedObject.FindProperty("jumpSound");
            jumpVolume = serializedObject.FindProperty("jumpVolume");
            landingSound = serializedObject.FindProperty("landingSound");
            landingVolume = serializedObject.FindProperty("landingVolume");
            walkFootsteps = serializedObject.FindProperty("walkFootsteps");
            walkVolume = serializedObject.FindProperty("walkVolume");
            runFootsteps = serializedObject.FindProperty("runFootsteps");
            runVolume = serializedObject.FindProperty("runVolume");
            sprintFootsteps = serializedObject.FindProperty("sprintFootsteps");
            sprintVolume = serializedObject.FindProperty("sprintVolume");
            walkStepInterval = serializedObject.FindProperty("walkStepInterval");
            runStepInterval = serializedObject.FindProperty("runStepInterval");
            sprintStepInterval = serializedObject.FindProperty("sprintStepInterval");
            minInputForFootsteps = serializedObject.FindProperty("minInputForFootsteps");
            useRandomPitch = serializedObject.FindProperty("useRandomPitch");
            pitchVariation = serializedObject.FindProperty("pitchVariation");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            CharacterAudioController script = (CharacterAudioController)target;

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Character Audio Controller manages all sound effects for the character including footsteps, jumps, and landings.", MessageType.Info);
            EditorGUILayout.Space();

            // Preset Management
            showPresetSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showPresetSettings, "Preset Management");
            if (showPresetSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox("Presets allow you to save and reuse audio configurations across multiple characters.", MessageType.Info);
                
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Load Preset", EditorStyles.boldLabel);
                presetToApply = (CharacterAudioPreset)EditorGUILayout.ObjectField("Preset", presetToApply, typeof(CharacterAudioPreset), false);
                
                EditorGUI.BeginDisabledGroup(presetToApply == null);
                if (GUILayout.Button("Apply Preset to Controller"))
                {
                    if (presetToApply != null)
                    {
                        Undo.RecordObject(script, "Apply Audio Preset");
                        presetToApply.ApplyToController(script);
                        EditorUtility.SetDirty(script);
                        serializedObject.Update();
                    }
                }
                EditorGUI.EndDisabledGroup();
                
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Save Current Settings", EditorStyles.boldLabel);
                
                if (GUILayout.Button("Create New Preset from Current Settings"))
                {
                    string path = EditorUtility.SaveFilePanelInProject(
                        "Save Audio Preset",
                        "CharacterAudioPreset",
                        "asset",
                        "Save audio preset as..."
                    );
                    
                    if (!string.IsNullOrEmpty(path))
                    {
                        CharacterAudioPreset newPreset = ScriptableObject.CreateInstance<CharacterAudioPreset>();
                        newPreset.LoadFromController(script);
                        AssetDatabase.CreateAsset(newPreset, path);
                        AssetDatabase.SaveAssets();
                        EditorUtility.FocusProjectWindow();
                        Selection.activeObject = newPreset;
                        Debug.Log("Audio preset created at: " + path);
                    }
                }
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();

            // Audio Source
            EditorGUILayout.LabelField("Audio Source", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(audioSource);
            EditorGUILayout.Space();

            // Jump & Landing Settings
            showJumpSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showJumpSettings, "Jump & Landing Sounds");
            if (showJumpSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(jumpSound);
                EditorGUILayout.PropertyField(jumpVolume);
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(landingSound);
                EditorGUILayout.PropertyField(landingVolume);
                
                if (Application.isPlaying)
                {
                    EditorGUILayout.Space(5);
                    if (GUILayout.Button("Test Jump Sound"))
                    {
                        script.PlayJumpSound();
                    }
                    if (GUILayout.Button("Test Landing Sound"))
                    {
                        script.PlayLandingSound();
                    }
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();

            // Footstep Settings
            showFootstepSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showFootstepSettings, "Footstep Sounds");
            if (showFootstepSettings)
            {
                EditorGUI.indentLevel++;
                
                // Walk
                EditorGUILayout.LabelField("Walk", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(walkFootsteps, true);
                EditorGUILayout.PropertyField(walkVolume);
                EditorGUILayout.PropertyField(walkStepInterval);
                EditorGUILayout.Space(5);
                
                // Run
                EditorGUILayout.LabelField("Run", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(runFootsteps, true);
                EditorGUILayout.PropertyField(runVolume);
                EditorGUILayout.PropertyField(runStepInterval);
                EditorGUILayout.Space(5);
                
                // Sprint
                EditorGUILayout.LabelField("Sprint", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(sprintFootsteps, true);
                EditorGUILayout.HelpBox("If sprint footsteps are empty, run footsteps will be used.", MessageType.Info);
                EditorGUILayout.PropertyField(sprintVolume);
                EditorGUILayout.PropertyField(sprintStepInterval);
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();

            // Advanced Settings
            showAdvancedSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showAdvancedSettings, "Advanced Settings");
            if (showAdvancedSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(minInputForFootsteps);
                EditorGUILayout.PropertyField(useRandomPitch);
                if (useRandomPitch.boolValue)
                {
                    EditorGUILayout.PropertyField(pitchVariation);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();

            // Warning messages
            EditorGUILayout.Space();
            if (walkFootsteps.arraySize == 0 && runFootsteps.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No footstep sounds assigned! Please add at least walk or run footstep sounds.", MessageType.Warning);
            }
        }
    }
}
