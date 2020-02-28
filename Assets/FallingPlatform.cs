using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float fallSpeed = 0.3f;

    bool triggered = false;

    private void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;

        if (other.gameObject.CompareTag("Lava"))
        {
            Debug.Log("HIT LAVA");

            Destroy(gameObject);
            return;
        }

        Debug.Log("HIT PLAYER");

        triggered = true;
    }

    private void FixedUpdate()
    {
        if (!triggered)
        {
            return;
        }

        transform.Translate(Vector3.down * fallSpeed);
    }
}
