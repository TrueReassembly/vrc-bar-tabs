
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class PrepareTabPool : UdonSharpBehaviour
{
    public int tabs = 20;
    public VRCObjectPool pool;
    void Awake()
    {
        pool.Pool = new GameObject[tabs];
        for (int i = 0; i < tabs; i++)
        {
            pool.Pool[i] = transform.GetChild(i).gameObject;
        }
    }
}
