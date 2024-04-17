using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Threading.Tasks;

public class ChatBubble : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject _chat;
    Player _player;
    [SerializeField] GameObject _chatbar;
    [SerializeField] Camera _cam;
    public GameObject canvas;
    private float Scale;
    private float delay = 100.0f;
    float ini_time;
    private void Start()
    {
        ini_time= Time.deltaTime;
        PhotonNetwork.AutomaticallySyncScene = true;
        _chatbar.SetActive(false);
        //2D轉換canvas及camera
        TransCoor();
        //實例一個對話框
        _chat = PhotonNetwork.Instantiate("text", transform.position, Quaternion.identity);
    }
    private void Update()
    {
        //設定Text 至Canvas
        TransCoor();
        //將對話框置於自己人物上
        GameObject player1 = GameObject.FindGameObjectWithTag("player");
        _chat.GetComponent<RectTransform>().anchoredPosition = Vector3.Scale(player1.transform.position, new Vector3(Scale, Scale, 1));
        //text.GetComponent<RectTransform>().anchoredPosition = Vector3.Scale(player1.transform.position, scaler);
        Chat();
    }
    private void Chat()
    {
        if (Input.GetKeyDown(KeyCode.Return) & !_chatbar.activeSelf)
        {
            _chatbar.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Return) & _chatbar.GetComponent<InputField>().text!="")
        {
            _chat.GetComponent<Text>().text = _chatbar.GetComponent<InputField>().text;
            _chatbar.GetComponent<InputField>().text="";
            _chatbar.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Return) & _chatbar.activeSelf)
        {
            _chatbar.SetActive(false);
        }
    }
    private void TransCoor()
    {
        Scale = canvas.GetComponent<RectTransform>().rect.height / _cam.orthographicSize / 2;
    }
}
