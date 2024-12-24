using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System.Threading;
using StateMachine;
using UnityEngine;

namespace FPS.Common
{
    public abstract class AppState<TPayload> : StateBase<AppStateType, TPayload> where TPayload : AppStatePayload
    {
        protected GameObject[] _instances;

        private readonly AppStateAssets _appStateAssets;

        public AppState(AppStateAssets appStateAssets)
        {
            _appStateAssets = appStateAssets;
        }

        public override async UniTask OnBeforeEnter(AppStateType trigger, TPayload payload, CancellationToken cancellationToken)
        {
            _instances = await _appStateAssets.InstantiateAssetsAsync(trigger);
        }

        public override UniTask OnEnter(AppStateType trigger, TPayload payload, CancellationToken cancellationToken)
        {
            foreach (var instance in _instances)
            {
                instance.SetActive(true);
            }

            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            if (_instances == null || _instances.Length == 0)
            {
                return;
            }

            foreach (var instance in _instances)
            {
                Addressables.ReleaseInstance(instance);
            }

            _instances = null;
        }

        protected virtual void PopulateViewService<TView>(IControllerService<TView> service) where TView : MonoBehaviour
        {
            if (!TryGetInstance(out TView view))
            {
                return;
            }

            service.SetController(view);
            service.Initialize();
        }

        private bool TryGetInstance<T>(out T component) where T : Component
        {
            if (_instances != null)
            {
                for (int i = 0; i < _instances.Length; ++i)
                {
                    if (_instances[i].TryGetComponent(out component))
                    {
                        return true;
                    }
                }
            }

            component = null;
            return false;
        }
    }
}