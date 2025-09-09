using UnityEngine;

public class Killzone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            // Option 1: Destroy player
            // Destroy(col.gameObject);

            // Option 2: Respawn (if you have a spawn point)
            col.transform.position = Vector3.zero; // set to spawn
            col.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }
}
