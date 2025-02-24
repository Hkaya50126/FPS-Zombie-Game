using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject connecting;
    public GameObject multiplayer;
    void Start()
    {
        Debug.Log("connecting to server");
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Joining lobby");
        PhotonNetwork.JoinLobby();

    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Ready for multiplayer");
        connecting.SetActive(false);
        multiplayer.SetActive(true);
    }
    public void FindMatch()
    {

        Debug.Log("Finding Room");
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        MakeRoom();
    }
    void MakeRoom()
    {
        int randomRoomName = Random.Range(0,5000);
        RoomOptions roomOptions = new RoomOptions() {
            IsVisible = true,
            IsOpen=true,
            MaxPlayers=6,
            PublishUserId=true
        };

        PhotonNetwork.CreateRoom("RoomName_"+randomRoomName,roomOptions);

        Debug.Log("Room Made: "+ randomRoomName);

    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Loading 1");
        //SceneManager.LoadScene(1);
        PhotonNetwork.LoadLevel(2);
    }
}
