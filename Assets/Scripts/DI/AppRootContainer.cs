using MiniContainer;
using FPS.Utils;

namespace FPS.DI
{
    public class AppRootContainer : RootContainer
    {
        protected override void Register(IBaseDIService builder)
        {
            NativeHeapUtils.ReserveMegabytes(10);
        }
    }
}