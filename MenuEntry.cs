using System;
using BestHTTP.Extensions;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace TabSystem
{
    public class MenuEntry : UdonSharpBehaviour
    {

        public TMP_Text name;
        public TMP_Text price;
        public TabController TabController;

        private int _priceInCopper;
        void Start()
        {
            
        }
        
        public void SetItem(string theName, int copper)
        {
            TabController = gameObject.transform.GetComponentInParent<TabController>();
            name.text = theName;
            _priceInCopper = copper;
            Debug.Log($"Called SetItem with price {copper}");
            TabController.AddCost(copper);
            price.text = TabController.GetCost(copper);
            TabController.UpdateFinalCost();
        }

        public void OnDeleteButtonClick()
        {
            Networking.SetOwner(Networking.LocalPlayer, TabController.gameObject);
            TabController.SubtractCost(_priceInCopper);
            TabController.UpdateFinalCost();
            TabController.SendCustomEventDelayedFrames(nameof(TabController.SyncAllLists), 2);
            Destroy(gameObject);
        }

        public override string ToString()
        {
            return $"{name.text};{_priceInCopper}";
        }
        
        public int GetPriceInCopper()
        {
            return _priceInCopper;
        }
        
    }
}