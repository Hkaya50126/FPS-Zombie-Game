using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class GameManagerScript : MonoBehaviourPunCallbacks
{
    public int enemiesAlive = 0;

    public int round = 0;
    public GameObject[] spawnPoints;
    public GameObject enemyPrefab;
    public GameObject endScreen;
    public GameObject pauseMenu;
    public Animator blackScreenAnimator;
    public TMP_Text roundText;
    public TMP_Text roundSurvived;

    public PhotonView photonView;


    void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawners");
    }

    // Update is called once per frame
    void Update()
    {
        if ((PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient) && !photonView.IsMine) 
        {
            return;
        }

        if (enemiesAlive == 0)
        {
            Debug.Log("girdi");
            round++;
            NextWave(round);
            if (PhotonNetwork.InRoom)
            {
                Hashtable hash=new Hashtable();
                hash.Add("currentRound", round) ;
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
            else
            {
                DisplayNextRound(round);

            }
            
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }
    private void DisplayNextRound(int round)
    {
        roundText.text = "Round: " + round.ToString();
    }
    public void NextWave(int round)
    {
        for (int i = 0; i < round; i++)
        {
            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemySpawned;
            if (PhotonNetwork.InRoom)
            {
                enemySpawned = PhotonNetwork.Instantiate("Zombie", spawnPoint.transform.position, Quaternion.identity);
            }
            else
            {
                enemySpawned = Instantiate(Resources.Load("Zombie"), spawnPoint.transform.position, Quaternion.identity) as GameObject ;

            }
            enemySpawned.GetComponent<EnemyManager>().gameManager = GetComponent<GameManagerScript>();

            enemiesAlive++;
        }

    }
    public void EndGame()
    {
        if (PhotonNetwork.InRoom)
        {
            Time.timeScale = 0;

        }
        Cursor.lockState = CursorLockMode.None;
        roundSurvived.text = round.ToString();
        endScreen.SetActive(true);
    }
    public void MainMenu()
    {
        if (PhotonNetwork.InRoom)
        {
            Time.timeScale = 1;

        }
        blackScreenAnimator.SetTrigger("FadeIn");
        Invoke("LoadMainMenuScene", 0.4f);
    }
    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(0);

    }
    public void Restart()
    {
        if (PhotonNetwork.InRoom)
        {
            Time.timeScale = 1;

        }
        SceneManager.LoadScene(1);
    }
    public void Pause()
    {
        if (PhotonNetwork.InRoom)
        {
            Time.timeScale = 0;

        }
        pauseMenu.SetActive(true);
        Cursor.lockState= CursorLockMode.None;
    }

    public void Continue()
    {
        if (PhotonNetwork.InRoom)
        {
            Time.timeScale = 1;

        }
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("Player" + targetPlayer + " changed " + changedProps) ;

        if (photonView.IsMine)
        {
            if (changedProps["currentRound"] != null)
            {
                DisplayNextRound((int)changedProps["currentRound"]);
            }
        }
    }
}
