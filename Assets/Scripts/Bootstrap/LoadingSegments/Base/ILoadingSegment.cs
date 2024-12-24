using Cysharp.Threading.Tasks;
using System;

namespace FPS.Bootstrap
{
    public interface ILoadingSegment
    {
        public event Action<ILoadingSegment> OnSegmentReadyEvent;

        public bool Ready { get; }
        public string Stage { get; }
        public float ReadyTime { get; }

        public UniTask Init();
        public UniTaskVoid StartLoad();
        public void AddDependency(ILoadingSegment dependency);
    }
}