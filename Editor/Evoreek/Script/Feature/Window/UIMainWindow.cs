using UnityEngine;

public class UIMainWindow : Window {
    private UIMainComponent comp;
    private UIMainGameObj mainGameObj;
    private int audioID;
    private GameObject audioGO;
    public override void Init(Game game, GameObj gameObj) {
        base.Init(game, gameObj);
        mainGameObj = (UIMainGameObj) gameObj;
        comp = mainGameObj.GetObj().GetComponent<UIMainComponent>();

        GameData.SetCursor("游戏内光标");
        
        comp.HostGameBtn.onClick.AddListener(() => {
            if (Game.Instance.netManager == null) {
                Logger.PrintE("Can not find network !");
                return;
            }
            Game.Instance.netManager.StartHost();
            gameSystem.messageCenter.Dispatcher(MessageConstant.StartGameMsg);
        });

        comp.BackToDeskBtn.onClick.AddListener(() => {
            game.Quit();
        });

        PlaySound();

        gameSystem.messageCenter.Reg(MessageConstant.StartGameMsg, ()=> {
            mainGameObj.GetObj().SetActive(false);
            gameSystem.GetSystem<AudioSystem>().RemoveSoundPlay(audioGO);
        });
        
        gameSystem.messageCenter.Reg(MessageConstant.BackToMainMsg, () => {
            mainGameObj.GetObj().SetActive(true);
            PlaySound();
        });
    }

    private void PlaySound() {
        audioGO = gameSystem.GetSystem<AudioSystem>().AddSoundPlay(new AudioData() {
            InstanceID = comp.InstanceID,
            tempAudioName = "游戏主界面",
            isLoop = true,
            is3D = false,
            isFollow = false,
        });
    }

    public override void Clear() {
        gameSystem.messageCenter.UnReg(MessageConstant.StartGameMsg, ()=> {
            mainGameObj.GetObj().SetActive(false);
        });

        gameSystem.messageCenter.UnReg(MessageConstant.BackToMainMsg, () => {
            mainGameObj.GetObj().SetActive(true);
        });
        base.Clear();
    }
}