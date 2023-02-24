using UnityEngine;


namespace AnimationInstancing
{
    public partial class AnimationGenerator
    {
        class AnimationBakeInfo
        {
            public SkinnedMeshRenderer[] meshRender;
            public Animator animator;
            public int workingFrame;
            public float length;
            public int layer;
            public AnimationInfo info;
        }
    }
}