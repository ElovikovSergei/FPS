using KINEMATION.FPSAnimationFramework.Runtime.Recoil;
using KINEMATION.FPSAnimationFramework.Runtime.Core;
using KINEMATION.KAnimationCore.Runtime.Input;
using System.Collections.Generic;
using Demo.Scripts.Runtime.Item;
using UnityEngine.InputSystem;
using UnityEngine;

namespace FPS
{
    public sealed class PlayerController : MonoBehaviour
    {
        private const string LOOK_LAYER_WEIGHT = "LookLayerWeight";

        public bool HasActiveAction
        {
            get
            {
                return _actionState != ActionState.None;
            }
        }

        public bool IsAiming
        {
            get
            {
                return _aimState == AimState.Aiming
                    || _aimState == AimState.PointAiming;
            }
        }

        [SerializeField] private PlayerMovementHandler _movementHandler;
        [SerializeField] private PlayerViewHandler _viewHandler;

        [Space(10)]
        [SerializeField] private PlayerControllerSettings _controllerSettings;
        [SerializeField] private UserInputController _userInput;
        [SerializeField] private RecoilPattern _recoilPattern;
        [SerializeField] private FPSAnimator _fpsAnimator;

        [Space(10)]
        [SerializeField] private CharacterController _controller;
        [SerializeField] private Animator _animator;

        [Space(10)]
        [SerializeField] private Transform _weaponBoneTransform;

        private ActionState _actionState;
        private AimState _aimState;

        private int _previousWeaponIndex;
        private int _currentWeaponIndex;

        private List<FPSItem> _instantiatedWeapons;

        #region Input
#if ENABLE_INPUT_SYSTEM
        public void OnReload()
        {
            if (_movementHandler.IsSprinting || HasActiveAction
                || !GetActiveItem().OnReload())
            {
                return;
            }

            _actionState = ActionState.PlayingAnimation;
        }

        public void OnThrowGrenade()
        {
            if (_movementHandler.IsSprinting || HasActiveAction
                || !GetActiveItem().OnGrenadeThrow())
            {
                return;
            }

            _actionState = ActionState.PlayingAnimation;
        }

        public void OnFire(InputValue value)
        {
            if (_movementHandler.IsSprinting)
            {
                return;
            }

            if (value.isPressed)
            {
                Fire();
                return;
            }

            ReleaseFire();
        }

        public void OnAim(InputValue value)
        {
            if (_movementHandler.IsSprinting)
            {
                return;
            }

            if (value.isPressed && !IsAiming)
            {
                if (GetActiveItem().OnAimPressed())
                {
                    _aimState = AimState.Aiming;
                }

                PlayTransitionMotion(_controllerSettings.AimingMotion);

                return;
            }

            if (!value.isPressed && IsAiming)
            {
                ReleaseAim();
                PlayTransitionMotion(_controllerSettings.AimingMotion);
            }
        }

        public void OnChangeWeapon()
        {
            if (_movementHandler.IsProne)
            {
                return;
            }

            if (HasActiveAction || _instantiatedWeapons.Count == 0)
            {
                return;
            }

            ChangeWeapon(_currentWeaponIndex + 1 > _instantiatedWeapons.Count - 1
                ? 0
                : _currentWeaponIndex + 1);
        }

        public void OnLean(InputValue value)
        {
            _userInput.SetValue(FPSANames.LeanInput, value.Get<float>() * _controllerSettings.LeanAngle);
            PlayTransitionMotion(_controllerSettings.LeanMotion);
        }

        public void OnCycleScope()
        {
            if (!IsAiming)
            {
                return;
            }

            GetActiveItem().OnCycleScope();
            PlayTransitionMotion(_controllerSettings.AimingMotion);
        }

        public void OnChangeFireMode()
        {
            GetActiveItem().OnChangeFireMode();
        }

        public void OnToggleAttachmentEditing()
        {
            if (HasActiveAction && _actionState != ActionState.AttachmentEditing)
            {
                return;
            }

            _actionState = _actionState == ActionState.AttachmentEditing
                ? ActionState.None
                : ActionState.AttachmentEditing;

            if (_actionState == ActionState.AttachmentEditing)
            {
                _animator.CrossFade(AnimatorParameters.InspectStart, 0.2f);

                return;
            }

            _animator.CrossFade(AnimatorParameters.InspectEnd, 0.3f);
        }

        public void OnDigitAxis(InputValue value)
        {
            if (!value.isPressed || _actionState != ActionState.AttachmentEditing)
            {
                return;
            }

            GetActiveItem().OnAttachmentChanged((int)value.Get<float>());
        }
#endif
        #endregion

        public void ResetAction()
        {
            _actionState = ActionState.None;
        }

        #region Event handlers
        private void OnSprintStarted()
        {
            ReleaseFire();
            ReleaseAim();

            _aimState = AimState.None;

            _userInput.SetValue(FPSANames.StabilizationWeight, 0f);
            _userInput.SetValue(LOOK_LAYER_WEIGHT, 0.3f);
        }

        private void OnSprintEnded()
        {
            _userInput.SetValue(FPSANames.StabilizationWeight, 1f);
            _userInput.SetValue(LOOK_LAYER_WEIGHT, 1f);
        }

