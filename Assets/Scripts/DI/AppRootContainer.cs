using FPS.Common.Core;
using MiniContainer;
using StateMachine;
using FPS.Common;
using FPS.Utils;

namespace FPS.DI
{
    public sealed class AppRootContainer : RootContainer
    {
        protected override void Register(IBaseDIService builder)
        {
            NativeHeapUtils.ReserveMegabytes(10);

            builder.RegisterStateMachine<AppStateType>(DIContainer);

            builder.RegisterSingleton<AppStateService>();
            builder.RegisterSingleton<InputService>();

            // player
            builder.RegisterSingleton<PlayerService>();
            builder.RegisterSingleton<IPlayerViewService, PlayerViewService>();

            // core
            builder.RegisterSingleton<InputPropertiesService>();
            builder.RegisterSingleton<IRecoilPatternService, RecoilPatternService>();

            // app states
            builder.RegisterScoped<GameState>();
        }
    }
}