﻿// Designed by KINEMATION, 2024.

using KINEMATION.FPSAnimationFramework.Runtime.Core;
using KINEMATION.KAnimationCore.Runtime.Attributes;
using KINEMATION.KAnimationCore.Runtime.Input;

using UnityEditor;
using UnityEngine;

using UnityEngine.Animations;
using UnityEngine.Experimental.Animations;
using UnityEngine.Playables;

namespace KINEMATION.FPSAnimationFramework.Runtime.Playables
{
    [HelpURL("https://kinemation.gitbook.io/scriptable-animation-system/workflow/components")]
    public class FPSPlayablesController : MonoBehaviour, IPlayablesController
    {
        public PlayableGraph playableGraph;
        public Animator Animator => _animator;
        
        [Header("General Settings")] 
        [HideInInspector] public AvatarMask upperBodyMask;

        [SerializeField] [InputProperty]
        protected string playablesWeightProperty = FPSANames.PlayablesWeight;
        
        [Header("Editor Animation Preview")]
        [SerializeField] protected AnimationClip animationToPreview;
        [SerializeField] protected AnimationClip defaultPose;
        [SerializeField] protected bool loopPreview;
        
        protected int _maxPoseCount = 3;
        protected int _maxAnimCount = 3;
        
        protected Animator _animator;
        
        protected FPSAnimatorMixer _overlayPoseMixer;
        protected FPSAnimatorMixer _slotMixer;
        protected FPSAnimatorMixer _overrideMixer;

        protected AnimationLayerMixerPlayable _dynamicAnimationMixer;
        protected AnimationLayerMixerPlayable _masterMixer;
        
        protected UserInputController _inputController;
        protected float _controllerWeight = 1f;

        protected int _playablesWeightPropertyIndex;

        protected virtual void Update()
        {
            if (!Application.isPlaying) return;
            
            _overlayPoseMixer.Update();
            _slotMixer.Update();
            _overrideMixer.Update();

            float weight = _controllerWeight;
            if (_inputController != null)
            {
                weight *= Mathf.Clamp01(_inputController.GetValue<float>(_playablesWeightPropertyIndex));
            }
            
            _masterMixer.SetInputWeight(1, Mathf.Clamp01(weight));
        }

        private void OnDestroy()
        {
            if (!playableGraph.IsValid())
            {
                return;
            }

            playableGraph.Stop();
            playableGraph.Destroy();
        }

        public virtual bool InitializeController()
        {
            if (playableGraph.IsValid())
            {
                playableGraph.Destroy();
            }
            
            _animator = GetComponent<Animator>();
            playableGraph = _animator.playableGraph;

            if (!playableGraph.IsValid())
            {
                Debug.LogWarning(gameObject.name + " Animator Controller is not valid!");
                return false;
            }
            
            playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            
            _masterMixer = AnimationLayerMixerPlayable.Create(playableGraph, 2);
            _dynamicAnimationMixer = AnimationLayerMixerPlayable.Create(playableGraph, 2);
            
            _overlayPoseMixer = new FPSAnimatorMixer(playableGraph, _maxPoseCount, 0);
            _slotMixer = new FPSAnimatorMixer(playableGraph, _maxAnimCount, 1);
            _overrideMixer = new FPSAnimatorMixer(playableGraph, _maxAnimCount, 1);
            
            _slotMixer.mixer.ConnectInput(0, _overlayPoseMixer.mixer, 0, 1f);
            _overrideMixer.mixer.ConnectInput(0, _slotMixer.mixer, 0, 1f);
            _dynamicAnimationMixer.ConnectInput(0, _overrideMixer.mixer, 0, 1f);

            var animatorOutput = playableGraph.GetOutput(0);
            
            _masterMixer.ConnectInput(0, animatorOutput.GetSourcePlayable(), 0, 1f);
            _masterMixer.ConnectInput(1, _dynamicAnimationMixer, 0, 1f);
            
            _masterMixer.SetLayerMaskFromAvatarMask(0, new AvatarMask());
            _masterMixer.SetLayerMaskFromAvatarMask(1, upperBodyMask);
            
            var output = AnimationPlayableOutput.Create(playableGraph, "FPSAnimatorGraph", _animator);
            output.SetSourcePlayable(_masterMixer);
            //output.SetSortingOrder(910);
                      
            playableGraph.Play();
            _inputController = GetComponent<UserInputController>();

            if (_inputController == null) return true;
            _playablesWeightPropertyIndex = _inputController.GetPropertyIndex(playablesWeightProperty);
            
            return true;
        }

        public virtual void UpdateAnimatorController(RuntimeAnimatorController newController)
        {
            if (newController == null)
            {
                return;
            }
            
            _animator.runtimeAnimatorController = newController;
        }
        
