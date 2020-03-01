using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntPlayer : MonoBehaviour
{
    public float huntSpeed = 11f;

    Transform player;
    GameManager gameManager;
    Renderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        gameManager = FindObjectOfType<GameManager>();
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Hack to only hunt the player when they are not looking at this object.
        // 
        // Caveat: the object is considered visible to the renderer if it's shadow is visible as well.
        if (_renderer.isVisible)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, player.position, huntSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Destroy(gameObject);

        gameManager?.RestartActiveLevel();
    }
}
