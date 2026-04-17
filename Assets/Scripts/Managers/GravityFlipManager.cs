using UnityEngine;
using System.Collections;

public class GravityFlipManager : MonoBehaviour
{
    public static GravityFlipManager Instance;
    
    [Header("Gravity Settings")]
    public Vector3 gravityDirection = Vector3.down; // Normal gravity
    public bool isGravityFlipped = false;
    
    [Header("References")]
    public Transform playerCamera;
    public CharacterController playerController;
    
    [Header("Effects")]
    public float flipDuration = 0.5f;
    public AudioClip flipSound;
    
    private Vector3 normalGravity = Vector3.down * 9.81f;
    private Vector3 flippedGravity = Vector3.up * 9.81f;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Start()
    {
        // Start with flipped gravity (upside down)
        FlipGravity(true);
    }
    
    void Update()
    {
        // Apply custom gravity to all rigidbodies
        ApplyGravityToRigidbodies();
    }
    
    void ApplyGravityToRigidbodies()
    {
        // Find all rigidbodies in scene and apply custom gravity
        Rigidbody[] rigidbodies = FindObjectsOfType<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            if (!rb.isKinematic)
            {
                rb.useGravity = false;
                rb.AddForce(gravityDirection * 9.81f, ForceMode.Acceleration);
            }
        }
    }
    
    public void FlipGravity(bool flipped)
    {
        isGravityFlipped = flipped;
        
        if (flipped)
        {
            gravityDirection = Vector3.up;
            Physics.gravity = flippedGravity;
        }
        else
        {
            gravityDirection = Vector3.down;
            Physics.gravity = normalGravity;
        }
        
        // Flip camera
        if (playerCamera != null)
        {
            StartCoroutine(SmoothCameraFlip(flipped));
        }
        
        // Play sound
        if (flipSound != null)
            AudioManager.Instance?.PlaySFX(flipSound);
    }
    
    IEnumerator SmoothCameraFlip(bool flipped)
    {
        float elapsed = 0f;
        Quaternion startRot = playerCamera.localRotation;
        Quaternion endRot = flipped ? Quaternion.Euler(180, 0, 0) : Quaternion.Euler(0, 0, 0);
        
        while (elapsed < flipDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flipDuration;
            playerCamera.localRotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }
        playerCamera.localRotation = endRot;
    }
    
    // For player movement inversion (if using custom movement)
    public Vector3 GetMovementDirection(Vector3 input)
    {
        if (isGravityFlipped)
        {
            // Invert Y movement when gravity is flipped
            return new Vector3(input.x, -input.y, input.z);
        }
        return input;
    }
}