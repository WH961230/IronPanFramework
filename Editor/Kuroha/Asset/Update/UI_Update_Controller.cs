using System;
using System.Collections;
using System.Collections.Generic;
using Kuroha.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Kuroha.Asset
{
    public class UI_Update_Controller : UIPanelController
    {
        private UI_Update_View View { get; }

        private float downloadSize;
        private List<string> catalogsNeedUpdate;
        private AsyncOperationHandle<List<IResourceLocator>> updateAsync;
        private AsyncOperationHandle downloadAsync;

        public UI_Update_Controller(in UIBaseManager manager, in UIPanelView view, in Type uiType) : base(in manager, in view, in uiType)
        {
            View = view as UI_Update_View;
        }

        public override void FirstOpen()
        {
            View.tmpLog.text = string.Empty;
            View.tmpButton.onClick.RemoveAllListeners();
            //View.tmpButton.onClick.AddListener(() => View.StartCoroutine(DoUpdateAddressadble()));
            View.tmpButton.onClick.AddListener(Check);
            
            View.tip.text = string.Empty;
            var sizeDelta = View.progressBg.sizeDelta;
            View.bgWidth = (int) sizeDelta.x;
            View.bgHeight = (int) sizeDelta.y;
        }

        public override void ReOpen() { }

        public override void FixedUpdate()
        {
            View.percent = Mathf.Clamp(View.percent, 0, 100);
            var width = Mathf.Clamp(View.percent / 100f * View.bgWidth, 32, View.bgWidth);
            View.progressPercent.sizeDelta = new Vector2(width, View.bgHeight);
        }

        private async void Check()
        {
            #region CheckForCatalogUpdates

            var checkAsync = Addressables.CheckForCatalogUpdates(false);
            await checkAsync.Task;

            if (checkAsync.Status == AsyncOperationStatus.Succeeded)
            {
                catalogsNeedUpdate ??= new List<string>();
                catalogsNeedUpdate.Clear();
                foreach (var updateCatalog in checkAsync.Result)
                {
                    catalogsNeedUpdate.Add(updateCatalog);
                }
            }
            else
            {
                Print($"CheckForCatalogUpdates Failed: ==> {checkAsync.OperationException}");
            }

            Addressables.Release(checkAsync);

            #endregion

            //------------------------------------------------------------------------

            #region UpdateCatalogs

            Print($"待更新数量 : catalogsNeedUpdate: {catalogsNeedUpdate.Count}!");

            if (catalogsNeedUpdate.Count <= 0)
            {
                Print("已是最新版本!");
                return;
            }

            updateAsync = Addressables.UpdateCatalogs(catalogsNeedUpdate, false);
            await updateAsync.Task;

            if (updateAsync.OperationException != null)
            {
                Print($"UpdateCatalogs Error: {updateAsync.OperationException}");
                Addressables.Release(updateAsync);
                return;
            }

            #endregion

            //------------------------------------------------------------------------

            #region GetDownloadSizeAsync

            var keys = new List<object>();
            foreach (var locator in updateAsync.Result)
            {
                keys.AddRange(locator.Keys);
            }

            var downloadSizeAsync = Addressables.GetDownloadSizeAsync((IEnumerable<object>) keys);
            await downloadSizeAsync.Task;

            if (downloadSizeAsync.OperationException != null)
            {
                Print($"GetDownloadSizeAsync Error! locator : {downloadSizeAsync.OperationException}");
                Addressables.Release(updateAsync);
                Addressables.Release(downloadSizeAsync);
                return;
            }

            // 单位: Result 的单位为字节 Byte, 转换为 KB
            downloadSize = downloadSizeAsync.Result / 1024f;
            Addressables.Release(downloadSizeAsync);

            #endregion

            //------------------------------------------------------------------------

            #region DownloadDependenciesAsync

            downloadAsync = Addressables.DownloadDependenciesAsync((IEnumerable<object>) keys, Addressables.MergeMode.Union);
            View.StartCoroutine(Percent());
            
            #endregion
        }

        private IEnumerator Percent()
        {
            while (downloadAsync.Status == AsyncOperationStatus.None)
            {
                var percent = (int) downloadAsync.GetDownloadStatus().Percent * 100;
                View.percent = percent;
                Print($"正在下载, 大小: {downloadSize} KB, 进度: {percent}");

                yield return null;
            }
            
            Addressables.Release(updateAsync);
            Addressables.Release(downloadAsync);
            
            Enter();
        }

        private void Enter()
        {
            Print("更新完毕, 正在进入游戏...");
            Addressables.LoadAssetAsync<GameObject>("UI_Update_Version").Completed += asyncOperationHandle =>
            {
                UnityEngine.Object.Instantiate(asyncOperationHandle.Result);
                Addressables.Release(asyncOperationHandle);
            };
        }

        private void Print(string log)
        {
            View.tip.text = log;
            View.tmpLog.text += $"{log}\n";
        }
    }
}
