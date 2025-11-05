using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class GhostMovement : MonoBehaviour
{
    // === Movement ===
    public enum GhostDirection { Left, Right }
    [SerializeField] private GhostDirection ghostDirection;
    [SerializeField] private float speedMovement;
    private SpriteRenderer spriteRend;
    private Rigidbody2D rb2D;

    // === Properties ===
    public GhostDirection Direction => ghostDirection;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        rb2D.linearVelocityX = speedMovement;
    }

    public void InitializeGhost(GhostDirection direction, float speed)
    {
        ghostDirection = direction;
        speedMovement = speed;

        if (ghostDirection == GhostDirection.Left)
        {
            spriteRend.flipX = true;
            if (speedMovement > 0) speedMovement *= -1;
        }
        else
        {
            spriteRend.flipX = false;
            if (speedMovement < 0) speedMovement *= -1;
        }
    }
}