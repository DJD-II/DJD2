using System;
using System.IO;
using UnityEngine;

sealed public class IO
{
    [Serializable]
    abstract public class Object : object
    {
    }

    public static bool Exists(string slotName)
    {
        CheckSavePath();

        return File.Exists(GetFilePath(slotName));
    }

    public static void Save(string slotName, Object saveGameObject)
    {
        CheckSavePath();

        try
        {
            using (Stream stream = File.Create(GetFilePath(slotName)))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, Utility.Encryptor.Encode(saveGameObject));
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public static T Load<T>(string slotName) where T : Object
    {
        CheckSavePath();

        if (!Exists(slotName))
            return null;

        try
        {
            using (Stream stream = File.Open(GetFilePath(slotName), FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)Utility.Encryptor.Decode((byte[])binaryFormatter.Deserialize(stream));
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return null;
        }
    }

    public static void Delete(string slotName)
    {
        CheckSavePath();

        string filePath = GetFilePath(slotName);

        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    static string GetFilePath(string slotName)
    {
        return Application.dataPath + "/Save/" + slotName + ".sv"; ;
    }

    static void CheckSavePath()
    {
        string directoryPath = Application.dataPath + "/Save";
        Debug.Log("Creating Directory");
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
    }
}