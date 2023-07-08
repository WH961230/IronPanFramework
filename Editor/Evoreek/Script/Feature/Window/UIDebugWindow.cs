public class UIDebugWindow : Window {
    private float clearTime = 0;
    private UIDebugComponent comp;
    private UIDebugData data;

    public override void Init(Game game, GameObj gameObj) {
        base.Init(game, gameObj);
        data = gameObj.GetData<UIDebugData>();
        comp = gameObj.GetObj().GetComponent<UIDebugComponent>();
        comp.DebugConsoleArea.SetActive(false);
        comp.DebugToolBtn.onClick.AddListener(() => {//控制台按钮打开菜单
            bool isActive = !comp.DebugConsoleArea.gameObject.activeSelf;
            comp.DebugConsoleArea.SetActive(isActive);
            if (isActive) {
                data.previousContent = null;
                comp.DebugCommandTxt.ActivateInputField();
            }
        });
    }
}