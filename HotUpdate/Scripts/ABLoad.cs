using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;
using UnityEngine.U2D;

public class ABLoad : MonoBehaviour
{
    public GameObject parent;
    public Image ima;
    Dropdown dropdown;
    void Start()
    {
        //string str = File.ReadAllText("1.json");
        //List<JXJson> jsons = JsonConvert.DeserializeObject<List<JXJson>>(str);
        //AssetBundle picture = AssetBundle.LoadFromFile(Application.dataPath + "/ABTest/_comm.u3d");//这是加载图集的依赖项

        //AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.dataPath + "/ABTest/obj.u3d");
        //AssetBundle abBundle = AssetBundle.LoadFromFile(Application.dataPath + "/ABTest/atlas.u3d");

        //foreach (var item in jsons)
        //{
        //    GameObject go = Instantiate(assetBundle.LoadAsset<Object>("obj") as GameObject,parent.transform);
        //    SpriteAtlas atlas = Instantiate(abBundle.LoadAsset<SpriteAtlas>("Atlas"));
        //    go.transform.GetChild(1).GetComponent<Text>().text = item.name;
        //    go.transform.GetChild(0).GetComponent<Image>().sprite = atlas.GetSprite(item.picname);
        //}

        // Sprite[] sprites= ABManager.GetSingleton().LoadSprites("atlas.u3d", "Atlas");
        
        //加载
        AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.dataPath+"/ABTest/cube.u3d");
        //实例化
        GameObject go = Instantiate(assetBundle.LoadAsset<GameObject>("cube"));




        SpriteAtlas a=  ABManager.GetSingleton().LoadOtherAsset<SpriteAtlas>("atlas.u3d", "Atlas");
        Sprite[] sprites = new Sprite[a.spriteCount];
        a.GetSprites(sprites);


        ima.GetComponent<Image>().sprite = sprites[0];



        SpriteAtlas sprite = ABManager.GetSingleton().LoadOtherAsset<SpriteAtlas>("","");
        Sprite[] sprite1 = new Sprite[sprite.spriteCount];
        sprite.GetSprites(sprite1 );


        Dropdown drop = ABManager.GetSingleton().LoadOtherAsset<Dropdown>("","");
        drop.options.Add(new Dropdown.OptionData("哈哈"));

        drop.onValueChanged.AddListener((index) => {
        

        });

        Button on = ABManager.GetSingleton().LoadOtherAsset<Button>("", "");
        on.onClick.AddListener(() => {

            Destroy(on);
        
        });

        Sprite atlas = ABManager.GetSingleton().LoadSprite("Atlas.u3d","Atlas","mug1001");
        Image image = ABManager.GetSingleton().LoadOtherAsset<Image>("","");
        image.transform.GetComponent<Image>().sprite = atlas;


        AssetBundle asset = AssetBundle.LoadFromFile(Application.dataPath + "/ABTest/cube.u3d");
        GameObject game = Instantiate(asset.LoadAsset<GameObject>("cube"));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class JXJson
{
    public int id;
    public string name;
    public string picname;
}
