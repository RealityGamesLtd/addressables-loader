using UnityEngine;
using UnityEngine.UI;

namespace AddressablesLoader.Example
{
    public class ExampleLoadGroup : MonoBehaviour
    {
        const string LOCAL_GROUP_LABEL = "Local";
        const string REMOTE_GROUP_LABEL = "Remote";

        [SerializeField] private Button _loadLocalGroup;
        [SerializeField] private Button _unloadLocalGroup;
        private AddressablesManager _addressableManager;

        void Awake()
        {
            _addressableManager = new AddressablesManager();

            _loadLocalGroup.onClick.AddListener(() =>
            {
                LoadAssetGroup();
            });

            _unloadLocalGroup.onClick.AddListener(() =>
            {
                UnloadGroup();
            });
        }

        async void LoadAssetGroup()
        {
            var handle = _addressableManager.LoadAssetGroup<Sprite>(LOCAL_GROUP_LABEL);
            await handle;
            var collection = handle.Result;

            for (int i = 0; i < collection.Count; i++)
            {
                GameObject go = new GameObject($"Sprite {i}");
                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = collection[i];
                const float range = 2f;
                float x = UnityEngine.Random.Range(-range, range);
                float y = UnityEngine.Random.Range(-range, range);
                go.transform.position = new Vector3(x, y, 0f);
                go.transform.localScale = new Vector3(.5f, .5f, .5f);
            }
        }

        void UnloadGroup()
        {
            _addressableManager.UnloadAssetGroup<Sprite>(LOCAL_GROUP_LABEL);
        }
    }
}