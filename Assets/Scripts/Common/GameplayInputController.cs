using UnityEngine.InputSystem;
using MiniContainer;
using UnityEngine;
using System;

namespace FPS.Common
{
    public sealed class GameplayInputController : MonoBehaviour, IRegistrable
    {
        public event Action<Vector2> OnLookEvent;

        public event Action<InputValue> OnSprintEvent;
        public event Action<Vector2> OnMoveEvent;
        public event Action OnCrouchEvent;
        public event Action OnSlideEvent;
        public event Action OnJumpEvent;

        public event Action OnToggleAttachmentEditingEvent;
        public event Action<InputValue> OnDigitAxisEvent;
        public event Action<InputValue> OnFireEvent;
        public event Action<InputValue> OnAimEvent;
        public event Action OnChangeFireModeEvent;
        public event Action OnThrowGrenadeEvent;
        public event Action OnChangeWeaponEvent;
        public event Action<float> OnLeanEvent;
        public event Action OnCycleScopeEvent;
        public event Action OnReloadEvent;

        private bool _isInputEnable;

        public void EnableInput()
        {
            _isInputEnable = true;
        }

        public void DisableInput()
        {
            _isInputEnable = false;
        }

#if ENABLE_INPUT_SYSTEM
        #region View
        public void OnLook(InputValue value)
        {
            if (!_isInputEnable)
            {
                return;
            }

            OnLookEvent.Invoke(value.Get<Vector2>());
        }
        #endregion

        #region Movement
        public void OnSprint(InputValue value)
        {
            if (!_isInputEnable)
            {
                return;
            }

            OnSprintEvent.Invoke(value);
        }

        public void OnMove(InputValue value)
        {
            if (!_isInputEnable)
            {
                return;
            }

            OnMoveEvent.Invoke(value.Get<Vector2>());
        }

        public void OnCrouch()
        {
            if (!_isInputEnable)
            {
                return;
            }

            OnCrouchEvent.Invoke();
        }

        public void OnSlide()
        {
            if (!_isInputEnable)
            {
                return;
            }

            OnSlideEvent.Invoke();
        }

        public void OnJump()
        {
            if (!_isInputEnable)
            {
                return;
            }

            OnJumpEvent.Invoke();
        }
        #endregion

        #region Controller
        public void OnToggleAttachmentEditing()
        {
            if (!_isInputEnable)
            {
                return;
            }

            OnToggleAttachmentEditingEvent.Invoke();
        }

        public void OnDigitAxis(InputValue value)
        {
            if (!_isInputEnable)
            {
                return;
            }

            OnDigitAxisEvent.Invoke(value);
        }

        public void OnFire(InputValue value)
        {
            if (!_isInputEnable)
            {
                return;
            }

            OnFireEvent.Invoke(value);
        }

        public void OnAim(InputValue value)
        {
            if (!_isInputEnable)
            {
                return;
            }

            OnAimEvent.Invoke(value);
        }

        public void OnChangeFireMode()
        {
            if (!_isInputEnable)
            {
                return;
            }

            OnChangeFireModeEvent.Invoke();
        }

        public void OnThrowGrenade()
        {
            if (!_isInputEnable)
            {
                return;
            }

            OnThrowGrenadeEvent.Invoke();
        }

        public void OnChangeWeapon()
        {
            if (!_isInputEnable)
            {
                return;
            }

            OnChangeWeaponEvent.Invoke();
        }

        public void OnLean(InputValue value)
        {
            if (!_isInputEnable)
            {
                return;
            }

            OnLeanEvent.Invoke(value.Get<float>());
        }

        public void OnCycleScope()
        {
            if (!_isInputEnable)
            {
                return;
            }

            OnCycleScopeEvent.Invoke();
        }

        public void OnReload()
        {
            if (!_isInputEnable)
            {
                return;
            }

            OnReloadEvent.Invoke();
        }
        #endregion
#endif
    }
}