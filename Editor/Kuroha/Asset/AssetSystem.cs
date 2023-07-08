using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Kuroha.Asset
{
    public class AssetSystem
    {
        public async void OnEnable()
        {
            var initAsync = Addressables.LoadAssetAsync<TMP_FontAsset>("Font");
            await initAsync.Task;
            Addressables.Release(initAsync);
        }

        public T Load<T>(string address)
        {
            return Addressables.LoadAssetAsync<T>(address).WaitForCompletion();
        }

        public AsyncOperationHandle<T> LoadAsync<T>(string address)
        {
            return Addressables.LoadAssetAsync<T>(address);
        }
    }
}
