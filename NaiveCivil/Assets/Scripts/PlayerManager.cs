using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static TurnManager turnManager;
    public static GameObject LocalPlayerInstance;
    public Material clientMaterial;
    public const int maxStamina = 3;
    public int stamina = -1;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
            turnManager = GetComponent<TurnManager>();
            turnManager.TurnManagerListener = new PlayerListener(this);
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(gameObject);

        if (PhotonNetwork.MasterClient != photonView.Controller)
        {
            GetComponent<MeshRenderer>().material = clientMaterial;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            turnManager.ResetTurnCount();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            ProcessInputs();
        }
    }

    Dictionary<KeyCode, Vector3> moveDict = new Dictionary<KeyCode, Vector3>
    {
        { KeyCode.UpArrow, new Vector3(-1, 0, 0) },
        { KeyCode.DownArrow, new Vector3(1, 0, 0) },
        { KeyCode.RightArrow, new Vector3(0, 0, 1) },
        { KeyCode.LeftArrow, new Vector3(0, 0, -1) }
    };

    
    void ProcessInputs()
    {
        if (stamina <= 0) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("End turn");
            stamina = 0;
            turnManager.SendMove(KeyCode.Space, true);
            return;
        }
        foreach (var pair in moveDict)
        {
            if (Input.GetKeyDown(pair.Key))
            {
                this.transform.position += pair.Value;
                stamina -= 1;
                Debug.Log($"stamina: {stamina}");
                turnManager.SendMove(pair.Key, stamina <= 0);
                break;
            }
        }
    }

    public void CompleteTurn(int turn)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            turnManager.BeginTurn();
        }
    }
    public void BeginTurn(int turn)
    {
        Debug.Log("BeginTurn called.");
        stamina = maxStamina;
    }
}

public class PlayerListener : ITurnManagerCallbacks
{
    PlayerManager manager;
    public PlayerListener(PlayerManager playerManager)
    {
        manager = playerManager;
    }

    public void OnPlayerFinished(Player player, int turn, object move)
    {
        
    }

    public void OnPlayerMove(Player player, int turn, object move)
    {
        
    }

    public void OnTurnBegins(int turn)
    {
        Debug.Log("OnTurnBegins called.");
        manager.BeginTurn(turn);
    }

    public void OnTurnCompleted(int turn)
    {
        manager.CompleteTurn(turn);
    }

    public void OnTurnTimeEnds(int turn)
    {
        
    }
}
