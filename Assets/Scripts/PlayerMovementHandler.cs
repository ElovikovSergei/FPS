using KINEMATION.FPSAnimationFramework.Runtime.Core;
using KINEMATION.KAnimationCore.Runtime.Input;
using KINEMATION.KAnimationCore.Runtime.Core;
using UnityEngine.InputSystem;
using UnityEngine;
using System;

namespace FPS
{
    public sealed class PlayerMovementHandler : MonoBehaviour
    {
        public bool IsInAir
        {
            get
            {
                return !_controller.isGrounded;
            }
        }

        public bool IsMoving
        {
            get
            {
                return !Mathf.Approximately(_moveDirection.normalized.magnitude, 0f);
            }
        }

        public bool IsSprinting
        {
            get
            {
                return CurrentMovementState == MovementState.Sprinting;
            }
        }

        public bool IsProne
        {
            get
            {
                return CurrentPoseState == PoseState.Prone;
            }
        }

        public bool CanUnCrouch
        {
            get
            {
                float height = _originalHeight - _controller.radius * 2f;
                var position = transform.TransformPoint(_originalCenter + Vector3.up * height / 2f);

                return !Physics.CheckSphere(position, _controller.radius, _movementSettings.GroundMask);
            }
        }

        public bool CanSprint
        {
            get
            {
                bool conditionCheck = false;

                if (SprintConditionFunc != null)
                {
                    conditionCheck = SprintConditionFunc.Invoke();
                }

                return CurrentPoseState == PoseState.Standing && conditionCheck;
            }
        }

        public bool CanSlide
        {
            get
            {
                return CurrentMovementState is MovementState.Sprinting
                    && CurrentPoseState is PoseState.Standing
                    && (SlideConditionFunc == null || SlideConditionFunc.Invoke());
            }
        }

        public float Speed
        {
            get
            {
                return new Vector3(_velocity.x, 0f, _velocity.z).magnitude;
            }
        }

        public event Action OnSprintStartedEvent;
        public event Action OnSprintEndedEvent;

        public event Action OnSlideStartedEvent;
        public event Action OnSlideEndedEvent;

        public event Action OnMoveStartedEvent;
        public event Action OnMoveEndedEvent;

        public event Action<bool> OnCrouchedEvent;
        public event Action OnLandedEvent;
        public event Action OnJumpedEvent;

        public event Func<bool> SprintConditionFunc;
        public event Func<bool> SlideConditionFunc;

        public MovementState CurrentMovementState { get; private set; }
        public PoseState CurrentPoseState { get; private set; }

        private Vector2 _moveInput;

        private Vector3 _velocity;
        private Vector2 _animatorVelocity;

        private Vector3 _moveDirection;
        private Vector3 _slideDirection;

        private MovementStateSettings _currentStateSettings;
        private MovementStateSettings _cachedStateSettings;

        private Vector3 _originalCenter;
        private float _originalHeight;

        private float _stateChangingProgress;
        private float _slideProgress;

        private bool _consumeMoveInput;
        private bool _wasMoving;

        private MovementState _cachedMovementState;

        [SerializeField] private MovementStatesSettings _movementSettings;

        private CharacterController _controller;
        private UserInputController _userInput;
        private Animator _animator;



#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            if (!_consumeMoveInput)
            {
                return;
            }

            _moveInput = value.Get<Vector2>();
        }

        public void OnCrouch()
        {
            if (!_consumeMoveInput)
            {
                return;
            }

            if (CurrentMovementState != MovementState.Idle
                && CurrentMovementState != MovementState.Walking)
            {
                return;
            }

            if (CurrentPoseState == PoseState.Standing)
            {
                Crouch();
                _currentStateSettings = _movementSettings.Crouching;

                return;
            }

            //if (CurrentPoseState is PoseState.Prone)
            //{
            //    OnProneDisabled();
            //    return;
            //}

            UnCrouch();
            _currentStateSettings = _movementSettings.Walking;
        }

        public void OnJump()
        {
            if (!_consumeMoveInput)
            {
                return;
            }

            if (CurrentMovementState == MovementState.InAir
                || CurrentPoseState == PoseState.Crouching)
            {
                return;
            }

            //if (CurrentPoseState is PoseState.Prone)
            //{
            //    OnProneDisabled();
            //    return;
            //}

            CurrentMovementState = MovementState.InAir;
            _velocity.y = _movementSettings.JumpHeight;
        }

        public void OnSlide()
        {
            if (!_consumeMoveInput)
            {
                return;
            }

            if (!CanSlide)
            {
                return;
            }

            _slideProgress = 0f;
            CurrentMovementState = MovementState.Sliding;
        }

