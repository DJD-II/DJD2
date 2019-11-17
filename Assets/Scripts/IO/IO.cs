using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class IO
{
    public static class Serializer
    {
        /// <summary>
        /// <para>Binder</para>
        /// <para>Serialization type Binder.</para>
        /// </summary>
        sealed class Binder : SerializationBinder
        {
            /// <summary>
            /// <para>Binding of a serialized object to a type.</para>
            /// </summary>
            /// <param name="assemblyName">Assemby Name.</param>
            /// <param name="typeName">Type Name.</param>
            /// <returns>Sysgtem.Type</returns>
            override public System.Type BindToType(string assemblyName, string typeName)
            {
                return System.Type.GetType(System.String.Format("{0}, {1}", typeName, Assembly.GetExecutingAssembly().FullName)); ;
            }
        }

        /// <summary>
        /// <para>Serialize object to byte collection.</para>
        /// </summary>
        /// <param name="objectToSerialize">Object to serialize (System.Serializable objects only).</param>
        /// <returns>Collection of bytes.</returns>
        static public byte[] Serialize(System.Object objectToSerialize)
        {
            IFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            binaryFormatter.Binder = new Binder();
            memoryStream.Position = 0;
            binaryFormatter.Serialize(memoryStream, objectToSerialize);
            memoryStream.Flush();

            byte[] result = memoryStream.ToArray();
            memoryStream.Close();
            memoryStream.Dispose();

            return result;
        }

        /// <summary>
        /// <para>Deserialize from byte collection to object.</para>
        /// </summary>
        /// <param name="dataStream">Collection of bytes to be Deserialized.</param>
        /// <returns>Object (Use an explicit cast or AS operator to convert to the original type).</returns>
        static public System.Object Deserialize(byte[] dataStream)
        {
            MemoryStream memoryStream = new MemoryStream(dataStream);
            IFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Binder = new Binder();
            memoryStream.Flush();
            memoryStream.Position = 0;
            memoryStream.Seek(0, SeekOrigin.Begin);
            System.Object result = binaryFormatter.Deserialize(memoryStream);
            memoryStream.Close();
            memoryStream.Dispose();

            return result;
        }
    }

    public static class Encryptor
    {
        /// <summary>
        /// <para>TransformBlockMode</para>
        /// <para>Defines how to transform data blocks.</para>
        /// </summary>
        enum TransformBlockMode : ushort
        {
            Encrypt = 0,
            Decrypt = 1
        }

        static List<string> _keys = new List<string>() { "A8EyAtQ0t22i4QoLD9yODw==" },
                            _IVs = new List<string>() { "1AECfwQFBhcICQ7LDAhODw==" };
        static int _keyIndex = 0,
                            _ivIndex = 0;

        //Create Rijndael object.
        private static RijndaelManaged CreateRijndael(CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            RijndaelManaged rijndael = new RijndaelManaged
            {
                Key = Convert.FromBase64String(key),
                IV = Convert.FromBase64String(IV),
                Mode = cipherMode,
                Padding = paddingMode
            };
            return rijndael;
        }

        //Transform block of bytes through Rijndael object
        private static byte[] RijndaelTransformBlock(RijndaelManaged rijndael, byte[] bytes, TransformBlockMode transformMode = TransformBlockMode.Encrypt)
        {
            if (transformMode == TransformBlockMode.Encrypt)
                return rijndael.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
            else
                return rijndael.CreateDecryptor().TransformFinalBlock(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// <para>Serialize and encrypt object.</para>
        /// </summary>
        /// <param name="objectToEncode">System.Object</param>
        /// <param name="key">string</param>
        /// <param name="IV">string</param>
        /// <param name="cipherMode">CipherMode</param>
        /// <param name="paddingMode">PaddingMode</param>
        /// <returns>byte[]</returns>
        public static byte[] Encode(System.Object objectToEncode, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            return RijndaelTransformBlock(CreateRijndael(cipherMode, paddingMode), Serializer.Serialize(objectToEncode));
        }

        /// <summary>
        /// <para>Decrypt and deserialize object.</para>
        /// </summary>
        /// <param name="bytes">byte[]</param>
        /// <param name="key">string</param>
        /// <param name="IV">string</param>
        /// <param name="cipherMode">CipherMode</param>
        /// <param name="paddingMode">PaddingMode</param>
        /// <returns>System.Object</returns>
        public static System.Object Decode(byte[] bytes, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            return Serializer.Deserialize(RijndaelTransformBlock(CreateRijndael(cipherMode, paddingMode), bytes, TransformBlockMode.Decrypt));
        }

        public static string EncodeString(string text, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            byte[] bytes = RijndaelTransformBlock(CreateRijndael(cipherMode, paddingMode), UTF8Encoding.UTF8.GetBytes(text));
            return System.Convert.ToBase64String(bytes, 0, bytes.Length);
        }

        public static string DecodeString(string text, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            return UTF8Encoding.UTF8.GetString(RijndaelTransformBlock(CreateRijndael(cipherMode, paddingMode), System.Convert.FromBase64String(text), TransformBlockMode.Decrypt));
        }

        public static bool AddKey(string encryptionKey)
        {
            if (_keys.Contains(encryptionKey))
                return false;

            _keys.Add(encryptionKey);
            return true;
        }

        public static bool RemoveKey(string encryptionKey)
        {
            return _keys.Remove(encryptionKey);
        }

        public static bool AddIV(string EncryptionIV)
        {
            if (_IVs.Contains(EncryptionIV))
                return false;

            _IVs.Add(EncryptionIV);
            return true;
        }

        public static bool RemoveIV(string EncryptionIV)
        {
            return _IVs.Remove(EncryptionIV);
        }

        public static int keyIndex
        {
            get
            {
                return _keyIndex;
            }

            set
            {
                value = System.Math.Min(value, _keys.Count - 1);
                value = System.Math.Max(value, 0);

                _keyIndex = value;
            }
        }
        public static int ivIndex
        {
            get
            {
                return _ivIndex;
            }

            set
            {
                value = System.Math.Min(value, _IVs.Count - 1);
                value = System.Math.Max(value, 0);

                _ivIndex = value;
            }
        }
        public static string key { get { return _keys[_keyIndex]; } }
        public static string IV { get { return _IVs[_keyIndex]; } }
    }

    [Serializable]
    abstract public class Object
    {
    }

    public static readonly string tempFilename = "TempFile.ts";

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
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream,
                                          Encryptor.Encode(saveGameObject));
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
                var binaryFormatter = new BinaryFormatter();
                return (T)Encryptor.Decode((byte[])binaryFormatter.Deserialize(stream));
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

    public static FileInfo[] GetFilenames(string pattern = "*.sv")
    {
        CheckSavePath();

        return new DirectoryInfo(Path.Combine(Application.dataPath, "Save")).GetFiles(pattern);
    }

    static string GetFilePath(string slotName)
    {
        return Path.Combine(Path.Combine(Application.dataPath, "Save"), slotName);
    }

    private static void CheckSavePath()
    {
        string directoryPath = Path.Combine(Application.dataPath, "Save");
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
    }
}