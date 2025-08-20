using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public BaseState activeState;
    
    public void Initialize() {
        ChangeState(new PatrolState());
    }
    void Update() {
        if (activeState != null) {
            activeState.Perform();
        }
    }

    public void ChangeState(BaseState newState) {
        if (activeState != null) {
            activeState.Exit();
        }
        activeState = newState;
        if (activeState != null) {
            activeState.stateMachine = this;
            activeState.enemy = GetComponent<Enemy>();
            activeState.Enter();
        }
    }
}
