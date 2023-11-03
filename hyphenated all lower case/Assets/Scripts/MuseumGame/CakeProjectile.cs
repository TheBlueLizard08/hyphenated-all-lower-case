using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CakeProjectile : MonoBehaviour
{
    public float throwAngleMod = 10;
	public float lifeTime = 4.0f;
	Rigidbody rb;

    public void Initialize(Transform target)
    {
		rb = GetComponent<Rigidbody>();

		float throwAngle = Vector3.Distance(transform.position, target.position) * throwAngleMod;
		throwAngle = Mathf.Clamp(throwAngle, 0, 35);
		rb.velocity = CalcBallisticVelocityVector(transform.position, target.position, throwAngle);
		StartCoroutine(LifeTimerC());
    }

	IEnumerator LifeTimerC()
    {
		yield return new WaitForSeconds(lifeTime);
		Destroy(gameObject);
    }

	//Code stolen from: https://discussions.unity.com/t/calculate-force-needed-to-reach-certain-point-addforce/188605
	Vector3 CalcBallisticVelocityVector(Vector3 source, Vector3 target, float angle)
	{
		Vector3 direction = target - source;
		float h = direction.y;
		direction.y = 0;
		float distance = direction.magnitude;
		float a = angle * Mathf.Deg2Rad;
		direction.y = distance * Mathf.Tan(a);
		distance += h / Mathf.Tan(a);

		// calculate velocity
		float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
		return velocity * direction.normalized;
	}
}
