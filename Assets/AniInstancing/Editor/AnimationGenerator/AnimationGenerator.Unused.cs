using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditorInternal;
using System.IO;


namespace AnimationInstancing
{
    public partial class AnimationGenerator
    {
        private List<AnimationClip> GetClips(bool bakeFromAnimator)
        {
            Object[] gameObject = new Object[] { generatedPrefab };
            var clips = EditorUtility.CollectDependencies(gameObject).ToList();
            if (bakeFromAnimator)
            {
                clips.Clear();
                clips.AddRange(generatedPrefab.GetComponentInChildren<Animator>().runtimeAnimatorController.animationClips);
            }
            else
            {
                clips = EditorUtility.CollectDependencies(gameObject).ToList();
                foreach (var obj in clips.ToArray())
                    clips.AddRange(AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(obj)));
                clips.AddRange(customClips.Select(q => (Object)q));
                clips.RemoveAll(q => q is AnimationClip == false || q == null);
            }

            foreach (AnimationClip clip in clips)
            {
                if (generateAnims.ContainsKey(clip.name) == false)
                    generateAnims.Add(clip.name, true);
            }
            clips.RemoveAll(q => generateAnims.ContainsKey(q.name) == false);
            clips.RemoveAll(q => generateAnims[q.name] == false);

            var distinctClips = clips.Select(q => (AnimationClip)q).Distinct().ToList();
            for (int i = 0; i < distinctClips.Count; i++)
            {
                if (generateAnims.ContainsKey(distinctClips[i].name) == false)
                    generateAnims.Add(distinctClips[i].name, true);
            }
            return distinctClips;
        }
    }
}