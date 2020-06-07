using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SaveData
{
    public string saveState;
    public string currentScene;
    public List<string> currentCharacters;
    public SaveData(InkTestingScript script) {
        saveState = script.storyState;//the json string containing the state of the story
        currentScene = script.currentScene;
        currentCharacters = script.currentCharacters;

    }
   
}
