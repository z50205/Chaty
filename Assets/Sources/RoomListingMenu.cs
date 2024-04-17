using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform _content;
    [SerializeField] private RoomList _roomLisiting;

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomList();
        foreach (RoomInfo info in roomList)
        {
            Debug.Log(_content.position);
            RoomList listing = Instantiate(_roomLisiting,_content);
            if(listing!=null)
            listing.SetRoomInfo(info);
        }
    }
    private void  ClearRoomList()
    {
        Transform content=transform.Find("Scroll View/Viewport/Content");
        foreach(Transform a in content)Destroy(a.gameObject);
    }
}
