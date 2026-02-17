using System.Collections; // Required for using Coroutines (timed events)
using UnityEngine; // Required for Unity-specific functions

// Ensures these components exist on the GameObject so the script doesn't crash
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]



public class PlayerController : MonoBehaviour
{

    public bool isDead = false;

    // --- MOVEMENT SETTINGS ---
    public LayerMask groundLayer;       // Which objects count as "Ground" (e.g., floor, platforms)
    public float moveSpeed = 5f;        // How fast Mario moves horizontally
    public float jumpForce = 10f;       // How high Mario jumps
    public float groundCheckRadius = 0.2f; // How large the circle is that checks for the ground

    // --- AERIAL SETTINGS ---
    [Header("Animation Settings")]
    public float floatGravity = 0.5f;   // Low gravity for floating (Parachute)
    public float bombGravity = 10f;     // High gravity for the "Bomb" drop
    private float defaultGravity;       // To store the normal gravity value

    // --- COMPONENT REFERENCES ---
    private Rigidbody2D _rb;            // Reference to the physics body
    private Collider2D _collider;       // Reference to the physical shape
    private SpriteRenderer _sr;         // Reference to the image renderer (to flip it)
    private Animator _animator;         // Reference to the animation controller
    private GroundCheck _groundCheck;   // Helper class to check if we are on the ground

    // --- STATE VARIABLES ---
    private bool _isGrounded = false;   // Is Mario touching the floor?
    private bool _isAttacking = false;  // Is Mario currently attacking?
    private bool _wasFloating = false;  // Did we just use the parachute?

    // --- INITIALIZATION ---
    void Start()
    {
        // Get the specific components attached to this GameObject
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        // Create the ground checker using our collider and physics settings
        _groundCheck = new GroundCheck(_collider, _rb, groundCheckRadius, groundLayer);

        // Save the original gravity scale so we can reset it later
        defaultGravity = _rb.gravityScale;

        // Calculate a lighter gravity for the floating effect
        floatGravity = defaultGravity / 3f;
    }

