using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Kuroha.UI
{
    public class UIPanelManager : UIBaseManager
    {
        /// <summary>
        /// UI 栈
        /// </summary>
        private readonly Stack<UIPanelController> uiStack = new Stack<UIPanelController>(10);

        /// <summary>
        /// UI 缓存池 (缓存所有已经打开过的 UI)
        /// </summary>
        private readonly Dictionary<Type, UIPanelController> uiPool = new Dictionary<Type, UIPanelController>(10);

        /// <summary>
        /// 当前 UI
        /// </summary>
        private UIPanelController current;

        /// <summary>
        /// 构造器
        /// </summary>
        public UIPanelManager(UISystem uiSystem, Transform position, string prefabPath) : base(uiSystem, position, prefabPath) { }

        public T Open<T>() where T : UIPanelController
        {
            // 获取 UI 界面类型
            var uiType = typeof(T);

            // 检查是否是首次打开
            if (current == null)
            {
                Create<T>(uiType);
            }

            // 检查目标 UI 是否就是当前 UI
            else if (current.UIType == uiType)
            {
                UIDebug.Log($"界面 {uiType} 当前处于打开状态, 请勿重复打开!", "red");
            }

            // 最后一种情况: 打开另一个 UI
            else
            {
                // 先关闭当前 UI, 之后打开目标 UI
                Hide(current);

                // 检查目标 UI 是否已经在缓存池中了
                if (uiPool.ContainsKey(uiType))
                {
                    Show(uiPool[uiType]);
                    uiPool[uiType].ReOpen();
                    uiStack.Push(uiPool[uiType]);
                }
                else
                {
                    Create<T>(uiType);
                }
            }

            // 更新 current
            current = uiStack.Count == 0 ? null : uiStack.Peek();
            return current as T;
        }

        public void Close()
        {
            if (current != null)
            {
                // 隐藏当前 UI
                Hide(current);

                // 当前 UI 出栈
                uiStack.Pop();

                // 更新 current 并显示
                current = uiStack.Count == 0 ? null : uiStack.Peek();
                if (current != null)
                {
                    Show(current);
                }
            }
        }
        
        public override void Update()
        {
            current?.Update();
        }

        public override void FixedUpdate()
        {
            current?.FixedUpdate();
        }

        public override void LateUpdate()
        {
            current?.LateUpdate();
        }

        #region 私有

        private void Create<T>(Type uiType) where T : UIPanelController
        {
            var uiName = uiType.Name.Replace("_Controller", string.Empty);
            var prefabAddress = $"{AssetPrefix}{uiName}";
            var loadHandle = Addressables.LoadAssetAsync<GameObject>(prefabAddress);
            loadHandle.WaitForCompletion();
            
            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                var uiPrefab = loadHandle.Result;
                var newUI = UnityEngine.Object.Instantiate(uiPrefab, UIParent, false);
                if (newUI.TryGetComponent<UIPanelView>(out var newView))
                {
                    if (Activator.CreateInstance(typeof(T), this, newView, uiType) is T newController)
                    {
                        newController.FirstOpen();
                        uiStack.Push(newController);
                        uiPool[uiType] = newController;
                    }
                    else
                    {
                        UIDebug.LogError($"创建 {uiType} 失败!", UIDebug.红);
                    }
                }
                else
                {
                    UIDebug.LogWarning($"UI {newUI} 没有挂载 UIView 组件!", UIDebug.山吹);
                }
            }
            else
            {
                UIDebug.LogError($"未获取到资源 {prefabAddress} !", UIDebug.红);
            }
        }

        private void Hide<T>(T controller) where T : UIPanelController
        {
            base.Hide(controller);
        }

        private void Show<T>(T controller) where T : UIPanelController
        {
            base.Show(controller);
        }

        #endregion
    }
}
