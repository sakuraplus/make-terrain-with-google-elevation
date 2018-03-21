/*
This script orbits the camera around a specific object.

Email :: thomas.ir.rasor@gmail.com
*/

using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
	public Transform target;
	public float distance = 2f;
	public float lerpSpeed = 3f;
	public float scrollSpeed = 5f;
	public float minDist = 0.5f;
	public float maxDist = 5f;

	public bool raycastedDistance = false;

	float wdist, cdist;
	Vector3 wrot, crot;
	Vector3 worigin, corigin;

	void Start ()
	{
		wdist = distance;
        transform.position = corigin - transform.forward * wdist;
	}

	void Update ()
	{
		float t = Time.deltaTime * 2f * lerpSpeed;

		wdist -= Input.GetAxis( "Mouse ScrollWheel" ) * Time.deltaTime * 5f * scrollSpeed;
		wdist = Mathf.Clamp( wdist , minDist , maxDist );
		cdist = Mathf.Lerp( cdist , wdist , t );

		if ( Input.GetMouseButton( 1 ))
		{
			wrot.y += Input.GetAxis( "Mouse X" ) * 3f;
			wrot.x -= Input.GetAxis( "Mouse Y" ) * 3f;
		}
		wrot.z = 0f;

		crot.x = Mathf.LerpAngle( crot.x , wrot.x , t );
		crot.y = Mathf.LerpAngle( crot.y , wrot.y , t );
		crot.z = Mathf.LerpAngle( crot.z , wrot.z , t );

		transform.rotation = Quaternion.Euler( crot );

		if ( target != null )
			worigin = target.position;
		else
			worigin = Vector3.zero;

		if ( raycastedDistance )
		{
			RaycastHit h;
			Ray r = new Ray( worigin - transform.forward * ( cdist + 1f ) , transform.forward );
			if(Physics.Raycast(r,out h))
			{
				worigin = h.point;
			}
		}

		corigin = Vector3.Lerp( corigin , worigin , t );

        transform.position = corigin - transform.forward * cdist;
	}

	void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere( worigin , 0.2f );
		Gizmos.DrawLine( worigin , transform.position );
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere( corigin , 0.2f );
		Gizmos.DrawLine( corigin , transform.position );
	}

}
