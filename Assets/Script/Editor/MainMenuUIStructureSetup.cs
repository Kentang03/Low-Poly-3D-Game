using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// Editor script untuk membantu setup struktur UI Main Menu
/// yang memisahkan background dari button group
/// </summary>
public class MainMenuUIStructureSetup : EditorWindow
{
    [MenuItem("Tools/UI Setup/Main Menu Structure Setup")]
    public static void ShowWindow()
    {
        GetWindow<MainMenuUIStructureSetup>("Main Menu UI Setup");
    }

    private GameObject mainMenuPanel;
    private GameObject buttonGroupParent;
    
    void OnGUI()
    {
        GUILayout.Label("Main Menu UI Structure Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "Tool ini membantu setup struktur UI Main Menu dimana:\n" +
            "• Background tetap terlihat saat membuka panel\n" +
            "• Hanya button group yang disembunyikan/ditampilkan\n" +
            "• MainMenuPanel berisi background dan semua elemen\n" +
            "• ButtonGroup berisi hanya button Play, Settings, Credits, Exit",
            MessageType.Info
        );
        
        GUILayout.Space(10);
        
        mainMenuPanel = EditorGUILayout.ObjectField("Main Menu Panel", mainMenuPanel, typeof(GameObject), true) as GameObject;
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Auto Setup Structure"))
        {
            AutoSetupStructure();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Create Button Group Manually"))
        {
            CreateButtonGroupManually();
        }
        
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "Langkah manual:\n" +
            "1. Buat Empty GameObject sebagai child dari Main Menu Panel\n" +
            "2. Namakan 'MainMenuButtonsGroup'\n" +
            "3. Pindahkan semua button (Play, Settings, Credits, Exit) ke dalam group ini\n" +
            "4. Assign MainMenuButtonsGroup ke script MainMenuManager",
            MessageType.None
        );
    }
    
    void AutoSetupStructure()
    {
        if (mainMenuPanel == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign Main Menu Panel first!", "OK");
            return;
        }
        
        // Cari atau buat ButtonGroup
        Transform existingButtonGroup = mainMenuPanel.transform.Find("MainMenuButtonsGroup");
        GameObject buttonGroup;
        
        if (existingButtonGroup != null)
        {
            buttonGroup = existingButtonGroup.gameObject;
            Debug.Log("Found existing MainMenuButtonsGroup");
        }
        else
        {
            // Buat ButtonGroup baru
            buttonGroup = new GameObject("MainMenuButtonsGroup");
            buttonGroup.transform.SetParent(mainMenuPanel.transform);
            
            // Reset transform
            RectTransform rectTransform = buttonGroup.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            Debug.Log("Created new MainMenuButtonsGroup");
        }
        
        // Cari button-button dan pindahkan ke ButtonGroup
        string[] buttonNames = { "PlayButton", "Play Button", "StartButton", "Start Button",
                                "SettingsButton", "Settings Button", "OptionsButton", "Options Button",
                                "CreditsButton", "Credits Button", "CreditButton", "Credit Button",
                                "ExitButton", "Exit Button", "QuitButton", "Quit Button" };
        
        int movedButtons = 0;
        
        foreach (string buttonName in buttonNames)
        {
            Transform button = FindButtonRecursive(mainMenuPanel.transform, buttonName);
            if (button != null && button.parent != buttonGroup.transform)
            {
                button.SetParent(buttonGroup.transform, true);
                Debug.Log($"Moved {button.name} to ButtonGroup");
                movedButtons++;
            }
        }
        
        // Cari MainMenuManager dan assign reference
        MainMenuManager menuManager = FindObjectOfType<MainMenuManager>();
        if (menuManager != null)
        {
            SerializedObject serializedObject = new SerializedObject(menuManager);
            SerializedProperty buttonGroupProperty = serializedObject.FindProperty("mainMenuButtonsGroup");
            
            if (buttonGroupProperty != null)
            {
                buttonGroupProperty.objectReferenceValue = buttonGroup;
                serializedObject.ApplyModifiedProperties();
                Debug.Log("Assigned MainMenuButtonsGroup to MainMenuManager");
            }
        }
        
        EditorUtility.DisplayDialog("Setup Complete", 
            $"Structure setup completed!\n" +
            $"Moved {movedButtons} buttons to ButtonGroup.\n" +
            $"Please verify the setup in the Inspector.", 
            "OK");
    }
    
    void CreateButtonGroupManually()
    {
        if (mainMenuPanel == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign Main Menu Panel first!", "OK");
            return;
        }
        
        // Buat ButtonGroup
        GameObject buttonGroup = new GameObject("MainMenuButtonsGroup");
        buttonGroup.transform.SetParent(mainMenuPanel.transform);
        
        // Setup RectTransform untuk full screen
        RectTransform rectTransform = buttonGroup.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        // Select the created object
        Selection.activeGameObject = buttonGroup;
        
        EditorUtility.DisplayDialog("Button Group Created", 
            "MainMenuButtonsGroup has been created!\n" +
            "Now manually drag your buttons (Play, Settings, Credits, Exit) into this group.\n" +
            "Then assign this group to MainMenuManager script.", 
            "OK");
    }
    
    Transform FindButtonRecursive(Transform parent, string name)
    {
        // Cari dengan nama exact
        Transform found = parent.Find(name);
        if (found != null) return found;
        
        // Cari di semua children dengan case insensitive
        foreach (Transform child in parent)
        {
            if (child.name.ToLower().Contains(name.ToLower().Replace(" ", "").Replace("button", "")))
            {
                // Pastikan ini adalah button
                if (child.GetComponent<Button>() != null)
                    return child;
            }
            
            // Recursive search
            found = FindButtonRecursive(child, name);
            if (found != null) return found;
        }
        
        return null;
    }
}