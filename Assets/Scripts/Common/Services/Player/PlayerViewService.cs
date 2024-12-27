using KINEMATION.FPSAnimationFramework.Runtime.Core;
using FPS.Common.Core;
using MiniContainer;
using UnityEngine;

namespace FPS.Common
{
    public interface IPlayerViewService : IControllerService<PlayerViewHandler>
    {
        void OnLook(Vector2 value);
    }


    public sealed class PlayerViewService : IPlayerViewService, IContainerUpdateListener
    {
        private readonly InputPropertiesService _inputProperties;
        private readonly RecoilPatternService _recoilPattern;

        private PlayerViewHandler _controller;

        private Vector2 _viewInput;
        private Vector2 _viewDirection;

        private float _sensitivity;
        private int _sensitivityMultiplierPropertyIndex;

        public PlayerViewService(InputPropertiesService inputProperties, RecoilPatternService recoilPattern)
        {
            _inputProperties = inputProperties;
            _recoilPattern = recoilPattern;

            Initialize();
        }

        public void Initialize()
        {
            _sensitivityMultiplierPropertyIndex = _inputProperties.GetPropertyIndex("SensitivityMultiplier");
            _sensitivity = 1f;
        }

        public void SetController(PlayerViewHandler controller)
        {
            _controller = controller;
        }

        public void OnLook(Vector2 value)
        {
            _viewInput = value;
        }

        public void Update()
        {
            float scale = _inputProperties.GetValue<float>(_sensitivityMultiplierPropertyIndex);

            float deltaMouseX = _viewInput.x * _sensitivity * scale;
            float deltaMouseY = -_viewInput.y * _sensitivity * scale;

            _viewDirection.y += deltaMouseY;
            _viewDirection.x += deltaMouseX;

            _viewDirection += _recoilPattern.GetRecoilDelta();
            deltaMouseX += _recoilPattern.GetRecoilDelta().x;

            float proneWeight = 0f/*_animator.GetFloat(_proneWeightHash)*/;
            var pitchClamp = Vector2.Lerp(new Vector2(-90f, 90f), new Vector2(-30, 0f), proneWeight);

            _viewDirection.y = Mathf.Clamp(_viewDirection.y, pitchClamp.x, pitchClamp.y);

            _controller.UpdateLook(deltaMouseX);

            _inputProperties.SetValue(FPSANames.MouseDeltaInput, new Vector4(deltaMouseX, deltaMouseY));
            _inputProperties.SetValue(FPSANames.MouseInput, new Vector4(_viewDirection.x, _viewDirection.y));
        }
    }
}