using System;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

public class VisionOCR : MonoBehaviour
{
    [SerializeField] SecretKeys _secretKeys;
    [SerializeField] Texture2D _texture;

    // private async void Start()
    // {
    //     string a = await DetectText(_texture);
    //     Debug.Log(a);
    // }

    private const string VisionUrl =
        "https://vision.googleapis.com/v1/images:annotate?key=";

    public async UniTask<string> DetectText(Texture2D texture)
    {
        // ① JPEG化
        Texture2D readable = ConvertToReadable(texture);
        byte[] jpg = readable.EncodeToJPG(75);

        // ② Base64化
        string base64 = System.Convert.ToBase64String(jpg);

        // ③ JSON作成
        var requestBody = new
        {
            requests = new[]
            {
                new {
                    image = new { content = base64 },
                    features = new[]
                    {
                        new { type = "TEXT_DETECTION" }
                    }
                }
            }
        };

        string json = JsonConvert.SerializeObject(requestBody);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using var request = new UnityWebRequest(VisionUrl + _secretKeys.GoogleCloudVisionApiKey, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        await request.SendWebRequest();


        string visionJson = ExtractText(request.downloadHandler.text);
        Debug.Log(visionJson);
        
        return visionJson;
    }
    
    private Texture2D ConvertToReadable(Texture2D source)
    {
        RenderTexture rt = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear
        );

        Graphics.Blit(source, rt);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D readable = new Texture2D(
            source.width,
            source.height,
            TextureFormat.RGB24,
            false
        );

        readable.ReadPixels(
            new Rect(0, 0, rt.width, rt.height),
            0,
            0
        );
        readable.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return readable;
    }
    
    public string ExtractText(string visionJson)
    {
        var obj = JObject.Parse(visionJson);

        string text = obj["responses"]?[0]?["fullTextAnnotation"]?["text"]?.ToString();

        return text;
    }
}