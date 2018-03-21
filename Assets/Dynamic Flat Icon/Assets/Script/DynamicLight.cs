using UnityEngine;  
using UnityEngine.UI;


public class DynamicLight : MonoBehaviour {


	Material diffuseMap;
	public GameObject lightSource;
	float rot;
	[SerializeField ]
	float rotadd;
	[SerializeField ]
	float sizemax=5;
	[SerializeField ]
	float sizemin=0;
	[SerializeField ]
	bool Flip=false;

	[SerializeField ]
	bool SetIntensity=false;
	[SerializeField ]
	float Intensity=1;
	void Update () {
		if (lightSource == null) {
			return;
		}
		float h = lightSource.transform.position.x - gameObject.transform.position.x;
		float w = lightSource.transform.position.y - gameObject.transform.position.y;
		float L = lightSource.transform.position.z - gameObject.transform.position.z;

		Vector3 VL = Vector3.Normalize (new Vector3 (h, w, L));
		diffuseMap.SetFloat  ("_OffsetSize", Mathf.Lerp(sizemax,sizemin,Mathf.Abs(VL.z)));

		if (w == 0 && h > 0) {
			rot = 90;
		} else if (w == 0 && h < 0) {
			rot = 270;
		} else {
			rot = Mathf.Atan2 (h, w);
			rot *= Mathf.Rad2Deg;
		}
		rot -= rotadd;
		if (Flip) {
			rot = 360 - rot;
		}
		if (rot < 0) {
			rot += 360;
		}

		diffuseMap.SetFloat  ("_rot", rot);
		if (SetIntensity) {
			diffuseMap.SetFloat  ("_ShadowRange", Intensity);
		}
	}
		
	void Start () {
		if (gameObject.GetComponent<Renderer> () != null) {
			if (diffuseMap == null) {
				diffuseMap = gameObject.GetComponent<Renderer> ().material;
			}
		}
		else if (gameObject.GetComponent<Image> ()) {			
			if (diffuseMap == null) {
				diffuseMap = gameObject.GetComponent<Image> ().material;
			}
		}

	}



}
