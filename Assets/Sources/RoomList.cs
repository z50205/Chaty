using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class RoomList : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text _text;
    public void SetRoomInfo(RoomInfo roominfo)
    {
        _text.text=roominfo.Name;
        
    }
}