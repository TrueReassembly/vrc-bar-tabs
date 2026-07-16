using System;
using UdonSharp;
using UnityEngine;

namespace TabSystem
{
    public class CreateNewTabButton : UdonSharpBehaviour
    {
        public GameObject templateSection;
        public GameObject contentWindow;
        
        void Start()
        {
        
        }

        public void OnClick()
        {
            var obj = Instantiate(templateSection, contentWindow.transform, true);
            obj.transform.position.Set
            (
                contentWindow.transform.position.x,
                contentWindow.transform.position.y,
                contentWindow.transform.position.z
            );
        }
    }
}
