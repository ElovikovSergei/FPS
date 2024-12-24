using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MiniContainer;
using System.Linq;
using System;

namespace FPS.Bootstrap
{
    public abstract class Bootstrap : SubContainer
    {
        private List<LoadingFlowElement> _loadingFlowList;
        private Dictionary<Type, ILoadingSegment> _segments;

        private bool _allSegmentsAreLoaded;
        private int _readySegmentsCount;

        protected override void Register(IBaseDIService builder)
        {
            _loadingFlowList = new List<LoadingFlowElement>();

            UpdateProgressor(0);
            RegisterLoadingFlow();

            foreach (LoadingFlowElement loadFlowElement in _loadingFlowList)
            {
                builder.Register(loadFlowElement.Segment, ServiceLifeTime.Transient);
            }
        }

        protected override void Resolve()
        {
            _segments = new Dictionary<Type, ILoadingSegment>();

            foreach (LoadingFlowElement loadingFlowElement in _loadingFlowList)
            {
                var segment = (ILoadingSegment)DIContainer.Resolve(loadingFlowElement.Segment);

                segment.OnSegmentReadyEvent += OnSegmentReady;

                _segments.Add(loadingFlowElement.Segment, segment);
            }

            foreach (var item in _loadingFlowList)
            {
                if (item.Dependencies != null && item.Dependencies.Count != 0)
                {
                    var loadSegment = _segments[item.Segment];

                    foreach (var dependency in item.Dependencies)
                    {
                        if (_segments.TryGetValue(dependency.Segment, out var dependencySegment))
                        {
                            loadSegment.AddDependency(dependencySegment);
                        }
                        else
                        {
                            throw new KeyNotFoundException($"Invalid dependency: {dependency.Segment}");
                        }
                    }
                }
            }

            StartLoading().Forget();
        }

        protected bool TryGetSegment<TSegment>(out TSegment segment) where TSegment : ILoadingSegment
        {
            if (_segments.TryGetValue(typeof(TSegment), out var seg))
            {
                segment = (TSegment)seg;
                return true;
            }

            segment = default;
            return false;
        }

        protected bool TryGetSegment(Type segmentType, out ILoadingSegment segment)
        {
            return _segments.TryGetValue(segmentType, out segment);
        }

        protected LoadingFlowElement RegisterLoadingSegment<T>()
        {
            return RegisterLoadingFlowElement(typeof(T));
        }

        protected LoadingFlowElement RegisterLoadingFlowElement(Type elementType)
        {
            var element = new LoadingFlowElement(elementType);

            _loadingFlowList.Add(element);

            return element;
        }

        protected abstract void RegisterLoadingFlow();
        protected virtual void SetProgressorSettings() { }
        protected virtual void UpdateProgressor(float value) { }

        protected virtual void OnReady()
        {

        }

        private void OnSegmentReady(ILoadingSegment obj)
        {
            _allSegmentsAreLoaded = CheckAllSegmentsReady();
            UpdateProgressor(_readySegmentsCount / (float)_segments.Count * 0.9f);
        }

        private async UniTaskVoid StartLoading()
        {
            SetProgressorSettings();

            foreach (var segment in _segments)
            {
                segment.Value.StartLoad().Forget();
            }

            await UniTask.WaitUntil(() => _allSegmentsAreLoaded);

            if (!_allSegmentsAreLoaded)
            {
                await UniTask.WaitUntil(() => _allSegmentsAreLoaded);
            }

            UpdateProgressor(1.0f);
            OnReady();
        }

        private bool CheckAllSegmentsReady()
        {
            _readySegmentsCount = _segments.Count(segment => segment.Value.Ready);
            return _readySegmentsCount == _segments.Count;
        }
    }
}
