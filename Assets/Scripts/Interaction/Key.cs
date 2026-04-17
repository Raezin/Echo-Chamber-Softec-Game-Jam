using UnityEngine;

public class Key : Interactable
{
    [Header("Key Settings")]
    public AudioClip pickupSound;
    public GameObject keyMesh;
    public Light keyGlow;
    
    [Header("Target Door")]
    public Door targetDoor; // Drag the exit door here
    
    private bool isCollected = false;
    
    void Start()
    {
        // Ensure key has a trigger collider
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;
        
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }
    
    // For raycast interaction (if using Interactable base class)
    public override void Interact()
    {
        if (isCollected) return;
        Collect();
    }
    
    void Collect()
    {
        isCollected = true;
        
        // Play sound
        if (pickupSound != null)
            AudioManager.Instance?.PlaySFX(pickupSound);
        
        // Show message
        UIManager.Instance?.ShowTempMessage("Key collected!");
        
        // Disable visuals
        if (keyMesh != null)
            keyMesh.SetActive(false);
        
        if (keyGlow != null)
            keyGlow.enabled = false;
        
        // ----- UNLOCK THE TARGET DOOR -----
        if (targetDoor != null)
        {
            targetDoor.requiresKey = false;
            Debug.Log("Key: Door unlocked!");
            UIManager.Instance?.ShowTempMessage("Exit door is now unlocked!");
        }
        else
        {
            Debug.LogWarning("Key: No target door assigned!");
        }
        
        // Destroy the key object
        Destroy(gameObject, destroyDelay);
    }
}