using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using xuexue.f3dModel;

public class CsvLoader{

    private string[] headerLine;
    public int Count;
    public List<Dictionary<string, string>> tables = new List<Dictionary<string, string>>();

    public CsvLoader()
    {

    }

    public void ReadFile(string filePath)
    {
        FileInfo fi = new FileInfo(filePath);
        if (fi.Extension.ToLower() == ".csv")
        {
            IFileReader fileReader = new CSVFile();
            try
            {
                fileReader.OpenFile(filePath);
                ReadConfigFilesAll(fileReader);
            }
            catch (System.Exception e)
            {
                Debug.LogError("CsvLoader.ReadFile():异常" + e.Message);
            }
            fileReader.Close();

            Debug.Log("CsvLoader.ReadFile():读文件结束,当前 有效记录Count =" + Count);
        }
        else
        {
            Debug.Log("CsvLoader.ReadFile():不支持的文件" + fi.FullName);
        }
    }

    /// <summary>
    /// 私有方法：读整个文件
    /// </summary>
    /// <param name="fileReader"></param>
    private void ReadConfigFilesAll(IFileReader fileReader)
    {
        headerLine = fileReader.ReadHeaderLine();
        if (headerLine == null)
        {
            Debug.LogError("CsvLoader.ReadConfigFilesAll():读文件第一行Header错误!");
        }
        else
        {
            Debug.Log("CsvLoader.ReadConfigFilesAll():Header = " + headerLine.ToStr());
        }

        int lineCount = 0;
        while (true)
        {
            var line = fileReader.ReadLine2();
            if (line == null)
            {
                break;
            }
            tables.Add(line);
            lineCount++;

            if (CheckIsEmptyLine(line))
            {
                Debug.Log("CsvLoader.ReadConfigFilesAll():读到一个空行 ");//如果是空行就忽略
                continue;
            }
        }
    }

    /// <summary>
    /// 检查一行是否是空行
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    private bool CheckIsEmptyLine(Dictionary<string, string> line)
    {
        foreach (var kvp in line)
        {
            if (!string.IsNullOrEmpty(kvp.Value))//只要有一行不是空行，那么就返回false
                return false;
        }
        return true;
    }
}
