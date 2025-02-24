using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace  AddressablesLoader
{
    //Class repsonsible for loading assets from bundles by its reference.
    public static class LoadAssetByReference
    {
        /// <summary>
        /// Load asset async. Returns object if succeeded,null if failed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetReference"></param>
        /// <returns></returns>
        public static async Task<T> LoadAssetAsync<T>(AssetReference assetReference)
        {
            AsyncOperationHandle<T> handle = assetReference.LoadAssetAsync<T>();
            await handle.Task;
            return HandleAsyncOperation(handle);
        }

        /// <summary>
        /// Returns Task<AsyncOperationHandle> (async) so you can handle result yourself.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetReference"></param>
        /// <returns></returns>
        public static async Task<AsyncOperationHandle<T>> LoadAssetAsyncTask<T>(AssetReference assetReference)
        {
            AsyncOperationHandle<T> handle = assetReference.LoadAssetAsync<T>();
            await handle.Task;
            return handle;
        }

        /// <summary>
        /// Returns AsyncOperationHandle (without await) for asset reference if you want to handle/control loading yourself.
        /// You can't cancel loading Addressables but you can decrease reference count so when addressables is loaded will be unloaded automatically.
        /// To decrease reference count you need to use handle.Release();
        /// AsyncOperationHandle also allows you to track progress.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetReference"></param>
        /// <returns></returns>
        public static AsyncOperationHandle<T> LoadAssetAsyncHandleOperation<T>(AssetReference assetReference)
        {
            AsyncOperationHandle<T> handle = assetReference.LoadAssetAsync<T>();
            return handle;
        }

        /// <summary>
        /// Load assetReference with your callback.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetReference"></param>
        /// <param name="onComplete"></param>
        public static void LoadAssetByCallback<T>(AssetReference assetReference,
        System.Action<AsyncOperationHandle<T>> onComplete)
        {
            var handle = assetReference.LoadAssetAsync<T>();
            handle.Completed += onComplete;
        }

        /// <summary>
        /// Load sprite with couroutine. You can still handle result yourself.
        /// </summary>
        /// <param name="referenceSprite"></param>
        /// <returns></returns>
        public static IEnumerator<AsyncOperationHandle<T>> LoadSpriteCoroutine<T>(AssetReference assetReference)
        {
            var handle = assetReference.LoadAssetAsync<T>();
            yield return handle;
        }

        public static T HandleAsyncOperation<T>(AsyncOperationHandle<T> operationHandle)
        {
            if (operationHandle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"Addressable: loading operation failed");
            }

            if (operationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Addressable: loading operation succeeded");
            }

            return operationHandle.Result;
        }

        public static void UnloadAssetReference(AssetReference assetReference)
        {
            assetReference.ReleaseAsset();
        }

        public static void UnloadHandleAsyncOperation(AsyncOperationHandle asyncOperationHandle)
        {
            Addressables.Release(asyncOperationHandle);
        }
    }
}

