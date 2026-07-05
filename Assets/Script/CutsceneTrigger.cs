using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    public string cutsceneName;
    public bool triggerOnce = true;
    public bool triggerOnStart = false;
    public bool requirePlayerTag = true;
    public string playerTag = "Player";
    
    [Header("Trigger Conditions")]
    public KeyCode triggerKey = KeyCode.None;
    public bool showPrompt = true;
    public string promptText = "Press E to interact";
    
    [Header("UI")]
    public GameObject promptUI;
    
    private bool hasTriggered = false;
    private bool playerInRange = false;
    private CutsceneManager cutsceneManager;

    private void Start()
    {
        cutsceneManager = CutsceneManager.Instance;
        
        if (cutsceneManager == null)
        {
            Debug.LogError("CutsceneManager not found! Make sure it's in the scene.");
        }

        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }

        if (triggerOnStart && !hasTriggered)
        {
            TriggerCutscene();
        }
    }

    private void Update()
    {
        // Handle key input for manual trigger
        if (playerInRange && triggerKey != KeyCode.None && Input.GetKeyDown(triggerKey))
        {
            TriggerCutscene();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (requirePlayerTag && !other.CompareTag(playerTag))
            return;

        playerInRange = true;

        // Show prompt if manual trigger
        if (triggerKey != KeyCode.None && showPrompt)
        {
            ShowPrompt(true);
        }
        // Auto trigger if no key required
        else if (triggerKey == KeyCode.None)
        {
            TriggerCutscene();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (requirePlayerTag && !other.CompareTag(playerTag))
            return;

        playerInRange = false;
        ShowPrompt(false);
    }

    private void ShowPrompt(bool show)
    {
        if (promptUI != null)
        {
            promptUI.SetActive(show && !hasTriggered);
        }
        
        // You can also implement UI text updates here
        if (show && !hasTriggered)
        {
            // Example: Update prompt text
            var textComponent = promptUI?.GetComponentInChildren<UnityEngine.UI.Text>();
            if (textComponent != null)
            {
                textComponent.text = promptText.Replace("{key}", triggerKey.ToString());
            }
        }
    }

    public void TriggerCutscene()
    {
        if (hasTriggered && triggerOnce)
            return;

        if (cutsceneManager == null)
        {
            Debug.LogError("CutsceneManager not available!");
            return;
        }

        if (string.IsNullOrEmpty(cutsceneName))
        {
            Debug.LogError("Cutscene name not set!");
            return;
        }

        hasTriggered = true;
        ShowPrompt(false);

        // Play cutscene
        cutsceneManager.PlayCutscene(cutsceneName, OnCutsceneComplete);
    }

    private void OnCutsceneComplete()
    {
        Debug.Log($"Cutscene '{cutsceneName}' completed!");
        
        // You can add additional logic here
        // For example, enable/disable objects, trigger events, etc.
    }

    // Public method to manually trigger from other scripts
    public void ManualTrigger()
    {
        TriggerCutscene();
    }

    // Reset trigger (useful for testing or repeatable cutscenes)
    public void ResetTrigger()
    {
        hasTriggered = false;
    }

    private void OnDrawGizmos()
    {
        // Draw trigger area in scene view
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = hasTriggered ? Color.red : Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            
            if (col is BoxCollider box)
            {
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (col is SphereCollider sphere)
            {
                Gizmos.DrawWireSphere(sphere.center, sphere.radius);
            }
        }
    }
}