using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class TestGame : MonoBehaviour
{
    LuaEnv luaEnv;
    void Start()
    {
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
}
