using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingPlatform : MonoBehaviour
{
    public float fallSpeed = 60f;
    public float fallDelay = 0.9f;

    bool triggered = false;

    Rigidbody rb;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Trigger()
    {
        triggered = true;
        rb.useGravity = true;
        rb.velocity = new Vector3(0, -fallSpeed, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player"))
        {
            return;
        }

        Invoke("Trigger", fallDelay);
    }
}
