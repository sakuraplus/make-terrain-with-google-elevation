/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

/*   http://www.infinity-code.com   */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Xml;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MeshToTerrainUpdater : EditorWindow 
{
    private MeshToTerrainUpdateChannel channel = MeshToTerrainUpdateChannel.stable;
    private string invoiceNumber;
    private Vector2 scrollPosition;
    private List<MeshToTerrainUpdateItem> updates;

    private void CheckNewVersions()
    {
        if (string.IsNullOrEmpty(invoiceNumber))
        {
            EditorUtility.DisplayDialog("Error", "Please enter the Invoice Number.", "OK");
            return;
        }

        int inum;

        if (!int.TryParse(invoiceNumber, out inum))
        {
            EditorUtility.DisplayDialog("Error", "Wrong Invoice Number.", "OK");
            return;
        }

        SavePrefs();

        string updateKey = GetUpdateKey();
        GetUpdateList(updateKey);
    }

    private string GetUpdateKey()
    {
        WebClient client = new WebClient();
        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
        string updateKey = client.UploadString("http://infinity-code.com/products_update/getupdatekey.php",
            "key=" + invoiceNumber + "&package=" + WWW.EscapeURL("Mesh to Terrain"));

        return updateKey;
    }

    private void GetUpdateList(string updateKey)
    {
        WebClient client = new WebClient();
        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

        string response;

        try
        {
            response = client.UploadString("http://infinity-code.com/products_update/checkupdates.php",
            "k=" + WWW.EscapeURL(updateKey) + "&v=" + MeshToTerrain.version + "&c=" + (int)channel);
        }
        catch
        {
            return;
        }
        
        XmlDocument document = new XmlDocument();
        document.LoadXml(response);

        XmlNode firstChild = document.FirstChild;
        updates = new List<MeshToTerrainUpdateItem>();

        foreach (XmlNode node in firstChild.ChildNodes) updates.Add(new MeshToTerrainUpdateItem(node));
    }

    private void OnEnable()
    {
        if (EditorPrefs.HasKey("MeshToTerrainInvoiceNumber"))
            invoiceNumber = EditorPrefs.GetString("MeshToTerrainInvoiceNumber");
        else invoiceNumber = "";

        if (EditorPrefs.HasKey("MeshToTerrainUpdateChannel"))
            channel = (MeshToTerrainUpdateChannel)EditorPrefs.GetInt("MeshToTerrainUpdateChannel");
        else channel = MeshToTerrainUpdateChannel.stable;
    }

    private void OnDestroy()
    {
        SavePrefs();
    }

    private void OnGUI()
    {
        invoiceNumber = EditorGUILayout.TextField("Invoice Number:", invoiceNumber).Trim(new[] { ' ' });
        channel = (MeshToTerrainUpdateChannel) EditorGUILayout.EnumPopup("Channel:", channel);

        if (GUILayout.Button("Check new versions")) CheckNewVersions();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        if (updates != null)
        {
            foreach (MeshToTerrainUpdateItem update in updates) update.Draw();
            if (updates.Count == 0) GUILayout.Label("No updates");
        }

        EditorGUILayout.EndScrollView();
    }

    [MenuItem("Window/Infinity Code/Mesh to Terrain/Check Updates", false, 2)]
    public static void OpenWindow()
    {
        GetWindow<MeshToTerrainUpdater>(false, "Mesh to Terrain Updater", true);
    }

    private void SavePrefs()
    {
        EditorPrefs.SetString("MeshToTerrainInvoiceNumber", invoiceNumber);
        EditorPrefs.SetInt("MeshToTerrainUpdateChannel", (int)channel);
    }
}

public class MeshToTerrainUpdateItem
{
    private string version;
    private int type;
    private string changelog;
    private string download;
    private string date;

    private static GUIStyle _changelogStyle;
    private static GUIStyle _titleStyle;

    private static GUIStyle changelogStyle
    {
        get { return _changelogStyle ?? (_changelogStyle = new GUIStyle(EditorStyles.label) {wordWrap = true}); }
    }

    private static GUIStyle titleStyle
    {
        get
        {
            return _titleStyle ??
                   (_titleStyle = new GUIStyle(EditorStyles.boldLabel) {alignment = TextAnchor.MiddleCenter});
        }
    }

    public MeshToTerrainUpdateItem(XmlNode node)
    {
        version = node.SelectSingleNode("Version").InnerText;
        type = int.Parse(node.SelectSingleNode("Type").InnerText);
        changelog = node.SelectSingleNode("ChangeLog").InnerText;
        download = node.SelectSingleNode("Download").InnerText;
        date = node.SelectSingleNode("Date").InnerText;

        string[] vars = version.Split(new[] {'.'});
        string[] vars2 = new string[4];
        vars2[0] = vars[0];
        vars2[1] = int.Parse(vars[1].Substring(0, 2)).ToString();
        vars2[2] = int.Parse(vars[1].Substring(2, 2)).ToString();
        vars2[3] = int.Parse(vars[1].Substring(4, 4)).ToString();
        version = string.Join(".", vars2);
    }

    public void Draw()
    {
        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label("Version: " + version + " (" + typeStr + "). " + date, titleStyle);

        GUILayout.Label(changelog, changelogStyle);

        if (GUILayout.Button("Download"))
        {
            Process.Start("http://infinity-code.com/products_update/download.php?k=" + download);
        }

        GUILayout.EndVertical();
    }

    public string typeStr
    {
        get { return Enum.GetName(typeof (MeshToTerrainUpdateChannel), type); }
    }
}

public enum MeshToTerrainUpdateChannel
{
    stable = 10,
    releaseCandidate = 20,
    beta = 30,
    alpha = 40,
    working = 50
}
