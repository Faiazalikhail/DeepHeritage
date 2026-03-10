using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public bool isDead = false;

    // --- MOVEMENT SETTINGS ---
    public LayerMask groundLayer;
    public float moveSpeed = 5f;
    public float jumpForce = 20f;
    public float groundCheckRadius = 0.2f;

    // --- AERIAL SETTINGS ---
    [Header("Animation Settings")]
    public float floatGravity = 0.5f;
    public float bombGravity = 10f;
    private float defaultGravity;

    // --- DAMAGE SETTINGS ---
    private bool isInvincible = false;
    private bool _isHit = false;
    public float invincibleDuration = 3f;
    public float knockbackForce = 8f;

    // --- COMPONENT REFERENCES ---
    private Rigidbody2D _rb;
    private Collider2D _collider;
    private SpriteRenderer _sr;
    private Animator _animator;

    // NOTE: _groundCheck has been completely deleted to fix the NullReference bug!

    // --- STATE VARIABLES ---
    private bool _isGrounded = false;
    private bool _isAttacking = false;
    private bool _wasFloating = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        defaultGravity = _rb.gravityScale;
        floatGravity = defaultGravity / 3f;
    }

    void Update()
    {
        if (isDead) return;
        if (_isHit) return;

        // THE FIX: Direct Physics Check at Mario's feet!
        Vector2 feetPosition = new Vector2(_collider.bounds.center.x, _collider.bounds.min.y);
        _isGrounded = Physics2D.OverlapCircle(feetPosition, groundCheckRadius, groundLayer);

        float horizontalInput = Input.GetAxis("Horizontal");
        bool jumpInput = Input.GetButtonDown("Jump");
        bool crouchPressed = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        bool floatInput = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);

        // Attack
        if (Input.GetKeyDown(KeyCode.LeftControl) && !_isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }

        SpriteFlip(horizontalInput);

        Vector2 velocity = _rb.linearVelocity;

        if (_isAttacking)
            velocity.x = 0;
        else
            velocity.x = horizontalInput * moveSpeed;

        if (crouchPressed && _isGrounded)
            velocity.x = 0;

        _rb.linearVelocity = velocity;

        // Jump
        if (jumpInput && _isGrounded)
        {
            _rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }

        // Floating logic
        if (_isGrounded)
            _wasFloating = false;

        if (!_isGrounded && floatInput)
        {
            _rb.gravityScale = floatGravity;
            _animator.SetBool("Floating", true);
            _wasFloating = true;
        }
        else if (!_isGrounded && _wasFloating)
        {
            _rb.gravityScale = bombGravity;
            _animator.SetBool("Floating", false);
        }
        else
        {
            _rb.gravityScale = defaultGravity;
            _animator.SetBool("Floating", false);
        }

        // Animator updates
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

    IEnumerator AttackRoutine()
    {
        _isAttacking = true;
        _animator.SetTrigger("Attack");

        if (_isGrounded)
            yield return new WaitForSeconds(1f);
        else
            yield return new WaitUntil(() => _isGrounded);

        _isAttacking = false;
    }

    public void FinishAttack() { }

    // ================= DAMAGE & DEATH SYSTEM =================

    public void TakeDamage()
    {
        TakeDamage(Vector2.up);
    }

    public void TakeDamage(Vector2 hitDirection)
    {
        if (isDead || isInvincible) return;

        if (GameManager.Instance != null && GameManager.Instance.lives <= 1)
        {
            GameManager.Instance.LoseLife();
            StartCoroutine(DeathRoutine());
        }
        else
        {
            StartCoroutine(DamageRoutine(hitDirection));
        }
    }

    private IEnumerator DamageRoutine(Vector2 hitDirection)
    {
        isInvincible = true;
        _isHit = true;

        if (GameManager.Instance != null) GameManager.Instance.LoseLife();

        _animator.SetTrigger("Hit");

        _rb.linearVelocity = Vector2.zero;
        float pushDir = (hitDirection.x == 0) ? (_sr.flipX ? 1f : -1f) : hitDirection.x;
        _rb.AddForce(new Vector2(pushDir * knockbackForce, knockbackForce), ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.4f);
        _isHit = false;

        float flashTimer = 0.4f;
        while (flashTimer < invincibleDuration)
        {
            _sr.enabled = !_sr.enabled;
            yield return new WaitForSeconds(0.1f);
            flashTimer += 0.1f;
        }

        _sr.enabled = true;
        isInvincible = false;
    }

    private IEnumerator DeathRoutine()
    {
        isDead = true;

        _collider.enabled = false;

        _rb.gravityScale = 3f;
        _rb.linearVelocity = Vector2.zero;
        float popDirection = _sr.flipX ? 5f : -5f;
        _rb.AddForce(new Vector2(popDirection, 20f), ForceMode2D.Impulse);

        _sr.flipY = true;
        _animator.SetTrigger("Hit");

        yield return new WaitForSeconds(2f);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGameOver();
        }
    }

    // ================= POWER UPS =================

    public void ActivateSpeedBoost() { StartCoroutine(SpeedRoutine()); }
    IEnumerator SpeedRoutine()
    {
        float originalSpeed = moveSpeed; moveSpeed = 10f; _sr.color = Color.red;
        yield return new WaitForSeconds(5f);
        moveSpeed = originalSpeed; _sr.color = Color.white;
    }

    public void ActivateJumpBoost() { StartCoroutine(JumpRoutine()); }
    IEnumerator JumpRoutine()
    {
        float originalJump = jumpForce; jumpForce = 25f; _sr.color = Color.green;
        yield return new WaitForSeconds(5f);
        jumpForce = originalJump; _sr.color = Color.white;
    }

    public void ActivateFloatBoost() { StartCoroutine(FloatRoutine()); }
    IEnumerator FloatRoutine()
    {
        float originalGravity = defaultGravity; defaultGravity = 0.5f; _rb.gravityScale = defaultGravity; _sr.color = Color.cyan;
        yield return new WaitForSeconds(5f);
        defaultGravity = originalGravity; _rb.gravityScale = defaultGravity; _sr.color = Color.white;
    }
}