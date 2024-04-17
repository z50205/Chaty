using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System.Linq;
using Photon.Pun;
using UnityEngine.UI;

public class PSSServer : MonoBehaviourPunCallbacks
{
    [SerializeField] int turn;
    [SerializeField] ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
    //Evaluate variables
    [SerializeField] int[] _playersactor = new int[20];
    [SerializeField] string[] _playerresult = new string[20];
    Player[] players = PhotonNetwork.PlayerList;
    [SerializeField] int[] paper;
    [SerializeField] int[] scissor;
    [SerializeField] int[] stone;
    [SerializeField] int[] result;
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        turn = 1;
        StartCoroutine("StartPSSServer");
    }
    IEnumerator StartPSSServer()
    {
        this.hash["Server_GamePhase"] = 0;
        StartCoroutine("StartnewgameServer");
        StartCoroutine("StartendturnServer");
        yield return null;
    }
    IEnumerator StartnewgameServer()
    {
        //check every client and server enter a new game!
        while (true)
        {
            while (!PhotonNetwork.PlayerList.All(p => p.CustomProperties.ContainsKey("GamePhase") && p.CustomProperties.ContainsKey("Ready") && (int)p.CustomProperties["GamePhase"] == 0 && (bool)p.CustomProperties["Ready"]))
            {
                yield return null;
                Debug.Log("SERVER");
            }
            this.hash["Server_GamePhase"] = 1;
            this.hash["Allready"] = true;
            PhotonNetwork.MasterClient.SetCustomProperties(this.hash);
            yield return null;
        }
    }
    IEnumerator StartendturnServer()
    {
        while (true)
        {
            //check every client and server enter turn and choose a choice!
            while (!PhotonNetwork.PlayerList.All(p => p.CustomProperties.ContainsKey("Endturn") && ((int)p.CustomProperties["GamePhase"] == 1||(int)p.CustomProperties["GamePhase"] == 3) && (bool)p.CustomProperties["Endturn"]))
            {
                yield return null;
                Debug.Log("SERVER");
                Debug.Log(PhotonNetwork.PlayerList[0].CustomProperties.ToString());
            }
            //Evaluate the result!
            string log = Evaluate();
            this.hash["_playersactor"] = _playersactor;
            this.hash["_playerresult"] = _playerresult;
            this.hash["Server_GamePhase"] = 2;
            this.hash["_log"] = log;
            PhotonNetwork.MasterClient.SetCustomProperties(hash);
            while((int)PhotonNetwork.MasterClient.CustomProperties["Server_GamePhase"]!=2);
            this.turn = this.turn + 1;
            while (!PhotonNetwork.PlayerList.All(p => p.CustomProperties.ContainsKey("Endturn") && (int)p.CustomProperties["GamePhase"] == 2 && !(bool)p.CustomProperties["Endturn"]))
            {
                yield return null;
                Debug.Log("SERVER");
                Debug.Log(PhotonNetwork.PlayerList[0].CustomProperties.ToString());
            }
            this.hash["Server_GamePhase"] = 3;
            Debug.Log("SERVER3end");
            PhotonNetwork.MasterClient.SetCustomProperties(hash);
            while((int)PhotonNetwork.MasterClient.CustomProperties["Server_GamePhase"]!=3);
            yield return null;
        }
    }


    // public void setServerAllreadytrue()
    // {
    //     this.hash["Allready"] = true;
    //     PhotonNetwork.MasterClient.SetCustomProperties(hash);
    //     while (!(bool)PhotonNetwork.MasterClient.CustomProperties["Allready"]) ;
    // }










    private string Evaluate()
    {
        PSSpreprocessing();
        PSSResult();
        return PSSSetResult();
    }
    private void PSSpreprocessing()
    {

        this.paper = new int[20];
        this.scissor = new int[20];
        this.stone = new int[20];
        int paper_i = 0;
        int scissor_i = 0;
        int stone_i = 0;
        players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            Debug.Log("第" + i + "個，玩家總長為" + players.Length);

            if (players[i].CustomProperties.ContainsKey("PSS"))
            {
                switch (players[i].CustomProperties["PSS"])
                {
                    case 0:
                        paper[paper_i] = players[i].ActorNumber;
                        paper_i++;
                        break;
                    case 1:
                        scissor[scissor_i] = players[i].ActorNumber;
                        scissor_i++;
                        break;
                    case 2:
                        stone[stone_i] = players[i].ActorNumber;
                        stone_i++;
                        break;
                }
            }
        }
    }
    private void PSSResult()
    {
        bool _paper_empty = this.paper.All(v => v.Equals(0));
        bool _scissor_empty = this.scissor.All(v => v.Equals(0));
        bool _stone_empty = this.stone.All(v => v.Equals(0));
        if (_paper_empty && !_scissor_empty && !_stone_empty)
        { Debug.Log("Scissor lose,Stone win"); this.result = new int[] { -1, 0, 1 }; }
        else if (!_paper_empty && _scissor_empty && !_stone_empty)
        { Debug.Log("Paper win,Stone lose"); this.result = new int[] { 1, -1, 0 }; }
        else if (!_paper_empty && !_scissor_empty && _stone_empty)
        { Debug.Log("Paper lose,Scissor win"); this.result = new int[] { 0, 1, -1 }; }
        else
        { Debug.Log("Draw"); this.result = new int[] { -1, -1, -1 }; }
    }
    private string PSSSetResult()
    {
        string s = "";
        if (PhotonNetwork.MasterClient.CustomProperties.ContainsKey("_log"))
            s = (string)PhotonNetwork.MasterClient.CustomProperties["_log"];
        s = string.Concat(s, this.turn + " turn\n");
        for (int i = 0; i < players.Length; i++)
        {
            string[] temp = PSSResultInterpret((int)players[i].CustomProperties["PSS"], result);
            this._playersactor[i] = players[i].ActorNumber;
            this._playerresult[i] = temp[1];
            string temp_log = "Actor" + _playersactor[i] + ":出" + temp[0] + "結果為" + _playerresult[i] + "\n";
            s = string.Concat(s, temp_log);
        }
        return s;

    }
    private string[] PSSResultInterpret(int local, int[] result)
    {
        string[] s = new string[2];
        switch (result[local])
        {
            case -1:
                s[1] = "Draw";
                break;
            case 0:
                s[1] = "Lose!";
                break;
            case 1:
                s[1] = "Win!";
                break;
        }
        switch (local)
        {
            case 0:
                s[0] = "Paper";
                break;
            case 1:
                s[0] = "Scissor";
                break;
            case 2:
                s[0] = "Stone";
                break;
        }
        return s;
    }
}
