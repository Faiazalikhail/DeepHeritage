using System.Collections; // <--- Needed for Coroutines
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public LayerMask groundLayer;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float groundCheckRadius = 0.2f;

    [Header("Animation Settings")]
    public float floatGravity = 0.5f;
    private float defaultGravity;

    private Rigidbody2D _rb;
    private Collider2D _collider;
    private SpriteRenderer _sr;
    private Animator _animator;
    private GroundCheck _groundCheck;

    private bool _isGrounded = false;
    private bool _isAttacking = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _groundCheck = new GroundCheck(_collider, _rb, groundCheckRadius, groundLayer);

        defaultGravity = _rb.gravityScale;
        floatGravity = defaultGravity / 3f;
    }

    void Update()
    {
        _isGrounded = _groundCheck.IsGrounded();

        //inputs
        float horizontalInput = Input.GetAxis("Horizontal");
        bool jumpInput = Input.GetButtonDown("Jump");
        bool crouchPressed = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);

        // 1. Modified Attack Logic
        if (Input.GetKeyDown(KeyCode.LeftControl) && !_isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }

        bool floatInput = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);

        SpriteFlip(horizontalInput);

        //movement
        Vector2 velocity = _rb.linearVelocity;

        if (_isAttacking)
        {
            velocity.x = 0;
        }
        else
        {
            velocity.x = horizontalInput * moveSpeed;
        }

        if (crouchPressed && _isGrounded) velocity.x = 0;

        _rb.linearVelocity = velocity;

        if (jumpInput && _isGrounded)
        {
            _rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }

        if (!_isGrounded && floatInput)
        {
            _rb.gravityScale = floatGravity;
            _animator.SetBool("Floating", true);
        }
        else
        {
            _rb.gravityScale = defaultGravity;
            _animator.SetBool("Floating", false);
        }

        _animator.SetBool("isGrounded", _isGrounded);
        _animator.SetFloat("moveInput", Mathf.Abs(horizontalInput));
        _animator.SetFloat("yVel", _rb.linearVelocity.y);
        _animator.SetBool("isCrouching", crouchPressed);
    }

    private void SpriteFlip(float horizontalInput)
    {
        if (horizontalInput != 0)
            _sr.flipX = (horizontalInput < 0);
    }

    // 2. NEW: The Logic for waiting
    IEnumerator AttackRoutine()
    {
        _isAttacking = true;
        _animator.SetTrigger("Attack");

        if (_isGrounded)
        {
            // If on ground, wait 1 second
            yield return new WaitForSeconds(1f);
        }
        else
        {
            // If in air, wait until we hit the ground
            yield return new WaitUntil(() => _isGrounded);
        }

        _isAttacking = false;
    }

    // 3. Keep this empty so the Animation Event doesn't crash the game
    public void FinishAttack()
    {
        // Do nothing! The Coroutine handles the timing now.
    }
}