    // --- MAIN LOOP (Runs every frame) ---
    void Update()
    {
        // 1. Check Physics Status
        _isGrounded = _groundCheck.IsGrounded(); // Update our grounded status

        // 2. Read Player Inputs
        float horizontalInput = Input.GetAxis("Horizontal"); // Get Left/Right input (-1 to 1)
        bool jumpInput = Input.GetButtonDown("Jump");        // specific frame the Jump button is pressed
        bool crouchPressed = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S); // Is Down/S held?
        bool floatInput = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);      // Is Up/W held?

        // 3. Handle Attack Logic
        // If Control is pressed AND we aren't already attacking...
        if (Input.GetKeyDown(KeyCode.LeftControl) && !_isAttacking)
        {
            StartCoroutine(AttackRoutine()); // ...start the attack timer routine
        }

        // 4. Handle Visuals
        SpriteFlip(horizontalInput); // Flip the sprite to face the direction we are moving

        // 5. Handle Movement Calculation
        Vector2 velocity = _rb.linearVelocity; // Get current speed

        if (_isAttacking)
        {
            velocity.x = 0; // If attacking, freeze horizontal movement
        }
        else
        {
            velocity.x = horizontalInput * moveSpeed; // Otherwise, move normally
        }

        // If crouching on the ground, stop moving entirely
        if (crouchPressed && _isGrounded) velocity.x = 0;

        _rb.linearVelocity = velocity; // Apply the calculated speed back to the body

        // 6. Handle Jumping
        if (jumpInput && _isGrounded)
        {
            // Apply an instant upward force for the jump
            _rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }

        // 7. Handle Floating / Bomb Drop Logic
        if (_isGrounded)
        {
            _wasFloating = false; // Reset the float memory if we are on the ground
        }

        if (!_isGrounded && floatInput)
        {
            // STATE: Floating (Parachute Open)
            _rb.gravityScale = floatGravity;     // Make gravity weak
            _animator.SetBool("Floating", true); // Play float animation
            _wasFloating = true;                 // Remember that we opened the parachute
        }
        else if (!_isGrounded && _wasFloating)
        {
            // STATE: Bomb Attack (In air, Key released, but we WERE just floating)
            _rb.gravityScale = bombGravity;       // Make gravity very strong (drop fast)
            _animator.SetBool("Floating", false); // Stop float animation
        }
        else
        {
            // STATE: Normal Jumping/Falling
            _rb.gravityScale = defaultGravity;    // Reset to normal gravity
            _animator.SetBool("Floating", false); // Stop float animation
        }

        // 8. Update Animator Parameters
        _animator.SetBool("isGrounded", _isGrounded);          // Tell animator if we are grounded
        _animator.SetFloat("moveInput", Mathf.Abs(horizontalInput)); // Tell animator our speed (always positive)
        _animator.SetFloat("yVel", _rb.linearVelocity.y);      // Tell animator our vertical speed
        _animator.SetBool("isCrouching", crouchPressed);       // Tell animator if we are crouching
    }

    // --- HELPER FUNCTIONS ---

    // Flips the sprite to face left or right
    private void SpriteFlip(float horizontalInput)
    {
        if (horizontalInput != 0) // Only flip if we are actually moving
        {
            _sr.flipX = (horizontalInput < 0); // Flip if moving left (negative), unflip if right
        }
    }

    // Coroutine to handle the Attack delay
    IEnumerator AttackRoutine()
    {
        _isAttacking = true;            // Lock movement
        _animator.SetTrigger("Attack"); // Play attack animation

        if (_isGrounded)
        {
            yield return new WaitForSeconds(1f); // If on ground, freeze for 1 second
        }
        else
        {
            yield return new WaitUntil(() => _isGrounded); // If in air, freeze until landing
        }

        _isAttacking = false; // Unlock movement
    }

    // Empty function to catch the Animation Event so it doesn't cause errors
    public void FinishAttack()
    {
        // Do nothing! The Coroutine handles the timing now.
    }


    // --- SUPERPOWER FUNCTIONS ---

    // RED: Super Speed
    public void ActivateSpeedBoost()
    {
        StartCoroutine(SpeedRoutine());
    }

    IEnumerator SpeedRoutine()
    {
        Debug.Log("Speed Boost STARTED");
        moveSpeed = 10f; 
        _sr.color = Color.red;

        yield return new WaitForSeconds(5f);

        moveSpeed = 5f; 
        _sr.color = Color.white;
        Debug.Log("Speed Boost ENDED");
    }

    // GREEN: Super Jump (Adjusted)
    public void ActivateJumpBoost()
    {
        StartCoroutine(JumpRoutine());
    }

    IEnumerator JumpRoutine()
    {
        Debug.Log("Jump Boost STARTED");
        jumpForce = 10f;
        _sr.color = Color.green;

        yield return new WaitForSeconds(5f);

        jumpForce = 7f; 
        _sr.color = Color.white;
        Debug.Log("Jump Boost ENDED");
    }

    // BLUE: Zero Gravity
    public void ActivateFloatBoost()
    {
        StartCoroutine(FloatRoutine());
    }

    IEnumerator FloatRoutine()
    {
        Debug.Log("Float Boost STARTED");
        defaultGravity = 0.5f; 
        _rb.gravityScale = defaultGravity;
        _sr.color = Color.cyan;

        yield return new WaitForSeconds(5f);

        defaultGravity = 1f;
        _rb.gravityScale = defaultGravity;
        _sr.color = Color.white;
        Debug.Log("Float Boost ENDED");
    }
    //Damage logic for mario 
    public void TakeDamage()
    {
        if (isDead) return;

        Debug.Log("Mario Died!");
        isDead = true;

        // Play death animation if you have one
        // anim.SetTrigger("Death");

        // Disable movement
        moveSpeed = 0;
        jumpForce = 0;
        _rb.linearVelocity = Vector2.zero;

        // Optional: Reload scene after 2 seconds
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}

