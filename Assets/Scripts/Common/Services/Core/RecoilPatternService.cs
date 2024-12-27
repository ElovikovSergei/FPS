using MiniContainer;
using FPS.Settings;
using UnityEngine;

namespace FPS.Common.Core
{
    public interface IRecoilPatternService
    {
        void Initialize(RecoilPatternSettings settings);

        void OnFireStart();
        void OnFireEnd();
        void Update();

        Vector2 GetRecoilDelta();
    }


    [HelpURL("https://kinemation.gitbook.io/scriptable-animation-system/recoil-system/recoil-pattern")]
    public sealed class RecoilPatternService : IRecoilPatternService, IContainerUpdateListener
    {
        private readonly InputPropertiesService _inputProperties;
        private RecoilPatternSettings _settings;

        private Vector2 _accumulatedRecoil;
        private Vector2 _targetRecoil;
        private Vector2 _cachedRecoil;
        private Vector2 _compensation;
        private Vector2 _recoil;

        private bool _isFiring;
        private bool _isInitialized;

        private int _deltaLookInputPropertyIndex;

        public RecoilPatternService(InputPropertiesService inputProperties)
        {
            _inputProperties = inputProperties;
        }

        public void Initialize(RecoilPatternSettings settings)
        {
            _settings = settings;
            _deltaLookInputPropertyIndex = _inputProperties.GetPropertyIndex(settings.DeltaLookInputProperty);

            ResetRecoilValues();

            _isInitialized = true;
        }

        public void OnFireStart()
        {
            if (!_isFiring)
            {
                _compensation = _accumulatedRecoil = Vector2.zero;
            }

            _isFiring = true;

            _targetRecoil.x += Random.Range(_settings.HorizontalRecoil.x, _settings.HorizontalRecoil.y);
            _targetRecoil.y += Random.Range(_settings.VerticalRecoil.x, _settings.VerticalRecoil.y);
        }

        public void OnFireEnd()
        {
            _isFiring = false;

            _recoil.x *= Compensate(_recoil.x, _compensation.x);
            _recoil.y *= Compensate(_recoil.y, _compensation.y);

            _cachedRecoil = _recoil;
            _targetRecoil = _recoil;
        }

        public Vector2 GetRecoilDelta()
        {
            return _accumulatedRecoil;
        }

        public void Update()
        {
            if (!_isInitialized)
            {
                return;
            }

            if (_isFiring)
            {
                // Accumulate player delta input when firing.
                Vector4 deltaInput = _inputProperties.GetValue<Vector4>(_deltaLookInputPropertyIndex);

                _compensation.x += deltaInput.x;
                _compensation.y += deltaInput.y;
            }

            float alpha = Math.ExpDecayAlpha(_settings.HorizontalSmoothing, Time.deltaTime);
            _recoil.x = Mathf.Lerp(_recoil.x, _targetRecoil.x, alpha);

            alpha = Math.ExpDecayAlpha(_settings.VerticalSmoothing, Time.deltaTime);
            _recoil.y = Mathf.Lerp(_recoil.y, _targetRecoil.y, alpha);

            if (!_isFiring)
            {
                alpha = Math.ExpDecayAlpha(_settings.Damping, Time.deltaTime);
                _targetRecoil = Vector2.Lerp(_targetRecoil, Vector2.zero, alpha);
            }

            _accumulatedRecoil = _recoil - _cachedRecoil;
            _cachedRecoil = _recoil;
        }

        private float Compensate(float recoil, float compensation)
        {
            float multiplier = 1f;
            bool isOpposite = recoil * compensation <= 0f;

            if (!Mathf.Approximately(compensation, 0f) && isOpposite)
            {
                multiplier -= Mathf.Clamp01(Mathf.Abs(compensation / recoil));
            }

            return multiplier;
        }

        private void ResetRecoilValues()
        {
            _accumulatedRecoil = Vector2.zero;
            _targetRecoil = Vector2.zero;
            _cachedRecoil = Vector2.zero;
            _compensation = Vector2.zero;
            _recoil = Vector2.zero;
        }
    }
}