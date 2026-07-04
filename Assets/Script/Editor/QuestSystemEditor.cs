using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuestSystem))]
public class QuestSystemEditor : Editor
{
    private QuestSystem questSystem;
    private SerializedProperty questNameProp;
    private SerializedProperty questDescriptionProp;
    private SerializedProperty questItemsProp;
    private SerializedProperty currentQuestIndexProp;
    private SerializedProperty questCompletedProp;
    
    void OnEnable()
    {
        questSystem = (QuestSystem)target;
        
        questNameProp = serializedObject.FindProperty("questName");
        questDescriptionProp = serializedObject.FindProperty("questDescription");
        questItemsProp = serializedObject.FindProperty("questItems");
        currentQuestIndexProp = serializedObject.FindProperty("currentQuestIndex");
        questCompletedProp = serializedObject.FindProperty("questCompleted");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        // Header
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("QUEST SYSTEM", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Quest Info
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Quest Information", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(questNameProp);
        EditorGUILayout.PropertyField(questDescriptionProp);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();
        
        // Quest Progress (Runtime Only)
        if (Application.isPlaying)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Runtime Information", EditorStyles.boldLabel);
            
            GUI.enabled = false;
            EditorGUILayout.PropertyField(currentQuestIndexProp, new GUIContent("Current Quest Index"));
            EditorGUILayout.PropertyField(questCompletedProp, new GUIContent("Quest Completed"));
            
            if (questSystem != null)
            {
                EditorGUILayout.LabelField("Quest Status", questSystem.GetQuestStatus());
                EditorGUILayout.Slider("Progress", questSystem.GetQuestProgress(), 0f, 1f);
            }
            GUI.enabled = true;
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        
        // Quest Items
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Quest Items", EditorStyles.boldLabel);
        
        if (questItemsProp.arraySize == 0)
        {
            EditorGUILayout.HelpBox("No quest items configured. Add quest items to create a sequential quest.", MessageType.Info);
        }
        
        for (int i = 0; i < questItemsProp.arraySize; i++)
        {
            DrawQuestItem(i);
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Quest Item"))
        {
            questItemsProp.arraySize++;
        }
        
        GUI.enabled = questItemsProp.arraySize > 0;
        if (GUILayout.Button("Remove Last Item"))
        {
            questItemsProp.arraySize--;
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();
        
        // Events
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        // Control Buttons
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Quest Controls", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        
        if (Application.isPlaying)
        {
            if (GUILayout.Button("Start Quest"))
            {
                questSystem.StartQuest();
            }
            
            GUI.enabled = questSystem.currentQuestIndex < questSystem.questItems.Count && !questSystem.questCompleted;
            if (GUILayout.Button("Complete Current Item"))
            {
                questSystem.DebugCompleteCurrentItem();
            }
            GUI.enabled = true;
            
            if (GUILayout.Button("Reset Quest"))
            {
                questSystem.ResetQuest();
            }
        }
        else
        {
            GUI.enabled = false;
            GUILayout.Button("Start Quest (Play Mode Only)");
            GUILayout.Button("Complete Current Item (Play Mode Only)");
            GUILayout.Button("Reset Quest (Play Mode Only)");
            GUI.enabled = true;
        }
        
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();
        
        // Setup and Validation
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Setup & Validation", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Auto Setup Quest System"))
        {
            AutoSetupQuestSystem();
        }
        
        if (GUILayout.Button("Validate Quest Setup"))
        {
            ValidateQuestSetup();
        }
        
        if (GUILayout.Button("Create Example Quest Items"))
        {
            CreateExampleQuestItems();
        }
        
        EditorGUILayout.EndVertical();
        
        serializedObject.ApplyModifiedProperties();
    }
    
    void DrawQuestItem(int index)
    {
        SerializedProperty itemProp = questItemsProp.GetArrayElementAtIndex(index);
        
        EditorGUILayout.BeginVertical("box");
        
        // Item header
        string itemName = itemProp.FindPropertyRelative("itemName").stringValue;
        if (string.IsNullOrEmpty(itemName)) itemName = $"Quest Item {index + 1}";
        
        bool isExpanded = itemProp.isExpanded;
        string foldoutText = $"{index + 1}. {itemName}";
        
        // Runtime status indicator
        if (Application.isPlaying && questSystem != null)
        {
            if (index < questSystem.currentQuestIndex)
                foldoutText += " ✅ (Collected)";
            else if (index == questSystem.currentQuestIndex && !questSystem.questCompleted)
                foldoutText += " 🎯 (Active)";
            else
                foldoutText += " ⏳ (Waiting)";
        }
        
        itemProp.isExpanded = EditorGUILayout.Foldout(isExpanded, foldoutText, true);
        
        if (itemProp.isExpanded)
        {
            EditorGUI.indentLevel++;
            
            EditorGUILayout.PropertyField(itemProp.FindPropertyRelative("itemName"));
            EditorGUILayout.PropertyField(itemProp.FindPropertyRelative("itemDescription"));
            EditorGUILayout.PropertyField(itemProp.FindPropertyRelative("questDescriptionForThisItem"));
            EditorGUILayout.PropertyField(itemProp.FindPropertyRelative("itemIcon"));
            EditorGUILayout.PropertyField(itemProp.FindPropertyRelative("itemPrefab"));
            EditorGUILayout.PropertyField(itemProp.FindPropertyRelative("spawnPosition"));
            
            EditorGUILayout.Space();
            
            // Runtime status
            if (Application.isPlaying)
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(itemProp.FindPropertyRelative("isActive"));
                EditorGUILayout.PropertyField(itemProp.FindPropertyRelative("isCollected"));
                GUI.enabled = true;
            }
            
            // Events
            EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(itemProp.FindPropertyRelative("OnItemActivated"));
            EditorGUILayout.PropertyField(itemProp.FindPropertyRelative("OnItemCollected"));
            
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.EndVertical();
    }
    
    void AutoSetupQuestSystem()
    {
        QuestSystemSetup setup = questSystem.GetComponent<QuestSystemSetup>();
        if (setup == null)
        {
            setup = questSystem.gameObject.AddComponent<QuestSystemSetup>();
        }
        
        setup.SetupQuestSystem();
        EditorUtility.DisplayDialog("Quest System Setup", "Quest System has been automatically configured!", "OK");
    }
    
    void ValidateQuestSetup()
    {
        bool isValid = true;
        string validationReport = "QUEST SYSTEM VALIDATION:\n\n";
        
        // Check basic setup
        if (string.IsNullOrEmpty(questSystem.questName))
        {
            validationReport += "❌ Quest name is empty\n";
            isValid = false;
        }
        else
        {
            validationReport += $"✅ Quest name: {questSystem.questName}\n";
        }
        
        // Check quest items
        if (questSystem.questItems.Count == 0)
        {
            validationReport += "❌ No quest items configured\n";
            isValid = false;
        }
        else
        {
            validationReport += $"✅ {questSystem.questItems.Count} quest items configured\n";
            
            for (int i = 0; i < questSystem.questItems.Count; i++)
            {
                var item = questSystem.questItems[i];
                
                if (string.IsNullOrEmpty(item.itemName))
                {
                    validationReport += $"⚠️ Item {i + 1}: Missing item name\n";
                }
                
                if (item.itemPrefab == null)
                {
                    validationReport += $"⚠️ Item {i + 1}: Missing item prefab\n";
                }
                
                if (item.spawnPosition == null)
                {
                    validationReport += $"⚠️ Item {i + 1}: Missing spawn position\n";
                }
            }
        }
        
        // Check InventorySystem
        if (FindObjectOfType<InventorySystem>() == null)
        {
            validationReport += "❌ InventorySystem not found in scene\n";
            isValid = false;
        }
        else
        {
            validationReport += "✅ InventorySystem found\n";
        }
        
        // Check QuestUI
        if (FindObjectOfType<QuestUI>() == null)
        {
            validationReport += "⚠️ QuestUI not found - UI won't be displayed\n";
        }
        else
        {
            validationReport += "✅ QuestUI found\n";
        }
        
        validationReport += "\n";
        if (isValid)
        {
            validationReport += "🎉 Validation passed! Quest system is ready to use.";
        }
        else
        {
            validationReport += "💥 Validation failed! Please fix the issues above.";
        }
        
        EditorUtility.DisplayDialog("Quest System Validation", validationReport, "OK");
    }
    
    void CreateExampleQuestItems()
    {
        questItemsProp.arraySize = 3;
        
        // Item 1: Crystal
        var item1 = questItemsProp.GetArrayElementAtIndex(0);
        item1.FindPropertyRelative("itemName").stringValue = "Ancient Crystal";
        item1.FindPropertyRelative("itemDescription").stringValue = "A mystical crystal that glows with ancient power";
        
        // Item 2: Scroll
        var item2 = questItemsProp.GetArrayElementAtIndex(1);
        item2.FindPropertyRelative("itemName").stringValue = "Sacred Scroll";
        item2.FindPropertyRelative("itemDescription").stringValue = "An old scroll containing forgotten knowledge";
        
        // Item 3: Key
        var item3 = questItemsProp.GetArrayElementAtIndex(2);
        item3.FindPropertyRelative("itemName").stringValue = "Golden Key";
        item3.FindPropertyRelative("itemDescription").stringValue = "A golden key that opens the final chamber";
        
        serializedObject.ApplyModifiedProperties();
        
        EditorUtility.DisplayDialog("Example Quest Created", "Three example quest items have been created. Don't forget to assign prefabs and spawn positions!", "OK");
    }
}