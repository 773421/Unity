using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;

public class AB : Editor
{
    static string abPath = Application.streamingAssetsPath + "/ABTest";
    //打包
    [MenuItem("Tools/ABTool")]
    static void Build()
    {
        string outpath = Application.streamingAssetsPath + "/ABTest/";
        string path = Application.dataPath + "/Resources/";
        string[] filepath = Directory.GetFiles(path, ".", SearchOption.AllDirectories);

        foreach (var item in filepath)
        {
            if (Path.GetExtension(item).Contains("meta")) continue;
            Debug.Log("-------------------");
            Debug.Log(item);
            //abName = string.Empty;
            string fileName = item.Replace(Application.dataPath, "Assets");
            AssetImporter assetImporter = AssetImporter.GetAtPath(fileName);
            string abName = fileName.Replace("Assets/Resources/", string.Empty);
            abName = abName.Replace("\\", "/");
            Debug.Log("replace before::" + abName);
            if (item.Contains("_Comm"))
            {
                abName = abName.Replace("/" + Path.GetFileName(item), string.Empty);
                Debug.Log("SSS::" + abName);
            }
            else
            {
                abName = abName.Replace(Path.GetExtension(item), string.Empty);
            }
            assetImporter.assetBundleName = abName;
            Debug.Log(abName);
            assetImporter.assetBundleVariant = "u3d";
            //assetImporter.assetBundleName = "cube";
            //assetImporter.assetBundleVariant = "u3d";
        }
        BuildPipeline.BuildAssetBundles(outpath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);

        //清楚包名
        foreach (var item in filepath)
        {
            if (Path.GetExtension(item).Contains("meta")) continue;
            Debug.Log("-------------------");

            string fileName = item.Replace(Application.dataPath, "Assets");
            AssetImporter assetImporter = AssetImporter.GetAtPath(fileName);
            string abName = fileName.Replace("Assets/Resources/", string.Empty);
        }

        AssetDatabase.Refresh();
    }
    //把lua里的东西里连带文件拷贝到ABTest文件夹里面
    [MenuItem("Tools/CopyLua")]
    static void CopyLua()
    {
        string luaPath = Application.dataPath + "/LuaScripts/";
        string[] filePaths = Directory.GetFiles(luaPath, ".", SearchOption.AllDirectories);
        foreach (var item in filePaths)
        {
            if (Path.GetExtension(item).Contains("meta")) continue;
            
            string path = item.Replace(Application.dataPath, abPath);
            Debug.Log(path);
            string dir = Path.GetDirectoryName(path).Replace("\\","/");
            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.Copy(item, path);

        }
        AssetDatabase.Refresh();
    }

    //生成Version文件
    static VersionData versionData = new VersionData();
    [MenuItem("Tools/MakeVersion")]
    static void MakeVersion()
    {
        versionData.downLoadUrl = "http://127.0.0.1/MyAB/";
        versionData.version = "1.0.0";
        versionData.versionCode = 3;

        if (versionData.assetDatas == null)
        {
            versionData.assetDatas = new List<AssetData>();
        }
        else
        {
            versionData.assetDatas.Clear();
        }

        string abPath = Application.streamingAssetsPath + "/ABTest/";
        string[] filePaths = Directory.GetFiles(abPath, ".", SearchOption.AllDirectories);
        foreach (var item in filePaths)
        {
            if (Path.GetExtension(item).Contains("meta") || Path.GetExtension(item).Contains("manifest")) continue;
            string abName = item.Replace("\\", "/");
            abName = abName.Replace(abPath, "");
            Debug.Log(abName);
            int len = File.ReadAllBytes(item).Length;
            //生成MD5码
            string Md5 = FileMD5(item);

            AssetData assetData = new AssetData();
            assetData.abName = abName;
            assetData.len = len;
            assetData.Md5 = Md5;

            versionData.assetDatas.Add(assetData);
        }
        //解析json文件 写入version文件
        string version = JsonConvert.SerializeObject(versionData);
        File.WriteAllText(abPath + "/version.txt", version);
        AssetDatabase.Refresh();
    }
    static StringBuilder sb = new StringBuilder();
    private static string FileMD5(string item)
    {
        FileStream file = new FileStream(item, FileMode.Open);
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] bytes = md5.ComputeHash(file);
        file.Close();

        sb.Clear();
        for (int i = 0; i < bytes.Length; i++)
        {
            sb.Append(bytes[i].ToString("X2"));
        }
        return sb.ToString();
    }
    [MenuItem("Tools/打开P目录")]
    static void OpenP()
    {
        System.Diagnostics.Process.Start(Application.persistentDataPath);
    }
    [MenuItem("Tools/打开S目录")]
    static void OpenS()
    {
        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }
        System.Diagnostics.Process.Start(Application.streamingAssetsPath);
    }
}

