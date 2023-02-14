using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StatsManager : MonoBehaviour
{
    [SerializeField] private string fileName;
    [SerializeField] private string[] columns;

    [SerializeField] private string runCountFile;

    // Start is called before the first frame update
    void Start()
    {
        fileName = Application.dataPath + "/Data/" + fileName + ".csv";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetRuns()
    {
        string countFile = Application.dataPath + "/Data/" + runCountFile + ".txt";
        StreamReader reader = new StreamReader(countFile);
        int runs = int.Parse(reader.ReadLine());
        reader.Close();
        return runs;
    }

    public void UpdateRuns()
    {
        int runs = GetRuns();

        string countFile = Application.dataPath + "/Data/" + runCountFile + ".txt";
        StreamWriter writer = new StreamWriter(countFile);
        writer.Write(runs + 1);
        writer.Close();
    }

    public void CreateCSV(string name)
    {
        try
        {
            fileName = Application.dataPath + "/Data/" + name + ".csv";
            TextWriter tw = new StreamWriter(fileName, false);
            tw.WriteLine(string.Join(";", columns));
            tw.Close();
            Debug.Log("Successfully created CSV file at " + fileName);
        } catch
        {
            Debug.LogError("Issue creating CSV file at " + fileName);
        }
    }

    public void Write(float[] data)
    {
        TextWriter tw = new StreamWriter(fileName, true);

        string csvData = string.Join(";", data);
        tw.WriteLine(csvData);

        tw.Close();
    }
}
