using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string interactionPrompt = "Press E to interact";
    public bool destroyOnInteract = false;
    public float destroyDelay = 0f;
    
    // This method will be overridden by specific interactable objects
    public abstract void Interact();
    
    // Optional: Show prompt when player looks at object
    void OnMouseOver()
    {
        // Show UI prompt (optional)
        UIManager.Instance?.ShowTempMessage(interactionPrompt);
    }
    
    void OnMouseExit()
    {
        // Hide prompt (optional)
    }
}