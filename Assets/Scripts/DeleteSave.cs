using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class DeleteSave : MonoBehaviour
{
    public GameObject saveSlotButton;
    //make delete trigger more user friendly
    public void DeleteSelf() {     
        Destroy(saveSlotButton);
        string filename = saveSlotButton.GetComponentInChildren<Text>().text;
        string path = Application.persistentDataPath + "/" + filename + ".story";
        if (File.Exists(path)) {
            File.Delete(path);
        }

    }
}
