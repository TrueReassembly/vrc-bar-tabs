using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace TabSystem
{
    public class FoodHandler : UdonSharpBehaviour
    {
        public TMP_Dropdown dropdown;
        public GameObject templateTab;
        public GameObject contentWindow;
        public TabController tabController;

        private Transform entries;
        public bool IsClearing { get; private set; }

        private void Start()
        {
            templateTab.SetActive(false);
            IsClearing = false;
        }
        
        public void FromString(string text)
        {
            var parsed = text.Split(';');
            if (!Networking.IsOwner(Networking.LocalPlayer, tabController.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, tabController.gameObject);
            }
            Create(parsed[0], Convert.ToInt32(parsed[1]), false);
        }
        
        public void OnDropdownSelect()
        {
            int price = GetPrice(dropdown.value);
            string name = GetName(dropdown.value);
            if (price == -1) return;
            dropdown.SetValueWithoutNotify(0);
            Create(name, price, true);
        }
        
        private void Create(string name, int price, bool sync)
        {
            GameObject entry = Instantiate(templateTab, contentWindow.transform);
            entry.SetActive(true);
            
            // Take ownership of the new entry
            if (!Networking.IsOwner(Networking.LocalPlayer, entry))
            {
                Networking.SetOwner(Networking.LocalPlayer, entry);
            }
            
            var menuEntry = entry.GetComponent<MenuEntry>();
            menuEntry.SetItem(name, price);
            
            if (sync)
            {
                Debug.Log("Syncing");
                if (!Networking.IsOwner(Networking.LocalPlayer, tabController.gameObject))
                {
                    Networking.SetOwner(Networking.LocalPlayer, tabController.gameObject);
                }
                tabController.SyncFood();
            }
        }

        private static string GetName(int value)
        {
            switch (value)
            {
                // These should match what's set in the dropdown
                case 1: return "A missile that knows";
                case 2: return "Spaghetti Bolognese";
                case 3: return "Bread";
                case 4: return "Ice Cream Man";
            }

            return "";
        }
        
        // 1 Copper 
        // 1 Silver = 10 Copper
        // 1 Gold = 100 Copper
        // 1 Platinum = 1000 Copper
        private static int GetPrice(int value)
        {
            switch (value)
            {
                // These should match what's set in the dropdown. The prices are labelled in the lowest denomination you use
                case 0: return -1; // Nothing Selected
                case 1: return 5534; // Missile
                case 2: return 40; // Spag Bol
                case 3: return 300; // Bread
                case 4: return 1; // Ice Cream man
            }

            return -1;
        }
        
        public void Clear()
        {
            entries = transform.Find("Scroll View").Find("Viewport").Find("Content").Find("Entries");
            IsClearing = true;
        }

        private void Update()
        {
            if (!IsClearing) return;
            if (entries.childCount > 0)
            {
                Destroy(entries.GetChild(0).gameObject);
            }
            else
            {
                IsClearing = false;
                Debug.Log("Cleared Items from Food");
            }
        }
    }
}