using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace AddressablesLoader
{
    /// <summary>
    /// Load asset by string path
    /// </summary>
    public class AddressablesPathManager
    {
        private Dictionary<string, AsyncOperationHandle> _pathInUse = new Dictionary<string, AsyncOperationHandle>();

        /// <summary>
        /// Loads asseth by path.
        /// You need to cast result to correct type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public AsyncOperationHandle HandleLoadAssetPath<T>(string path)
        {
            if (!_pathInUse.ContainsKey(path))
            {
                AsyncOperationHandle handle = Addressables.LoadAssetAsync<T>(path);
                _pathInUse.Add(path, handle);
            }

            return _pathInUse[path];
        }

        public void ReleasePath(string path)
        {
            if(!_pathInUse.ContainsKey(path)) return;

            Addressables.Release(_pathInUse[path]);
            _pathInUse.Remove(path);
        }
    }
}

