using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class NPCController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private bool lockDirection = true;
    public Vector2 movement = Vector2.zero;
    public Vector2 direction = Vector2.right;

    private int spriteDirection = 0;
    private Vector2 lastDirection;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        applyDirectionSprite();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void applyDirectionSprite()
    {
        Vector2 directionTarget = lastDirection;
        if (lockDirection)
        {
            directionTarget = direction.normalized;
        }

        if (movement.magnitude > 0.1f)
        {
            directionTarget = movement.normalized;
            lastDirection = directionTarget;
        }

        // Tentukan arah dominan: horizontal atau vertikal
        if (Mathf.Abs(directionTarget.x) > Mathf.Abs(directionTarget.y))
        {
            spriteDirection = directionTarget.x > 0 ? 1 : 3; // 1: kanan, 3: kiri
        }
        else
        {
            spriteDirection = directionTarget.y > 0 ? 2 : 0; // 2: atas, 0: bawah
        }

        if (animator != null)
        {
            animator.SetFloat("linearVelocity", movement.magnitude);
            animator.SetInteger("direction", spriteDirection);
        }
    }

    private void Move()
    {
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
    }
}
