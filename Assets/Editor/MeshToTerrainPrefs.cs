using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public enum MeshToTerrainPhase
{
	idle,
	prepare,
	createTerrains,
	generateHeightmaps,
	generateTextures,
	finish
}

public class MeshToTerrainObject
{
	public readonly GameObject gameobject;
	public readonly int layer;

	public MeshToTerrainObject(GameObject gameObject)
	{
		gameobject = gameObject;
		layer = gameObject.layer;
	}
}

[System.Serializable]
public class MeshToTerrainPrefs
{

    private readonly int[] availableHeights = { 33, 65, 129, 257, 513, 1025, 2049, 4097 };
    private readonly int[] availableResolutions = { 32, 64, 128, 256, 512, 1024, 2048 };

    private string[] availableResolutionsStr;
    private string[] availableHeightsStr;

    public int alphamapResolution = 512;
    public int baseMapResolution = 1024;
    public int detailResolution = 1024;
    public bool generateTextures;
    public int heightmapResolution = 513;
    public List<GameObject> meshes = new List<GameObject>();
    public int meshLayer = 31;
    public int newTerrainCountX = 1;
    public int newTerrainCountY = 1;
    public int resolutionPerPatch = 16;
    public int smoothingFactor = 1;
    public List<Terrain> terrains = new List<Terrain>();
    public Color textureEmptyColor = Color.white;
    public int textureHeight = 1024;
    public int textureWidth = 1024;
    public bool useHeightmapSmoothing = true;
    public int yRangeValue = 1000;

    private Vector2 scrollPos = Vector2.zero;
    private bool showTextures = true;

    public static void DrawField(string label, ref int value, string tooltip, string href, string[] displayedOptions = null, int[] optionValues = null)
    {
        if (displayedOptions != null) value = EditorGUILayout.IntPopup(string.Empty, value, displayedOptions, optionValues);
        else value = EditorGUILayout.IntField("", value);
        EditorGUILayout.EndHorizontal();
    }
		


    private static int LimitPowTwo(int val, int min = 32, int max = 4096)
    {
        return Mathf.Clamp(Mathf.ClosestPowerOfTwo(val), min, max);
    }

    public void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        OnMeshesGUI();
        OnTerrainsGUI();
        OnTexturesGUI();

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Start")) MeshToTerrain.phase = MeshToTerrainPhase.prepare;
    }

    private void OnMeshesGUI()
    {
   		OnMeshesGUIGameObjects();
    }

    private void OnMeshesGUIGameObjects()
    {
        meshes.RemoveAll(m => m == null);
        for (int i = 0; i < meshes.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            meshes[i] = (GameObject)EditorGUILayout.ObjectField("Mesh " + (i + 1) + ": ", meshes[i], typeof(GameObject), true);
            if (GUILayout.Button("X", GUILayout.ExpandWidth(false))) meshes[i] = null;
            EditorGUILayout.EndHorizontal();
        }
        meshes.RemoveAll(m => m == null);

        GameObject newMesh = (GameObject)EditorGUILayout.ObjectField("Mesh GameObject: ", null, typeof(GameObject), true);
        if (newMesh != null)
        {
            if (!meshes.Contains(newMesh)) meshes.Add(newMesh);
            else EditorUtility.DisplayDialog("Warning", "GameObject already added", "OK");
        }
    }

    private void OnTerrainsGUI()
    {
		
        OnTerrainsGUINew();
        useHeightmapSmoothing = GUILayout.Toggle(useHeightmapSmoothing, "Use smoothing of height maps.");

        if (useHeightmapSmoothing)
        {
            smoothingFactor = EditorGUILayout.IntField("Smoothing factor: ", smoothingFactor);
            if (smoothingFactor < 1) smoothingFactor = 1;
            else if (smoothingFactor > 128) smoothingFactor = 128;
        }

    }

    private void OnTerrainsGUINew()
    {
        if (availableResolutionsStr == null || availableHeightsStr == null)
        {
            availableHeightsStr = availableHeights.Select(h => h.ToString()).ToArray();
            availableResolutionsStr = availableResolutions.Select(r => r.ToString()).ToArray();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Count terrains. X: ", GUILayout.ExpandWidth(false));
        newTerrainCountX = Mathf.Max(EditorGUILayout.IntField(newTerrainCountX, GUILayout.ExpandWidth(false)), 1);
        GUILayout.Label("Y: ", GUILayout.ExpandWidth(false));
        newTerrainCountY = Mathf.Max(EditorGUILayout.IntField(newTerrainCountY, GUILayout.ExpandWidth(false)), 1);
        GUILayout.EndHorizontal();

        detailResolution = detailResolution / resolutionPerPatch * resolutionPerPatch;
    }
		

    private void OnTexturesGUI()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.BeginHorizontal();
        showTextures = GUILayout.Toggle(showTextures, GUIContent.none, EditorStyles.foldout, GUILayout.ExpandWidth(false));
        generateTextures = GUILayout.Toggle(generateTextures, "Textures");
        EditorGUILayout.EndHorizontal();

        EditorGUI.BeginDisabledGroup(!generateTextures);
        if (showTextures)
        {
            textureWidth = LimitPowTwo(EditorGUILayout.IntField("Width: ", textureWidth));
            textureHeight = LimitPowTwo(EditorGUILayout.IntField("Height: ", textureHeight));
            textureEmptyColor = EditorGUILayout.ColorField("Empty color: ", textureEmptyColor);
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndVertical();
    }
		
}
