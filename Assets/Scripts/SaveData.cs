using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SaveData
{
    public string saveState;

    public SaveData(InkTestingScript script) {
        saveState = script.storyState;

    }
   
}
