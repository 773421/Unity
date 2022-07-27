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
        //���pĿ¼������ �򴴽�һ��pĿ¼
        if (!Directory.Exists(pPath))
        {
            Directory.CreateDirectory(pPath);
            //����Э��  ��sĿ¼����Ķ���������pĿ¼��
            StartCoroutine(Copy());
        }
        else
        {
            //�����ȸ���Э��
            StartCoroutine(CheclUpdate());
        }
        
    }

    IEnumerator Copy()
    {
        print(1);
        //sĿ¼�����version�ļ�
        string streamingAssetsPathVersion = SPath + "version.txt";
        string versionContent = "";

        //�����ȡversion�ļ�
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
        //����version�ļ�·��
        string localVersion = pPath + "version.txt";
        //��ȡ·���µ��ļ�
        string localVersionContent = File.ReadAllText(localVersion);

        VersionData localVersionData = JsonConvert.DeserializeObject<VersionData>(localVersionContent);

        //��Դ���ݵ��ֵ�
        Dictionary<string, AssetData> versionDic = new Dictionary<string, AssetData>();

        for (int i = 0; i < localVersionData.assetDatas.Count; i++)
        {
            AssetData assetData = localVersionData.assetDatas[i];
            //ab�����ֵ��� ���ݵ����� �����ֵ�
            versionDic.Add(assetData.abName, assetData);
        }


        //Զ��Version�ļ���ַ
        string remoteVersion = localVersionData.downLoadUrl + "ABTest/version.txt";
        string remoteVersionContent = "";

        //�����ȡԶ��version�ļ�
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

        //��Ҫ���µ���Դ����
        List<AssetData> updatalist = new List<AssetData>();

        if (localVersionData.versionCode < remoteVersionData.versionCode)//�Ƚϰ汾��
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
            print("����Ҫ����");
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
        print("�������");
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
