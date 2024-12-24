using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

namespace FPS.Bootstrap
{
    public class LoadingSegment : ILoadingSegment
    {
        public event Action<ILoadingSegment> OnSegmentReadyEvent;

        public bool Ready { get; private set; }
        public string Stage { get; protected set; }
        public float ReadyTime { get; private set; }

        private List<ILoadingSegment> _dependencies;
        private float _startInitTime;

        public void AddDependency(ILoadingSegment dependency)
        {
            if (dependency == null)
            {
                throw new ArgumentNullException(nameof(dependency));
            }
            if (dependency == this)
            {
                throw new ArgumentException("Circular dependency", nameof(dependency));
            }

            _dependencies ??= new List<ILoadingSegment>();
            _dependencies.Add(dependency);
        }

        public virtual UniTask Init()
        {
            return UniTask.CompletedTask;
        }

        public async UniTaskVoid StartLoad()
        {
            Stage = "PreInit";
            await PreInit();
            Stage = "Init";
            await Init();
            Stage = "PostInit";
            PostInit();
        }

        private async UniTask PreInit()
        {
            _startInitTime = Time.realtimeSinceStartup;

            if (_dependencies != null && _dependencies.Count != 0)
            {
                await WaitForDependencies();
            }
        }

        private void PostInit()
        {
            Ready = true;
            ReadyTime = Time.realtimeSinceStartup - _startInitTime;
            OnSegmentReadyEvent?.Invoke(this);
        }

        private async UniTask WaitForDependencies()
        {
            await UniTask.WaitWhile(() => _dependencies.Exists(segment => !segment.Ready));
        }
    }
}