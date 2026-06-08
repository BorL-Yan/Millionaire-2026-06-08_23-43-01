using System;

namespace Data
{
    /// <summary>
    /// JsonUtility in Unity 6 cannot deserialize a top-level JSON array, so we
    /// wrap the array in a private [Serializable] container before parsing.
    /// </summary>

    [Serializable]
    public class QuestionDataArray
    {
         public QuestionData[] items;
    }
}