using UnityEngine.InputSystem;
using MiniContainer;
using UnityEngine;
using System;

namespace FPS.Common
{
    public sealed class InputController : MonoBehaviour, IRegistrable
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

#if ENABLE_INPUT_SYSTEM
        #region View
        public void OnLook(InputValue value)
        {
            OnLookEvent?.Invoke(value.Get<Vector2>());
        }
        #endregion

        #region Movement
        public void OnSprint(InputValue value)
        {
            OnSprintEvent?.Invoke(value);
        }

        public void OnMove(InputValue value)
        {
            OnMoveEvent?.Invoke(value.Get<Vector2>());
        }

        public void OnCrouch()
        {
            OnCrouchEvent?.Invoke();
        }

        public void OnSlide()
        {
            OnSlideEvent?.Invoke();
        }

        public void OnJump()
        {
            OnJumpEvent?.Invoke();
        }
        #endregion

        #region Controller
        public void OnToggleAttachmentEditing()
        {
            OnToggleAttachmentEditingEvent?.Invoke();
        }

        public void OnDigitAxis(InputValue value)
        {
            OnDigitAxisEvent?.Invoke(value);
        }

        public void OnFire(InputValue value)
        {
            OnFireEvent?.Invoke(value);
        }

        public void OnAim(InputValue value)
        {
            OnAimEvent?.Invoke(value);
        }

        public void OnChangeFireMode()
        {
            OnChangeFireModeEvent?.Invoke();
        }

        public void OnThrowGrenade()
        {
            OnThrowGrenadeEvent?.Invoke();
        }

        public void OnChangeWeapon()
        {
            OnChangeWeaponEvent?.Invoke();
        }

        public void OnLean(InputValue value)
        {
            OnLeanEvent?.Invoke(value.Get<float>());
        }

        public void OnCycleScope()
        {
            OnCycleScopeEvent?.Invoke();
        }

        public void OnReload()
        {
            OnReloadEvent?.Invoke();
        }
        #endregion
#endif
    }
}