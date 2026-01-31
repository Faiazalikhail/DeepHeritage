using UnityEngine;

public class GroundCheck
{
    private bool isGrounded = false;

    private LayerMask groundLayer;
    private Collider2D col;
    private Rigidbody2D rb;
    private float groundCheckRadius;

    private Vector2 groundCheckPosition => new Vector2(col.bounds.center.x, col.bounds.min.y);

    // This is a "Constructor" - it runs when we write 'new GroundCheck(...)'
    public GroundCheck(Collider2D col, Rigidbody2D rb, float radius, LayerMask groundLayer)
    {
        this.col = col;
        this.rb = rb;
        this.groundCheckRadius = radius;
        this.groundLayer = groundLayer;
    }

    public bool IsGrounded()
    {
        // This check prevents us from "landing" while moving upwards (jumping)
        if (!isGrounded && rb.linearVelocity.y > 0)
        {
            return false;
        }

        // Otherwise, check for the ground
        isGrounded = Physics2D.OverlapCircle(groundCheckPosition, groundCheckRadius, groundLayer);
        return isGrounded;
    }
}