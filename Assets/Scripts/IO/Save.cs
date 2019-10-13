using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Reflection;

sealed public class SaveGame
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
        catch(Exception e)
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
        catch(Exception e)
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
        string filePath = "";

        switch (Application.platform)
        {
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                filePath = Application.persistentDataPath + "/Save/" + slotName + ".sv";
                break;
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.LinuxPlayer:
            case RuntimePlatform.LinuxEditor:
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.OSXEditor:
            default:
                filePath = Application.dataPath + "/Save/" + slotName + ".sv";
                break;
        }

        return filePath;
    }

    static void CheckSavePath()
    {
        string directoryPath = "";

        switch (Application.platform)
        {
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                directoryPath = Application.persistentDataPath + "/Save";
                break;
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.LinuxPlayer:
            case RuntimePlatform.LinuxEditor:
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.OSXEditor:
            default:
                directoryPath = Application.dataPath + "/Save";
                break;
        }

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
    }
}