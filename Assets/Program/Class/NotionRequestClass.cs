using System;

[Serializable]
public class NotionRequest
{
    public Parent parent;
    public Properties properties;
}

[Serializable]
public class Parent
{
    public string database_id;
}

[Serializable]
public class Properties
{
    public NameProperty Name;
    public DateProperty due;
}

[Serializable]
public class NameProperty
{
    public TitleText[] title;
}

[Serializable]
public class TitleText
{
    public TextContent text;
}

[Serializable]
public class TextContent
{
    public string content;
}

[Serializable]
public class DateProperty
{
    public DateValue date;
}

[Serializable]
public class DateValue
{
    public string start;
}