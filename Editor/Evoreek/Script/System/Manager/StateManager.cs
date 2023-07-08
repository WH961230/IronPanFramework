using System.Collections.Generic;

public class StateManager : IManager {
    public Dictionary<int, List<GameCondition>> allGameCond = new Dictionary<int, List<GameCondition>>();
    public Dictionary<int, GameCondition> currentGameCond = new Dictionary<int, GameCondition>();

    public void Register(int id, GameCondition gameCond) {
        if (allGameCond.ContainsKey(id)) {
            
        } else {
            allGameCond.Add(id, new List<GameCondition>() {
                gameCond
            });
        }
    }

    public void UnRegister(int id, GameCondition gameCond) {
        
    }

    public void Get(int id, GameCondition gameCondition) {
        
    }
}