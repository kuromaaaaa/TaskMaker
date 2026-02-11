using UnityEngine;

public static class TaskParser
{
    public static TaskList ParseTaskList(string json)
    {
        string wrappedJson = "{ \"tasks\": " + json + " }";
        return JsonUtility.FromJson<TaskList>(wrappedJson);
    }

}