        public void OnSprint(InputValue value)
        {
            if (!_consumeMoveInput
                || CurrentMovementState == MovementState.InAir
                || CurrentMovementState == MovementState.Sliding)
            {
                return;
            }

            if (value.isPressed && CanSprint)
            {
                CurrentMovementState = MovementState.Sprinting;

                return;
            }

            CurrentMovementState = MovementState.Walking;
        }
#endif

        public void Initialize(CharacterController controller, UserInputController userInput, Animator animator)
        {
            _controller = controller;
            _userInput = userInput;
            _animator = animator;

            _consumeMoveInput = true;
            _wasMoving = false;

            _slideProgress = 0f;
            _stateChangingProgress = 0f;

            _originalHeight = _controller.height;
            _originalCenter = _controller.center;

            CurrentPoseState = PoseState.Standing;

            _cachedMovementState = CurrentMovementState = MovementState.Idle;
            _currentStateSettings = _cachedStateSettings = _movementSettings.Idle;
        }

        public void AllowConsumingInput()
        {
            _consumeMoveInput = true;
        }

        private void Crouch()
        {
            float crouchedHeight = _originalHeight * _movementSettings.CrouchRatio;
            float heightDifference = _originalHeight - crouchedHeight;

            _controller.height = crouchedHeight;

            var crouchedCenter = _originalCenter;

            crouchedCenter.y -= heightDifference / 2;
            _controller.center = crouchedCenter;

            CurrentPoseState = PoseState.Crouching;

            _animator.SetBool(AnimatorParameters.Crouching, true);
            OnCrouchedEvent?.Invoke(true);
        }

        private void UnCrouch()
        {
            _controller.height = _originalHeight;
            _controller.center = _originalCenter;

            CurrentPoseState = PoseState.Standing;

            _animator.SetBool(AnimatorParameters.Crouching, false);
            OnCrouchedEvent?.Invoke(false);
        }

        private void UpdateGroundState()
        {
            var normalizedInput = _moveInput.normalized;
            var targetDirection = transform.right * normalizedInput.x + transform.forward * normalizedInput.y;

            float maxAccelerationTime = _movementSettings.AccelerationCurve.keys[^1].time;
            _stateChangingProgress = Mathf.Min(_stateChangingProgress + Time.deltaTime, maxAccelerationTime);

            float t = _movementSettings.AccelerationCurve.Evaluate(_stateChangingProgress);
            t = Mathf.Lerp(_cachedStateSettings.Velocity, _currentStateSettings.Velocity, t);

            targetDirection *= Mathf.Lerp(_cachedStateSettings.Velocity, _currentStateSettings.Velocity, t);
            targetDirection = Vector3.Lerp(_velocity, targetDirection, Math.ExpDecayAlpha(_currentStateSettings.VelocitySmoothing, Time.deltaTime));

            _velocity = targetDirection;

            targetDirection.y = -2f;
            _moveDirection = targetDirection;
        }

        private void UpdateInAirState()
        {
            var normalizedInput = _moveInput.normalized;

            _velocity.y -= _movementSettings.Gravity * Time.deltaTime;
            _velocity.y = Mathf.Max(-_movementSettings.MaxFallVelocity, _velocity.y);

            var desiredVelocity = transform.right * normalizedInput.x + transform.forward * normalizedInput.y;

            desiredVelocity *= _currentStateSettings.Velocity;
            desiredVelocity = Vector3.Lerp(_velocity, desiredVelocity * _movementSettings.AirFriction,
                Math.ExpDecayAlpha(_movementSettings.AirVelocity, Time.deltaTime));

            desiredVelocity.y = _velocity.y;

            _velocity = desiredVelocity;
            _moveDirection = desiredVelocity;
        }

        private void UpdateSlidingState()
        {
            float slideAmount = _movementSettings.SlideCurve.Evaluate(_slideProgress) * _movementSettings.SlideSpeed;

            _velocity = _slideDirection.normalized * slideAmount;

            var desiredVelocity = _velocity;

            desiredVelocity.y = -2f;

            _moveDirection = desiredVelocity;
            _slideProgress = Mathf.Clamp01(_slideProgress + Time.deltaTime);
        }

        private void UpdateMovement()
        {
            float playablesWeight = 1f - _animator.GetFloat(AnimatorParameters.FullBodyWeight);

            _controller.Move(_moveDirection * Time.deltaTime);
            _userInput.SetValue(FPSANames.PlayablesWeight, playablesWeight);
        }