        private void OnSlideStarted()
        {
            _animator.CrossFade(AnimatorParameters.Slide, 0.2f);
        }

        private void OnMoveEnded()
        {
            PlayTransitionMotion(_controllerSettings.StopMotion);
        }

        private void OnCrouched(bool inCrouch)
        {
            PlayTransitionMotion(_controllerSettings.CrouchingMotion);
        }

        private void OnLanded()
        {
            PlayTransitionMotion(_controllerSettings.JumpingMotion);
        }

        private void OnJumped()
        {
            PlayTransitionMotion(_controllerSettings.JumpingMotion);
        }
        #endregion

        private bool NoneActiveAction()
        {
            return !HasActiveAction;
        }

        private FPSItem GetActiveItem()
        {
            return _instantiatedWeapons[_currentWeaponIndex];
        }

        private void Fire()
        {
            if (_instantiatedWeapons.Count == 0)
            {
                return;
            }

            if (HasActiveAction)
            {
                return;
            }

            GetActiveItem().OnFirePressed();
        }

        private void ReleaseFire()
        {
            if (_instantiatedWeapons.Count == 0)
            {
                return;
            }

            GetActiveItem().OnFireReleased();
        }

        private void ReleaseAim()
        {
            if (_instantiatedWeapons.Count == 0)
            {
                return;
            }

            if (!GetActiveItem().OnAimReleased())
            {
                return;
            }

            _aimState = AimState.None;
        }

        private void ChangeWeapon(int newWeaponIndex)
        {
            if (newWeaponIndex == _currentWeaponIndex || newWeaponIndex > _instantiatedWeapons.Count - 1)
            {
                return;
            }

            UnequipWeapon();
            ReleaseFire();

            Invoke(nameof(EquipWeapon), _controllerSettings.EquipDelay);

            _previousWeaponIndex = _currentWeaponIndex;
            _currentWeaponIndex = newWeaponIndex;
        }

        private void EquipWeapon()
        {
            if (_instantiatedWeapons.Count == 0)
            {
                return;
            }

            _instantiatedWeapons[_previousWeaponIndex].gameObject.SetActive(false);

            var currentWeapon = GetActiveItem();

            currentWeapon.gameObject.SetActive(true);
            currentWeapon.OnEquip(gameObject);

            _actionState = ActionState.None;
        }

        private void UnequipWeapon()
        {
            ReleaseAim();

            _actionState = ActionState.WeaponChange;

            GetActiveItem().OnUnEquip();
        }

        private void PlayTransitionMotion(FPSAnimatorLayerSettings layerSettings)
        {
            if (layerSettings == null)
            {
                return;
            }

            _fpsAnimator.LinkAnimatorLayer(layerSettings);
        }

        private void InitializeWeapons()
        {
            _instantiatedWeapons = new List<FPSItem>();

            foreach (var prefab in _controllerSettings.WeaponPrefabs)
            {
                var weapon = Instantiate(prefab, transform.position, Quaternion.identity);
                var weaponTransform = weapon.transform;

                weapon.gameObject.SetActive(false);

                weaponTransform.parent = _weaponBoneTransform;
                weaponTransform.localPosition = Vector3.zero;
                weaponTransform.localRotation = Quaternion.identity;

                _instantiatedWeapons.Add(weapon.GetComponent<FPSItem>());
            }
        }

        private void Start()
        {
            Cursor.visible = false; // to do: del this
            Cursor.lockState = CursorLockMode.Locked; // to do: del this
            Application.targetFrameRate = 144; // to do: del this

            //_weaponBoneTransform = GetComponentInChildren<KRigComponent>().GetRigTransform(_controllerSettings.WeaponBone);

            _fpsAnimator.Initialize();

            InitializeWeapons();

            _actionState = ActionState.None;

            EquipWeapon();

            _movementHandler.Initialize(_controller, _userInput, _animator);
            _viewHandler.Initialize(_userInput, _recoilPattern);
        }

        private void Awake()
        {
            _movementHandler.OnSprintStartedEvent += OnSprintStarted;
            _movementHandler.OnSprintEndedEvent += OnSprintEnded;

            _movementHandler.OnSlideStartedEvent += OnSlideStarted;

            _movementHandler.OnMoveEndedEvent += OnMoveEnded;

            _movementHandler.OnCrouchedEvent += OnCrouched;
            _movementHandler.OnLandedEvent += OnLanded;
            _movementHandler.OnJumpedEvent += OnJumped;

            _movementHandler.SprintConditionFunc += NoneActiveAction;
            _movementHandler.SlideConditionFunc += NoneActiveAction;
        }

        private void OnDestroy()
        {
            _movementHandler.OnSprintStartedEvent -= OnSprintStarted;
            _movementHandler.OnSprintEndedEvent -= OnSprintEnded;

            _movementHandler.OnSlideStartedEvent -= OnSlideStarted;

            _movementHandler.OnMoveEndedEvent -= OnMoveEnded;

            _movementHandler.OnCrouchedEvent -= OnCrouched;
            _movementHandler.OnLandedEvent -= OnLanded;
            _movementHandler.OnJumpedEvent -= OnJumped;

            _movementHandler.SprintConditionFunc -= NoneActiveAction;
            _movementHandler.SlideConditionFunc -= NoneActiveAction;
        }
    }
}