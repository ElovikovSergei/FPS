using System;

namespace FPS.Common
{
    public sealed class PlayerService : IControllerService<PlayerController>, IDisposable
    {
        private PlayerController _controller;

        private readonly InputService _input;

        public PlayerService(InputService input)
        {
            _input = input;
        }

        public void SetController(PlayerController controller)
        {
            _controller = controller;
        }

        public void Initialize()
        {
            AddInputListeners();
        }

        public void Dispose()
        {
            RemoveInputListeners();
        }

        private void AddInputListeners()
        {
            _input.GameplayInput.OnLookEvent += _controller.OnLook;

            _input.GameplayInput.OnSprintEvent += _controller.OnSprint;
            _input.GameplayInput.OnCrouchEvent += _controller.OnCrouch;
            _input.GameplayInput.OnSlideEvent += _controller.OnSlide;
            _input.GameplayInput.OnJumpEvent += _controller.OnJump;
            _input.GameplayInput.OnMoveEvent += _controller.OnMove;

            _input.GameplayInput.OnToggleAttachmentEditingEvent += _controller.OnToggleAttachmentEditing;
            _input.GameplayInput.OnChangeFireModeEvent += _controller.OnChangeFireMode;
            _input.GameplayInput.OnChangeWeaponEvent += _controller.OnChangeWeapon;
            _input.GameplayInput.OnThrowGrenadeEvent += _controller.OnThrowGrenade;
            _input.GameplayInput.OnCycleScopeEvent += _controller.OnCycleScope;
            _input.GameplayInput.OnDigitAxisEvent += _controller.OnDigitAxis;
            _input.GameplayInput.OnReloadEvent += _controller.OnReload;
            _input.GameplayInput.OnFireEvent += _controller.OnFire;
            _input.GameplayInput.OnLeanEvent += _controller.OnLean;
            _input.GameplayInput.OnAimEvent += _controller.OnAim;

            _input.GameplayInput.EnableInput();
        }

        private void RemoveInputListeners()
        {
            _input.GameplayInput.DisableInput();

            _input.GameplayInput.OnLookEvent -= _controller.OnLook;

            _input.GameplayInput.OnSprintEvent -= _controller.OnSprint;
            _input.GameplayInput.OnCrouchEvent -= _controller.OnCrouch;
            _input.GameplayInput.OnSlideEvent -= _controller.OnSlide;
            _input.GameplayInput.OnJumpEvent -= _controller.OnJump;
            _input.GameplayInput.OnMoveEvent -= _controller.OnMove;

            _input.GameplayInput.OnToggleAttachmentEditingEvent -= _controller.OnToggleAttachmentEditing;
            _input.GameplayInput.OnChangeFireModeEvent -= _controller.OnChangeFireMode;
            _input.GameplayInput.OnChangeWeaponEvent -= _controller.OnChangeWeapon;
            _input.GameplayInput.OnThrowGrenadeEvent -= _controller.OnThrowGrenade;
            _input.GameplayInput.OnCycleScopeEvent -= _controller.OnCycleScope;
            _input.GameplayInput.OnDigitAxisEvent -= _controller.OnDigitAxis;
            _input.GameplayInput.OnReloadEvent -= _controller.OnReload;
            _input.GameplayInput.OnFireEvent -= _controller.OnFire;
            _input.GameplayInput.OnLeanEvent -= _controller.OnLean;
            _input.GameplayInput.OnAimEvent -= _controller.OnAim;
        }
    }
}