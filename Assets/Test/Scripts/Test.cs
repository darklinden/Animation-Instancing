using System.Collections;
using System.Collections.Generic;
using AnimationInstancing;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject showObj;

    // Start is called before the first frame update
    async UniTaskVoid Start()
    {
        Log.D("Start");
        // prepare
        await LoadUtil.Load<TextAsset>(AnimationInstancingPrefab.TEST.ToAddr());

        Log.D("Load");

        await UniTask.Yield();

        Log.D("Yield");

        showObj.SetActive(true);

        Log.D("SetActive");
    }
}
