using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }    
    public string playerName;

    [Header("Grading Game")]
    public List<GameObject> gradingGameObjectsOff;

    [Header("Desktop Game")]
    public List<GameObject> desktopGameObjectsOn;
    public List<GameObject> desktopGameObjectsOff;


    private void Awake()
    {
        if(Instance != null) { Destroy(Instance); }
        Instance = this;        
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetPlayerName(string _name)
    {
        playerName = _name;
    }

    public void SwitchToDesktopGame()
    {
        foreach(GameObject o in gradingGameObjectsOff)
        {
            o.SetActive(false);
        }

        foreach(GameObject o in desktopGameObjectsOn)
        {
            o.SetActive(true);
        }

        StartCoroutine(GradingGameStartC());
    }

    IEnumerator GradingGameStartC()
    {
        yield return null; yield return null; yield return null;

        TransparentWindow.ThrowWindowsError(
            "Error code 0x440083284\n\nhypenated all lower case.exe has encountered an unhandled Exception of type ArgumentOutOfRangeException(\"user handsomeness cannot exceed 1.0\") and has stopped working", "hyphenated all lower case.exe has crashed");
    }
}
