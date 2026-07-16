
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class SpawnTabButton : UdonSharpBehaviour
{

    public VRCObjectPool tabPool;
    
    void Start()
    {
        
    }

    public override void Interact()
    {
        Debug.Log("Spawned Tab");
        // GameObject tab = Instantiate(templateTab, tabContainer.transform);
        Networking.SetOwner(Networking.LocalPlayer, tabPool.gameObject);
        GameObject tab = tabPool.TryToSpawn();
        if (tab == null)
        {
            Debug.Log("Couldn't spawn the tab, aborting.");
            return;
        }
        Networking.SetOwner(Networking.LocalPlayer, tab);
        Vector3 location = Networking.LocalPlayer.GetPosition() + Networking.LocalPlayer.GetRotation() * (Vector3.forward * 1.2f);
        location.y = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position.y / 1.2f;
            
        tab.transform.position = location;
        tab.transform.LookAt(Networking.LocalPlayer.GetPosition());
        tab.transform.rotation = Quaternion.Euler(tab.transform.rotation.eulerAngles.x + 270f, tab.transform.rotation.eulerAngles.y + 180f, tab.transform.rotation.eulerAngles.z);
    }
}
