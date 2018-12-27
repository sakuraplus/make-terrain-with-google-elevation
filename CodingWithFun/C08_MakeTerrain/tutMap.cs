using UnityEngine;
using System.Collections;

public class tutMap : MonoBehaviour {
	[SerializeField]
	Texture2D tex2d;
	[SerializeField]
	int sx=20;
	[SerializeField]
	int sy=20;
	[SerializeField]
	float width=100;
	[SerializeField]
	float heightscale=1;


	void Start () {
		generateTerrain ();
	}

	public void generateTerrain(){
		float stepx = (tex2d.width-1) /(float) sx;
		float stepy = (tex2d.height -1)/ (float)sy;
		_vertices=new Vector3[(sx+1)*(sy+1)] ;
		for (int yy = 0; yy <= sy; yy++) {
			for (int xx = 0; xx <=sx; xx++) {
				int self = xx + yy *(sx+1);
				_vertices [self].x = xx * width/sx;
				_vertices [self].z = yy * width /sy;
				Color cc = tex2d.GetPixel (Mathf.FloorToInt( xx*stepx),Mathf.FloorToInt( yy*stepy));
				float height = -10000 + (Mathf.Round (cc.r * 256) * 256 * 256 + Mathf.Round (cc.g * 256) * 256 + Mathf.Round (cc.b * 256)) / 10;
					//https://blog.mapbox.com/global-elevation-data-6689f1d0ba65
				height *= heightscale/width;
				_vertices [self].y = height;
			}
		}
	//	setUV();
		setMeshTriangles ();
		DrawMesh ();
		DrawTexture ();
	}

	Mesh mesh;
	Vector3[] _vertices;
	Vector2[] _uvs;
	int[] _triangles;
	void DrawMesh()
	{
		if (gameObject.GetComponent <MeshFilter> () == null) {
			mesh = new Mesh ();
			mesh.name="ssss";
			gameObject.AddComponent<MeshFilter> ().sharedMesh=mesh ;
		} else {
			mesh= gameObject.GetComponent <MeshFilter> ().sharedMesh;
		}
		////给mesh 赋值
		mesh.Clear();
		mesh.vertices = _vertices ;
		mesh.uv = _uvs;
		mesh.triangles = _triangles;
		mesh.RecalculateNormals();////重置法线
		mesh.RecalculateBounds();////重置范围
	}


	void setUV()
	{
		_uvs = new Vector2[(sx+1)*(sy+1)];
		float u = 1.0F / sx;
		float v = 1.0F / sy;
		for (int yy = 0; yy <= sy; yy++)
		{
			for (int xx = 0; xx <= sx; xx++)
			{	
				int self = xx + yy *(sx+1);
				_uvs[self] = new Vector2(xx * u, yy * v);
			}
		}
	}

	void setMeshTriangles()
	{
		_triangles = new int[sx * sy * 6];
		int index = 0;
		for (int yy = 0; yy < sy; yy++)
		{
			for (int xx = 0; xx < sx; xx++)
			{
				int self = xx + (yy * (sx + 1));                
				int next = xx + (yy + 1) * (sx + 1);
				_triangles [index] = self;
				_triangles [index + 1] = next + 1;
				_triangles [index + 2] = self + 1;
				_triangles [index + 3] = self;
				_triangles [index + 4] = next;
				_triangles [index + 5] = next + 1;	
				index += 6;
			}
		}
	}

	void DrawTexture(){
		if (gameObject.GetComponent <MeshRenderer> ()==null ) {
			gameObject.AddComponent<MeshRenderer> ();
		}	
		Material diffuseMap = new Material (Shader.Find ("Standard"));
		gameObject.GetComponent<Renderer> ().material = diffuseMap;
	}
}
