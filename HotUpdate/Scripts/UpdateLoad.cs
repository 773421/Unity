using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;
using XLua;

public class UpdateLoad : MonoBehaviour
{
    private LuaEnv luaEnv;

    void Start()
    {
        //如果p目录不存在 则创建一个p目录
        if (!Directory.Exists(pPath))
        {
            Directory.CreateDirectory(pPath);
            //开启协程  把s目录里面的东西拷贝到p目录里
            StartCoroutine(Copy());
        }
        else
        {
            //开启热更新协程
            StartCoroutine(CheclUpdate());
        }
        
    }

    IEnumerator Copy()
    {
        print(1);
        //s目录下面的version文件
        string streamingAssetsPathVersion = SPath + "version.txt";
        string versionContent = "";

        //请求获取version文件
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(streamingAssetsPathVersion);
        yield return unityWebRequest.SendWebRequest();


        if (unityWebRequest.isHttpError)
        {
            Debug.Log(unityWebRequest.error);
        }
        else
        {
            versionContent = unityWebRequest.downloadHandler.text;
        }

#if UNITY_ANDROID

#else
        versionContent = File.ReadAllText(streamingAssetsPathVersion);
#endif
        VersionData versionData = JsonConvert.DeserializeObject<VersionData>(versionContent);

        for (int i = 0; i < versionData.assetDatas.Count; i++)
        {
            AssetData assetData = versionData.assetDatas[i];
            string sPath = SPath + assetData.abName;
            string fileName = Path.GetFileName(sPath);
            string dir = Path.GetDirectoryName(SPath).Replace("\\", "/") + "/";
            dir = dir.Replace(SPath, pPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            dir = dir + fileName;
            File.Copy(sPath, dir);
        }

        File.WriteAllText(pPath + "/version.txt", versionContent);
        StartCoroutine(CheclUpdate());
        yield return null;

    }

    IEnumerator CheclUpdate()
    {
        //本地version文件路径
        string localVersion = pPath + "version.txt";
        //读取路径下的文件
        string localVersionContent = File.ReadAllText(localVersion);

        VersionData localVersionData = JsonConvert.DeserializeObject<VersionData>(localVersionContent);

        //资源数据的字典
        Dictionary<string, AssetData> versionDic = new Dictionary<string, AssetData>();

        for (int i = 0; i < localVersionData.assetDatas.Count; i++)
        {
            AssetData assetData = localVersionData.assetDatas[i];
            //ab包名字当键 数据当内容 存入字典
            versionDic.Add(assetData.abName, assetData);
        }


        //远程Version文件地址
        string remoteVersion = localVersionData.downLoadUrl + "ABTest/version.txt";
        string remoteVersionContent = "";

        //请求获取远程version文件
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(remoteVersion);
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.isNetworkError)
        {

        }
        else
        {
            remoteVersionContent = unityWebRequest.downloadHandler.text;
        }
        VersionData remoteVersionData = JsonConvert.DeserializeObject<VersionData>(remoteVersionContent);

        //需要更新的资源集合
        List<AssetData> updatalist = new List<AssetData>();

        if (localVersionData.versionCode < remoteVersionData.versionCode)//比较版本号
        {
            for (int i = 0; i < remoteVersionData.assetDatas.Count; i++)
            {
                AssetData assetData = remoteVersionData.assetDatas[i];

                if (versionDic.ContainsKey(assetData.abName))
                {
                    if (versionDic[assetData.abName].Md5 != assetData.Md5)
                    {
                        updatalist.Add(assetData);
                    }
                }
                else
                {
                    updatalist.Add(assetData);
                }
            }
        }
        else
        {
            EnterGame();
            print("不需要更新");
            //gameObject.AddComponent<>();
            yield break;
        }

        for (int i = 0; i < updatalist.Count; i++)
        {
            string abName = updatalist[i].abName;
            UnityWebRequest updateAsset = UnityWebRequest.Get(remoteVersionData.downLoadUrl +"ABTest/"+ abName);
            yield return updateAsset.SendWebRequest();

            if (updateAsset.isNetworkError)
            {

            }
            else
            {
                string perPath = pPath + abName;
                string dir = Path.GetDirectoryName(perPath).Replace("\\", "/");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.WriteAllBytes(dir + "/" + abName, updateAsset.downloadHandler.data);
            }
        }
        File.WriteAllText(pPath + "version.txt", remoteVersionContent);
        print("更新完成");
        EnterGame();
        //gameObject.AddComponent<>();
        yield return null;
    }

    private void EnterGame()
    {
        //  GameObject go = ABManager.GetSingleton().LoadGameObject("cube.u3d", "Cube");
        luaEnv = new LuaEnv();
        luaEnv.AddLoader(Custom);
        luaEnv.DoString("require'main'");
    }
    private byte[] Custom(ref string filepath)
    {
        string path = Application.dataPath + "/LuaScripts/" + filepath + ".lua";
        return File.ReadAllBytes(path);
    }



    // Update is called once per frame
    void Update()
    {

    }
    public static string SPath
    {
        get
        {
            return Application.streamingAssetsPath + "/ABTest/";
        }
    }
    public static string pPath
    {
        get
        {
            return Application.persistentDataPath + "/ABTest/";
        }
    }
}
