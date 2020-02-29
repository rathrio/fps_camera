using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float fallSpeed = 20f;
    public float fallDelay = 0.8f;

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

    private void Update()
    {
        if (!triggered)
        {
            return;
        }

        if (transform.position.y < -50f)
        {
            Destroy(gameObject);
            return;
        }

        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }
}
