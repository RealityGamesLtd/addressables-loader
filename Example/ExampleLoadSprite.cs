using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace AddressablesLoader.Example
{
    public class ExampleLoadSprite : MonoBehaviour
    {
        [SerializeField] private AssetReferenceSprite _referenceSpriteLocal;
        [SerializeField] private AssetReferenceSprite _referenceSpriteRemote;
        [SerializeField] private SpriteRenderer _spriteRendererLocal;
        [SerializeField] private SpriteRenderer _spriteRendererRemote;
        [SerializeField] private Button _buttonLocal;
        [SerializeField] private Button _buttonRemote;
        [SerializeField] private Button _buttonUnloadLocal;
        [SerializeField] private Button _buttonUnloadRemote;

        void Awake()
        {
            _buttonLocal.onClick.AddListener(() =>
            {
                SpriteLoading("Loading local",_referenceSpriteLocal,_spriteRendererLocal);
            });

            _buttonRemote.onClick.AddListener(() =>
            {
                SpriteLoading("Loading remote",_referenceSpriteRemote,_spriteRendererRemote);
            });

            _buttonUnloadLocal.onClick.AddListener(() =>
            {
                ReleaseSpriteLocal();
            });

            _buttonUnloadRemote.onClick.AddListener(() =>
            {
                ReleaseSpriteRemote();
            });
        }

        private void ReleaseSpriteLocal()
        {
            Debug.Log("Unloading asset local");
            LoadAssetByReference.UnloadAssetReference(_referenceSpriteLocal);
        }

        private void ReleaseSpriteRemote()
        {
            Debug.Log("Unloading asset remote");
            LoadAssetByReference.UnloadAssetReference(_referenceSpriteRemote);
        }

        private async Task SpriteLoading(string label,AssetReferenceSprite assetReferenceSprite,
        SpriteRenderer spriteRenderer)
        {
            Debug.Log(label);
            var location = Addressables.LoadResourceLocationsAsync(assetReferenceSprite);
            location.Completed += (listlocation) =>
            {
                Debug.Log(listlocation.DebugName);
            };

            spriteRenderer.sprite = await LoadAssetByReference.LoadAssetAsync<Sprite>(assetReferenceSprite);
        }
    }
}
