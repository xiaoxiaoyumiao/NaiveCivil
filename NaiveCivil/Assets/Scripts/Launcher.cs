using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
public class Launcher : MonoBehaviourPunCallbacks
{
    public string roomName;
    public string levelName;
    public Dropdown levelNameChoices;

    public string gameVersion = "1";

    bool isConnecting;

    RoomOptions roomOptions;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        levelName = levelNameChoices.options[levelNameChoices.value].text;
        roomName = levelName + "firstRoom";
        roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateLevelSelection(int choice)
    {
        levelName = levelNameChoices.options[levelNameChoices.value].text;
        roomName = levelName + "firstRoom";
    }

    public void Connect()
    {
        isConnecting = true;
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        } else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = this.gameVersion;
        }
    }

    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        isConnecting = false;
    }

    public override void OnJoinedRoom()
    {
        // #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LoadLevel(levelName);
        }
    }
}


public class Character
{
    public string id;
    Player owner = null;

    public Player Owner
    {
        get { if (owner.ActorNumber == -1) owner = null; return owner; }
        set { owner = value; }
    }
    public bool Allocated
    {
        get { return Owner != null; }
    }
    public Character(string uuid)
    {
        id = uuid;
    }
}

public class CharacterPool
{
    List<Character> characters;
    public CharacterPool(List<string> ids)
    {
        characters = new List<Character>();
        foreach (string id in ids)
        {
            characters.Add(new Character(id));
        }
    }

    public Character AllocateCharacter(Player player)
    {
        foreach (Character chara in characters)
        {
            if (!chara.Allocated)
            {
                chara.Owner = player;
                return chara;
            }
        }
        return null;
    }

    public void FreeCharacter(Character character)
    {
        var target = characters.Find((Character chara) => { return chara.id == character.id; });
        if (target != null)
        {
            target.Owner = null;
        }
    }
}
