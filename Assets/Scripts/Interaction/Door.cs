using UnityEngine;
using TMPro;
using System.Collections;

public class Door : Interactable
{
    [Header("Key Requirements")]
    public bool requiresKey = true;
    public string doorID; // For matching with specific key
    
    [Header("Sliding Settings")]
    public Vector3 slideDirection = Vector3.left;
    public float slideDistance = 2f;
    public float slideSpeed = 2f;
    
    [Header("Trigger Area")]
    public BoxCollider triggerArea;
    
    [Header("UI Messages")]
    public TextMeshProUGUI messageText;
    public float messageFadeDuration = 0.5f;
    public float messageDisplayTime = 2f;
    
    [Header("Audio")]
    public AudioClip doorOpenSound;
    public AudioClip doorLockedSound;
    
    [Header("References")]
    public Transform doorTransform;
    
    private bool isOpen = false;
    private bool isSliding = false;
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool playerInTrigger = false;
    private Coroutine activeMessageCoroutine;
    
    void Start()
    {
        if (doorTransform == null)
            doorTransform = transform;
        
        closedPosition = doorTransform.localPosition;
        openPosition = closedPosition + slideDirection.normalized * slideDistance;
        
        if (triggerArea != null)
        {
            triggerArea.isTrigger = true;
        }
        
        if (messageText != null)
        {
            messageText.alpha = 0f;
        }
    }
    
    void Update()
    {
        if (isSliding)
        {
            float step = slideSpeed * Time.deltaTime;
            doorTransform.localPosition = Vector3.MoveTowards(
                doorTransform.localPosition, 
                openPosition, 
                step
            );
            
            if (Vector3.Distance(doorTransform.localPosition, openPosition) < 0.01f)
            {
                isSliding = false;
            }
        }
        
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }
    
    public override void Interact()
    {
        TryInteract();
    }
    
    void TryInteract()
    {
        if (isOpen) return;
        
        // Check if this is the exit door and first interaction
        if (doorID == "Exit" && DoorManager.Instance != null && !DoorManager.Instance.hasInteractedWithExitDoor)
        {
            DoorManager.Instance.OnExitDoorFirstInteraction();
            
            if (doorLockedSound != null)
                AudioManager.Instance?.PlaySFX(doorLockedSound);
            ShowMessage("Door is locked", Color.red);
            return;
        }
        
        // Check if this is bathroom door
        if (doorID == "Bathroom" && DoorManager.Instance != null)
        {
            DoorManager.Instance.OnBathroomDoorInteraction();
            
            if (!DoorManager.Instance.hasInteractedWithExitDoor)
            {
                if (doorLockedSound != null)
                    AudioManager.Instance?.PlaySFX(doorLockedSound);
                ShowMessage("Locked", Color.red);
                return;
            }
        }
        
        // Normal key check
        if (requiresKey)
        {
            bool hasKey = DoorManager.Instance != null && DoorManager.Instance.hasKey;
            
            if (!hasKey)
            {
                if (doorLockedSound != null)
                    AudioManager.Instance?.PlaySFX(doorLockedSound);
                ShowMessage("Door is locked. Find the key.", Color.red);
                return;
            }
        }
        
        OpenDoor();
    }
    
    void OpenDoor()
    {
        isOpen = true;
        
        if (doorOpenSound != null)
            AudioManager.Instance?.PlaySFX(doorOpenSound);
        
        isSliding = true;
        GetComponent<Collider>().enabled = false;
        ShowMessage("Door opened!", Color.green);
        
        if (triggerArea != null)
            triggerArea.enabled = false;
    }
    
    void ShowMessage(string message, Color color)
    {
        if (messageText == null) return;
        
        if (activeMessageCoroutine != null)
            StopCoroutine(activeMessageCoroutine);
        
        activeMessageCoroutine = StartCoroutine(FadeMessage(message, color));
    }
    
    IEnumerator FadeMessage(string message, Color color)
    {
        messageText.text = message;
        messageText.color = color;
        
        float elapsed = 0f;
        while (elapsed < messageFadeDuration)
        {
            elapsed += Time.deltaTime;
            messageText.alpha = Mathf.Lerp(0f, 1f, elapsed / messageFadeDuration);
            yield return null;
        }
        messageText.alpha = 1f;
        
        yield return new WaitForSeconds(messageDisplayTime);
        
        elapsed = 0f;
        while (elapsed < messageFadeDuration)
        {
            elapsed += Time.deltaTime;
            messageText.alpha = Mathf.Lerp(1f, 0f, elapsed / messageFadeDuration);
            yield return null;
        }
        messageText.alpha = 0f;
        
        activeMessageCoroutine = null;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }
    
    // ===== THIS IS THE METHOD THAT WAS MISSING =====
    public void SetInteractable(bool interactable)
    {
        if (triggerArea != null)
            triggerArea.enabled = interactable;
        
        Collider doorCollider = GetComponent<Collider>();
        if (doorCollider != null)
            doorCollider.enabled = interactable;
        
        if (!interactable)
        {
            ShowMessage("Cannot enter yet.", Color.gray);
        }
    }
    // ================================================
    
    public void ResetDoor()
    {
        isOpen = false;
        isSliding = false;
        playerInTrigger = false;
        doorTransform.localPosition = closedPosition;
        GetComponent<Collider>().enabled = true;
        
        if (triggerArea != null)
            triggerArea.enabled = true;
    }
}