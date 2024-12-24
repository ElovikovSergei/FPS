using KINEMATION.FPSAnimationFramework.Runtime.Recoil;
using KINEMATION.FPSAnimationFramework.Runtime.Core;
using KINEMATION.KAnimationCore.Runtime.Input;
using UnityEngine.InputSystem;
using UnityEngine;

namespace FPS
{
    public sealed class PlayerViewHandler : MonoBehaviour
    {
        private Vector2 _viewInput;
        private Vector2 _viewDirection;

        private float _sensitivity;
        private int _sensitivityMultiplierPropertyIndex;

        private UserInputController _userInput;
        private RecoilPattern _recoilPattern;

#if ENABLE_INPUT_SYSTEM
        public void OnLook(InputValue value)
        {
            _viewInput = value.Get<Vector2>();
        }
#endif

        public void Initialize(UserInputController userInput, RecoilPattern recoilPattern)
        {
            _userInput = userInput;
            _recoilPattern = recoilPattern;

            _sensitivityMultiplierPropertyIndex = _userInput.GetPropertyIndex("SensitivityMultiplier");
            _sensitivity = 1f;
        }

        private void UpdateView()
        {
            float scale = _userInput.GetValue<float>(_sensitivityMultiplierPropertyIndex);

            float deltaMouseX = _viewInput.x * _sensitivity * scale;
            float deltaMouseY = -_viewInput.y * _sensitivity * scale;

            _viewDirection.y += deltaMouseY;
            _viewDirection.x += deltaMouseX;

            if (_recoilPattern != null)
            {
                _viewDirection += _recoilPattern.GetRecoilDelta();
                deltaMouseX += _recoilPattern.GetRecoilDelta().x;
            }

            float proneWeight = 0f/*_animator.GetFloat(_proneWeightHash)*/;
            var pitchClamp = Vector2.Lerp(new Vector2(-90f, 90f), new Vector2(-30, 0f), proneWeight);

            _viewDirection.y = Mathf.Clamp(_viewDirection.y, pitchClamp.x, pitchClamp.y);

            transform.rotation *= Quaternion.Euler(0f, deltaMouseX, 0f);

            _userInput.SetValue(FPSANames.MouseDeltaInput, new Vector4(deltaMouseX, deltaMouseY));
            _userInput.SetValue(FPSANames.MouseInput, new Vector4(_viewDirection.x, _viewDirection.y));
        }

        private void Update()
        {
            UpdateView();
        }
    }
}