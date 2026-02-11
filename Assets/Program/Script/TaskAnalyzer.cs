using System;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

public class TaskAnalyzer : MonoBehaviour
{
    private const string APIUrl = "https://openrouter.ai/api/v1/chat/completions";
    
    [SerializeField] SecretKeys _secretKeys;

    [SerializeField] NotionCreateTask _notionCreateTask;
    private async void Start()
    {
        string taskJson = await AnalyzeTasksAsync("明日の10時までに履歴書を提出してください。\n金曜日は株式会社ルクレの面接があります。\nあと時間があればESを修正しておきたい。");
        
        TaskList tasks = TaskParser.ParseTaskList(taskJson);

        foreach (var task in tasks.tasks)
        {
            await _notionCreateTask.CreateTask(task.title, task.due);
        }
    }


    public async UniTask<string> AnalyzeTasksAsync(string inputText)
    {
        string today = DateTime.Now.ToString();
        var prompt =
            $@"今日は{today}です。
次の文章からタスクを抽出してください。
JSON配列のみを出力してください。
説明文は禁止。
```json などの記号は禁止。
出力は [ から始まり ] で終わること。
時間が書かれていない場合は時間を追加しない
推測で時間を決めない

文章:
{inputText}

出力形式:
[
  {{ ""title"": ""タスク名"", ""due"": ""YYYY-MM-DD HH:mm"" }}
]";

        var requestBody = new ChatRequest
        {
            model = "mistralai/mistral-7b-instruct",
            messages = new Message[]
            {
                new Message
                {
                    role = "user",
                    content = prompt
                }
            }
        };

        string json = JsonUtility.ToJson(requestBody, false);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using var request = new UnityWebRequest(APIUrl, "POST");

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Authorization", $"Bearer {_secretKeys.openRouterKeys}");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("HTTP-Referer", "http://localhost");
        request.SetRequestHeader("X-Title", "UnityTaskAnalyzer");

        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.text);
            return null;
        }

        string apiResponse = request.downloadHandler.text;

        // OpenRouterの外側JSONをパース
        ChatResponse response =
            JsonUtility.FromJson<ChatResponse>(apiResponse);

        // AIが出したJSON文字列だけ取得
        string taskJson = response.choices[0].message.content;
        
        return taskJson;
    }
    
    [Serializable]
    public class ChatRequest
    {
        public string model;
        public Message[] messages;
    }

    [Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    [Serializable]
    public class ChatResponse
    {
        public Choice[] choices;
    }

    [Serializable]
    public class Choice
    {
        public Message message;
    }
}