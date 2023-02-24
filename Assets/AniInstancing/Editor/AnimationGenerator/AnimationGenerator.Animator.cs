
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AnimationInstancing
{
    public partial class AnimationGenerator
    {
        void AnalyzeStateMachine(UnityEditor.Animations.AnimatorStateMachine stateMachine,
             Animator animator,
             SkinnedMeshRenderer[] meshRender,
             int layer,
             int bakeFPS,
             int animationIndex)
        {
            AnimationInstancing instance = generatedPrefab.GetComponent<AnimationInstancing>();
            if (instance == null)
            {
                Log.E("You should select a prefab with AnimationInstancing component.");
                return;
            }

            for (int i = 0; i != stateMachine.states.Length; ++i)
            {
                ChildAnimatorState state = stateMachine.states[i];
                AnimationClip clip = state.state.motion as AnimationClip;
                bool needBake = false;
                if (clip == null)
                    continue;
                if (!generateAnims.TryGetValue(clip.name, out needBake))
                    continue;
                foreach (var obj in generateInfo)
                {
                    if (obj.info.animationName == clip.name)
                    {
                        needBake = false;
                        break;
                    }
                }

                if (!needBake)
                    continue;

                AnimationBakeInfo bake = new AnimationBakeInfo();
                bake.length = clip.averageDuration;
                bake.animator = animator;
                bake.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                bake.meshRender = meshRender;
                bake.layer = layer;
                bake.info = new AnimationInfo();
                bake.info.animationName = clip.name;
                bake.info.animationNameHash = state.state.nameHash;
                bake.info.animationIndex = animationIndex;
                bake.info.totalFrame = (int)(bake.length * bakeFPS + 0.5f) + 1;
                bake.info.totalFrame = Mathf.Clamp(bake.info.totalFrame, 1, bake.info.totalFrame);
                bake.info.fps = bakeFPS;
                bake.info.rootMotion = true;
                bake.info.wrapMode = clip.isLooping ? WrapMode.Loop : clip.wrapMode;
                if (bake.info.rootMotion)
                {
                    bake.info.velocity = new Vector3[bake.info.totalFrame];
                    bake.info.angularVelocity = new Vector3[bake.info.totalFrame];
                }
                generateInfo.Add(bake);
                animationIndex += bake.info.totalFrame;
                totalFrame += bake.info.totalFrame;

                bake.info.eventList = new List<AnimationEvent>();
                //AnimationClip clip = state.state.motion as AnimationClip;
                foreach (var evt in clip.events)
                {
                    AnimationEvent aniEvent = new AnimationEvent();
                    aniEvent.function = evt.functionName;
                    aniEvent.floatParameter = evt.floatParameter;
                    aniEvent.intParameter = evt.intParameter;
                    aniEvent.stringParameter = evt.stringParameter;
                    aniEvent.time = evt.time;
                    if (evt.objectReferenceParameter != null)
                        aniEvent.objectParameter = evt.objectReferenceParameter.name;
                    else
                        aniEvent.objectParameter = "";
                    bake.info.eventList.Add(aniEvent);
                }

                cacheTransition.Add(state.state, state.state.transitions);
                state.state.transitions = null;
                cacheAnimationEvent.Add(clip, clip.events);
                UnityEngine.AnimationEvent[] tempEvent = new UnityEngine.AnimationEvent[0];
                UnityEditor.AnimationUtility.SetAnimationEvents(clip, tempEvent);
            }
            for (int i = 0; i != stateMachine.stateMachines.Length; ++i)
            {
                AnalyzeStateMachine(stateMachine.stateMachines[i].stateMachine, animator, meshRender, layer, bakeFPS, animationIndex);
            }
        }

        private List<AnimationClip> GetClips(Animator animator)
        {
            UnityEditor.Animations.AnimatorController controller = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            return GetClipsFromStatemachine(controller.layers[0].stateMachine);
        }

        private List<AnimationClip> GetClipsFromStatemachine(UnityEditor.Animations.AnimatorStateMachine stateMachine)
        {
            List<AnimationClip> list = new List<AnimationClip>();
            for (int i = 0; i != stateMachine.states.Length; ++i)
            {
                UnityEditor.Animations.ChildAnimatorState state = stateMachine.states[i];
                if (state.state.motion is UnityEditor.Animations.BlendTree)
                {
                    UnityEditor.Animations.BlendTree blendTree = state.state.motion as UnityEditor.Animations.BlendTree;
                    ChildMotion[] childMotion = blendTree.children;
                    for (int j = 0; j != childMotion.Length; ++j)
                    {
                        list.Add(childMotion[j].motion as AnimationClip);
                    }
                }
                else if (state.state.motion != null)
                    list.Add(state.state.motion as AnimationClip);
            }
            for (int i = 0; i != stateMachine.stateMachines.Length; ++i)
            {
                list.AddRange(GetClipsFromStatemachine(stateMachine.stateMachines[i].stateMachine));
            }

            var distinctClips = list.Select(q => (AnimationClip)q).Distinct().ToList();
            for (int i = 0; i < distinctClips.Count; i++)
            {
                if (distinctClips[i] && generateAnims.ContainsKey(distinctClips[i].name) == false)
                    generateAnims.Add(distinctClips[i].name, true);
            }
            return list;
        }
    }
}