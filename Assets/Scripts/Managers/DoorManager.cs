using UnityEngine;
using TMPro;
using System.Collections;

public class DoorManager : MonoBehaviour
{
    public static DoorManager Instance;
    
    [Header("Door References")]
    public Door exitDoor;
    public Door bathroomDoor;
    
    [Header("UI Messages")]
    public TextMeshProUGUI subtitleText;
    public float subtitleDisplayTime = 3f;
    
    [Header("State Tracking")]
    public bool hasInteractedWithExitDoor = false;
    public bool hasKey = false;
    
    private Coroutine activeSubtitle;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Start()
    {
        // Initially disable bathroom door interaction
        if (bathroomDoor != null)
        {
            bathroomDoor.requiresKey = true; // Locked
            bathroomDoor.SetInteractable(false);
        }
        
        // Exit door starts locked
        if (exitDoor != null)
        {
            exitDoor.requiresKey = true;
        }
    }
    
    public void OnExitDoorFirstInteraction()
    {
        if (!hasInteractedWithExitDoor)
        {
            hasInteractedWithExitDoor = true;
            ShowSubtitle("Door is locked. I need to find a way to open it.");
            
            // Unlock bathroom door so player can enter
            if (bathroomDoor != null)
            {
                bathroomDoor.SetInteractable(true);
                bathroomDoor.requiresKey = false;
            }
        }
        else
        {
            // After first interaction, show different message if no key
            if (!hasKey)
            {
                ShowSubtitle("Still locked. The key must be somewhere... maybe the bathroom?");
            }
            else
            {
                ShowSubtitle("I have the key! Now I can open it.");
            }
        }
    }
    
    public void OnBathroomDoorInteraction()
    {
        if (!hasInteractedWithExitDoor)
        {
            // Player hasn't tried exit door yet
            ShowSubtitle("This door leads to my bathroom. The exit door is the other way.");
        }
        else
        {
            // Player has already tried exit door, bathroom is now accessible
            ShowSubtitle("The key must be in here somewhere...");
        }
    }
    
    public void OnKeyCollected()
    {
        hasKey = true;
        
        if (exitDoor != null)
        {
            exitDoor.requiresKey = false;
            ShowSubtitle("Got the key! Now I can open the exit door.");
        }
    }
    
    void ShowSubtitle(string message)
    {
        if (subtitleText == null) return;
        
        if (activeSubtitle != null)
            StopCoroutine(activeSubtitle);
        
        activeSubtitle = StartCoroutine(DisplaySubtitle(message));
    }
    
    IEnumerator DisplaySubtitle(string message)
    {
        subtitleText.text = message;
        subtitleText.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(subtitleDisplayTime);
        
        subtitleText.gameObject.SetActive(false);
        subtitleText.text = "";
        activeSubtitle = null;
    }
}