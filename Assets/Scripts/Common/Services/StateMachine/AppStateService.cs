using Cysharp.Threading.Tasks;
using System.Threading;
using StateMachine;

namespace FPS.Common
{
    public sealed class AppStateService
    {
        private readonly IStateMachine<AppStateType> _stateMachine;

        public AppStateService(IStateMachine<AppStateType> stateMachine)
        {
            _stateMachine = stateMachine;

            Initialize();
        }

        public async UniTask GoToState<TPayload>(AppStateType state, TPayload payload)
           where TPayload : IStatePayload
        {
            await _stateMachine.Fire(state, payload, CancellationToken.None);
        }

        private void Initialize()
        {
            //_stateMachine.Register<LoadingState>(AppStateType.Loading)
            //    .AllowTransition(AppStateType.Game);

            //_stateMachine.Register<MenuState>(AppStateType.Menu)
            //    .AllowTransition(AppStateType.Game);

            _stateMachine.Register<GameState>(AppStateType.Game)
                /*.AllowTransition(AppStateType.Menu)*/;
        }
    }
}