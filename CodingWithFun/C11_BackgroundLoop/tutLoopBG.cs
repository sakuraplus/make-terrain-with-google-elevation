using UnityEngine;

public class tutLoopBG : MonoBehaviour {

	Material material;
	Vector2 offsetuv;

	[SerializeField,Range(0,360)]
	float rotation;
	[SerializeField]
	float speed;
	void Start () {
		if (gameObject.GetComponent<Renderer> () && material==null) {
			material = gameObject.GetComponent<Renderer> ().sharedMaterial;
			//material = gameObject.GetComponent<Renderer> ().material;//Use the instanced temporary material (if exist)
			offsetuv=new Vector2(0,0);

		}	
	}

	void Update () {
		float s = Time.deltaTime* speed*Mathf.Sin (rotation * Mathf.Deg2Rad);
		float c = Time.deltaTime* speed*Mathf.Cos (rotation * Mathf.Deg2Rad);
		offsetuv.x += s;
		offsetuv.y += c;

		//Vector4 tt = material.GetVector ("_MainTex_ST");
		//material.SetVector ("_MainTex_ST",new Vector4(tt.x,tt.y,offsetuv.x,offsetuv.y));

		material.SetFloat ("_uvx", offsetuv.x);
		material.SetFloat ("_uvy", offsetuv.y);
	}
}
