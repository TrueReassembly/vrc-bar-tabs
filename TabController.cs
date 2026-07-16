using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.Serialization;
using VRC.SDK3.Components;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

namespace TabSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class TabController : UdonSharpBehaviour
    {
        public TMP_Text priceText;
        public TMP_InputField nameField;
        public GameObject confirmationElements;
        public DrinkHandler drinkHandler;
        public FoodHandler foodHandler;
        public VRCObjectPool tabpool;
        
        private bool flaggedForDeletion;
        [SerializeField, UdonSynced] private int _totalPrice = 0;

        private DataList drinks = new DataList();
        [SerializeField, UdonSynced] private string _tokenizedDrinks = "";
        private DataList food = new DataList();
        [SerializeField, UdonSynced] private string _tokenizedFood = "";
        [SerializeField, UdonSynced] private bool _syncingDrinks = false;
        [SerializeField, UdonSynced] private string _tabName;
        void Start()
        {
            _totalPrice = 0;
            confirmationElements.SetActive(false);
        }

        public void AddCost(int amount)
        {
            _totalPrice += amount;
        }

        public void SubtractCost(int amount)
        {
            _totalPrice -= amount;
        }

        public static string GetCost(int copper)
        {
            // 1 Copper 
            // 1 Silver = 10 Copper
            // 1 Gold = 100 Copper
            // 1 Platinum = 1000 Copper
            int platinum = copper / 1000;
            copper %= 1000;
            int gold = copper / 100;
            copper %= 100;
            int silver = copper / 10;
            copper %= 10;

            return $"{platinum}P {gold}G {silver}S {copper}C";
        }

        public string GetCost()
        {
            return GetCost(_totalPrice);
        }

        public void UpdateFinalCost()
        {
            priceText.text = GetCost();
        }

        public void OnCloseTabButtonClick()
        {
            if (!flaggedForDeletion)
            {
                confirmationElements.SetActive(true);
                flaggedForDeletion = true;
                return;
            }
            Networking.SetOwner(Networking.LocalPlayer, transform.parent.gameObject);
            SendCustomEventDelayedFrames(nameof(ResetTab), 1);
        }

        public void ResetTab()
        {
            if (!Networking.IsOwner(Networking.LocalPlayer, transform.parent.gameObject))
            {
                SendCustomEventDelayedFrames(nameof(ResetTab), 1);
                return;
            }

            _tabName = null;
            nameField.text = null;
            flaggedForDeletion = false;
            confirmationElements.SetActive(false);
            drinkHandler.Clear();
            foodHandler.Clear();
            SyncAllLists();
            UpdateFinalCost();
            tabpool.Return(transform.parent.gameObject);
        }
        
        public void OnCloseTabRejectButtonClick()
        {
            flaggedForDeletion = false;
            confirmationElements.SetActive(false);
        }
        
        // Networking code
        public void SyncDrinks()
        {
            drinks.Clear();
            var entries = transform.Find("Drinks").Find("Scroll View").Find("Viewport").Find("Content").Find("Entries");
            foreach (Transform entry in entries)
            {
                if (!entry.name.Contains("Entry")) continue;
                
                var entryData = entry.GetComponent<MenuEntry>();
                if (entryData == null) continue;
                
                drinks.Add(entryData.ToString());
            }

            if (!VRCJson.TrySerializeToJson(drinks, JsonExportType.Minify, out DataToken json))
            {
                Debug.LogWarning("Failed serializing the drinks JSON, aborting.");
                Debug.LogWarning(json.ToString());
                return;
            }

            _tokenizedDrinks = json.String;
            _tabName = nameField.text;
            RequestSerialization();
        }
        
        public void SyncFood()
        {
            food.Clear();
            var entries = transform.Find("Food").Find("Scroll View").Find("Viewport").Find("Content").Find("Entries");
            foreach (Transform entry in entries)
            {
                if (!entry.name.Contains("Entry")) continue;
                
                var entryData = entry.GetComponent<MenuEntry>();
                if (entryData == null) continue;
                
                food.Add(entryData.ToString());
            }

            if (!VRCJson.TrySerializeToJson(food, JsonExportType.Minify, out DataToken json))
            {
                Debug.LogWarning("Failed serializing the food JSON, aborting");
                Debug.LogWarning(json.ToString());
                return;
            }

            _tokenizedFood = json.String;
            _tabName = nameField.text;
            RequestSerialization();
        }

        public override void OnDeserialization()
        {
            _totalPrice = 0;
            nameField.text = _tabName;
            // Handle drinks
            if (!string.IsNullOrEmpty(_tokenizedDrinks))
            {
                if (VRCJson.TryDeserializeFromJson(_tokenizedDrinks, out var drinksResult))
                {
                    drinks = drinksResult.DataList;
                    drinkHandler.Clear();
                    SendCustomEventDelayedFrames(nameof(CheckAndPopulateDrinks), 1);
                }
            }

            // Handle food
            if (!string.IsNullOrEmpty(_tokenizedFood))
            {
                if (VRCJson.TryDeserializeFromJson(_tokenizedFood, out var foodResult))
                {
                    food = foodResult.DataList;
                    foodHandler.Clear();
                    SendCustomEventDelayedFrames(nameof(CheckAndPopulateFood), 1);
                }
            }
        }

        public void CheckAndPopulateDrinks()
        {
            if (drinkHandler.IsClearing)
            {
                SendCustomEventDelayedFrames(nameof(CheckAndPopulateDrinks), 1);
                return;
            }
            Debug.LogFormat("Drinks Length = {0}", drinks.ToArray().Length);
            Debug.Log("Preparing to write to the drinks list.");
            foreach (var drinkToken in drinks.ToArray())
            {
                string drink = drinkToken.String;
                Debug.LogFormat("Writing {0} to the drink list", drink);
                drinkHandler.FromString(drink);
            }
            
            RecalculateTotalPrice();
        }
        
        public void CheckAndPopulateFood()
        {
            if (foodHandler.IsClearing)
            {
                SendCustomEventDelayedFrames(nameof(CheckAndPopulateFood), 1);
                return;
            }
            Debug.LogFormat("Food Length = {0}", food.ToArray().Length);
            Debug.Log("Preparing to write to the food list.");
            foreach (var foodToken in food.ToArray())
            {
                string theFood = foodToken.String;
                Debug.LogFormat("Writing {0} to the food list", theFood);
                foodHandler.FromString(theFood);
            }
            
            RecalculateTotalPrice();
        }
        
        public void RecalculateTotalPrice()
        {
            _totalPrice = 0;
            var drinkEntries = transform.Find("Drinks").Find("Scroll View").Find("Viewport").Find("Content").Find("Entries");
            var foodEntries = transform.Find("Food").Find("Scroll View").Find("Viewport").Find("Content").Find("Entries");
            
            foreach (Transform entry in drinkEntries)
            {
                if (!entry.name.Contains("Entry")) continue;
                var menuEntry = entry.GetComponent<MenuEntry>();
                if (menuEntry != null)
                {
                    _totalPrice += menuEntry.GetPriceInCopper();
                }
            }
            
            foreach (Transform entry in foodEntries)
            {
                if (!entry.name.Contains("Entry")) continue;
                var menuEntry = entry.GetComponent<MenuEntry>();
                if (menuEntry != null)
                {
                    _totalPrice += menuEntry.GetPriceInCopper();
                }
            }

            UpdateFinalCost();
        }

        public void SyncAllLists()
        {
            if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
            {
                Debug.LogWarning("Not owner of TabController, cannot sync lists");
                return;
            }

            if (drinkHandler.IsClearing || foodHandler.IsClearing)
            {
                SendCustomEventDelayedFrames(nameof(SyncAllLists), 1);
                return;
            }

            Debug.Log("Syncing all lists");
            SyncDrinks();
            SendCustomEventDelayedFrames(nameof(SyncFood), 1);
        }
    }
}