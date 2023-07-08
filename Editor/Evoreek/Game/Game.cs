using Mirror;
using UnityEngine;

public class Game {
    public GameSystem gameSystem;
    public NetworkManager netManager;
    public NetworkCenter netCenter;
    public LoggerType loggerType;

    private static Game instance;
    public static Game Instance => instance ??= new Game();

    public void Start() {
        Init();
    }

    private void Init() {
        gameSystem = new GameSystem();
        gameSystem.Init(this);

        GameData.gameSystem = gameSystem;

        netManager = Object.FindObjectOfType<NetworkManager>();
        netCenter = netManager.GetComponent<NetworkCenter>();
        GameData.IsUnityPause = false;
    }

    public void Quit() {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        gameSystem = null;
    }

    public void Update() {
        gameSystem.Update();
        QuickHotshort();
    }

    public void FixedUpdate() {
        gameSystem.FixedUpdate();
    }

    public void LateUpdate() {
        gameSystem.LateUpdate();
    }

    private void QuickHotshort() {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F5)) {
            UnityEditor.EditorApplication.isPaused = true;
        } else if (Input.GetKeyDown(KeyCode.Q)) {
            UnityEditor.EditorApplication.isPlaying = false;
        } else if (Input.GetKeyDown(KeyCode.L)) {
            if (Cursor.lockState == CursorLockMode.Locked) {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            } else {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
#endif
    }
}