public class BigHeadCameraGameObj : GameObj {
    private BigHeadCameraData bigheadcameraData;
    private BigHeadCameraComponent bigHeadCameraComponent;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        bigheadcameraData = (BigHeadCameraData)data;
        bigHeadCameraComponent = (BigHeadCameraComponent) MyComp;
        Register();
    }
    
    private void Register() {
        foreach (var f in bigheadcameraData.FunctionTypes) {
            Logger.Print($"物体: {bigheadcameraData.MyObjName} 注册功能 {f.ToString()}");
            Register(f, new B<int>() {
                t1 = bigheadcameraData.InstanceID,
            });
        }
    }

    public void Display(int waveNum) {
        foreach (var tempInfo in bigHeadCameraComponent.monsterDisplayInfo) {
            if (tempInfo.waveNum == waveNum) {
                //显示父节点
                bigHeadCameraComponent.displayAnimator.transform.gameObject.SetActive(true);
                tempInfo.monsterText.gameObject.SetActive(true);
                ClockUtil.Instance.AlarmAfter(1, () => {
                    tempInfo.monsterGO.SetActive(true);
                });
                ClockUtil.Instance.AlarmAfter(5, () => {
                    tempInfo.monsterGO.SetActive(false);
                    bigHeadCameraComponent.displayAnimator.SetTrigger("Hide");
                    ClockUtil.Instance.AlarmAfter(1, () => {
                        bigHeadCameraComponent.displayAnimator.transform.gameObject.SetActive(false);
                        tempInfo.monsterText.gameObject.SetActive(false);
                    });
                });
            }
        }
    }

    public override void Clear() {
        base.Clear();
    }

    public override void Update() {
        base.Update();
    }
}