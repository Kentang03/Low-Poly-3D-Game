using UnityEngine;

/// <summary>
/// Contoh implementasi berbagai event yang bisa digunakan dengan Quest System
/// Attach script ini ke GameObject dan assign method-method ini ke UnityEvents di QuestSystem
/// </summary>
public class QuestEventExamples : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip questStartSound;
    public AudioClip itemActivatedSound;
    public AudioClip itemCollectedSound;
    public AudioClip questCompleteSound;
    public AudioSource audioSource;
    
    [Header("Visual Effects")]
    public ParticleSystem questStartEffect;
    public ParticleSystem itemActivatedEffect;
    public ParticleSystem itemCollectedEffect;
    public ParticleSystem questCompleteEffect;
    
    [Header("UI Elements")]
    public GameObject questStartPanel;
    public GameObject questCompletePanel;
    public GameObject hintPanel;
    
    [Header("Game Objects")]
    public GameObject[] objectsToActivate;
    public GameObject[] objectsToDeactivate;
    public Light[] lightsToToggle;
    public Animator[] animatorsToTrigger;
    
    [Header("Scene Management")]
    public string nextSceneName = "";
    public float sceneTransitionDelay = 3f;
    
    void Start()
    {
        // Setup audio source jika tidak ada
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    #region Quest Events
    
    /// <summary>
    /// Event ketika quest dimulai - assign ke OnQuestStarted
    /// </summary>
    public void OnQuestStarted()
    {
        Debug.Log("🎯 Quest Started!");
        
        // Play sound
        PlaySound(questStartSound);
        
        // Show visual effect
        PlayEffect(questStartEffect);
        
        // Show start panel
        ShowPanel(questStartPanel, 3f);
        
        // Activate initial objects
        ActivateObjects(objectsToActivate);
        
        // Trigger welcome message
        ShowMessage("Quest dimulai! Cari item pertama.");
    }
    
    /// <summary>
    /// Event ketika item quest menjadi aktif - assign ke OnQuestItemActivated
    /// </summary>
    public void OnQuestItemActivated()
    {
        Debug.Log("✨ New Quest Item Activated!");
        
        // Play sound
        PlaySound(itemActivatedSound);
        
        // Show visual effect
        PlayEffect(itemActivatedEffect);
        
        // Show hint
        ShowHint();
        
        // Flash lights untuk memberikan hint
        FlashLights();
        
        // Get current item info
        if (QuestSystem.Instance != null)
        {
            QuestItem currentItem = QuestSystem.Instance.GetCurrentQuestItem();
            if (currentItem != null)
            {
                ShowMessage($"Item baru tersedia: {currentItem.itemName}");
            }
        }
    }
    
    /// <summary>
    /// Event ketika item quest dikumpulkan - assign ke OnQuestItemCollected
    /// </summary>
    public void OnQuestItemCollected()
    {
        Debug.Log("💎 Quest Item Collected!");
        
        // Play sound
        PlaySound(itemCollectedSound);
        
        // Show visual effect
        PlayEffect(itemCollectedEffect);
        
        // Trigger animators
        TriggerAnimators("ItemCollected");
        
        // Update game state
        UpdateGameState();
        
        // Show progress message
        if (QuestSystem.Instance != null)
        {
            int current = QuestSystem.Instance.currentQuestIndex;
            int total = QuestSystem.Instance.questItems.Count;
            ShowMessage($"Item dikumpulkan! Progress: {current}/{total}");
        }
    }
    
    /// <summary>
    /// Event ketika quest selesai - assign ke OnQuestCompleted
    /// </summary>
    public void OnQuestCompleted()
    {
        Debug.Log("🏆 Quest Completed!");
        
        // Play sound
        PlaySound(questCompleteSound);
        
        // Show visual effect
        PlayEffect(questCompleteEffect);
        
        // Show completion panel
        ShowPanel(questCompletePanel, 0f);
        
        // Trigger completion animations
        TriggerAnimators("QuestComplete");
        
        // Deactivate objects
        DeactivateObjects(objectsToDeactivate);
        
        // Turn on victory lights
        SetLightsState(true);
        
        // Show victory message
        ShowMessage("Quest selesai! Semua item telah dikumpulkan!");
        
        // Transition to next scene if specified
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            StartCoroutine(TransitionToNextScene());
        }
        
        // Stop game timer
        CompleteGameTimer();
    }
    
    /// <summary>
    /// Event ketika quest direset - assign ke OnQuestReset
    /// </summary>
    public void OnQuestReset()
    {
        Debug.Log("🔄 Quest Reset!");
        
        // Hide all panels
        HidePanel(questStartPanel);
        HidePanel(questCompletePanel);
        HidePanel(hintPanel);
        
        // Reset game objects
        DeactivateObjects(objectsToActivate);
        ActivateObjects(objectsToDeactivate);
        
        // Reset lights
        SetLightsState(false);
        
        // Reset animators
        ResetAnimators();
        
        ShowMessage("Quest telah direset!");
    }
    
    #endregion
    
    #region Item-Specific Events
    
    /// <summary>
    /// Event khusus untuk item pertama - assign ke quest item OnItemActivated
    /// </summary>
    public void OnFirstItemActivated()
    {
        Debug.Log("🔮 First Item (Crystal) Activated!");
        ShowMessage("Kristal kuno telah muncul! Cari cahaya biru di area temple.");
        
        // Special effect for first item
        if (lightsToToggle.Length > 0)
            lightsToToggle[0].color = Color.blue;
    }
    
    /// <summary>
    /// Event khusus untuk item kedua - assign ke quest item OnItemActivated
    /// </summary>
    public void OnSecondItemActivated()
    {
        Debug.Log("📜 Second Item (Scroll) Activated!");
        ShowMessage("Gulungan suci telah muncul! Periksa area perpustakaan.");
        
        // Special effect for second item
        if (lightsToToggle.Length > 1)
            lightsToToggle[1].color = Color.yellow;
    }
    
    /// <summary>
    /// Event khusus untuk item ketiga - assign ke quest item OnItemActivated
    /// </summary>
    public void OnThirdItemActivated()
    {
        Debug.Log("🗝️ Third Item (Key) Activated!");
        ShowMessage("Kunci emas telah muncul! Lihat di chamber terakhir.");
        
        if (lightsToToggle.Length > 2)
            lightsToToggle[2].color = Color.green;
    }
    
    /// <summary>
    /// Event ketika item pertama dikumpulkan
    /// </summary>
    public void OnFirstItemCollected()
    {
        Debug.Log("Crystal collected! Unlocking path to scroll...");
        
        // Unlock path to second area
        if (objectsToActivate.Length > 0)
        {
            objectsToActivate[0].SetActive(true); // Buka jalan ke area kedua
        }
        
        ShowMessage("Kristal dikumpulkan! Jalan menuju gulungan terbuka.");
    }
    
    /// <summary>
    /// Event ketika item kedua dikumpulkan
    /// </summary>
    public void OnSecondItemCollected()
    {
        Debug.Log("Scroll collected! Unlocking final chamber...");
        
        // Unlock final chamber
        if (objectsToActivate.Length > 1)
        {
            objectsToActivate[1].SetActive(true); // Buka chamber terakhir
        }
        
        ShowMessage("Gulungan dikumpulkan! Chamber terakhir terbuka.");
    }
    
    /// <summary>
    /// Event ketika item ketiga dikumpulkan
    /// </summary>
    public void OnThirdItemCollected()
    {
        Debug.Log("Key collected! Ready for final victory!");
        
        ShowMessage("Kunci emas dikumpulkan! Semua item telah terkumpul!");
    }
    
    #endregion
    
    #region Helper Methods
    
    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    void PlayEffect(ParticleSystem effect)
    {
        if (effect != null)
        {
            effect.Play();
        }
    }
    
    void ShowPanel(GameObject panel, float hideDelay = 0f)
    {
        if (panel != null)
        {
            panel.SetActive(true);
            
            if (hideDelay > 0f)
            {
                StartCoroutine(HidePanelAfterDelay(panel, hideDelay));
            }
        }
    }
    
    void HidePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }
    
    System.Collections.IEnumerator HidePanelAfterDelay(GameObject panel, float delay)
    {
        yield return new WaitForSeconds(delay);
        HidePanel(panel);
    }
    
    void ShowMessage(string message)
    {
        Debug.Log($"💬 {message}");
        
        // Bisa integrate dengan UI message system jika ada
        // MessageSystem.Instance?.ShowMessage(message);
    }
    
    void ShowHint()
    {
        if (hintPanel != null)
        {
            ShowPanel(hintPanel, 5f); // Show hint for 5 seconds
        }
    }
    
    void ActivateObjects(GameObject[] objects)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }
    
    void DeactivateObjects(GameObject[] objects)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
    
    void SetLightsState(bool state)
    {
        foreach (Light light in lightsToToggle)
        {
            if (light != null)
                light.enabled = state;
        }
    }
    
    void FlashLights()
    {
        StartCoroutine(FlashLightsCoroutine());
    }
    
    System.Collections.IEnumerator FlashLightsCoroutine()
    {
        for (int i = 0; i < 3; i++)
        {
            SetLightsState(true);
            yield return new WaitForSeconds(0.2f);
            SetLightsState(false);
            yield return new WaitForSeconds(0.2f);
        }
        SetLightsState(true);
    }
    
    void TriggerAnimators(string triggerName)
    {
        foreach (Animator animator in animatorsToTrigger)
        {
            if (animator != null)
                animator.SetTrigger(triggerName);
        }
    }
    
    void ResetAnimators()
    {
        foreach (Animator animator in animatorsToTrigger)
        {
            if (animator != null)
            {
                animator.Rebind();
                animator.Update(0f);
            }
        }
    }
    
    void UpdateGameState()
    {
        // Update any game state as needed
        // Example: Update score, unlock achievements, etc.
        
        if (QuestSystem.Instance != null)
        {
            float progress = QuestSystem.Instance.GetQuestProgress();
            Debug.Log($"Quest progress: {progress * 100:F1}%");
        }
    }
    
    void CompleteGameTimer()
    {
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.CompleteGame();
        }
    }
    
    System.Collections.IEnumerator TransitionToNextScene()
    {
        yield return new WaitForSeconds(sceneTransitionDelay);
        
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(nextSceneName);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
    }
    
    #endregion
    
    #region Debug Methods
    
    [ContextMenu("Test Quest Start Event")]
    public void TestQuestStartEvent()
    {
        OnQuestStarted();
    }
    
    [ContextMenu("Test Item Activated Event")]
    public void TestItemActivatedEvent()
    {
        OnQuestItemActivated();
    }
    
    [ContextMenu("Test Item Collected Event")]
    public void TestItemCollectedEvent()
    {
        OnQuestItemCollected();
    }
    
    [ContextMenu("Test Quest Complete Event")]
    public void TestQuestCompleteEvent()
    {
        OnQuestCompleted();
    }
    
    #endregion
}