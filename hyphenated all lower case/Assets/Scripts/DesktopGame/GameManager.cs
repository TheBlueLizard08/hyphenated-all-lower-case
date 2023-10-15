using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }    
    public string playerName;

    [Header("Grading Game")]
    public List<GameObject> gradingGameObjectsOff;

    [Header("Desktop Game")]
    public List<GameObject> desktopGameObjectsOn;
    public List<GameObject> desktopGameObjectsOff;
    public GameObject dgPlayer;

    [Header("Museum Game")]
    public List<GameObject> museumGameObjectsOn;
    public List<GameObject> museumGameObjectsOff;
    //public RenderTexture desktopRT;
    public Renderer desktopPlane;


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
        yield return new WaitForSeconds(1.0f);
        yield return new WaitForEndOfFrame();

        TakeScreenshot();
        yield return null;

        SwitchToMuseumGame();

        //yield return new WaitForSeconds(0.5f);
        //dgPlayer.SetActive(true);
    }

    void SwitchToMuseumGame()
    {
        foreach (GameObject o in desktopGameObjectsOff)
        {
            o.SetActive(false);
        }

        foreach (GameObject o in museumGameObjectsOn)
        {
            o.SetActive(true);
        }
    }

    void TakeScreenshot()
    {
        var tex = DesktopScreenshot.Capture(new RectInt(0, 0, Screen.width, Screen.height));
        desktopPlane.material.mainTexture = tex;
        //ScreenCapture.CaptureScreenshot("TEST_SCREENIE.png");
    
        //ScreenCapture.CaptureScreenshotIntoRenderTexture(desktopRT);        
    }
}
