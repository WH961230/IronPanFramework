
using System.Reflection;
using UnityEngine;

public class CreaterSystem : GameSys {
    private SOCreaterSetting setting;
    private GameSystem gameSystem;

    #region 敌人

    private Clock clock;
    
    private int EnemyCreateWaveNum = 30;//敌人波数
    private float EnemyCreateInterval = 2;//敌人生成间隔
    private int EnemyCreateNum = 10;//敌人生成数量
    
    //runtime
    private int runtimeEnemyCreateWaveNum = 1;//敌人波数
    private int runtimeEnemyCreateNum = 0;//敌人生成数量

    #endregion
    
    public CreaterSystem(GameSystem gameSystem) {
        this.gameSystem = gameSystem;
        Init(gameSystem);
    }

    protected override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        setting = gameSystem.SoData.SOGameSetting.GetSetting<SOCreaterSetting>();
        base.setting = this.setting;
        
        gameSystem.messageCenter.Reg(MessageConstant.TerrainLoadFinished, () => {
            ClockUtil.Instance.AlarmAfter(5, () => {
                gameSystem.messageCenter.Dispatcher(MessageConstant.EnemyWaveTipMsg, 3f, " 第 1 波怪物来袭！");
            });
            if (setting.testMode) {
                ClockUtil.Instance.AlarmAfter(2, () => {
                    int id = gameSystem.GetSystem<EnemySystem>().Instance();
                    Data data = gameSystem.MyGameObjFeature.Get(id).GetData();
                    data.canReborn = true;
                    data.RebornTime = 0;
                    
                    id = gameSystem.GetSystem<EnemySystem>().Instance();
                    data = gameSystem.MyGameObjFeature.Get(id).GetData();
                    data.canReborn = true;
                    data.RebornTime = 0;
                });
            } else {
                clock = ClockUtil.Instance.AlarmRepeat(5, EnemyCreateInterval, () => {
                    // if (runtimeEnemyCreateNum == 0 && runtimeEnemyCreateWaveNum == 1) {
                    //     gameSystem.GetSystem<EnemySystem>().InstanceBoss(runtimeEnemyCreateWaveNum);
                    // }

                    if (runtimeEnemyCreateNum >= EnemyCreateNum) {
                        runtimeEnemyCreateNum = 0;
                        if (runtimeEnemyCreateWaveNum + 1 > EnemyCreateWaveNum) {
                            // gameSystem.messageCenter.Dispatcher(MessageConstant.GameOverMsg, true);//当坚持到最后一波之后胜利
                            ClockUtil.Instance.Stop(clock);
                        } else {
                            runtimeEnemyCreateWaveNum++;
                            gameSystem.GetSystem<EnemySystem>().InstanceBoss(runtimeEnemyCreateWaveNum);
                            gameSystem.messageCenter.Dispatcher(MessageConstant.EnemyWaveTipMsg, 3f, $" 第 {runtimeEnemyCreateWaveNum} 波怪物来袭！");
                        }
                    }

                    if (runtimeEnemyCreateNum == 0) {
                        GameData.BigHeadCamera.Display(runtimeEnemyCreateWaveNum);
                    }
                    
                    gameSystem.GetSystem<EnemySystem>().Instance();
                    runtimeEnemyCreateNum++;//生成怪物
                });
            }
        });
    }

    public override void Update() {
        base.Update();
    }

    protected override void Clear() {
        gameSystem.messageCenter.UnReg(MessageConstant.TerrainLoadFinished, () => {
            ClockUtil.Instance.AlarmAfter(5, () => {
                gameSystem.messageCenter.Dispatcher(MessageConstant.EnemyWaveTipMsg, 3f, " 第 1 波怪物来袭！");
            });
            clock = ClockUtil.Instance.AlarmRepeat(5, EnemyCreateInterval, () => {
                gameSystem.GetSystem<EnemySystem>().Instance();
                runtimeEnemyCreateNum++;
                if (runtimeEnemyCreateNum >= EnemyCreateNum) {
                    runtimeEnemyCreateNum = 0;
                    if (runtimeEnemyCreateWaveNum + 1 > EnemyCreateWaveNum) {
                        // gameSystem.messageCenter.Dispatcher(MessageConstant.GameOverMsg, true);//当坚持到最后一波之后胜利
                        ClockUtil.Instance.Stop(clock);
                    } else {
                        runtimeEnemyCreateWaveNum++;
                        if (runtimeEnemyCreateWaveNum >= 2) {
                            gameSystem.GetSystem<EnemySystem>().InstanceBoss(runtimeEnemyCreateWaveNum);
                        }
                        gameSystem.messageCenter.Dispatcher(MessageConstant.EnemyWaveTipMsg, 3f, $" 第 {runtimeEnemyCreateWaveNum} 波怪物来袭！");
                    }
                }
            });
        });
        base.Clear();
    }

    #region 不同实例创建接口

    /// <summary>
    /// 实例化 GameObject
    /// </summary>
    public int Instance(ObjectType createrType, Vector3 startPos) {
        return Instance<CreaterGameObj, CreaterEntity, CreaterData>((data) => {
            data.MyObjName = "生成器";
            data.MyStartPointInfo = new PointInfo() {
                vec = startPos,
                engle = Vector3.zero,
            };
            data.createrType = createrType;
            data.CreaterMatInfoList = setting.CreaterMatInfoList;//染色信息
        });
    }

    #endregion
}