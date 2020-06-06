using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public class SaveSystem 
{
    public static void SaveData(InkTestingScript script, string filename) {
        BinaryFormatter formatter = new BinaryFormatter();
        //System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
        string path = Application.persistentDataPath +"/"+ filename+ ".story";
        FileStream stream = new FileStream(path, FileMode.Create);
        SaveData data = new SaveData(script);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    //LoadData(string filename)
    public static SaveData LoadData(string filename) {
        string path = Application.persistentDataPath + "/" + filename +".story";
        if (File.Exists(path))
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();
            return data;
        }
        else {
            Debug.LogError("Save file not found");
            return null;
        }
    }

    //save and update all slots
    public static void SaveSlotList(InkTestingScript script) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/saveSlots.story";
        FileStream stream = new FileStream(path, FileMode.Create);
        SaveSlotList data = new SaveSlotList(script);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    //load slot list
    public static SaveSlotList LoadSlotList()
    {
        string path = Application.persistentDataPath + "/saveSlots.story";
        if (File.Exists(path))
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            SaveSlotList data = formatter.Deserialize(stream) as SaveSlotList;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found");
            return null;
        }

    }


}
