using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomName : MonoBehaviour
{
    [SerializeField] Text _text;
    public GameObject playerPrefab;
    public int maxX;
    public int minX;
    public int maxY;
    public int minY;
    void Start()
    {
        SetRoomName();
        SpawnPlayer();
    }
    public void SetRoomName()
    {
        _text.text = "RoomName:" + PhotonNetwork.CurrentRoom.Name;
    }
    public void SpawnPlayer()
    {
        Vector2 randomPosition = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
    }
}
