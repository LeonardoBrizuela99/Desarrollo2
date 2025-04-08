using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class JumpController : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float minJumpForce = 5f;
    [SerializeField] private float maxJumpForce = 15f;
    [SerializeField] private float chargeSpeed = 20f;

    private Rigidbody rb;
    private bool isCharging = false;
    private float currentJumpForce;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        inputReader.jumpStarted += StartCharging;
        inputReader.jumpCanceled += Jump;
    }

    private void OnDisable()
    {
        inputReader.jumpStarted -= StartCharging;
        inputReader.jumpCanceled -= Jump;
    }

    private void Update()
    {
        if (isCharging)
        {
            currentJumpForce += chargeSpeed * Time.deltaTime;
            currentJumpForce = Mathf.Clamp(currentJumpForce, minJumpForce, maxJumpForce);
        }
    }

    private void StartCharging()
    {
        isCharging = true;
        currentJumpForce = minJumpForce;
    }

    private void Jump()
    {
        if (!isCharging) return;

        rb.AddForce(Vector3.up * currentJumpForce, ForceMode.Impulse);
        isCharging = false;
        currentJumpForce = 0f;
    }
}
