using UnityEngine;
using UnityEditor;
using System.IO;

namespace Invector.vCharacterController
{
    /// <summary>
    /// Setup wizard untuk membantu konfigurasi Character Audio Controller
    /// </summary>
    public class CharacterAudioSetupWizard : EditorWindow
    {
        private GameObject targetCharacter;
        private CharacterAudioController audioController;
        private Vector2 scrollPosition;

        // Audio clips to assign
        private AudioClip jumpSound;
        private AudioClip landingSound;
        private AudioClip[] walkFootsteps = new AudioClip[0];
        private AudioClip[] runFootsteps = new AudioClip[0];
        private AudioClip[] sprintFootsteps = new AudioClip[0];

        [MenuItem("Invector/Character Audio Setup Wizard")]
        static void Init()
        {
            CharacterAudioSetupWizard window = (CharacterAudioSetupWizard)EditorWindow.GetWindow(typeof(CharacterAudioSetupWizard));
            window.titleContent = new GUIContent("Audio Setup");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }

        void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Character Audio Setup Wizard", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Wizard ini membantu Anda setup audio untuk karakter dengan mudah.", MessageType.Info);
            EditorGUILayout.Space(10);

            // Step 1: Select Character
            DrawStep1_SelectCharacter();
            EditorGUILayout.Space(10);

            // Step 2: Auto-Setup
            if (targetCharacter != null)
            {
                DrawStep2_AutoSetup();
                EditorGUILayout.Space(10);
            }

            // Step 3: Assign Audio Clips
            if (audioController != null)
            {
                DrawStep3_AssignAudio();
                EditorGUILayout.Space(10);
            }

            // Step 4: Quick Assign from Folder
            if (audioController != null)
            {
                DrawStep4_QuickAssign();
                EditorGUILayout.Space(10);
            }

            // Step 5: Final Actions
            if (audioController != null)
            {
                DrawStep5_FinalActions();
            }

