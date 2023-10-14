using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DG_Player : MonoBehaviour
{
    public float moveSpeed, rotSpeed;
    public float minDistance = 0.05f, maxDistance = 4;
    public LayerMask raycastLayerMask;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition;
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 20, raycastLayerMask))
        {
            targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        }
        else
        {
            targetPosition = transform.position;
        }

        float dis = Vector3.Distance(targetPosition, transform.position);

        if (dis > minDistance && dis < maxDistance)
        {
            rb.rotation = Quaternion.RotateTowards(rb.rotation,
                Quaternion.LookRotation(targetPosition - transform.position), rotSpeed * Time.deltaTime);
            rb.velocity = transform.forward * moveSpeed;

        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void OnEnable()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 20, raycastLayerMask))
        {
            transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z + 1.0f);
        }
    }
}
