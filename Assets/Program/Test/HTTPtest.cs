using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPtest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        Debug.Log("HTTPtest Start");
        var request = UnityWebRequest.Get("https://example.com");

        await request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("成功！");
        }
        else
        {
            Debug.Log("失敗");
            Debug.Log(request.error);
        }
    }
}
