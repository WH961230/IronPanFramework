using System;
using Mirror;

public class NetworkCenter : NetworkBehaviour {
    #region networkData

    [Server]
    public void ServerRpc(int netCode) {
        ObserversRpc(netCode);
    }

    [ClientRpc]
    private void ObserversRpc(int netCode) {
        switch (netCode) {
            case 0:
                if (Game.Instance.gameSystem.MyGameState != GameState.GameStart) {
                    Game.Instance.gameSystem.MyGameState = GameState.GameStart;
                    Logger.Print("StartGame message");
                }
                break;
        }
    }

    #endregion
}

public enum GameState {
    None,
    GameStart,
    GameOver,
}

