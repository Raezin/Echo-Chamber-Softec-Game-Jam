using UnityEngine;
using TMPro;
using System.Collections;

public class KeyWithJumpscare : MonoBehaviour
{
    [Header("Jumpscare")]
    public GameObject jumpscareImage;
    public AudioClip jumpscareSound;
    public Light flashlight;
    
    [Header("Messages")]
    public TextMeshProUGUI subtitleText;
    
    [Header("Key Settings")]
    public Door targetDoor;
    
    private bool triggered = false;
    private bool keyCollected = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;
        
        triggered = true;
        StartCoroutine(JumpscareSequence());
    }
    
    IEnumerator JumpscareSequence()
    {
        // Flashlight flickers and turns off
        if (flashlight != null)
        {
            flashlight.enabled = false;
            yield return new WaitForSeconds(0.1f);
            flashlight.enabled = true;
            yield return new WaitForSeconds(0.05f);
            flashlight.enabled = false;
            yield return new WaitForSeconds(0.15f);
            flashlight.enabled = true;
            yield return new WaitForSeconds(0.05f);
            flashlight.enabled = false;
        }
        
        // Play sound
        if (jumpscareSound != null)
            AudioManager.Instance?.PlaySFX(jumpscareSound);
        
        // Show jumpscare image
        if (jumpscareImage != null)
        {
            jumpscareImage.SetActive(true);
            yield return new WaitForSeconds(0.6f);
            jumpscareImage.SetActive(false);
        }
        
        // Show subtitle
        if (subtitleText != null)
        {
            subtitleText.text = "What was that?!";
            subtitleText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f);
            subtitleText.gameObject.SetActive(false);
        }
        
        // Turn flashlight back on
        if (flashlight != null)
            flashlight.enabled = true;
        
        // Now key can be collected
        keyCollected = true;
        
        // Show pickup prompt (optional)
        UIManager.Instance?.ShowTempMessage("Key is here! Press E to pick up.");
    }
    
    void OnTriggerStay(Collider other)
    {
        if (!keyCollected) return;
        if (!other.CompareTag("Player")) return;
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Give key to door
            if (targetDoor != null)
                targetDoor.requiresKey = false;
            
            // Add memory fragment
            if (GameManager.Instance != null)
                GameManager.Instance.AddMemoryFragment();
            
            // Show success
            UIManager.Instance?.ShowTempMessage("Key collected!");
            
            // Destroy key
            Destroy(gameObject);
        }
    }
}