        private void OnCurrentMovementStateChanged()
        {
            if (_cachedMovementState == MovementState.InAir)
            {
                OnLandedEvent?.Invoke();
            }

            if (_cachedMovementState == MovementState.Sprinting)
            {
                OnSprintEndedEvent?.Invoke();
            }

            if (_cachedMovementState == MovementState.Sliding)
            {
                OnSlideEndedEvent?.Invoke();

                if (CanUnCrouch)
                {
                    UnCrouch();
                }
            }

            if (CurrentMovementState == MovementState.Idle)
            {
                _currentStateSettings = _movementSettings.Idle;
                return;
            }

            if (CurrentMovementState == MovementState.InAir)
            {
                OnJumpedEvent?.Invoke();
                return;
            }

            if (CurrentMovementState == MovementState.Sprinting)
            {
                _stateChangingProgress = 0f;
                _cachedStateSettings = _currentStateSettings;

                OnSprintStartedEvent?.Invoke();
                _currentStateSettings = _movementSettings.Sprinting;

                return;
            }

            if (CurrentMovementState == MovementState.Sliding)
            {
                _currentStateSettings.VelocitySmoothing = _movementSettings.SlideDirectionSmoothing;
                _slideDirection = _velocity;
                _slideProgress = 0f;

                OnSlideStartedEvent?.Invoke();
                Crouch();

                return;
            }

            if (CurrentPoseState == PoseState.Crouching)
            {
                _currentStateSettings = _movementSettings.Crouching;
                return;
            }

            if (CurrentPoseState == PoseState.Prone)
            {
                _stateChangingProgress = 0f;
                _cachedStateSettings = _currentStateSettings;
                _currentStateSettings = _movementSettings.Prone;

                return;
            }

            if (_cachedMovementState == MovementState.Idle)
            {
                _stateChangingProgress = 0f;
                _cachedStateSettings = _currentStateSettings;
            }

            _currentStateSettings = _movementSettings.Walking;
        }

        private void UpdateCurrentMovementState()
        {
            if (CurrentMovementState == MovementState.Sliding && !Mathf.Approximately(_slideProgress, 1f))
            {
                return;
            }

            if (CurrentMovementState == MovementState.InAir)
            {
                return;
            }

            if (CurrentMovementState == MovementState.Sprinting
                && _moveInput.y > 0f && Mathf.Approximately(_moveInput.x, 0f))
            {
                return;
            }

            CurrentMovementState = !IsMoving
                ? MovementState.Idle
                : MovementState.Walking;
        }

        private void UpdateAnimatorParameters()
        {
            var animatorVelocity = _moveInput;

            animatorVelocity *= CurrentMovementState == MovementState.InAir
                ? 0f
                : 1f;

            _animatorVelocity = Vector2.Lerp(_animatorVelocity, animatorVelocity,
                KMath.ExpDecayAlpha(_currentStateSettings.VelocitySmoothing, Time.deltaTime));

            _animator.SetFloat(AnimatorParameters.MoveX, _animatorVelocity.x);
            _animator.SetFloat(AnimatorParameters.MoveY, _animatorVelocity.y);
            _animator.SetFloat(AnimatorParameters.Velocity, _animatorVelocity.magnitude);
            _animator.SetBool(AnimatorParameters.InAir, IsInAir);
            _animator.SetBool(AnimatorParameters.Moving, IsMoving);

            float sprintWeight = _animator.GetFloat(AnimatorParameters.Sprinting);
            float t = KMath.ExpDecayAlpha(_currentStateSettings.VelocitySmoothing, Time.deltaTime);

            sprintWeight = Mathf.Lerp(sprintWeight, CurrentMovementState == MovementState.Sprinting
                ? 1f
                : 0f, t);

            _animator.SetFloat(AnimatorParameters.Sprinting, sprintWeight);
            _userInput.SetValue(FPSANames.MoveInput, new Vector4(_animatorVelocity.x, _animatorVelocity.y));
        }

        private void HandleMove()
        {
            UpdateCurrentMovementState();

            if (_cachedMovementState != CurrentMovementState)
            {
                OnCurrentMovementStateChanged();
            }

            bool isMoving = IsMoving;

            if (_wasMoving != isMoving)
            {
                if (isMoving)
                {
                    OnMoveStartedEvent?.Invoke();
                }
                else
                {
                    OnMoveEndedEvent?.Invoke();
                }
            }

            _wasMoving = isMoving;

            if (CurrentMovementState == MovementState.InAir)
            {
                UpdateInAirState();
            }
            else if (CurrentMovementState == MovementState.Sliding)
            {
                UpdateSlidingState();
            }
            else
            {
                UpdateGroundState();
            }

            UpdateMovement();
            UpdateAnimatorParameters();

            _cachedMovementState = CurrentMovementState;
        }

        private void HandleLateMove()
        {
            if (CurrentMovementState != MovementState.InAir && IsInAir)
            {
                CurrentMovementState = MovementState.InAir;
            }

            if (CurrentMovementState == MovementState.InAir && !IsInAir)
            {
                CurrentMovementState = MovementState.Idle;
            }
        }

        private void Update()
        {
            HandleMove();
        }

        private void LateUpdate()
        {
            HandleLateMove();
        }
    }
}