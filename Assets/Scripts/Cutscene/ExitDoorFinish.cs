using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ExitDoorFinish : MonoBehaviour
{
    [Header("UI References")]
    public GameObject finishPanel;
    public TextMeshProUGUI titleText;
    public Image fadeImage;
    
    [Header("Audio")]
    public AudioClip finishSound;
    
    [Header("Settings")]
    public float fadeDuration = 2f;
    public float displayTime = 3f;
    
    private bool finished = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (finished) return;
        if (!other.CompareTag("Player")) return;
        
        finished = true;
        StartCoroutine(FinishGame());
    }
    
    IEnumerator FinishGame()
    {
        // Disable player
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null) player.enabled = false;
        
        // Disable mouse look script if exists
        MonoBehaviour mouseLook = FindObjectOfType<MonoBehaviour>() as MonoBehaviour;
        
        // Play sound
        if (finishSound != null)
            AudioManager.Instance?.PlaySFX(finishSound);
        
        // Turn off flashlight (find by tag or component)
        Light flashlight = GameObject.FindGameObjectWithTag("Flashlight")?.GetComponent<Light>();
        if (flashlight == null)
            flashlight = FindObjectOfType<Light>();
        if (flashlight != null) flashlight.enabled = false;
        
        // Fade to black
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            float elapsed = 0f;
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;
            
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                fadeImage.color = new Color(color.r, color.g, color.b, alpha);
                yield return null;
            }
        }
        
        // Show finish panel with title
        if (finishPanel != null)
        {
            finishPanel.SetActive(true);
            
            if (titleText == null)
                titleText = finishPanel.GetComponentInChildren<TextMeshProUGUI>();
            
            if (titleText != null)
                titleText.text = "YOU ESCAPED";
        }
        
        // Wait before loading main menu
        yield return new WaitForSeconds(displayTime);
        
        // Load main menu
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}