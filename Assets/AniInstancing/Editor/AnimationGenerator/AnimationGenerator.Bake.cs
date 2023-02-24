
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AnimationInstancing
{
    public partial class AnimationGenerator
    {
        void BakeWithAnimator()
        {
            if (generatedPrefab != null)
            {
                generatedObject = Instantiate(generatedPrefab);
                Selection.activeGameObject = generatedObject;
                generatedObject.transform.position = Vector3.zero;
                generatedObject.transform.rotation = Quaternion.identity;
                Animator animator = generatedObject.GetComponentInChildren<Animator>();

                AnimationInstancing script = generatedObject.GetComponent<AnimationInstancing>();
                Debug.Assert(script);
                SkinnedMeshRenderer[] meshRender = generatedObject.GetComponentsInChildren<SkinnedMeshRenderer>();
                List<Matrix4x4> bindPose = new List<Matrix4x4>(150);
                Transform[] boneTransform = RuntimeHelper.MergeBone(meshRender, bindPose);

                // calculate the bindpose of attached points
                if (generatedFbx)
                {
                    List<Transform> listExtra = new List<Transform>();
                    Transform[] trans = generatedFbx.GetComponentsInChildren<Transform>();
                    Transform[] bakedTrans = generatedObject.GetComponentsInChildren<Transform>();
                    foreach (var obj in selectExtraBone)
                    {
                        if (!obj.Value)
                            continue;

                        for (int i = 0; i != trans.Length; ++i)
                        {
                            Transform tran = trans[i] as Transform;
                            if (tran.name == obj.Key)
                            {
                                bindPose.Add(tran.localToWorldMatrix);
                                listExtra.Add(bakedTrans[i]);
                            }
                        }
                    }

                    Transform[] totalTransform = new Transform[boneTransform.Length + listExtra.Count];
                    System.Array.Copy(boneTransform, totalTransform, boneTransform.Length);
                    System.Array.Copy(listExtra.ToArray(), 0, totalTransform, boneTransform.Length, listExtra.Count);
                    boneTransform = totalTransform;
                    //boneTransform = boneTransform;

                    extraBoneInfo = new ExtraBoneInfo();
                    extraBoneInfo.extraBone = new string[listExtra.Count];
                    extraBoneInfo.extraBindPose = new Matrix4x4[listExtra.Count];
                    for (int i = 0; i != listExtra.Count; ++i)
                    {
                        extraBoneInfo.extraBone[i] = listExtra[i].name;
                        extraBoneInfo.extraBindPose[i] = bindPose[bindPose.Count - listExtra.Count + i];
                    }
                }
                Reset();
                AddMeshVertex2Generate(meshRender, boneTransform, bindPose.ToArray());

                Transform rootNode = meshRender[0].rootBone;
                for (int j = 0; j != meshRender.Length; ++j)
                {
                    meshRender[j].enabled = true;
                }
                animator.applyRootMotion = true;
                totalFrame = 0;

                UnityEditor.Animations.AnimatorController controller = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
                Debug.Assert(controller.layers.Length > 0);
                cacheTransition.Clear();
                cacheAnimationEvent.Clear();
                UnityEditor.Animations.AnimatorControllerLayer layer = controller.layers[0];
                AnalyzeStateMachine(layer.stateMachine, animator, meshRender, 0, aniFps, 0);
                generateCount = generateInfo.Count;
            }
        }
    }
}