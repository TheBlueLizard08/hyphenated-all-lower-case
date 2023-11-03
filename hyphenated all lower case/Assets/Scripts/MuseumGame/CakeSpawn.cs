using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CakeSpawn : MonoBehaviour
{
    public GameObject cakePrefab;
    public float fireTime = 1.0f;
    bool armed = false;
    bool playerInPosition = false;
    bool isThrowing = false;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.cakeSpawns.Add(this);
    }

    public void ArmCakeSpawns(bool setArmed)
    {
        armed = setArmed;
    }

    public void SetPlayerInPosition(bool isPlayerInPosition)
    {
        playerInPosition = isPlayerInPosition;
        if (playerInPosition) { StartCoroutine(ThrowCakeC()); }
    }

    IEnumerator ThrowCakeC()
    {
        if (isThrowing) { yield break; }
        isThrowing = true;

        while(armed && playerInPosition)
        {
            CakeProjectile cake = Instantiate(cakePrefab, transform.position, Quaternion.identity).GetComponent<CakeProjectile>();
            cake.Initialize(Camera.main.transform);
            yield return new WaitForSeconds(fireTime);
        }

        isThrowing = false;
    }
}