        public void SetControllerWeight(float weight)
        {
            if (!playableGraph.IsValid() || !_masterMixer.IsValid())
            {
                return;
            }

            _controllerWeight = Mathf.Clamp01(weight);
        }

        public virtual bool PlayPose(FPSAnimationAsset asset)
        {
            if (asset.clip == null)
            {
                return false;
            }
            
            FPSAnimatorPlayable animPlayable = new FPSAnimatorPlayable(playableGraph, asset.clip, null)
            {
                blendTime = asset.blendTime,
                autoBlendOut = false
            };

            animPlayable.playable.SetTime(0f);
            animPlayable.playable.SetSpeed(1f);
            _overlayPoseMixer.Play(animPlayable, upperBodyMask);

            return true;
        }

        public virtual bool PlayAnimation(FPSAnimationAsset asset, float startTime = 0f)
        {
            if (asset.clip == null)
            {
                return false;
            }

            BlendTime blendTime = asset.blendTime;
            blendTime.startTime = startTime;

            FPSAnimatorPlayable animPlayable = new FPSAnimatorPlayable(playableGraph, asset.clip, 
                asset.curves.ToArray())
            {
                blendTime = blendTime,
                autoBlendOut = true
            };

            animPlayable.playable.SetTime(startTime);
            animPlayable.playable.SetSpeed(blendTime.rateScale);

            _slotMixer.Play(animPlayable, asset.mask == null ? upperBodyMask : asset.mask, asset.isAdditive);
            
            FPSAnimatorPlayable overridePlayable = new FPSAnimatorPlayable(playableGraph, asset.clip, null)
            {
                blendTime = blendTime,
                autoBlendOut = true
            };

            overridePlayable.playable.SetTime(startTime);
            overridePlayable.playable.SetSpeed(blendTime.rateScale);

            if (asset.overrideMask != null)
            {
                _overrideMixer.Play(overridePlayable, asset.overrideMask);
            }

            return true;
        }

        public virtual void StopAnimation(float blendOutTime)
        {
            _slotMixer.Stop(blendOutTime);
            _overrideMixer.Stop(blendOutTime);
        }

        public bool IsPlaying()
        {
            return playableGraph.IsValid() && playableGraph.IsPlaying();
        }

        public virtual float GetCurveValue(string curveName, bool isAnimator = false)
        {
            return isAnimator ? _animator.GetFloat(curveName) : _slotMixer.GetCurveValue(curveName);
        }
        
#if UNITY_EDITOR
        public virtual bool InitializeControllerEditor()
        {
            _animator = GetComponent<Animator>();

            if (_animator == null)
            {
                Debug.LogWarning("FPSAnimator Preview: Animator component not found!");
                return false;
            }

            if (playableGraph.IsValid())
            {
                playableGraph.Destroy();
            }

            if (_masterMixer.IsValid())
            {
                _masterMixer.Destroy();
            }
            
            playableGraph = PlayableGraph.Create();
            _masterMixer = AnimationLayerMixerPlayable.Create(playableGraph, 1);
            
            var output = AnimationPlayableOutput.Create(playableGraph, "FPSAnimatorEditorGraph", _animator);
            output.SetSourcePlayable(_masterMixer);
            
            return true;
        }

        public virtual void StartEditorPreview()
        {
            if (!InitializeControllerEditor())
            {
                return;
            }

            if (animationToPreview != null)
            {
                var previewPlayable = AnimationClipPlayable.Create(playableGraph, animationToPreview);
                previewPlayable.SetTime(0f);
                previewPlayable.SetSpeed(1f);

                if (_masterMixer.GetInput(0).IsValid())
                {
                    _masterMixer.DisconnectInput(0);
                }

                _masterMixer.ConnectInput(0, previewPlayable, 0, 1f);
                EditorApplication.update += LoopEditorPreview;
            }
            else
            {
                var controllerPlayable = AnimatorControllerPlayable.Create(playableGraph,
                    _animator.runtimeAnimatorController);
                
                _masterMixer.ConnectInput(0, controllerPlayable, 0, 1f);
            }

            playableGraph.Play();
            
            EditorApplication.QueuePlayerLoopUpdate();
        }

        public virtual void LoopEditorPreview()
        {
            if (!playableGraph.IsPlaying())
            {
                EditorApplication.update -= LoopEditorPreview;
            }
            
            if (loopPreview && playableGraph.IsValid() 
                            && _masterMixer.GetInput(0).GetTime() >= animationToPreview.length)
            {
                _masterMixer.GetInput(0).SetTime(0f);
            }
        }

        public virtual void StopEditorPreview()
        {
            if (!playableGraph.IsValid()) return;
            
            _masterMixer.DisconnectInput(0);
            playableGraph.Stop();
            playableGraph.Destroy();
            
            EditorApplication.update -= LoopEditorPreview;

            if (defaultPose != null)
            {
                defaultPose.SampleAnimation(gameObject, 0f);
            }
        }
#endif
    }
}