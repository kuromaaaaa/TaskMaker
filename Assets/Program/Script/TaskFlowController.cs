using System;
using UnityEngine;

public class TaskFlowController : MonoBehaviour
{
    [SerializeField] TaskAnalyzer _taskAnalyzer;
    [SerializeField] NotionCreateTask _notionCreateTask;
    [SerializeField] VisionOCR _visionOCR;
    

    public async void Flow(Texture2D texture)
    {
        string a = await _visionOCR.DetectText(texture);
        string taskJson = await _taskAnalyzer.AnalyzeTasksAsync(a);
        Debug.Log(taskJson);
        TaskList tasks = TaskParser.ParseTaskList(taskJson);

        foreach (var task in tasks.tasks)
        {
            await _notionCreateTask.CreateTask(task.title, task.due);
        }

        Application.Quit();
    }
}
