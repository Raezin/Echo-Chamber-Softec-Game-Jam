using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityBox : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 3f;
    public float holdDistance = 2f;
    
    private Rigidbody rb;
    private bool isHolding = false;
    private Transform holdPoint;
    private Collider boxCollider;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<Collider>();
        
        // Create hold point at camera position
        GameObject hold = new GameObject("HoldPoint");
        hold.transform.SetParent(Camera.main.transform);
        hold.transform.localPosition = new Vector3(0, 0, holdDistance);
        holdPoint = hold.transform;
        
        // Make box reactive to gravity
        rb.useGravity = false; // We'll use custom gravity
    }
    
    void Update()
    {
        // Handle pickup/drop with E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isHolding)
            {
                TryPickUp();
            }
            else
            {
                Drop();
            }
        }
        
        // If holding, move box to hold point
        if (isHolding && holdPoint != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.MovePosition(holdPoint.position);
        }
    }
    
    void TryPickUp()
    {
        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        if (distance < pickupRange)
        {
            PickUp();
        }
    }
    
    void PickUp()
    {
        isHolding = true;
        rb.isKinematic = true;
        rb.useGravity = false;
        
        if (boxCollider != null)
            boxCollider.enabled = false;
        
        UIManager.Instance?.ShowTempMessage("Holding box. Press E to drop.");
    }
    
    void Drop()
    {
        isHolding = false;
        rb.isKinematic = false;
        rb.useGravity = false; // Still false because GravityFlipManager handles gravity
        rb.linearVelocity = Vector3.zero;
        
        if (boxCollider != null)
            boxCollider.enabled = true;
        
        UIManager.Instance?.ShowTempMessage("Box dropped.");
    }
}