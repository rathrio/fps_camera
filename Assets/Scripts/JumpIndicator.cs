using UnityEngine;
using UnityEngine.UI;

public class JumpIndicator : MonoBehaviour
{
    PlayerMovement player;
    Image cursor;

    readonly Color white = new Color32(255, 255, 255, 255);
    readonly Color red = new Color32(235, 62, 49, 255);
    readonly Color yellow = new Color32(237, 194, 64, 255);

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        cursor = gameObject.GetComponent<Image>();
    }

    void Update()
    {
        // Default
        if (player.IsGrounded)
        {
            cursor.color = white;
            return;
        }

        // No more jump available
        if (player.HasDoubleJumped)
        {
            cursor.color = red;
            return;
        }

        // Can double jump
        cursor.color = yellow;
    }
}
