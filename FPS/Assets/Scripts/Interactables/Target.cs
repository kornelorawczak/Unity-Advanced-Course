using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 100;

    public void TakeDamage(float amount) {
        health -= amount;
        if (health <= 0f) {
            Die();
        }
    }

    private void Die() {
        Destroy(gameObject);
    }
}
