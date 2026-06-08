using System;
using System.Collections.Generic;

[Serializable]
public struct QuestionData
{
    public string question;
    public List<string> options;
    public byte correct;
}


[Serializable]
public struct QuestionList
{
    public List<QuestionData> QuestionDatas;
}