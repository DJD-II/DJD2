using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

sealed public class IO
{
    [Serializable]
    abstract public class Object : object
    {
    }

    public static readonly string tempFilename = "TempFile_3_3eedds_fjd329074_sdfjn_sdklfj_439057.ts";

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

    public static FileInfo[] GetFilenames (string pattern = "*.sv")
    {
        CheckSavePath();

        return new DirectoryInfo(Application.dataPath + "/Save").GetFiles(pattern);
    }

    static string GetFilePath(string slotName)
    {
        return Application.dataPath + "/Save/" + slotName; ;
    }

    static void CheckSavePath()
    {
        string directoryPath = Application.dataPath + "/Save";
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        string directoryPath2 = Application.dataPath + "/Save";
        if (!Directory.Exists(directoryPath2))
            Directory.CreateDirectory(directoryPath2);
    }
}