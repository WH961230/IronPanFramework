using System.Collections.Generic;

public class GameObjManager : IManager {
    private Dictionary<int, GameObj> gameObjDict = new Dictionary<int, GameObj>();
    private List<GameObj> gameObjs = new List<GameObj>();

    public T Register<T>(Game game, Data data) where T : GameObj, new() {
        T instance = new T();
        gameObjDict.Add(data.InstanceID, instance);
        gameObjs.Add(instance);
        instance.Init(game, data);
        return instance;
    }

    public void Remove(int id) {
        int index = Find(id);
        if (index != -1) {
            GameObj instance = gameObjs[index];
            gameObjDict.Remove(id);
            gameObjs.RemoveAt(index);
            instance.Clear();
        }
    }

    public void RemoveAll() {
        for (int i = 0; i < gameObjs.Count; i++) {
            gameObjs[i].Clear();
        }

        gameObjs.Clear();
        gameObjDict.Clear();
    }

    private int Find(int id) {
        if (gameObjDict.TryGetValue(id, out GameObj ret)) {
            int count = gameObjs.Count;
            for (int i = 0; i < count; i++) {
                if (gameObjs[i] == ret) {
                    return i;
                }
            }
        }

        return -1;
    }

    private int Find(GameObj gameObj) {
        int count = gameObjs.Count;
        for (int i = 0; i < count; i++) {
            if (gameObjs[i] == gameObj) {
                return i;
            }
        }

        return -1;
    }

    public List<GameObj> GetAll() {
        return gameObjs;
    }

    public Dictionary<int, GameObj> GetAllDic() {
        return gameObjDict;
    }

    public GameObj Get(int id) {
        if (gameObjDict.TryGetValue(id, out GameObj ret)) {
            return ret;
        }

        return null;
    }

    public void Update() {
        for (int i = 0; i < gameObjs.Count; i++) {
            gameObjs[i].Update();
        }
    }

    public void FixedUpdate() {
        for (int i = 0; i < gameObjs.Count; i++) {
            gameObjs[i].FixedUpdate();
        }
    }

    public void LateUpdate() {
        for (int i = 0; i < gameObjs.Count; i++) {
            gameObjs[i].LateUpdate();
        }
    }
}