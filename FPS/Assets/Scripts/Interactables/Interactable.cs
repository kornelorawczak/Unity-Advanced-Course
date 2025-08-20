using UnityEngine;


// template of a class of any interactable object 
public abstract class Interactable : MonoBehaviour
{
    public bool useEvents;
    public string promptMessage;
    public void BaseInteract() {
        if (useEvents) {
            GetComponent<InteractionEvent>().OnInteract.Invoke();
        }
        Interact();
    }
    protected virtual void Interact() {

    }
}
