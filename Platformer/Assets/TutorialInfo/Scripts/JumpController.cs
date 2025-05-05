using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class JumpController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;

    [Header("Jump Force Settings")]
    [SerializeField] private float minJumpForce = 5f;
    [SerializeField] private float maxFirstJumpForce = 12f;
    [SerializeField] private float maxSecondJumpForce = 8f;
    [SerializeField] private float chargeSpeed = 15f;

    [Header("Jump Limits & Ground Check")]
    [SerializeField] private int maxJumpCount = 2;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDistance = 0.1f;

    private Rigidbody rb;
    private bool isCharging;
    private float currentJumpForce;
    private int jumpCount;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        inputReader.jumpStarted += StartCharging;
        inputReader.jumpCanceled += PerformJump;
    }

    private void OnDisable()
    {
        inputReader.jumpStarted -= StartCharging;
        inputReader.jumpCanceled -= PerformJump;
    }

    private void Update()
    {
        // Charge jump height if button held and jumps remaining
        if (isCharging && jumpCount < maxJumpCount)
        {
            currentJumpForce += chargeSpeed * Time.deltaTime;
            float maxForce = (jumpCount == 0) ? maxFirstJumpForce : maxSecondJumpForce;
            currentJumpForce = Mathf.Clamp(currentJumpForce, minJumpForce, maxForce);
        }
    }

    private void StartCharging()
    {
        // Only start charging if under jump limit
        if (jumpCount < maxJumpCount)
        {
            isCharging = true;
            currentJumpForce = minJumpForce;
        }
    }

    private void PerformJump()
    {
        // Apply jump if we were charging and have jumps left
        if (isCharging && jumpCount < maxJumpCount)
        {
            // Determine force cap based on jump count
            float appliedForce = Mathf.Clamp(currentJumpForce, minJumpForce, (jumpCount == 0) ? maxFirstJumpForce : maxSecondJumpForce);
            rb.AddForce(Vector3.up * appliedForce, ForceMode.Impulse);
            jumpCount++;
        }
        isCharging = false;
        currentJumpForce = 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Reset jump count when colliding with ground
        if (((1 << collision.gameObject.layer) & groundMask) != 0)
        {
            jumpCount = 0;
        }
    }
}
