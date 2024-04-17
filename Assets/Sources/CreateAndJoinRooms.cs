using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    [SerializeField] InputField joincreatename;
    [SerializeField] Button joinbtn;
    private void Start() {
        PhotonNetwork.JoinLobby();
    }
    public void CreateRoom()
    {
        RoomOptions _roomoption=new RoomOptions{ IsVisible = true};//PlayerTtl=600000
        PhotonNetwork.JoinOrCreateRoom(joincreatename.text,_roomoption,null);
        joinbtn.interactable=false;
    }
    public void RejoinRoom()
    {
        bool a=PhotonNetwork.RejoinRoom(joincreatename.text);
        Debug.Log(a);
    }
    public override void OnJoinedRoom()
    {
        //Debug.Log("HI");
        PhotonNetwork.LoadLevel("Main");
    }
}
