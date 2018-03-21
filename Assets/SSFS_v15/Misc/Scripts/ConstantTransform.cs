/*
This script applies a constant transformation to an object. 

Email :: thomas.ir.rasor@gmail.com
*/

using UnityEngine;

public class ConstantTransform : MonoBehaviour
{
	public Vector3 translation;
	public Space translationSpace = Space.World;
	public Vector3 rotation;
	public Space rotationSpace = Space.World;

	void Update ()
	{
		transform.Translate( translation * Time.deltaTime , translationSpace );
		transform.Rotate( rotation * Time.deltaTime , rotationSpace );
	}
}