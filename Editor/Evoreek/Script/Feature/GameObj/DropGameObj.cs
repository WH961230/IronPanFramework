using UnityEngine;

public class DropGameObj : GameObj {
    private DropData dropData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        dropData = (DropData)data;

        Renderer renderer = MyObj.GetComponentInChildren<Renderer>();
        Material gradeMat = null;
        foreach (var tempGrade in dropData.gradeInfo) {
            if (tempGrade.gradeType == dropData.gradeType) {
                gradeMat = tempGrade.gradeTypeMat;
                break;
            }
        }
        renderer.material = gradeMat;

        if (GameData.GetTerrainPos(dropData.MyStartPointInfo.vec, out var vec)) {
            MyObj.transform.position = vec + new Vector3(0, 0.2f, 0);
        }

        Register();
        ClockUtil.Instance.AlarmAfter(5, () => {
            MyObj.SetActive(false);
        });
    }

    private void Register() {
        foreach (var f in dropData.FunctionTypes) {
            // Logger.Print($"物体: {dropData.MyObjName} 注册功能 {f.ToString()}");
            Register(f, new B<int>() {
                t1 = dropData.InstanceID,
            });
        }
    }

    public override void Clear() {
        base.Clear();
    }

    public override void Update() {
        base.Update();
    }
}