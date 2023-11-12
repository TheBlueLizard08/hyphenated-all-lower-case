using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MuseumPlayer : MonoBehaviour
{
    public GameObject checkPlane, throwCakeUIPanel, playerCakeProjectilePrefab, cakeProjectilePrefab;
    public Transform playerThrownCakeSpawn, playerThrownCakeTarget, thrownBackCakeSpawn;
    public LayerMask raycastMask;
    public PlayerMovement museumPlayerMovement;
    public MouseLook[] museumMouseLooks;
    public AudioSource cakeSplatAS;
    public RectTransform cakeSplat;
    public int carSceneIndex = 2;

    bool canThrowCake = false;
    bool cakeIsThrown = false;
    bool hasDied = false;

    private void Update()
    {
        throwCakeUIPanel.gameObject.SetActive(canThrowCake && !cakeIsThrown);

        if(Input.GetMouseButtonDown(0) && canThrowCake && !cakeIsThrown)
        {
            Debug.Log("YEET!");
            StartCoroutine(ThrowCakeC());
        }
    }

    IEnumerator ThrowCakeC()
    {
        if (cakeIsThrown) { yield break; }
        cakeIsThrown = true;
        //Disable player code
        DisablePlayer();

        //yeet cake from spawn to target point
        CakeProjectile cake = Instantiate(playerCakeProjectilePrefab, playerThrownCakeSpawn.position, Quaternion.identity).GetComponent<CakeProjectile>();
        cake.Initialize(playerThrownCakeTarget);

        //Wait a moment
        yield return new WaitForSeconds(2.0f);

        //yeet cake back
        CakeProjectile cake2 = Instantiate(cakeProjectilePrefab, thrownBackCakeSpawn.position, Quaternion.identity).GetComponent<CakeProjectile>();
        cake2.Initialize(Camera.main.transform);

        //scene switch happens automatically because of death
    }

    void DisablePlayer()
    {
        museumPlayerMovement.enabled = false;
        foreach (MouseLook ml in museumMouseLooks) { ml.enabled = false; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Death"))
        {
            Debug.Log("Got hit by cake LOL scrub");
            StartCoroutine(HitByCakeC());
        }
    }

    IEnumerator HitByCakeC()
    {
        if (hasDied) { yield break; }
        hasDied = true;

        DisablePlayer();

        cakeSplatAS.Play();

        cakeSplat.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        cakeSplat.gameObject.SetActive(true);

        float t = 0.0f;
        while(t<=1.0f)
        {
            yield return null;
            t += Time.deltaTime * 4.0f;
            cakeSplat.localScale = Vector3.Lerp(new Vector3(0.05f, 0.05f, 0.05f), Vector3.one, t);            
        }

        yield return new WaitForSeconds(1.0f);
        Debug.Log("SCENE SWITCH");

        SceneManager.LoadScene(carSceneIndex);
    }

    //Function called from a triggerStay event
    public void CheckCakeThrow()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out hit, 10, raycastMask))
        {
            canThrowCake = hit.collider.gameObject == checkPlane;
        }
        else
        {
            canThrowCake = false;
        }
    }

    //Function called from a triggerExit event
    public void DisableCakeThrow()
    {
        canThrowCake = false;
    }
}
