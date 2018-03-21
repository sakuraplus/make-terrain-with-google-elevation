/*
This script rotates an object in smoothed increments.

Email :: thomas.ir.rasor@gmail.com
*/
using UnityEngine;

public class StepRotation : MonoBehaviour
{
    public int segments = 4;
    public float delay = 0.25f;

    float i = 0f;
    float t = 0f;

    Quaternion targetRotation = Quaternion.identity;

    void Update ()
    {
        transform.rotation = Quaternion.SlerpUnclamped( transform.rotation , targetRotation , Time.deltaTime * 10f );

        t += Time.deltaTime;

        if (t >= delay)
        {
            t = 0f;
            Rotate();
        }
    }

    public void Rotate()
    {
        i = Mathf.Repeat( i + 1f / segments , 1f );
        float theta = i * Mathf.PI * 2f;
        targetRotation = Quaternion.LookRotation( new Vector3( Mathf.Sin( theta ) , 0f , Mathf.Cos( theta ) ) , Vector3.up );
    }
}