using Cysharp.Threading.Tasks;

namespace FPS.Bootstrap
{
    public sealed class EmptyLoadingSegment : LoadingSegment
    {
        public override async UniTask Init()
        {
            await UniTask.Delay(1000);
        }
    }
}