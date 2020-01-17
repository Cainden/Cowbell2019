// Handling incoming/outgoing avatars

using MySpace;
using System.Collections;
using System.IO;
using UnityEngine;

public class LoadSaveManager : MonoBehaviour
{
    [HideInInspector]
    public static LoadSaveManager Ref { get; private set; } // For external access of script

    private string _SaveFileNameManList;
    private string _SaveFileNameRoomList;

    void Awake()
    {
        if (Ref == null) Ref = GetComponent<LoadSaveManager>();
        _SaveFileNameManList = Application.persistentDataPath + "/ManListSave.xml";
        _SaveFileNameRoomList = Application.persistentDataPath + "/RoomListSave.xml";
    }


    public void SaveCurrentState()
    {
        ManManager.Ref.SaveManList(_SaveFileNameManList);
        RoomManager.Ref.SaveRoomList(_SaveFileNameRoomList);

        GuiManager.Ref.Initiate_UserInfoSmall("Data saved to file!");
        Debug.Log("Saved: " + _SaveFileNameManList);
        Debug.Log("Saved: " + _SaveFileNameRoomList);
    }

    public IEnumerator LoadCurrentState()
    {
        if (!File.Exists(_SaveFileNameManList)) yield break;
        if (!File.Exists(_SaveFileNameRoomList)) yield break;

        StateManager.Ref.ClearAll();

        RoomManager.Ref.LoadRoomList(_SaveFileNameRoomList);

        yield return null;

        ManManager.Ref.LoadManList(_SaveFileNameManList);

        yield return null;

        StateManager.Ref.SetGameState(Enums.GameStates.Normal);
        GuiManager.Ref.Initiate_UserInfoSmall("Data loaded from file!");
    }
}
