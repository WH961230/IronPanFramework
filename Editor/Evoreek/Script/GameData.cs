using System.Collections.Generic;
using UnityEngine;

public static class GameData {
    public static bool IsUnityPause = false;// Unity 是否暂停
    public static AudioListener AudioListener;
    public static CameraGameObj WeaponCamera;
    public static PlayerGameObj MainPlayer;//本地玩家
    public static CameraGameObj MainCamera;
    public static BigHeadCameraGameObj BigHeadCamera;
    public static GameSystem gameSystem;
    public static PlayerViewType PlayerViewType;
    public static GameObject PoolRoot;
    public static int MainPlayerID => MainPlayer.GetData().InstanceID;//本地玩家 ID
    public static AudioSystem AudioSystem;
    public static Dictionary<int, int> DetectedAttackTargetID = new Dictionary<int, int>();//攻击的目标 ID

    /// <summary>
    /// 获取该点在地面的落点
    /// </summary>
    /// <returns></returns>
    public static bool GetTerrainPos(Vector3 pos, out Vector3 outPoint) {
        Ray ray = new Ray(pos, Vector3.down);
#if UNITY_EDITOR
        Debug.DrawRay(ray.origin, ray.direction, Color.red, 2);
#endif
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Terrain"))){
            outPoint = hit.point;
            return true;
        }
        outPoint = default;
        return false;
    }

    /// <summary>
    /// 射线检测 筛选层级
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool GetInputRay(string layerName, out RaycastHit hit) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool isDetectedObj = Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer(layerName));
        if (isDetectedObj) {
            return true;
        }

        return false;
    }

    public static void SetCursor(string sign) {
        Texture2D poolGo = AssetsLoad.GetAsset<Texture2D>("Evoreek_", sign, AssetsLoad.AssetType.Texture2D);
        Cursor.SetCursor(poolGo, Vector2.zero, CursorMode.ForceSoftware);
    }
}