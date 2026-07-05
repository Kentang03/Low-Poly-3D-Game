using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuestCutsceneIntegration : MonoBehaviour
{
    [System.Serializable]
    public class QuestCutsceneData
    {
        [Header("Quest Event")]
        public QuestEventType eventType;
        public int questItemIndex = -1; // -1 for any item, specific index for specific item
        
        [Header("Cutscene")]
        public string cutsceneName;
        public float delay = 0f;
        public bool skipIfAlreadyPlayed = true;
        
        [Header("Conditions")]
        public bool requirePlayerInRange = false;
        public float requiredDistance = 10f;
        public Transform referencePoint;
        
        private bool hasPlayed = false;
        
        public bool HasPlayed => hasPlayed;
        public void MarkAsPlayed() => hasPlayed = true;
        public void ResetPlayed() => hasPlayed = false;
    }

    public enum QuestEventType
    {
        QuestStarted,
        ItemActivated,
        ItemCollected,
        QuestCompleted
    }

    [Header("Integration Settings")]
    public QuestCutsceneData[] questCutscenes;
    
    [Header("References")]
    public QuestSystem questSystem;
    public CutsceneManager cutsceneManager;
    public Transform player;
    
    [Header("Debug")]
    public bool debugMode = true;
    
    // Private tracking fields
    private int lastActivatedItemIndex = -1;
    private int lastCollectedItemIndex = -1;

    private void Start()
    {
        // Find references if not assigned
        if (questSystem == null)
            questSystem = FindObjectOfType<QuestSystem>();
        
        if (cutsceneManager == null)
            cutsceneManager = CutsceneManager.Instance;
            
        if (player == null)
        {
            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
                player = playerGO.transform;
        }

        // Subscribe to quest events
        if (questSystem != null)
        {
            questSystem.OnQuestStarted.AddListener(OnQuestStarted);
            questSystem.OnQuestItemActivated.AddListener(OnQuestItemActivatedHandler);
            questSystem.OnQuestItemCollected.AddListener(OnQuestItemCollectedHandler);
            questSystem.OnQuestCompleted.AddListener(OnQuestCompleted);
        }
        else
        {
            Debug.LogError("QuestSystem not found! Make sure it exists in the scene.");
        }

        if (cutsceneManager == null)
        {
            Debug.LogError("CutsceneManager not found! Make sure it exists in the scene.");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (questSystem != null)
        {
            questSystem.OnQuestStarted.RemoveListener(OnQuestStarted);
            questSystem.OnQuestItemActivated.RemoveListener(OnQuestItemActivatedHandler);
            questSystem.OnQuestItemCollected.RemoveListener(OnQuestItemCollectedHandler);
            questSystem.OnQuestCompleted.RemoveListener(OnQuestCompleted);
        }
    }

    #region Quest Event Handlers

    private void OnQuestStarted()
    {
        if (debugMode)
            Debug.Log("[QuestCutsceneIntegration] Quest Started");
            
        TriggerCutscenesForEvent(QuestEventType.QuestStarted, -1);
    }

    private void OnQuestItemActivatedHandler()
    {
        // Get current item index from QuestSystem
        int itemIndex = GetCurrentQuestItemIndex();
        lastActivatedItemIndex = itemIndex;
        
        if (debugMode)
            Debug.Log($"[QuestCutsceneIntegration] Item {itemIndex} Activated");
            
        TriggerCutscenesForEvent(QuestEventType.ItemActivated, itemIndex);
    }

    private void OnQuestItemCollectedHandler()
    {
        // Use the last activated item as the collected item
        int itemIndex = lastActivatedItemIndex;
        lastCollectedItemIndex = itemIndex;
        
        if (debugMode)
            Debug.Log($"[QuestCutsceneIntegration] Item {itemIndex} Collected");
            
        TriggerCutscenesForEvent(QuestEventType.ItemCollected, itemIndex);
    }

    private void OnQuestCompleted()
    {
        if (debugMode)
            Debug.Log("[QuestCutsceneIntegration] Quest Completed");
            
        TriggerCutscenesForEvent(QuestEventType.QuestCompleted, -1);
    }

    // Legacy methods for backward compatibility
    private void OnQuestItemActivated(int itemIndex)
    {
        OnQuestItemActivatedHandler();
    }

    private void OnQuestItemCollected(int itemIndex)
    {
        OnQuestItemCollectedHandler();
    }

    private int GetCurrentQuestItemIndex()
    {
        if (questSystem == null) return -1;
        
        // Get current quest progress
        float progress = questSystem.GetQuestProgress();
        int totalItems = questSystem.questItems.Count;
        
        // Calculate current item index based on progress
        int currentIndex = Mathf.RoundToInt(progress * totalItems);
        return Mathf.Clamp(currentIndex, 0, totalItems - 1);
    }

    #endregion

    private void TriggerCutscenesForEvent(QuestEventType eventType, int itemIndex)
    {
        foreach (var cutsceneData in questCutscenes)
        {
            if (ShouldTriggerCutscene(cutsceneData, eventType, itemIndex))
            {
                StartCoroutine(PlayCutsceneWithDelay(cutsceneData));
            }
        }
    }

    private bool ShouldTriggerCutscene(QuestCutsceneData data, QuestEventType eventType, int itemIndex)
    {
        // Check event type
        if (data.eventType != eventType)
            return false;

        // Check if already played and should skip
        if (data.skipIfAlreadyPlayed && data.HasPlayed)
            return false;

        // Check item index (if specific item required)
        if (data.questItemIndex >= 0 && data.questItemIndex != itemIndex)
            return false;

        // Check player distance (if required)
        if (data.requirePlayerInRange)
        {
            if (player == null || data.referencePoint == null)
                return false;

            float distance = Vector3.Distance(player.position, data.referencePoint.position);
            if (distance > data.requiredDistance)
                return false;
        }

        return true;
    }

    private System.Collections.IEnumerator PlayCutsceneWithDelay(QuestCutsceneData data)
    {
        if (data.delay > 0f)
        {
            yield return new WaitForSeconds(data.delay);
        }

        if (cutsceneManager != null)
        {
            if (debugMode)
                Debug.Log($"[QuestCutsceneIntegration] Playing cutscene: {data.cutsceneName}");

            cutsceneManager.PlayCutscene(data.cutsceneName, () =>
            {
                data.MarkAsPlayed();
                OnCutsceneCompleted(data);
            });
        }
        else
        {
            Debug.LogError("CutsceneManager is null!");
        }
    }

    private void OnCutsceneCompleted(QuestCutsceneData data)
    {
        if (debugMode)
            Debug.Log($"[QuestCutsceneIntegration] Cutscene completed: {data.cutsceneName}");
    }

    #region Public API

    /// <summary>
    /// Play a cutscene manually by name
    /// </summary>
    public void PlayCutscene(string cutsceneName)
    {
        if (cutsceneManager != null)
        {
            cutsceneManager.PlayCutscene(cutsceneName);
        }
    }

    /// <summary>
    /// Reset all cutscene played states
    /// </summary>
    public void ResetAllCutscenes()
    {
        foreach (var data in questCutscenes)
        {
            data.ResetPlayed();
        }
        
        if (debugMode)
            Debug.Log("[QuestCutsceneIntegration] All cutscenes reset");
    }

    /// <summary>
    /// Check if a specific cutscene has been played
    /// </summary>
    public bool HasCutscenePlayed(string cutsceneName)
    {
        foreach (var data in questCutscenes)
        {
            if (data.cutsceneName == cutsceneName)
                return data.HasPlayed;
        }
        return false;
    }

    /// <summary>
    /// Force trigger cutscene for specific event
    /// </summary>
    public void ForceTriggerCutscene(QuestEventType eventType, int itemIndex = -1)
    {
        TriggerCutscenesForEvent(eventType, itemIndex);
    }

    #endregion

    #region Context Menu (Editor Only)

    [ContextMenu("Test - Quest Started Cutscenes")]
    private void TestQuestStartedCutscenes()
    {
        TriggerCutscenesForEvent(QuestEventType.QuestStarted, -1);
    }

    [ContextMenu("Test - Quest Completed Cutscenes")]
    private void TestQuestCompletedCutscenes()
    {
        TriggerCutscenesForEvent(QuestEventType.QuestCompleted, -1);
    }

    [ContextMenu("Reset All Cutscene States")]
    private void ResetAllCutsceneStates()
    {
        ResetAllCutscenes();
    }

    #endregion

    #region Debug Visualization

    private void OnDrawGizmosSelected()
    {
        if (questCutscenes == null) return;

        foreach (var data in questCutscenes)
        {
            if (data.requirePlayerInRange && data.referencePoint != null)
            {
                Gizmos.color = data.HasPlayed ? Color.green : Color.yellow;
                Gizmos.DrawWireSphere(data.referencePoint.position, data.requiredDistance);
                
                // Draw label
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(
                    data.referencePoint.position + Vector3.up * 2f,
                    $"Cutscene: {data.cutsceneName}\nEvent: {data.eventType}"
                );
                #endif
            }
        }
    }

    #endregion
}

// Extension untuk easy setup
public static class QuestCutsceneExtensions
{
    public static QuestCutsceneIntegration.QuestCutsceneData CreateQuestCutscene(
        QuestCutsceneIntegration.QuestEventType eventType,
        string cutsceneName,
        int itemIndex = -1,
        float delay = 0f)
    {
        return new QuestCutsceneIntegration.QuestCutsceneData
        {
            eventType = eventType,
            cutsceneName = cutsceneName,
            questItemIndex = itemIndex,
            delay = delay
        };
    }
}