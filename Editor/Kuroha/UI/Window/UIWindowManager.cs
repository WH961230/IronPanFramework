using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kuroha.UI
{
    using UIPool = Dictionary<Type, WindowStatus>;

    public class WindowStatus
    {
        public bool isOpened;
        public UIWindowController controller;
    }

    public class UIWindowManager : UIBaseManager
    {
        /// <summary>
        /// UI 池
        /// </summary>
        private readonly UIPool uiPool = new UIPool(10);

        /// <summary>
        /// 构造器
        /// </summary>
        public UIWindowManager(UISystem uiSystem, Transform position, string prefabPath) : base(uiSystem, position, prefabPath) { }

        /// <summary>
        /// 打开
        /// </summary>
        public T1 Open<T1, T2>(T2 displayContent) where T1 : UIWindowController
        {
            UIWindowController returnController;

            // 获取 UI 类型
            var uiType = typeof(T1);

            // 判断目标 UI 是否在缓存池中
            if (uiPool.TryGetValue(uiType, out var status))
            {
                uiPool[uiType].isOpened = true;
                uiPool[uiType].controller.ReOpen(displayContent);
                Show(uiPool[uiType].controller);

                returnController = status.controller;
                UIDebug.Log($"界面 {uiType} 已经在缓存池中了", "green");
            }
            else
            {
                returnController = InstantiateNewUI<T1, T2>(displayContent, uiType);
            }

            return returnController as T1;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close<T>() where T : UIWindowController
        {
            // 获取 UI 类型
            var uiType = typeof(T);

            if (uiPool.TryGetValue(uiType, out var status))
            {
                if (status.isOpened)
                {
                    status.isOpened = false;
                    Hide(status.controller);
                }
                else
                {
                    UIDebug.Log($"窗口 {uiType} 已经处于关闭状态, 请勿重复关闭!", "red");
                }
            }
        }

        /// <summary>
        /// 帧更新
        /// </summary>
        public override void Update()
        {
            foreach (var window in uiPool.Values)
            {
                if (window.isOpened)
                {
                    window.controller.Update();
                }
            }
        }

        /// <summary>
        /// 实例化新 UI
        /// </summary>
        private UIWindowController InstantiateNewUI<T1, T2>(T2 displayContent, Type uiType) where T1 : UIWindowController
        {
            // 获取 UI 预制体名称
            var uiName = uiType.Name.Replace("_Controller", string.Empty);

            // 获取 UI 预制体路径
            var prefabPath = $"{AssetPrefix}{uiName}/{uiName}";

            // 加载 UI 预制体
            var uiPrefab = Resources.Load<GameObject>(prefabPath);
            if (uiPrefab == null)
            {
                UIDebug.LogError($"未获取到 Assets/Resources/{prefabPath}.prefab 预制体!", "red");
            }

            // 实例化 UI 到场景
            var newUI = UnityEngine.Object.Instantiate(uiPrefab, UIParent, false);
            if (newUI.TryGetComponent<UIWindowView>(out var newView))
            {
                // 创建目标 Controller
                if (Activator.CreateInstance(typeof(T1), this, newView) is T1 newController)
                {
                    uiPool[uiType] = new WindowStatus
                    {
                        isOpened   = true,
                        controller = newController
                    };

                    newController.FirstOpen(displayContent);
                    Show(newController);
                    return newController;
                }

                UIDebug.LogError($"实例化 {uiType} 失败!", "red");
            }
            else
            {
                UIDebug.LogError($"UI {newUI} 没有挂载 UIView 组件!", "red");
            }

            return null;
        }
    }
}
