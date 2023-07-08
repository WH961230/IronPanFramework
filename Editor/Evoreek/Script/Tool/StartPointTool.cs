using System;
using System.Collections.Generic;
using UnityEngine;

public class StartPointTool : MonoBehaviour {
    public GameObject createGameObject;
    public float startPointHeight;//初始生成高度
    private GameObject targetGameObject;
    private int currentSetParam = 0;//当前设置的属性 1 位置 2 方向
    [SerializeField]
    public SettingType settingType;
    private void Start() {
        currentSetParam = 1;
    }

    void Update() {
        if (currentSetParam == 1) {
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity)) {
                    Vector3 hitPoint = hitInfo.point;
                    hitPoint.y += startPointHeight;
                    targetGameObject = CreateGameObjectInVec3(hitPoint, Quaternion.identity);
                }

                currentSetParam = 2;
                return;
            }
        }

        TargetRotationSet();
    }

    /// <summary>
    /// 设置初始旋转方向
    /// </summary>
    private void TargetRotationSet() {
        if (currentSetParam == 2) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity)) {
                Vector3 hitPoint = hitInfo.point;
                hitPoint.y = targetGameObject.transform.position.y;
                targetGameObject.transform.LookAt(hitPoint);
                if (Input.GetMouseButtonDown(0)) {
                    currentSetParam = 1;
                    targetGameObject = null;
                }
            }
        }
    }

    public GameObject CreateGameObjectInVec3(Vector3 vec, Quaternion qua) {
        var createTran = Instantiate(createGameObject).transform;
        createTran.SetParent(transform);
        createTran.position = vec;
        createTran.rotation = qua;
        return createTran.gameObject;
    }

    public List<PointInfo> GetAllPointInfo() {
        List<PointInfo> points = new List<PointInfo>();
        foreach (Transform t in transform) {
            PointInfo info = new PointInfo();
            info.vec = t.position;
            info.engle = t.rotation.eulerAngles;
            points.Add(info);
        }

        return points;
    }
}

[Serializable]
public enum SettingType {
    Player,
    Enemy,
}