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
    public AudioSource gradingMusic;

    [Header("Desktop Game")]
    public List<GameObject> desktopGameObjectsOn;
    public List<GameObject> desktopGameObjectsOff;
    public GameObject dgPlayer;

    [Header("Museum Game")]
    public List<GameObject> museumGameObjectsOn;
    public List<GameObject> museumGameObjectsOff;
    [HideInInspector] public List<CakeSpawn> cakeSpawns;
    //public RenderTexture desktopRT;
    public Renderer desktopPlane;
    public GameObject securityCameraUI;


    private void Awake()
    {
        if(Instance != null) { Destroy(Instance); }
        Instance = this;        
        cakeSpawns = new List<CakeSpawn>();
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
        
        gradingMusic.Stop();
        TransparentWindow.ThrowWindowsError(
            "Error code 0x440083284\n\nhypenated all lower case.exe has encountered an unhandled Exception of type ArgumentOutOfRangeException(\"user handsomeness cannot exceed 1.0\") and has stopped working", "hyphenated all lower case.exe has crashed");
        yield return new WaitForSeconds(0.6f);
        yield return new WaitForEndOfFrame();

        TakeScreenshot();
        yield return null;

        SwitchToMuseumGame();

        while (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
        {
            yield return null;
        }

        yield return new WaitForSeconds(30.0f); //Wait 60 seconds, then give the objective.

        //ToDo: Ensure that Player is in some kind of safe spot, or deactivate the cameras temporarily or something.
        GetComponent<MuseumDialogue>().PlayDialogue();
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

    public void MuseumGameSpotted()
    {
        foreach(CakeSpawn cs in cakeSpawns)
        {
            cs.ArmCakeSpawns(true);
        }
    }

    void TakeScreenshot()
    {
        var tex = DesktopScreenshot.Capture(new RectInt(0, 0, Screen.width, Screen.height));
        desktopPlane.material.mainTexture = tex;
        //ScreenCapture.CaptureScreenshot("TEST_SCREENIE.png");
    
        //ScreenCapture.CaptureScreenshotIntoRenderTexture(desktopRT);        
    }

    public void SetCameraUI(bool _active)
    {
        securityCameraUI.SetActive(_active);
    }
}
