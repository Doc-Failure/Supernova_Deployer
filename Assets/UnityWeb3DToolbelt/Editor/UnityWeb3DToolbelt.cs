using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.Extensions;
using Nethereum.JsonRpc.UnityClient;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.EditorCoroutines.Editor;
using System.Diagnostics;
using Debug=UnityEngine.Debug;


public class UnityWeb3DToolbelt : EditorWindow
{

    bool icp_network_is_Running=false;
    string icp_contract_name;
    string icp_contract_symbol;
    string icp_contract_logo;

    Texture2D texture;
    Texture2D objectToDeploy;
    Texture2D objtdeploy;

    List<ERC1155Metadata> tokenList = new List<ERC1155Metadata>();
    private string ERC1155ImagesCID = "";
    private string ERC1155CID = "";

    UnityEngine.Vector2 scrollPos;
    byte[] rawData;
    [MenuItem("Web3D Toolbelt Tools/NFT Deployer")]
    public static void ShowWindow()
    {
        GetWindow(typeof(UnityWeb3DToolbelt), true, "NFT Deployer");      //GetWindow is a method inherited from the EditorWindow class
    }

    void OnGUI()
    {

        GUILayout.BeginVertical();
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        if(icp_network_is_Running){
            if (GUILayout.Button("Start Local Network", GUILayout.ExpandWidth(true)))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(){ FileName = "/usr/local/bin/dfx", Arguments = "start --background --clean",
                WorkingDirectory ="/Users/conve/Project/Supernova_Deployer/Assets/Archetypes/dfinity-dip721-archetypes/" ,CreateNoWindow = true, UseShellExecute=false,  RedirectStandardOutput = true };
                Process proc = new Process(){ StartInfo = startInfo };
                proc.Start();
                Debug.Log("Listening on http://127.0.0.1:8000/");
                icp_network_is_Running=!icp_network_is_Running;
            }
        }else{
            if (GUILayout.Button("Stop Local Network", GUILayout.ExpandWidth(true)))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(){ FileName = "/usr/local/bin/dfx", Arguments = "stop", WorkingDirectory ="/Users/conve/Project/Supernova_Deployer/Assets/Archetypes/dfinity-dip721-archetypes/" ,CreateNoWindow = true, UseShellExecute=false,  RedirectStandardOutput = true };
                Process proc = new Process() { StartInfo = startInfo };
                proc.Start();
                Debug.Log("Server Stopped");
                icp_network_is_Running=!icp_network_is_Running;
            }
        }
        GUILayout.Label("");
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("NFT Builder", EditorStyles.boldLabel);
        GUILayout.Label("");
        
        GUILayout.Label("");
        //dfx deploy --no-wallet --argument '(record { name = "Numbers One Through Fifty"; symbol = "NOTF"; logo = null; custodians = null })'
        icp_contract_name = EditorGUILayout.TextField("Contract Name", icp_contract_name);
        icp_contract_symbol = EditorGUILayout.TextField("Contract Symbol", icp_contract_symbol);
        icp_contract_logo = EditorGUILayout.TextField("Contract Logo", icp_contract_logo);
        
        /* GUILayout.Label("");
        GUILayout.Label("ChainLink Integrated Features", EditorStyles.boldLabel);
        //GUILayout.BeginHorizontal(EditorStyles.toggleGroup);
        chainLinkRandomMinterOption = EditorGUILayout.BeginToggleGroup("VRF Token Minter", chainLinkRandomMinterOption);
        if (chainLinkRandomMinterOption)
        {
            MintPercentage = EditorGUILayout.FloatField("Probability to mint (%)", MintPercentage);
            MintQuantity = EditorGUILayout.FloatField("Quantity to mint", MintQuantity);
        }
        EditorGUILayout.EndToggleGroup(); */

        GUILayout.Label("");
        if (GUILayout.Button("Add Asset", GUILayout.ExpandWidth(false)))
        {
            tokenList.Add(new ERC1155Metadata());
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


        if (GUILayout.Button("Deploy NFT"))
        {
          /*   this.StartCoroutine(ImagesDeployer());
            this.StartCoroutine(MetadataDeployer());
            this.StartCoroutine(DeployToken()); */
        }
        GUILayout.Label("");
        EditorGUI.BeginDisabledGroup(true);
        if (GUILayout.Button("Deploy on Mainnet")){}
        EditorGUI.EndDisabledGroup();

        GUILayout.EndVertical();

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void contractDeployer(){
        ProcessStartInfo startInfo = new ProcessStartInfo(){ FileName = "/usr/local/bin/dfx", Arguments = "start --background --clean",
        WorkingDirectory ="/Users/conve/Project/Supernova_Dip721_Deployer/Assets/dfinity-dip721-archetypes/" ,CreateNoWindow = true, UseShellExecute=false,  RedirectStandardOutput = true };
        Process proc = new Process(){ StartInfo = startInfo };
        proc.Start();
        Debug.Log("Listening on http://127.0.0.1:8000/");
        icp_network_is_Running=!icp_network_is_Running;
    }

    //passare da DeployObject a DeployNFT
   /*  private IEnumerator ImagesDeployer()
    {
        WWWForm form = new WWWForm();

        for (int i = 0; i < tokenList.Count; i++)
        {
            Texture2D decopmpresseTex = DeCompress(tokenList[i].image);
            var bytes = decopmpresseTex.EncodeToPNG();
            form.AddBinaryData("file", bytes, "image_ID_" + i + ".png", "image/png");
        }

        UnityWebRequest www = UnityWebRequest.Post("https://api.nft.storage/upload", form);
        www.SetRequestHeader("Authorization", "Bearer " + NFTStorageBearerApi);
        www.SetRequestHeader("accept", "application/json");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            dynamic objects = JsonConvert.DeserializeObject<dynamic>(www.downloadHandler.text);
            ERC1155ImagesCID = objects["value"]["cid"].ToString();
        }
    } */

   /*  private IEnumerator MetadataDeployer()
    {
        WWWForm form = new WWWForm();

        //'meta=\'{"image":null,"name":"Storing the Worlds Most Valuable Virtual Assets with NFT.Storage","description":"The metaverse is here. Where is it all being stored?","properties":{"type":"blog-post","origins":{"http":"https://nft.storage/blog/post/2021-11-30-hello-world-nft-storage/","ipfs":"ipfs://bafybeieh4gpvatp32iqaacs6xqxqitla4drrkyyzq6dshqqsilkk3fqmti/blog/post/2021-11-30-hello-world-nft-storage/"},"authors":[{"name":"David Choi"}],"content":{"text/markdown":"The last year has witnessed the explosion of NFTs onto the world’s mainstage. From fine art to collectibles to music and media, NFTs are quickly demonstrating just how quickly grassroots Web3 communities can grow, and perhaps how much closer we are to mass adoption than we may have previously thought. <... remaining content omitted ...>"}}}\''

        for (int i = 0; i < tokenList.Count; i++)
        {
            string tokenMeta = "{";
            tokenMeta += "\"name\":\"" + tokenList[i].name + "\",";
            tokenMeta += "\"description\":\"" + tokenList[i].description + "\",";
            tokenMeta += "\"image\":\"https://" + ERC1155ImagesCID + ".ipfs.nftstorage.link/image_ID_" + i + ".png\",";
            tokenMeta += "\"attributes\":" + tokenList[i].attributes + "}";
            byte[] bodyRaw = Encoding.UTF8.GetBytes(tokenMeta);
            form.AddBinaryData("file", bodyRaw, "meta_ID_" + i + ".json", "application/json'");
        }
        UnityWebRequest www = UnityWebRequest.Post("https://api.nft.storage/upload", form);
        www.SetRequestHeader("Authorization", "Bearer " + NFTStorageBearerApi);
        www.SetRequestHeader("accept", "application/json");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            dynamic objects = JsonConvert.DeserializeObject<dynamic>(www.downloadHandler.text);
            ERC1155CID = objects["value"]["cid"].ToString();
        }
    } */

    //qui
    /* private IEnumerator DeployToken()
    {
        string nameTokenList = "";
        for (int i = 0; i < tokenList.Count; i++)
        {
            nameTokenList += tokenList[i].name;
            if (i < tokenList.Count - 1)
            {
                nameTokenList += ",";
            }
        }
        string request = "http://localhost:8080/generic-webhook-trigger/invoke?NAME=" + ContractName + "&TOKENS_LIST=" + nameTokenList + "&IPFS_ENDPOINT=https://" + ERC1155CID + ".ipfs.nftstorage.link&MINTING_PROBABILITY=" + MintPercentage + "&MINTING_QUANTITY=" + MintQuantity + "&CHAIN_ID=" + networksList.GetChainId(index);
        UnityWebRequest www = UnityWebRequest.Get(request);
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("Error While Sending: " + www.error);
        }
        else
        {
            Debug.Log("NFT Generation in progress");
        }
    } */

   /*  static Texture2D DeCompress(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    } */
}
