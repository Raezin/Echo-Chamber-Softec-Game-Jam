using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 5f;
    public float gravity = -20f;
    
    [Header("Mouse Settings")]
    public float mouseSensitivity = 2f;
    public bool invertY = false;
    
    [Header("Interaction")]
    public float interactRange = 3f;
    public LayerMask interactLayer = ~0; // Everything by default
    
    [Header("Camera")]
    public Camera playerCamera;
    public float cameraTiltOnMove = 0.5f;
    
    // Private components
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    
    // Mouse look
    private float xRotation = 0f;
    
    // Camera tilt (for movement feel)
    private float currentTilt = 0f;
    
    // Gravity flip support (for later)
    private bool isGravityFlipped = false;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // Get camera if not assigned
        if (playerCamera == null)
            playerCamera = Camera.main;
        
        // Lock cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleInteraction();
        HandleCameraTilt();
    }
    
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Apply invert Y if enabled
        if (invertY)
            mouseY = -mouseY;
        
        // Horizontal rotation (turning left/right)
        transform.Rotate(Vector3.up * mouseX);
        
        // Vertical rotation (looking up/down) - clamped to prevent over-rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    
    void HandleMovement()
    {
        // Check if player is on ground
        isGrounded = controller.isGrounded;
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep grounded
        }
        
        // Get input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        // Sprint (Left Shift)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        
        // Calculate movement direction relative to player rotation
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);
        
        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
        
        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    
    void HandleInteraction()
    {
        // Press E to interact
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Raycast from center of screen
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, interactRange, interactLayer))
            {
                // Check if object has an Interactable component
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                
                if (interactable != null)
                {
                    interactable.Interact();
                    Debug.Log("Interacted with: " + hit.collider.name);
                }
                else
                {
                    Debug.Log("Nothing to interact with: " + hit.collider.name);
                }
            }
        }
    }
    
    void HandleCameraTilt()
    {
        // Add slight camera tilt when moving left/right (optional feel)
        float tilt = Input.GetAxis("Horizontal") * cameraTiltOnMove;
        currentTilt = Mathf.Lerp(currentTilt, tilt, Time.deltaTime * 10f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, currentTilt);
    }
    
    // Called when player falls or needs reset
    public void ResetPosition(Vector3 newPosition)
    {
        controller.enabled = false;
        transform.position = newPosition;
        controller.enabled = true;
        velocity = Vector3.zero;
    }
    
    // For gravity flip (Loop 1)
    public void FlipGravity(bool flipped)
    {
        isGravityFlipped = flipped;
        
        if (flipped)
        {
            gravity = 20f; // Reverse gravity (pull up)
            // Rotate camera 180 degrees
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation + 180f, 0f, 0f);
        }
        else
        {
            gravity = -20f; // Normal gravity
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
}