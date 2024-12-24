using Cysharp.Threading.Tasks;
using MiniContainer;
using UnityEngine;
using FPS.Common;

namespace FPS.Bootstrap
{
    public sealed class AppBootstrap : Bootstrap
    {
        private AppStateService _stateService;

        protected override void Resolve()
        {
            base.Resolve();

            _stateService = DIContainer.Resolve<AppStateService>();
            _stateService.GoToState(AppStateType.Game, new AppStatePayload()).Forget();
        }

        protected override void RegisterLoadingFlow()
        {
            RegisterLoadingSegment<EmptyLoadingSegment>();
        }

        protected override void UpdateProgressor(float value)
        {

        }

        protected override void OnReady()
        {
            OnReadyAsync().Forget();
        }

        private async UniTaskVoid OnReadyAsync()
        {
            await UniTask.Delay(1);
        }

        protected override void Awake()
        {
            Cursor.visible = false; // to do: del this
            Cursor.lockState = CursorLockMode.Locked; // to do: del this
            Application.targetFrameRate = 60;

            base.Awake();
        }
    }
}