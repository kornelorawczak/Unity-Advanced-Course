using UnityEngine;

public class KeyPad : Interactable
{
    [SerializeField] private GameObject door;
    private bool doorOpen;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    protected override void Interact()
    {
        doorOpen = !doorOpen;
        door.GetComponent<Animator>().SetBool("IsOpen", doorOpen);
    }
}
