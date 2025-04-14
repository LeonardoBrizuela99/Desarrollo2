using System;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    public float maxSpeed = 10;
    public bool Grounded { get; set; }

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    private Animator animator;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;
    
    private float _animationBlend;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        AssignAnimationIDs();
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void Update()
    {
        ValidateIsGrounded();
        AnimateWalk();
    }

    private void AnimateWalk()
    {
        var linearVelocity = GetComponent<Rigidbody>().linearVelocity;
        linearVelocity.y = 0;
        _animationBlend = Mathf.Lerp(_animationBlend, linearVelocity.magnitude, Time.deltaTime);
        if (_animationBlend < 0.01f) _animationBlend = 0f;
        animator.SetFloat(_animIDSpeed, _animationBlend);
        animator.SetFloat(_animIDMotionSpeed, 1);
    }

    private void ValidateIsGrounded()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        animator?.SetBool(_animIDGrounded, Grounded);
    }
}
