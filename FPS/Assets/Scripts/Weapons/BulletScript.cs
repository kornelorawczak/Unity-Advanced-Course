using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [Header("Settings")]
    public float lifeTime = 1;

    private void Awake() {
        Destroy(gameObject, lifeTime);
    }
}
