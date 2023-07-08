using UnityEngine;

public class EnemyGameObj : GameObj {
    private EnemyData enemyData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        enemyData = (EnemyData)data;
        MyComp.dizzinessTran.gameObject.SetActive(false);
        Register();
        gameSystem.messageCenter.Reg(MessageConstant.BackToMainMsg, UnRegister);
    }

    public override void Clear() {
        gameSystem.messageCenter.UnReg(MessageConstant.BackToMainMsg, UnRegister);
        base.Clear();
    }

    public override void Update() {
        base.Update();
    }

    private void Register() {
        foreach (var f in enemyData.FunctionTypes) {
            Register(f, new B<int>() {
                t1 = enemyData.InstanceID,
            });
        }
    }

    private void UnRegister() {
        foreach (var f in enemyData.FunctionTypes) {
            UnRegister(f, new B<int>() {
                t1 = enemyData.InstanceID,
            });
        }
    }
}