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
            builder.RegisterSingleton<PlayerService>();
            builder.RegisterSingleton<InputService>();

            // app states
            builder.RegisterScoped<GameState>();
        }
    }
}