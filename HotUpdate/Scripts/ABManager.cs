using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ABManager : Singleton<ABManager>
{
    //清单列表
    AssetBundleManifest assetBundleManifest;
    //AB包路径
    string ABpath;
    public ABManager()
    {
        ABpath = Application.persistentDataPath + "/ABTest/";
        //获取ABTest文件的路径
        AssetBundle assetBundle = AssetBundle.LoadFromFile(ABpath+"ABTest");
        //加载清单列表
        assetBundleManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        //释放压缩的资源文件
        assetBundle.Unload(false);
    }

    //加载打包资源
    Dictionary<string, BundleData> dicBundles = new Dictionary<string, BundleData>();

    /// <summary>
    /// 加载Object类型的资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName">要加载的资源的名字</param>
    /// <returns></returns>
    public T[] LoadAsset<T>(string abName) where T:UnityEngine.Object
    {
        //获取所有依赖项  
        string[] dependencies = assetBundleManifest.GetAllDependencies(abName);

        //遍历所有依赖项 查看要加载的物体上是否含有依赖项
        foreach (var item in dependencies)
        {
            //如果不含依赖项  正常加载物体  添加到字典中 避免产生重复的依赖项
            if(!dicBundles.ContainsKey(item))
            {
                AssetBundle assetBundle = AssetBundle.LoadFromFile(ABpath + item);
                BundleData bundle = new BundleData(assetBundle);
                dicBundles.Add(item, bundle);
            }
            else
            {
                //如果包含依赖项 计数器加加
                dicBundles[item].count++;
            }
        }

        //判断要加载的物体是否是依赖项  如果不是依赖项正常加载  添加到字典内避免有重复
        if(!dicBundles.ContainsKey(abName))
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(ABpath + abName);
            BundleData bundleData = new BundleData(assetBundle);
            dicBundles.Add(abName, bundleData);
        }
        else
        {
            //如果是依赖项 计数器加1
            dicBundles[abName].count++;
        }
        //返回所有加载成功的资源
        return dicBundles[abName].ab.LoadAllAssets<T>();
    }

    Dictionary<int, string> dicGameject = new Dictionary<int, string>();

    /// <summary>
    /// 加载除了Object以外的资源
    /// </summary>
    /// <param name="abName">AB包名字</param>
    /// <param name="assetName">要加载的物体的名字</param>
    /// <returns></returns>
    public GameObject LoadGameObject(string abName,string assetName)
    {
        Object obj = LoadOtherAsset<GameObject>(abName, assetName);
        GameObject go = GameObject.Instantiate(obj) as GameObject;
        //获取对象唯一ID
        dicGameject.Add(go.GetInstanceID(),abName);
        return go;
    }
    /// <summary>
    /// 获取图集里面的一张图片
    /// </summary>
    /// <param name="abName">ab包名字</param>
    /// <param name="assetName">图集名字</param>
    /// <param name="spriteName">图集里图片的名字</param>
    /// <returns></returns>
    public Sprite LoadSprite(string abName, string assetName,string spriteName)
    {
        SpriteAtlas spriteAtlas = LoadOtherAsset<SpriteAtlas>(abName, assetName);
        return spriteAtlas.GetSprite(spriteName);
    }
    /// <summary>
    /// 获取图集
    /// </summary>
    /// <param name="abName">ab包名字</param>
    /// <param name="assetName">图集名字</param>
    /// <returns></returns>
    public Sprite[] LoadSprites(string abName, string assetName )
    {
        SpriteAtlas spriteAtlas = LoadOtherAsset<SpriteAtlas>(abName, assetName);
        //根据图集内图片数量 得到数组
        Sprite[] sprites = new Sprite[spriteAtlas.spriteCount];
        spriteAtlas.GetSprites(sprites);
        return sprites;
    }

    //封装加载除了Object以外资源的方法
    public T LoadOtherAsset<T>(string abName, string assetName) where T:Object
    {
        //遍历所有加载的Object类型的物体 如果有相同的名字 退出 没有相同的名字 返回并加载
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
        //获取对象唯一ID
        int id = go.GetInstanceID();
        //获取要删除的物体的名字
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

    //打包的数据类
    public class BundleData
    {
        public AssetBundle ab;
        public int count;//计数器
        public BundleData(AssetBundle ab)
        {
            this.ab = ab;
            this.count = 1;//默认为1
        }

        //封装卸载物体的方法
        public void UnLoad()
        {
            //销毁加载的物体
            ab.Unload(true);
        }
    }
   

}

