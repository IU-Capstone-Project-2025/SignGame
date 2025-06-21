using UnityEngine;

public class CoinDestroyer : MonoBehaviour
{
    [SerializeField] ParticleSystem collectEffect;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
            
    }
    private void OnDestroy()
    {
        ParticleSystem effect = Instantiate(collectEffect, transform.position, Quaternion.identity);
        effect.Play();
    }
}
