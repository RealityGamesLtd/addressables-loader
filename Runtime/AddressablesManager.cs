using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AddressablesLoader;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AddressablesLoader
{
    public class AddressablesManager
    {
        private Dictionary<string, object> _keysGroupsInUse = new Dictionary<string, object>();

        /// <summary>
        /// Load assets by label. If assets are already loaded it will return ReadOnlyCollection immediately.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyGroup"></param>
        /// <returns></returns>
        public async Task<ReadOnlyCollection<T>> LoadAssetGroup<T>(string keyGroup)
        {
            if (_keysGroupsInUse.ContainsKey(keyGroup))
            {
                var handle = _keysGroupsInUse[keyGroup];
                return GetListOfObjectsFromAsyncOperation<T>((AsyncOperationHandle<IList<T>>)handle);
            }
            else
            {
                var handle = Addressables.LoadAssetsAsync<T>(keyGroup);
                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _keysGroupsInUse.Add(keyGroup, handle);
                    Debug.Log($"Addressables label key added to the service: {keyGroup}");
                    return GetListOfObjectsFromAsyncOperation(handle);
                }
                else
                {
                    Debug.LogError($"Addressables: Can't load itmes with label: {keyGroup}");
                }
            }

            return null;
        }

        /// <summary>
        /// If you want to handle operation yourself (track progress) this method is for you.
        /// Remember to use GetListOfObjectsFromAsyncOperation if you want to get the result of this operation (it must be called on succeeded operation).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyGroup"></param>
        /// <returns></returns>

        public AsyncOperationHandle<IList<T>> HandleLoadAssetGroup<T>(string keyGroup)
        {
            if (!_keysGroupsInUse.ContainsKey(keyGroup))
            {
                var handle = Addressables.LoadAssetsAsync<T>(keyGroup);
                _keysGroupsInUse.Add(keyGroup, handle);
            }

            return (AsyncOperationHandle<IList<T>>)_keysGroupsInUse[keyGroup];
        }

        /// <summary>
        /// Loading asset group with callback if succeeded.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyGroup"></param>
        /// <param name="onComplete"></param>
        public void LoadAssetGroupWithCallback<T>(string keyGroup,
        Action<AsyncOperationHandle<IList<T>>> onComplete)
        {
            if (_keysGroupsInUse.ContainsKey(keyGroup))
            {
                var handle = (AsyncOperationHandle<IList<T>>)_keysGroupsInUse[keyGroup];

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    onComplete?.Invoke(handle);
                }
                else
                {
                    handle.Completed += onComplete;
                }
            }
            else
            {
                var handle = Addressables.LoadAssetsAsync<T>(keyGroup);
                handle.Completed += onComplete;
                _keysGroupsInUse.Add(keyGroup, handle);
            }
        }

        /// <summary>
        /// Load group with couroutine. Handle result yourself.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyGroup"></param>
        /// <returns></returns>
        public IEnumerator<AsyncOperationHandle<IList<T>>> LoadGroupCoroutine<T>(string keyGroup)
        {
            if (_keysGroupsInUse.ContainsKey(keyGroup))
            {
                yield return (AsyncOperationHandle<IList<T>>)_keysGroupsInUse[keyGroup];
            }
            else
            {
                var handle = Addressables.LoadAssetsAsync<T>(keyGroup);
                _keysGroupsInUse.Add(keyGroup,handle);
                yield return Addressables.LoadAssetsAsync<T>(keyGroup);
            }
        }

        public void UnloadAssetGroup<T>(string keyGroup)
        {
            if (_keysGroupsInUse.ContainsKey(keyGroup))
            {
                var handle = (AsyncOperationHandle<IList<T>>)_keysGroupsInUse[keyGroup];
                Addressables.Release(handle);
                _keysGroupsInUse.Remove(keyGroup);
                Debug.Log($"Unload label: {keyGroup}");
            }
        }

        /// <summary>
        /// Recommended function for getting assets from AsyncOperationHandle<IList<T>> if you handle loading yourself.
        /// Returns ReadOnlyCollection when async operation status is not succeeded.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asyncOperation"></param>
        /// <returns></returns>
        public ReadOnlyCollection<T> GetListOfObjectsFromAsyncOperation<T>(AsyncOperationHandle<IList<T>> asyncOperation)
        {
            if (asyncOperation.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Async operation has not status succeeded. Return ReadOnlyCollection with 0 items.");
                return new ReadOnlyCollection<T>(new List<T>());
            }

            List<T> result = new List<T>();

            for (int i = 0; i < asyncOperation.Result.Count; i++)
            {
                result.Add(asyncOperation.Result[i]);
            }

            return new ReadOnlyCollection<T>(result);
        }
    }
}