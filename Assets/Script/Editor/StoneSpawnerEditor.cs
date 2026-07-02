using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StoneSpawner))]
public class StoneSpawnerEditor : Editor
{
    private StoneSpawner spawner;
    private bool showAdvancedSettings = false;
    private bool showPhysicsSettings = false;
    private bool showVisualSettings = true;

    void OnEnable()
    {
        spawner = (StoneSpawner)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Stone Spawner Controller", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        // Quick Test Buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🪨 Spawn Stone", GUILayout.Height(30)))
        {
            if (Application.isPlaying)
                spawner.SpawnStone();
            else
                Debug.Log("Harus dalam Play Mode untuk spawn batu!");
        }
        
        if (GUILayout.Button("🔄 Toggle Auto Spawn", GUILayout.Height(30)))
        {
            if (Application.isPlaying)
                spawner.ToggleAutoSpawn();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        // Stone Settings Section
        EditorGUILayout.LabelField("🪨 Stone Configuration", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        
        SerializedProperty stonePrefab = serializedObject.FindProperty("stonePrefab");
        SerializedProperty spawnPoint = serializedObject.FindProperty("spawnPoint");
        
        EditorGUILayout.PropertyField(stonePrefab, new GUIContent("Stone Prefab", "Prefab batu yang akan di-spawn"));
        EditorGUILayout.PropertyField(spawnPoint, new GUIContent("Spawn Point", "Titik spawn batu (kosongkan untuk menggunakan transform object ini)"));
        
        EditorGUILayout.Space(10);

        // Trajectory Settings
        EditorGUILayout.LabelField("🎯 Trajectory Settings", EditorStyles.boldLabel);
        
        SerializedProperty throwForce = serializedObject.FindProperty("throwForce");
        SerializedProperty throwAngle = serializedObject.FindProperty("throwAngle");
        SerializedProperty throwDirection = serializedObject.FindProperty("throwDirection");
        
        EditorGUILayout.PropertyField(throwForce, new GUIContent("Throw Force", "Kekuatan lontaran"));
        EditorGUILayout.Slider(throwAngle, 0f, 90f, new GUIContent("Throw Angle", "Sudut lontaran (derajat)"));
        
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Throw Direction", EditorStyles.miniBoldLabel);
        Vector3 direction = EditorGUILayout.Vector3Field("", throwDirection.vector3Value);
        throwDirection.vector3Value = direction.normalized;
        
        // Direction preset buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Forward")) throwDirection.vector3Value = Vector3.forward;
        if (GUILayout.Button("Back")) throwDirection.vector3Value = Vector3.back;
        if (GUILayout.Button("Left")) throwDirection.vector3Value = Vector3.left;
        if (GUILayout.Button("Right")) throwDirection.vector3Value = Vector3.right;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        // Spawn Settings
        showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "⚙️ Advanced Spawn Settings");
        if (showAdvancedSettings)
        {
            EditorGUI.indentLevel++;
            
            SerializedProperty spawnInterval = serializedObject.FindProperty("spawnInterval");
            SerializedProperty autoSpawn = serializedObject.FindProperty("autoSpawn");
            SerializedProperty maxStones = serializedObject.FindProperty("maxStones");
            SerializedProperty stoneLifetime = serializedObject.FindProperty("stoneLifetime");
            
            EditorGUILayout.PropertyField(autoSpawn, new GUIContent("Auto Spawn", "Spawn otomatis setiap interval"));
            EditorGUILayout.PropertyField(spawnInterval, new GUIContent("Spawn Interval", "Interval waktu spawn (detik)"));
            EditorGUILayout.PropertyField(maxStones, new GUIContent("Max Stones", "Maksimal batu yang bisa ada bersamaan"));
            EditorGUILayout.PropertyField(stoneLifetime, new GUIContent("Stone Lifetime", "Berapa lama batu bertahan sebelum dihapus (detik)"));
            
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space(5);

        // Physics Settings
        showPhysicsSettings = EditorGUILayout.Foldout(showPhysicsSettings, "⚖️ Physics Settings");
        if (showPhysicsSettings)
        {
            EditorGUI.indentLevel++;
            
            SerializedProperty mass = serializedObject.FindProperty("mass");
            SerializedProperty drag = serializedObject.FindProperty("drag");
            SerializedProperty angularDrag = serializedObject.FindProperty("angularDrag");
            
            EditorGUILayout.PropertyField(mass, new GUIContent("Mass", "Massa batu"));
            EditorGUILayout.PropertyField(drag, new GUIContent("Drag", "Air resistance"));
            EditorGUILayout.PropertyField(angularDrag, new GUIContent("Angular Drag", "Rotational resistance"));
            
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space(5);

        // Visual Settings
        showVisualSettings = EditorGUILayout.Foldout(showVisualSettings, "👁️ Visual Helpers");
        if (showVisualSettings)
        {
            EditorGUI.indentLevel++;
            
            SerializedProperty showTrajectory = serializedObject.FindProperty("showTrajectory");
            SerializedProperty trajectoryPoints = serializedObject.FindProperty("trajectoryPoints");
            SerializedProperty trajectoryTimeStep = serializedObject.FindProperty("trajectoryTimeStep");
            SerializedProperty trajectoryColor = serializedObject.FindProperty("trajectoryColor");
            
            EditorGUILayout.PropertyField(showTrajectory, new GUIContent("Show Trajectory", "Tampilkan jalur lontaran di Scene view"));
            
            if (showTrajectory.boolValue)
            {
                EditorGUILayout.PropertyField(trajectoryPoints, new GUIContent("Trajectory Points", "Jumlah titik trajectory"));
                EditorGUILayout.PropertyField(trajectoryTimeStep, new GUIContent("Time Step", "Interval waktu antar titik"));
                EditorGUILayout.PropertyField(trajectoryColor, new GUIContent("Trajectory Color", "Warna garis trajectory"));
            }
            
            EditorGUI.indentLevel--;
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            SceneView.RepaintAll(); // Refresh scene view untuk trajectory
        }

        EditorGUILayout.Space(10);

        // Info Box
        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox("💡 Tips:\n" +
                "• Tekan SPACE di Play Mode untuk spawn manual\n" +
                "• Garis merah di Scene view menunjukkan trajectory\n" +
                "• Garis biru menunjukkan arah lontaran", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("ℹ️ Masuk ke Play Mode untuk menguji spawner", MessageType.Info);
        }

        // Debug Info saat playing
        if (Application.isPlaying)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("🔍 Runtime Info", EditorStyles.boldLabel);
            
            EditorGUI.BeginDisabledGroup(true);
            // Tampilkan info sederhana tanpa akses kompleks ke child objects
            EditorGUILayout.TextField("Status", "Running");
            EditorGUI.EndDisabledGroup();
        }
    }

    void OnSceneGUI()
    {
        if (spawner == null) return;

        // Gunakan serialized properties untuk mengakses data
        SerializedProperty spawnPointProp = serializedObject.FindProperty("spawnPoint");
        SerializedProperty throwDirectionProp = serializedObject.FindProperty("throwDirection");
        
        Transform spawnPoint = spawner.transform;
        if (spawnPointProp.objectReferenceValue != null)
            spawnPoint = spawnPointProp.objectReferenceValue as Transform;

        Vector3 position = spawnPoint.position;
        Vector3 direction = throwDirectionProp.vector3Value;
        
        // Direction handle - gunakan PositionHandle untuk lebih simple
        EditorGUI.BeginChangeCheck();
        Vector3 handlePosition = position + direction * 3f;
        Vector3 newHandlePosition = Handles.PositionHandle(handlePosition, Quaternion.LookRotation(direction));
        
        if (EditorGUI.EndChangeCheck())
        {
            Vector3 newDirection = (newHandlePosition - position).normalized;
            throwDirectionProp.vector3Value = newDirection;
            serializedObject.ApplyModifiedProperties();
        }

        // Draw direction line
        Handles.color = Color.cyan;
        Handles.DrawLine(position, position + direction * 3f);
        
        // Label di scene
        Handles.Label(position + Vector3.up * 2, "Stone Spawner\nPress SPACE to spawn");
    }
}