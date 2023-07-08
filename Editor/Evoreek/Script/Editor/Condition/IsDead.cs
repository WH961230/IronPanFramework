using System.Management.Instrumentation;
using UnityEngine;

[CreateAssetMenu(menuName = "Conditions/Is Dead")]
public class IsDead : Condition {
    public override bool CheckCondition(StateManager state) {
        return state.health <= 0;
    }
}