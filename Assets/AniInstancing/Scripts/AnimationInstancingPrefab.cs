using System;
using System.Collections.Generic;

namespace AnimationInstancing
{
    public enum AnimationInstancingPrefab
    {
        None,
        TEST
    }

    public static class AnimationInstancingPrefabExtension
    {
        private static Dictionary<int, string> s_enumToNames = null;

        private static Dictionary<int, string> GetEnumNames()
        {
            if (s_enumToNames == null)
            {
                System.Collections.IList list = Enum.GetValues(typeof(AnimationInstancingPrefab));

                var result = new Dictionary<int, string>(list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    result.Add((int)list[i], list[i].ToString());
                }

                s_enumToNames = result;
            }
            return s_enumToNames;
        }

        public static string ToName(this AnimationInstancingPrefab myEnum)
        {
            return GetEnumNames()[(int)myEnum];
        }

        private static Dictionary<int, string> s_enumToAddrs = null;

        private static Dictionary<int, string> GetEnumAddrs()
        {
            if (s_enumToAddrs == null)
            {
                System.Collections.IList list = Enum.GetValues(typeof(AnimationInstancingPrefab));

                var result = new Dictionary<int, string>(list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    result.Add((int)list[i], "Assets/AddrFiles/AnimationTexture/" + list[i].ToString() + ".bytes");
                }

                s_enumToAddrs = result;
            }
            return s_enumToAddrs;
        }

        public static string ToAddr(this AnimationInstancingPrefab myEnum)
        {
            return GetEnumAddrs()[(int)myEnum];
        }

        public static int ToInt(this AnimationInstancingPrefab myEnum)
        {
            return (int)myEnum;
        }
    }
}
