using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ReJoin : MonoBehaviourPunCallbacks
{
    [SerializeField] Button btn;

    public void RejoinRoom()
    {
        Debug.Log("HI");
        bool a=PhotonNetwork.ReconnectAndRejoin();
        Debug.Log(a);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("HI");
        PhotonNetwork.LoadLevel("Main");
    }

}
