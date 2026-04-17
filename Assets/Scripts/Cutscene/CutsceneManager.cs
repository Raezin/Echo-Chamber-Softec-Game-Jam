using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;
    public Light flashlight;
    public TextMeshProUGUI subtitleText;
    
    [Header("Post Processing (for blur)")]
    public Volume postProcessVolume;
    private DepthOfField depthOfField;
    
    [Header("Camera Bob/Get Up")]
    public float getUpHeight = 0.6f;
    public float getUpDuration = 1.2f;
    
    [Header("Settings")]
    public float subtitleDisplayTime = 3f;
    
    [Header("Objects to Unlock")]
    public GameObject bathroomDoor;
    public BoxCollider doorTriggerArea;
    
    [Header("Gravity Flip")]
    public GravityFlipManager gravityManager;
    
    private Vector3 originalCameraPos;
    private bool cutsceneComplete = false;
    private CharacterController playerController;
    private MonoBehaviour playerMovementScript;
    
    void Start()
    {
        // Get references
        playerController = FindObjectOfType<CharacterController>();
        playerMovementScript = FindObjectOfType<PlayerController>() as MonoBehaviour;
        
        // Find gravity manager if not assigned
        if (gravityManager == null)
            gravityManager = FindObjectOfType<GravityFlipManager>();
        
        // Setup post processing blur
        SetupDepthOfField();
        
        // Start cutscene
        StartCoroutine(PlayCutscene());
    }
    
    void SetupDepthOfField()
    {
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGet(out depthOfField);
            if (depthOfField == null)
            {
                depthOfField = postProcessVolume.profile.Add<DepthOfField>(true);
            }
        }
    }
    
    IEnumerator PlayCutscene()
    {
        // Disable player control
        if (playerMovementScript != null) playerMovementScript.enabled = false;
        if (playerController != null) playerController.enabled = false;
        
        // Apply gravity flip immediately (player starts upside down)
        if (gravityManager != null)
        {
            gravityManager.FlipGravity(true);
        }
        
        // Store original camera position
        originalCameraPos = playerCamera.localPosition;
        
        // Start with extreme blur
        if (depthOfField != null)
        {
            depthOfField.active = true;
            depthOfField.focalLength.value = 1f;
        }
        
        // Show first subtitle
        ShowSubtitle("Where am I...");
        yield return new WaitForSeconds(1.5f);
        
        // Get up animation (camera moves up from ground)
        float elapsed = 0f;
        Vector3 startPos = new Vector3(originalCameraPos.x, -getUpHeight, originalCameraPos.z);
        Vector3 endPos = originalCameraPos;
        
        while (elapsed < getUpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / getUpDuration;
            float smoothT = 1f - Mathf.Pow(1f - t, 3f);
            playerCamera.localPosition = Vector3.Lerp(startPos, endPos, smoothT);
            yield return null;
        }
        playerCamera.localPosition = originalCameraPos;
        
        // Reduce blur (focusing eyes)
        if (depthOfField != null)
        {
            elapsed = 0f;
            float startBlur = 1f;
            float endBlur = 0.1f;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 1f;
                depthOfField.focalLength.value = Mathf.Lerp(startBlur, endBlur, t);
                yield return null;
            }
            depthOfField.active = false;
        }
        
        // Show second subtitle
        ShowSubtitle("Why is it so dark?");
        yield return new WaitForSeconds(subtitleDisplayTime);
        
        // Turn on flashlight with flicker
        if (flashlight != null)
        {
            flashlight.enabled = true;
            yield return new WaitForSeconds(0.05f);
            flashlight.enabled = false;
            yield return new WaitForSeconds(0.03f);
            flashlight.enabled = true;
        }
        
        // Show third subtitle (player realizes they're upside down)
        ShowSubtitle("What the... I'm on the ceiling?!");
        yield return new WaitForSeconds(subtitleDisplayTime);
        
        // Enable player control
        if (playerMovementScript != null) playerMovementScript.enabled = true;
        if (playerController != null) playerController.enabled = true;
        
        cutsceneComplete = true;
        
        // Unlock bathroom door and trigger area
        if (bathroomDoor != null)
        {
            Collider doorCol = bathroomDoor.GetComponent<Collider>();
            if (doorCol != null) doorCol.enabled = true;
        }
        if (doorTriggerArea != null)
        {
            doorTriggerArea.enabled = true;
        }
    }
    
    void ShowSubtitle(string text)
    {
        if (subtitleText != null)
        {
            subtitleText.text = text;
            subtitleText.gameObject.SetActive(true);
            StartCoroutine(HideSubtitleAfterDelay());
        }
    }
    
    IEnumerator HideSubtitleAfterDelay()
    {
        yield return new WaitForSeconds(subtitleDisplayTime);
        if (subtitleText != null)
        {
            subtitleText.gameObject.SetActive(false);
            subtitleText.text = "";
        }
    }
}