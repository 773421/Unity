using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ABManager : Singleton<ABManager>
{
    //�嵥�б�
    AssetBundleManifest assetBundleManifest;
    //AB��·��
    string ABpath;
    public ABManager()
    {
        ABpath = Application.persistentDataPath + "/ABTest/";
        //��ȡABTest�ļ���·��
        AssetBundle assetBundle = AssetBundle.LoadFromFile(ABpath+"ABTest");
        //�����嵥�б�
        assetBundleManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        //�ͷ�ѹ������Դ�ļ�
        assetBundle.Unload(false);
    }

    //���ش����Դ
    Dictionary<string, BundleData> dicBundles = new Dictionary<string, BundleData>();

    /// <summary>
    /// ����Object���͵���Դ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName">Ҫ���ص���Դ������</param>
    /// <returns></returns>
    public T[] LoadAsset<T>(string abName) where T:UnityEngine.Object
    {
        //��ȡ����������  
        string[] dependencies = assetBundleManifest.GetAllDependencies(abName);

        //�������������� �鿴Ҫ���ص��������Ƿ���������
        foreach (var item in dependencies)
        {
            //�������������  ������������  ��ӵ��ֵ��� ��������ظ���������
            if(!dicBundles.ContainsKey(item))
            {
                AssetBundle assetBundle = AssetBundle.LoadFromFile(ABpath + item);
                BundleData bundle = new BundleData(assetBundle);
                dicBundles.Add(item, bundle);
            }
            else
            {
                //������������� �������Ӽ�
                dicBundles[item].count++;
            }
        }

        //�ж�Ҫ���ص������Ƿ���������  ���������������������  ��ӵ��ֵ��ڱ������ظ�
        if(!dicBundles.ContainsKey(abName))
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(ABpath + abName);
            BundleData bundleData = new BundleData(assetBundle);
            dicBundles.Add(abName, bundleData);
        }
        else
        {
            //����������� ��������1
            dicBundles[abName].count++;
        }
        //�������м��سɹ�����Դ
        return dicBundles[abName].ab.LoadAllAssets<T>();
    }

    Dictionary<int, string> dicGameject = new Dictionary<int, string>();

    /// <summary>
    /// ���س���Object�������Դ
    /// </summary>
    /// <param name="abName">AB������</param>
    /// <param name="assetName">Ҫ���ص����������</param>
    /// <returns></returns>
    public GameObject LoadGameObject(string abName,string assetName)
    {
        Object obj = LoadOtherAsset<GameObject>(abName, assetName);
        GameObject go = GameObject.Instantiate(obj) as GameObject;
        //��ȡ����ΨһID
        dicGameject.Add(go.GetInstanceID(),abName);
        return go;
    }
    /// <summary>
    /// ��ȡͼ�������һ��ͼƬ
    /// </summary>
    /// <param name="abName">ab������</param>
    /// <param name="assetName">ͼ������</param>
    /// <param name="spriteName">ͼ����ͼƬ������</param>
    /// <returns></returns>
    public Sprite LoadSprite(string abName, string assetName,string spriteName)
    {
        SpriteAtlas spriteAtlas = LoadOtherAsset<SpriteAtlas>(abName, assetName);
        return spriteAtlas.GetSprite(spriteName);
    }
    /// <summary>
    /// ��ȡͼ��
    /// </summary>
    /// <param name="abName">ab������</param>
    /// <param name="assetName">ͼ������</param>
    /// <returns></returns>
    public Sprite[] LoadSprites(string abName, string assetName )
    {
        SpriteAtlas spriteAtlas = LoadOtherAsset<SpriteAtlas>(abName, assetName);
        //����ͼ����ͼƬ���� �õ�����
        Sprite[] sprites = new Sprite[spriteAtlas.spriteCount];
        spriteAtlas.GetSprites(sprites);
        return sprites;
    }

    //��װ���س���Object������Դ�ķ���
    public T LoadOtherAsset<T>(string abName, string assetName) where T:Object
    {
        //�������м��ص�Object���͵����� �������ͬ������ �˳� û����ͬ������ ���ز�����
        UnityEngine.Object[] objects = LoadAsset<T>(abName);
        Object obj = null;
        foreach (var item in objects)
        {
            if(item.name==assetName)
            {
                obj = item; break;
            }
        }
        return obj as T;
    }

    public void DestoryGameObject(GameObject go)
    {
        //��ȡ����ΨһID
        int id = go.GetInstanceID();
        //��ȡҪɾ�������������
        string abName = dicGameject[id];
        GameObject.Destroy(go);
        dicGameject.Remove(id);
        UnLoadAB(abName);
    }

    private void UnLoadAB(string abName)
    {
        string[] dependencies = assetBundleManifest.GetAllDependencies(abName);

        foreach (var item in dependencies)
        {
            if(dicBundles.ContainsKey(item))
            {
                dicBundles[item].count--;
                if(dicBundles[item].count<=0)
                {
                    dicBundles[item].UnLoad();
                }
            }
        }
        if (dicBundles.ContainsKey(abName))
        {
            dicBundles[abName].count--;
            if (dicBundles[abName].count <= 0)
            {
                dicBundles[abName].UnLoad();
            }
        }

    }

    //�����������
    public class BundleData
    {
        public AssetBundle ab;
        public int count;//������
        public BundleData(AssetBundle ab)
        {
            this.ab = ab;
            this.count = 1;//Ĭ��Ϊ1
        }

        //��װж������ķ���
        public void UnLoad()
        {
            //���ټ��ص�����
            ab.Unload(true);
        }
    }
   

}

