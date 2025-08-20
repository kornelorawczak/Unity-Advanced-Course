using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision) {
        Transform hitTransform = collision.transform;  
        if (hitTransform.CompareTag("Player") && hitTransform.GetComponent<Animator>().GetBool("IsDead") == false) {
            hitTransform.GetComponent<PlayerHealth>().TakeDamage(30);
        }
        Destroy(gameObject, 1f);
    }
}