            EditorGUILayout.EndScrollView();
        }

        void DrawStep1_SelectCharacter()
        {
            EditorGUILayout.LabelField("Step 1: Select Character", EditorStyles.boldLabel);
            
            GameObject newTarget = (GameObject)EditorGUILayout.ObjectField(
                "Character GameObject", 
                targetCharacter, 
                typeof(GameObject), 
                true
            );

            if (newTarget != targetCharacter)
            {
                targetCharacter = newTarget;
                if (targetCharacter != null)
                {
                    audioController = targetCharacter.GetComponent<CharacterAudioController>();
                }
            }

            if (targetCharacter == null)
            {
                EditorGUILayout.HelpBox("Pilih GameObject karakter yang memiliki vThirdPersonInput atau vThirdPersonController component.", MessageType.Warning);
            }
            else
            {
                var controller = targetCharacter.GetComponent<vThirdPersonController>();
                var input = targetCharacter.GetComponent<vThirdPersonInput>();

                if (controller == null && input == null)
                {
                    EditorGUILayout.HelpBox("GameObject ini tidak memiliki vThirdPersonController atau vThirdPersonInput component!", MessageType.Error);
                }
                else
                {
                    EditorGUILayout.HelpBox("✓ Character valid ditemukan!", MessageType.Info);
                }
            }
        }

        void DrawStep2_AutoSetup()
        {
            EditorGUILayout.LabelField("Step 2: Auto Setup", EditorStyles.boldLabel);

            if (audioController == null)
            {
                EditorGUILayout.HelpBox("Character Audio Controller belum ada pada GameObject ini.", MessageType.Warning);
                
                if (GUILayout.Button("Add Character Audio Controller", GUILayout.Height(30)))
                {
                    audioController = targetCharacter.AddComponent<CharacterAudioController>();
                    EditorUtility.SetDirty(targetCharacter);
                    Debug.Log("Character Audio Controller added to " + targetCharacter.name);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("✓ Character Audio Controller sudah ada!", MessageType.Info);
                
                if (GUILayout.Button("Remove Character Audio Controller"))
                {
                    if (EditorUtility.DisplayDialog("Remove Component?", 
                        "Apakah Anda yakin ingin menghapus Character Audio Controller?", 
                        "Yes", "No"))
                    {
                        DestroyImmediate(audioController);
                        audioController = null;
                    }
                }
            }
        }

        void DrawStep3_AssignAudio()
        {
            EditorGUILayout.LabelField("Step 3: Assign Audio Clips", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Drag & drop audio clips ke field di bawah ini.", MessageType.Info);

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Jump & Landing", EditorStyles.boldLabel);
            jumpSound = (AudioClip)EditorGUILayout.ObjectField("Jump Sound", jumpSound, typeof(AudioClip), false);
            landingSound = (AudioClip)EditorGUILayout.ObjectField("Landing Sound", landingSound, typeof(AudioClip), false);

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Footsteps", EditorStyles.boldLabel);
            
            // Walk footsteps array
            EditorGUILayout.LabelField("Walk Footsteps:");
            ScriptableObject target = this;
            SerializedObject so = new SerializedObject(target);
            
            int newWalkSize = EditorGUILayout.IntField("Size", walkFootsteps.Length);
            if (newWalkSize != walkFootsteps.Length)
            {
                System.Array.Resize(ref walkFootsteps, newWalkSize);
            }
            for (int i = 0; i < walkFootsteps.Length; i++)
            {
                walkFootsteps[i] = (AudioClip)EditorGUILayout.ObjectField($"  Element {i}", walkFootsteps[i], typeof(AudioClip), false);
            }

            EditorGUILayout.Space(5);
            
            // Run footsteps array
            EditorGUILayout.LabelField("Run Footsteps:");
            int newRunSize = EditorGUILayout.IntField("Size", runFootsteps.Length);
            if (newRunSize != runFootsteps.Length)
            {
                System.Array.Resize(ref runFootsteps, newRunSize);
            }
            for (int i = 0; i < runFootsteps.Length; i++)
            {
                runFootsteps[i] = (AudioClip)EditorGUILayout.ObjectField($"  Element {i}", runFootsteps[i], typeof(AudioClip), false);
            }

            EditorGUILayout.Space(5);
            
            // Sprint footsteps array
            EditorGUILayout.LabelField("Sprint Footsteps (Optional):");
            int newSprintSize = EditorGUILayout.IntField("Size", sprintFootsteps.Length);
            if (newSprintSize != sprintFootsteps.Length)
            {
                System.Array.Resize(ref sprintFootsteps, newSprintSize);
            }
            for (int i = 0; i < sprintFootsteps.Length; i++)
            {
                sprintFootsteps[i] = (AudioClip)EditorGUILayout.ObjectField($"  Element {i}", sprintFootsteps[i], typeof(AudioClip), false);
            }

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Apply Audio Clips to Controller", GUILayout.Height(30)))
            {
                ApplyAudioClips();
            }
        }

        void DrawStep4_QuickAssign()
        {
            EditorGUILayout.LabelField("Step 4: Quick Assign from Folder", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Auto-assign semua audio clips dari folder yang dipilih.\n" +
                "File naming convention:\n" +
                "- *jump* → Jump sound\n" +
                "- *land* → Landing sound\n" +
                "- *walk* atau *step* → Walk footsteps\n" +
                "- *run* → Run footsteps\n" +
                "- *sprint* → Sprint footsteps", MessageType.Info);

            if (GUILayout.Button("Select Folder and Auto-Assign"))
            {
                string folderPath = EditorUtility.OpenFolderPanel("Select Audio Folder", "Assets", "");
                if (!string.IsNullOrEmpty(folderPath))
                {
                    AutoAssignFromFolder(folderPath);
                }
            }
        }

        void DrawStep5_FinalActions()
        {
            EditorGUILayout.LabelField("Step 5: Testing & Finalization", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox("Setup selesai! Anda dapat:\n" +
                "1. Test sounds di Play Mode\n" +
                "2. Adjust volume dan intervals di Inspector\n" +
                "3. Baca dokumentasi lengkap di CHARACTER_AUDIO_SETUP.md", MessageType.Info);

            if (GUILayout.Button("Select Character in Hierarchy"))
            {
                Selection.activeGameObject = targetCharacter;
                EditorGUIUtility.PingObject(targetCharacter);
            }

            if (GUILayout.Button("Open Documentation"))
            {
                string docPath = "Assets/ControllerPlayer/Scripts/CharacterController/CHARACTER_AUDIO_SETUP.md";
                if (File.Exists(docPath))
                {
                    System.Diagnostics.Process.Start(docPath);
                }
                else
                {
                    EditorUtility.DisplayDialog("Documentation Not Found", 
                        "File CHARACTER_AUDIO_SETUP.md tidak ditemukan di path:\n" + docPath, "OK");
                }
            }
        }

        void ApplyAudioClips()
        {
            if (audioController == null) return;

            Undo.RecordObject(audioController, "Apply Audio Clips");

            audioController.jumpSound = jumpSound;
            audioController.landingSound = landingSound;
            audioController.walkFootsteps = walkFootsteps;
            audioController.runFootsteps = runFootsteps;
            audioController.sprintFootsteps = sprintFootsteps;

            EditorUtility.SetDirty(audioController);
            
            Debug.Log("Audio clips applied to " + targetCharacter.name);
            EditorUtility.DisplayDialog("Success", "Audio clips berhasil di-apply ke Character Audio Controller!", "OK");
        }

        void AutoAssignFromFolder(string folderPath)
        {
            if (!folderPath.StartsWith(Application.dataPath))
            {
                EditorUtility.DisplayDialog("Invalid Folder", 
                    "Folder harus berada di dalam project Assets folder!", "OK");
                return;
            }

            // Convert to relative path
            string relativePath = "Assets" + folderPath.Substring(Application.dataPath.Length);
            
            // Find all audio clips in folder
            string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { relativePath });
            
            System.Collections.Generic.List<AudioClip> walkClips = new System.Collections.Generic.List<AudioClip>();
            System.Collections.Generic.List<AudioClip> runClips = new System.Collections.Generic.List<AudioClip>();
            System.Collections.Generic.List<AudioClip> sprintClips = new System.Collections.Generic.List<AudioClip>();

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
                
                if (clip != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(assetPath).ToLower();
                    
                    if (fileName.Contains("jump"))
                    {
                        jumpSound = clip;
                    }
                    else if (fileName.Contains("land"))
                    {
                        landingSound = clip;
                    }
                    else if (fileName.Contains("sprint"))
                    {
                        sprintClips.Add(clip);
                    }
                    else if (fileName.Contains("run"))
                    {
                        runClips.Add(clip);
                    }
                    else if (fileName.Contains("walk") || fileName.Contains("step"))
                    {
                        walkClips.Add(clip);
                    }
                }
            }

            walkFootsteps = walkClips.ToArray();
            runFootsteps = runClips.ToArray();
            sprintFootsteps = sprintClips.ToArray();

            Debug.Log($"Auto-assigned: {walkClips.Count} walk, {runClips.Count} run, {sprintClips.Count} sprint clips");
            
            EditorUtility.DisplayDialog("Auto-Assign Complete", 
                $"Found and assigned:\n" +
                $"- Jump: {(jumpSound != null ? "✓" : "✗")}\n" +
                $"- Landing: {(landingSound != null ? "✓" : "✗")}\n" +
                $"- Walk footsteps: {walkClips.Count}\n" +
                $"- Run footsteps: {runClips.Count}\n" +
                $"- Sprint footsteps: {sprintClips.Count}\n\n" +
                $"Klik 'Apply Audio Clips to Controller' untuk menyimpan.", "OK");
        }
    }
}
