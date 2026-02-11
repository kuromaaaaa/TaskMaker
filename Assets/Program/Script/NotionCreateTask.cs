using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using Newtonsoft.Json;  

public class NotionCreateTask : MonoBehaviour
{
    private const string NotionUrl = "https://api.notion.com/v1/pages";

    [SerializeField] SecretKeys _secretKeys;
    
     
    public async UniTask CreateTask(string title, string due)
    {
        var properties = new Properties
        {
            Name = new NameProperty
            {
                title = new[]
                {
                    new TitleText
                    {
                        text = new TextContent
                        {
                            content = title
                        }
                    }
                }
            }
        };
        
        properties.due = null;

        if (!string.IsNullOrWhiteSpace(due))
        {
            properties.due = new DateProperty
            {
                date = new DateValue
                {
                    start = due
                }
            };
        }

        var requestBody = new NotionRequest
        {
            parent = new Parent
            {
                database_id = _secretKeys.databaseId
            },
            properties = properties
        };
        

        var setting = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        
        string json = JsonConvert.SerializeObject(requestBody, setting);
        
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using var request = new UnityWebRequest(NotionUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Authorization", $"Bearer {_secretKeys.notionToken}");
        request.SetRequestHeader("Notion-Version", "2022-06-28");
        request.SetRequestHeader("Content-Type", "application/json");

        await request.SendWebRequest();
        
    }
}
