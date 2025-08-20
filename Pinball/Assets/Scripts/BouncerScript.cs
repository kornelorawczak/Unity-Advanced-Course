using UnityEngine;

public class BouncerScript : MonoBehaviour
{
    [SerializeField]
    Material collsionMaterial;
    [SerializeField]
    Material standardMaterial;
    [SerializeField, Min(0f)]
    float timeWaited = 0.5f;
    private void OnCollisionEnter(Collision other) {
        GetComponent<MeshRenderer>().material = collsionMaterial;
        Invoke("changeBack", timeWaited);
    }
    private void changeBack() {
        GetComponent<MeshRenderer>().material = standardMaterial;
    }
}
