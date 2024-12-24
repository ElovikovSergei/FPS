using Cysharp.Threading.Tasks;
using System.Threading;

namespace FPS.Common
{
    public sealed class GameState : AppState<AppStatePayload>
    {
        private readonly PlayerService _playerService;

        public GameState(AppStateAssets appStateAssets, PlayerService playerService) : base(appStateAssets)
        {
            _playerService = playerService;
        }

        public override async UniTask OnEnter(AppStateType trigger, AppStatePayload payload, CancellationToken cancellationToken)
        {
            await base.OnEnter(trigger, payload, cancellationToken);

            PopulateViewService(_playerService);
        }
    }
}