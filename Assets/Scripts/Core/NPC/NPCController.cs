using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class NPCController : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private bool lockDirection = true;
    public Vector2 movement = Vector2.zero;
    public Vector2 direction = Vector2.right;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Animate();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Animate()
    {
        Vector2 inputMovement = movement.normalized;

        bool isMoving = inputMovement.magnitude > 0.01f;

        animator.SetFloat("x", inputMovement.x);
        animator.SetFloat("y", inputMovement.y);

        animator.SetBool("move", isMoving);

        if (isMoving && !lockDirection)
        {
            animator.SetFloat("lastX", inputMovement.x);
            animator.SetFloat("lastY", inputMovement.y);
            direction = inputMovement;
        }
        else
        {
            animator.SetFloat("lastX", direction.x);
            animator.SetFloat("lastY", direction.y);
        }
    }

    private void Move()
    {
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
    }
}
