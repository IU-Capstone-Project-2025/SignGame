using Unity.VisualScripting;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    public int damage = 3;
    private Vector2 spikeRotation = Vector2.zero;
    private void Start()
    {
        float angle = (transform.rotation.eulerAngles.z - 90) * Mathf.Deg2Rad;
        float x = Mathf.Cos(angle);
        float y = Mathf.Sin(angle);
        spikeRotation = new Vector2(x, y).normalized;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log(spikeRotation);
            collision.gameObject.GetComponent<Player>().TakeDamage(damage, spikeRotation);
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider, true);
        }
    }
}
