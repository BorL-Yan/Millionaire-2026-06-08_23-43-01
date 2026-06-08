using System;
using System.Collections.Generic;
using Data;
using UnityEngine;



/// <summary>
/// Loads question data from a JSON file placed inside a Resources folder.
/// Returns a QuestionList domain object that can be safely shared (read-only) with the rest of the game.
/// </summary>
public class QuestionRepository
{
    /// <summary>
    /// Loads a JSON file from Resources/ and parses it into a QuestionList.
    /// </summary>
    /// <param name="resourceName">File name without extension (e.g. "low_questions").</param>
    /// <returns>QuestionList containing all questions from the file. Returns an empty list if the file is missing or malformed.</returns>
    public QuestionList Load(string resourceName)
    {
        var asset = Resources.Load<TextAsset>(resourceName);
        if (asset == null)
        {
            Debug.LogError($"QuestionRepository: TextAsset '{resourceName}' not found in any Resources folder.");
            return new QuestionList { QuestionDatas = new List<QuestionData>() };
        }

        QuestionData[] arr;
        try
        {
            // JsonUtility requires a JSON object at the root, not a bare array.
            // We wrap the file's array into a temporary object so it can be parsed.
            var wrapped = "{\"items\":" + asset.text + "}";
            var container = JsonUtility.FromJson<QuestionDataArray>(wrapped);
            arr = container?.items;
        }
        catch (Exception e)
        {
            Debug.LogError($"QuestionRepository: failed to parse '{resourceName}': {e.Message}");
            return new QuestionList { QuestionDatas = new List<QuestionData>() };
        }

        if (arr == null)
        {
            Debug.LogError($"QuestionRepository: '{resourceName}' parsed to null array.");
            return new QuestionList { QuestionDatas = new List<QuestionData>() };
        }

        var list = new List<QuestionData>(arr.Length);
        foreach (var q in arr)
        {
            if (q.options != null && q.options.Count == 4)
            {
                list.Add(q);
            }
            else
            {
                Debug.LogWarning($"QuestionRepository: skipping a malformed question (expected 4 options).");
            }
        }

        Debug.Log($"QuestionRepository: loaded {list.Count} questions from '{resourceName}'.");
        return new QuestionList { QuestionDatas = list };
    }
}
