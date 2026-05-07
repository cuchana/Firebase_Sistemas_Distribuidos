using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed = 8f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        LaunchBall();
    }

    void LaunchBall()
    {
        // Dirección inicial aleatoria
        Vector2 dir = new Vector2(
            Random.Range(-1f, 1f),
            1f
        ).normalized;

        rb.linearVelocity = dir * speed;
    }

    void FixedUpdate()
    {
        // Mantener velocidad constante
        rb.linearVelocity =
            rb.linearVelocity.normalized * speed;

        // Evitar movimiento muy horizontal
        if (Mathf.Abs(rb.linearVelocity.y) < 0.5f)
        {
            Vector2 dir = rb.linearVelocity;

            dir.y = 0.5f * Mathf.Sign(dir.y == 0 ? 1 : dir.y);

            rb.linearVelocity = dir.normalized * speed;
        }
    }
}