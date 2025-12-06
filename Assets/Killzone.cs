using UnityEngine;

public class Killzone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.transform.position = Vector3.zero;
            col.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
        if (col.CompareTag("Enemy"))
        {
            col.gameObject.SetActive(false);
        }
    }
}
