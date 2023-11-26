using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGP.Tools
{
    public class BoneCopier : MonoBehaviour
    {
        public SkinnedMeshRenderer model;
        public SkinnedMeshRenderer sample;
        public Transform rootBone;
        private List<Transform> newBones; 
        private Dictionary<string, Transform> dict;

        public void Generate()
        {
            newBones = new List<Transform>(); 
            dict = new Dictionary<string, Transform>();

            foreach (Transform bone in sample.bones)
            {
                dict.Add(bone.gameObject.name, bone);
            }

            foreach (Transform bone in model.bones)
            {
                try
                {
                    newBones.Add(dict[bone.gameObject.name]);
                }
                catch (System.Exception)
                {
                    newBones.Add(rootBone);
                }
            }
            
            model.bones = newBones.ToArray();
            model.rootBone = rootBone;
        }
    }
}
