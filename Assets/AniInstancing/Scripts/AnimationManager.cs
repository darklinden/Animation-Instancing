
/*
THIS FILE IS PART OF Animation Instancing PROJECT
AnimationInstancing.cs - The core part of the Animation Instancing library

©2017 Jin Xiaoyu. All Rights Reserved.
*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace AnimationInstancing
{
    public class AnimationManager : Singleton<AnimationManager>
    {
        // A request to create animation info, because we use async method
        struct CreateAnimationRequest
        {
            public GameObject prefab;
            public AnimationInstancing instance;
        }
        // A container to storage all animations info within game object
        public class InstanceAnimationInfo
        {
            public List<AnimationInfo> listAniInfo;
            public ExtraBoneInfo extraBoneInfo;
        }
        private List<CreateAnimationRequest> m_requestList;
        private Dictionary<int, InstanceAnimationInfo> m_animationInfo;

        public static AnimationManager GetInstance()
        {
            return Singleton<AnimationManager>.Instance;
        }

        private void Awake()
        {
            m_animationInfo = new Dictionary<int, InstanceAnimationInfo>();
            m_requestList = new List<CreateAnimationRequest>();
        }

        public async UniTask<InstanceAnimationInfo> FindAnimationInfo(AnimationInstancingPrefab prefab, AnimationInstancing instance)
        {
            InstanceAnimationInfo info = null;
            if (m_animationInfo.TryGetValue((int)prefab, out info))
            {
                return info;
            }

            return await CreateAnimationInfoFromFile(prefab);
        }


        private async UniTask<InstanceAnimationInfo> CreateAnimationInfoFromFile(AnimationInstancingPrefab prefab)
        {
            var w = await LoadUtil.LoadAsync<TextAsset>(prefab.ToAddr());
            if (w == null)
            {
                Log.E("Load AnimationTexture Error:", prefab.ToAddr());
                return null;
            }
            BinaryReader reader = new BinaryReader(new MemoryStream(w.bytes));
            InstanceAnimationInfo info = new InstanceAnimationInfo();
            info.listAniInfo = ReadAnimationInfo(reader);
            info.extraBoneInfo = ReadExtraBoneInfo(reader);
            m_animationInfo.Add(prefab.ToInt(), info);
            AnimationInstancingMgr.Instance.ImportAnimationTexture(prefab.ToName(), reader);

            return info;
        }

        private List<AnimationInfo> ReadAnimationInfo(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            List<AnimationInfo> listInfo = new List<AnimationInfo>();
            for (int i = 0; i != count; ++i)
            {
                AnimationInfo info = new AnimationInfo();
                //info.animationNameHash = reader.ReadInt32();
                info.animationName = reader.ReadString();
                info.animationNameHash = info.animationName.GetHashCode();
                info.animationIndex = reader.ReadInt32();
                info.textureIndex = reader.ReadInt32();
                info.totalFrame = reader.ReadInt32();
                info.fps = reader.ReadInt32();
                info.rootMotion = reader.ReadBoolean();
                info.wrapMode = (WrapMode)reader.ReadInt32();
                if (info.rootMotion)
                {
                    info.velocity = new Vector3[info.totalFrame];
                    info.angularVelocity = new Vector3[info.totalFrame];
                    for (int j = 0; j != info.totalFrame; ++j)
                    {
                        info.velocity[j].x = reader.ReadSingle();
                        info.velocity[j].y = reader.ReadSingle();
                        info.velocity[j].z = reader.ReadSingle();

                        info.angularVelocity[j].x = reader.ReadSingle();
                        info.angularVelocity[j].y = reader.ReadSingle();
                        info.angularVelocity[j].z = reader.ReadSingle();
                    }
                }
                int evtCount = reader.ReadInt32();
                info.eventList = new List<AnimationEvent>();
                for (int j = 0; j != evtCount; ++j)
                {
                    AnimationEvent evt = new AnimationEvent();
                    evt.function = reader.ReadString();
                    evt.floatParameter = reader.ReadSingle();
                    evt.intParameter = reader.ReadInt32();
                    evt.stringParameter = reader.ReadString();
                    evt.time = reader.ReadSingle();
                    evt.objectParameter = reader.ReadString();
                    info.eventList.Add(evt);
                }
                listInfo.Add(info);
            }
            listInfo.Sort(new ComparerHash());
            return listInfo;
        }

        private ExtraBoneInfo ReadExtraBoneInfo(BinaryReader reader)
        {
            ExtraBoneInfo info = null;
            if (reader.ReadBoolean())
            {
                info = new ExtraBoneInfo();
                int count = reader.ReadInt32();
                info.extraBone = new string[count];
                info.extraBindPose = new Matrix4x4[count];
                for (int i = 0; i != info.extraBone.Length; ++i)
                {
                    info.extraBone[i] = reader.ReadString();
                }
                for (int i = 0; i != info.extraBindPose.Length; ++i)
                {
                    for (int j = 0; j != 16; ++j)
                    {
                        info.extraBindPose[i][j] = reader.ReadSingle();
                    }
                }
            }
            return info;
        }
    }
}