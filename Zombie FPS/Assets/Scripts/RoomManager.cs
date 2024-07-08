using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(Instance);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += onSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= onSceneLoaded;

    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= onSceneLoaded;

    }
    private void onSceneLoaded(Scene scene,LoadSceneMode mode) 
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-3, 3), 2, Random.Range(-3, 3)) ;
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.Instantiate("First_Person_Player", spawnPosition, Quaternion.identity);
        }
        else
        {
            Instantiate(Resources.Load("First_Person_Player"), spawnPosition, Quaternion.identity);
        }
    }
}
