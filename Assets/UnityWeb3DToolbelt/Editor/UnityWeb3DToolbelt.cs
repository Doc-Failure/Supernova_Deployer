using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.EditorCoroutines.Editor;
using System.Diagnostics;
using Debug = UnityEngine.Debug;


public class UnityWeb3DToolbelt : EditorWindow
{

    bool icp_network_is_Running = false;
    string icp_contract_name;
    string icp_contract_symbol;
    Texture2D icp_contract_logo;


    public string[] icp_standards = new string[] { "Dip721" };
    public int icp_standards_index = 0;

    Texture2D texture;
    Texture2D objectToDeploy;
    Texture2D objtdeploy;

    List<Dip721Metadata> tokenList = new List<Dip721Metadata>();

    UnityEngine.Vector2 scrollPos;
    byte[] rawData;
    [MenuItem("Supernova Deployer/NFT Deployer")]
    public static void ShowWindow()
    {
        GetWindow(typeof(UnityWeb3DToolbelt), true, "NFT Deployer");      //GetWindow is a method inherited from the EditorWindow class
    }

    void OnGUI()
    {

        GUILayout.BeginVertical();
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        if (!icp_network_is_Running)
        {
            if (GUILayout.Button("Start Local Network", GUILayout.ExpandWidth(true)))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = "/usr/local/bin/dfx",
                    Arguments = "start --background --clean",
                    WorkingDirectory = "/Users/conve/Project/Supernova_Deployer/Assets/Archetypes/dfinity-dip721-archetypes/",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };
                Process proc = new Process() { StartInfo = startInfo };
                proc.Start();
                Debug.Log("Listening on http://127.0.0.1:8000/");
                icp_network_is_Running = !icp_network_is_Running;
            }
        }
        else
        {
            if (GUILayout.Button("Stop Local Network", GUILayout.ExpandWidth(true)))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "/usr/local/bin/dfx", Arguments = "stop", WorkingDirectory = "/Users/conve/Project/Supernova_Deployer/Assets/Archetypes/dfinity-dip721-archetypes/", CreateNoWindow = true, UseShellExecute = false, RedirectStandardOutput = true };
                Process proc = new Process() { StartInfo = startInfo };
                proc.Start();
                Debug.Log("Server Stopped");
                icp_network_is_Running = !icp_network_is_Running;
            }
        }
        GUILayout.Label("");
        GUILayout.BeginVertical(EditorStyles.helpBox);

        GUILayout.Label("NFT Builder", EditorStyles.boldLabel);
        GUILayout.Label("");

        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Select Standard Archetypes");
        icp_standards_index = EditorGUILayout.Popup(icp_standards_index, icp_standards);
        GUILayout.Label("");
        //dfx deploy --no-wallet --argument '(record { name = "Numbers One Through Fifty"; symbol = "NOTF"; logo = null; custodians = null })'
        icp_contract_name = EditorGUILayout.TextField("Contract Name", icp_contract_name);
        icp_contract_symbol = EditorGUILayout.TextField("Contract Symbol", icp_contract_symbol);
        icp_contract_logo = EditorGUILayout.ObjectField("Contract Logo", icp_contract_logo, typeof(Texture2D), true, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Texture2D;
        GUILayout.Label("");
        if (GUILayout.Button("Deploy"))
        {
            contractDeployer();
        }
        GUILayout.EndVertical();

        GUILayout.Label("");
        if (GUILayout.Button("Add Asset", GUILayout.ExpandWidth(false)))
        {
            tokenList.Add(new Dip721Metadata());
        }
        GUILayout.Label("");
        for (int i = 0; i < tokenList.Count; i++)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Token #" + i);
            tokenList[i].name = EditorGUILayout.TextField("Name", tokenList[i].name);
            tokenList[i].description = EditorGUILayout.TextField("Description", tokenList[i].description);
            tokenList[i].image = EditorGUILayout.ObjectField("Image", tokenList[i].image, typeof(Texture2D), true, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Texture2D;

            EditorGUILayout.LabelField("Attributes");
            tokenList[i].attributes = EditorGUILayout.TextArea(tokenList[i].attributes);

            if (GUILayout.Button("Delete Asset", GUILayout.ExpandWidth(false)))
            {
                tokenList.RemoveAt(i);
            }

            GUILayout.EndVertical();
            GUILayout.Label("");
        }


        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Mint Tokens", GUILayout.ExpandWidth(false)))
        {
            tokenMinter();
        }
        if (GUILayout.Button("Open Web Interface", GUILayout.ExpandWidth(false)))
        {
            openUI();
        }
        GUILayout.EndHorizontal();
        GUILayout.Label("");
        EditorGUI.BeginDisabledGroup(true);
        if (GUILayout.Button("Deploy on Mainnet")) { }

        EditorGUI.EndDisabledGroup();

        GUILayout.EndVertical();

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void contractDeployer()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = "/usr/local/bin/dfx",
            Arguments = "deploy --no-wallet --argument '(record { name = \"" + icp_contract_name + "\"; symbol = \"" + icp_contract_symbol + "\"; logo = null; custodians = null })'",
            WorkingDirectory = "/Users/conve/Project/Supernova_Deployer/Assets/Archetypes/dfinity-dip721-archetypes/",
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            RedirectStandardError = true
        };
        Process proc = new Process() { StartInfo = startInfo };
        proc.Start();
        Debug.Log("Contract Deployed!");
    }

    private void openUI()
    {
        string path = "/Users/conve/Project/Supernova_Deployer/Assets/Archetypes/dfinity-dip721-archetypes/.dfx/local/canister_ids.json";
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        string value = reader.ReadToEnd();
        reader.Close();

        dynamic objects = JsonConvert.DeserializeObject<dynamic>(value);
        string Candid_UI = objects["__Candid_UI"]["local"].ToString();
        string dip721_nft_container = objects["dip721_nft_container"]["local"].ToString();

        Application.OpenURL("http://127.0.0.1:8000/?canisterId=" + Candid_UI + "&id=" + dip721_nft_container);

    }

    private void tokenMinter()
    {
        for (int i = 0; i < tokenList.Count; i++)
        {
            string principal = "dslrb-t4kbs-vgf3x-vnuuw-cj4xz-y5mdo-yl57a-tgvo6-jvisx-hamko-jqe";
            //string request="\"(principal\"+principal+\",vec{record{purpose=variant{Rendered};data=blob\"hello\";key_val_data=vec{record{\"contentType\";variant{TextContent=\"text/plain\"};};record{\"locationType\";variant{Nat8Content=4:nat8}};}}},blob\"hello\")\"";
            string req = "dfx canister call dip721_nft_container mintDip721 \"(principal\"" + principal + "\",vec{record{purpose=variant{Rendered};data=blob\"hello\";key_val_data=vec{record{\"contentType\";variant{TextContent=\"text/plain\"};};record{\"locationType\";variant{Nat8Content=4:nat8}};}}},blob\"hello\")\"";
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "/usr/local/bin/dfx",
                Arguments = req,
                WorkingDirectory = "/Users/conve/Project/Supernova_Deployer/Assets/Archetypes/dfinity-dip721-archetypes/",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            Process proc = new Process() { StartInfo = startInfo };

            try
            {
                proc.Start();
            }
            finally
            {
                Debug.Log(proc.HasExited);
            }
        }
        Debug.Log(tokenList.Count + " Tokens Minted!");
    }
}
