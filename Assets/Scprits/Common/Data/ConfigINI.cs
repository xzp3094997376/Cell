using IniParser;
using IniParser.Model;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 
/// ConfigINI ci = new ConfigINI(Application.dataPath + "/product.ini");
/// ci.SetData("Product", "3DGlass", "1");
/// ci.SetData("Product", "3DTV", "2");
/// ci.Save();
///
///ci.ReadData("Product", "3DGlass");
/// </summary>
public class ConfigINI{
    private FileIniDataParser parser;
    private IniData iniData;
    private string path;
    public ConfigINI(string iniPath)
    {
        path = iniPath;
        parser = new FileIniDataParser();
        parser.Parser.Configuration.AllowDuplicateKeys = true;
        parser.Parser.Configuration.OverrideDuplicateKeys = true;
        parser.Parser.Configuration.AllowDuplicateSections = true;
        iniData = new IniData();
        if (File.Exists(iniPath))
        {
            iniData = parser.ReadFile(iniPath);//读ini文件
        }
    }

    public string ReadData(string section, string key)
    {
        string result = string.Empty;
        if(iniData != null)
        {
            result = iniData[section][key];
        }
        return result;
    }

    public void SetData(string section, string key,string val)
    {
        if(iniData != null)
        {
            iniData[section][key] = val;
        }
    }

    public void Save()
    {
        if(parser != null)
        {
            parser.WriteFile(path, iniData);
        }
    }
}
