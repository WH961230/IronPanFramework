using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Test/Add Health")]
public class ChangeHealth : StateAction {
    public override void Excute(StateManager stateManager) {
        stateManager.health += 10;
    }
}
