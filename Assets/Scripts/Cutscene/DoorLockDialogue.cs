using UnityEngine;
using TMPro;
using System.Collections;

public class DoorLockDialogue : MonoBehaviour
{
    public TextMeshProUGUI subtitleText;
    public float displayTime = 3f;
    public GameObject bathroomDoor;
    
    private bool dialoguePlayed = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (dialoguePlayed) return;
        if (!other.CompareTag("Player")) return;
        
        dialoguePlayed = true;
        
        ShowSubtitle("It seems that it is locked. There might be a key somewhere.");
        
        // Unlock bathroom door
        if (bathroomDoor != null)
        {
            // Enable the bathroom door collider or trigger
            Collider doorCollider = bathroomDoor.GetComponent<Collider>();
            if (doorCollider != null) doorCollider.enabled = true;
        }
    }
    
    void ShowSubtitle(string text)
    {
        if (subtitleText != null)
        {
            subtitleText.text = text;
            subtitleText.gameObject.SetActive(true);
            StartCoroutine(HideSubtitle());
        }
    }
    
    IEnumerator HideSubtitle()
    {
        yield return new WaitForSeconds(displayTime);
        if (subtitleText != null)
            subtitleText.gameObject.SetActive(false);
    }
}