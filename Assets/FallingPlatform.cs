using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float fallSpeed = 0.2f;
    public float fallDelay = 1f;

    bool triggered = false;

    void Trigger()
    {
        triggered = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
        {
            return;
        }

        Invoke("Trigger", fallDelay);
    }

    private void FixedUpdate()
    {
        if (!triggered)
        {
            return;
        }

        if (transform.position.y < -50f)
        {
            Destroy(gameObject);
        }

        transform.Translate(Vector3.down * fallSpeed);
    }
}
