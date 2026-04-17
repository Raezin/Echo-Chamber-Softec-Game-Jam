using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerGravityController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    
    [Header("Mouse")]
    public float mouseSensitivity = 2f;
    
    [Header("Gravity")]
    public bool startGravityFlipped = true;
    
    private CharacterController controller;
    private Camera playerCamera;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;
    private Quaternion originalPlayerRotation;
    private Quaternion originalCameraRotation;
    
    // Current up direction (changes when gravity flips)
    private Vector3 currentUp = Vector3.up;
    private Vector3 currentGravity = Vector3.down * 9.81f;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        
        Cursor.lockState = CursorLockMode.Locked;
        
        originalPlayerRotation = transform.rotation;
        originalCameraRotation = playerCamera.transform.localRotation;
        
        if (startGravityFlipped)
        {
            FlipGravity(true);
        }
    }
    
    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }
    
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Rotate player around world up axis (Yaw)
        transform.Rotate(Vector3.up * mouseX, Space.World);
        
        // Vertical look (Pitch) - relative to current orientation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        // Apply camera rotation based on current gravity direction
        if (currentUp == Vector3.down) // Upside down
        {
            // When upside down, camera needs to be rotated 180° around forward axis
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 180f);
        }
        else
        {
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
    
    void HandleMovement()
    {
        // Get input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        // Sprint
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        
        // Calculate movement relative to player's current orientation
        // Use transform.right/forward which now respect the flipped orientation
        Vector3 move = transform.right * x + transform.forward * z;
        
        // Apply movement
        controller.Move(move * currentSpeed * Time.deltaTime);
        
        // Apply gravity in the current gravity direction
        velocity += currentGravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        // Ground check (sphere cast in current down direction)
        Vector3 downDirection = -currentUp;
        isGrounded = Physics.CheckSphere(transform.position - downDirection * 0.2f, 0.2f, ~0, QueryTriggerInteraction.Ignore);
        
        if (isGrounded && velocity.magnitude < 0.1f)
        {
            velocity = Vector3.zero;
        }
    }
    
    public void FlipGravity(bool flipped)
    {
        if (flipped)
        {
            // Upside down: up becomes down
            currentUp = Vector3.down;
            currentGravity = Vector3.up * 9.81f; // Pulls player upward (toward ceiling)
            
            // Rotate player 180 degrees around X axis to flip orientation
            transform.rotation = Quaternion.Euler(180f, transform.eulerAngles.y, 0f);
        }
        else
        {
            // Normal: up is up
            currentUp = Vector3.up;
            currentGravity = Vector3.down * 9.81f;
            
            // Reset rotation
            transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        }
        
        // Update gravity manager if exists
        if (GravityFlipManager.Instance != null)
        {
            GravityFlipManager.Instance.isGravityFlipped = flipped;
            GravityFlipManager.Instance.gravityDirection = flipped ? Vector3.up : Vector3.down;
        }
        
        Debug.Log(flipped ? "Gravity flipped! Walking on ceiling." : "Gravity normal.");
    }
    
    void OnDrawGizmos()
    {
        // Visualize current up direction
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, currentUp * 1.5f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -currentUp * 1.5f);
    }
}