using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class GhostMovement : MonoBehaviour
{
    // === Movement ===
    public enum ScreenSide { Left, Right }
    [SerializeField] private ScreenSide screenSide;
    [SerializeField] private float speedMovement;
    private SpriteRenderer spriteRend;
    private Rigidbody2D rb2D;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        spriteRend = GetComponent<SpriteRenderer>();

        InitializeGhost();
    }

    void FixedUpdate()
    {
        rb2D.linearVelocityX = speedMovement;
    }

    private void InitializeGhost()
    {
        if (screenSide == ScreenSide.Right)
        {
            spriteRend.flipX = true;
            speedMovement *= -1;
        }
    }
}