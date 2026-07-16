
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RescaleHeight : UdonSharpBehaviour
{

    public GameObject entryContainer;
    public RectTransform mimicTransform;
    public int sectionHeight;
    private int count;
    void Start()
    {
        
    }

    public void LateUpdate()
    {
        count++;
        if (count < 30)
        {
            return;
        }

        count = 0;
        int amount = entryContainer.transform.childCount;

        mimicTransform.sizeDelta = new Vector2(mimicTransform.sizeDelta.x, amount * sectionHeight);
    }
}
