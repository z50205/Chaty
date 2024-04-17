using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class GameHost : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private GameHolder _localplayer;
    [SerializeField] private Text _log;
    [SerializeField] public int whoseturn;
    [SerializeField] public int[] _playersactornum;
    [SerializeField] public bool[] _playersendturn;
    private void Start()
    {
        _log.text = "剪刀石頭布";
        _playersactornum = new int[20];
        _playersendturn = new bool[20];
        //Debug.Log(_playersactornum.Length);
    }
    private void Update()
    {
        Update_playersactornum();
        Update_endturnstate();
        //     _localplayer.getstate(_playersactornum[whoseturn] == PhotonNetwork.LocalPlayer.ActorNumber);
    }
    private void Update_playersactornum()
    {
        //Debug.Log(_playersactornum.Length);
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            //Debug.Log(i+","+PhotonNetwork.PlayerList[i].ActorNumber);
            this._playersactornum.SetValue(PhotonNetwork.PlayerList[i].ActorNumber, i);
        }
    }
    private async void Update_endturnstate()
    {
        // Debug.Log(_playersactornum.Length);
        // Debug.Log(_playersactornum.ToString());
        int pos=-1;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (_playersactornum[i]==PhotonNetwork.LocalPlayer.ActorNumber)
            pos=i;
        }
        // _playersendturn[pos] = _localplayer._endturn;

    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.whoseturn);
            stream.SendNext(this._playersactornum);
            stream.SendNext(this._playersendturn);
        }
        else if (stream.IsReading)
        {
            this.whoseturn = (int)stream.ReceiveNext();
            this._playersactornum = (int[])stream.ReceiveNext();
            this._playersendturn = (bool[])stream.ReceiveNext();
        }
    }
}
