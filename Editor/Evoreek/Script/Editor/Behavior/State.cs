using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class State : ScriptableObject {
    
    public List<StateAction> stateActions = new List<StateAction>();
    public List<Transition> transitions = new List<Transition>();
    public void Tick() {
        
    }

    public Transition AddTransition() {
        Transition retVal = new Transition();
        transitions.Add(retVal);
        return retVal;
    }

    public void ClearTransition(Transition transition) {
        if (transitions.Contains(transition)) {
            transitions.Remove(transition);
        }
    }

    public void ClearTransition() {
        transitions.Clear();
    }
}