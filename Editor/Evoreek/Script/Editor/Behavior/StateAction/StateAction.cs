using UnityEngine;

public abstract class StateAction : ScriptableObject {
    public abstract void Excute(StateManager stateManager);
}