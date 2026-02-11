using System;
using UnityEngine;
[Serializable]
public class TaskData
{
    public string title;
    public string due;
}

[Serializable]
public class TaskList
{
    public TaskData[] tasks;
}