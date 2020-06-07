using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class InkTestingScript : MonoBehaviour
{
    public TextAsset inkJSON;
    private Story story;
    public TextMeshProUGUI storyText;
    public Button buttonPrefab;
    public string storyState="";
    public List<string> saveSlots;
    public GameObject scrollList;
    public Button saveSlot;
    public bool hasLoadedButtons = false;
    public string currentScene;//name of scene
    public List<string> currentCharacters;//list of character names in scene
    public List<string> storyLog;//log of all previous text
    // Start is called before the first frame update
    void Start()
    {
        SaveSlotList slots = SaveSystem.LoadSlotList();
        saveSlots = slots.saveSlots;
        PopulateScrollList();
        story = new Story(inkJSON.text);
        RefreshUI();

    }
    public void PopulateScrollList() {
        foreach (string filename in saveSlots) {
            CreateSaveSlot(filename);
        }
    }
    public void ResetState()
    {
        story.ResetState();
        RefreshUI();
    }
    //get scene name and characters from tags
    public void UpadateSceneAndCharacters() {
        List<string> tags = story.currentTags;
        if (tags.Count > 0)
        {
            currentScene = tags[0];;
            for (int i = 1; i < tags.Count; i++) {
                currentCharacters.Add(tags[i]);
            }

        }

    }
    public void KeepLoadingStory()
    {
        if (story.canContinue)
        {
            string text = story.Continue();
            storyText.text = text;
            storyLog.Add(text);//add to log
            UpadateSceneAndCharacters();
        }
        else
        {
            if ((story.currentChoices.Count != 0) && !hasLoadedButtons)
            {
                Debug.Log("LOAD THE FUCKING BUTTONS!");
                hasLoadedButtons = true;
                LoadButtons();

            }
        }
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            KeepLoadingStory();
        }
        

    }


    public void LoadButtons() {
        foreach (Choice choice in story.currentChoices)
        {

            Button choiceButton = Instantiate(buttonPrefab) as Button;
            Text choiceText = choiceButton.GetComponentInChildren<Text>();
            choiceText.text = choice.text;
            choiceButton.transform.SetParent(this.transform, false);
            choiceButton.onClick.AddListener(delegate {
                ChooseStoryChoice(choice);
                
            });
            
        }
    }
    //refresh the ui after a load or button press
    public void RefreshUI()
    {
        hasLoadedButtons = false;
        EraseUI();
        storyState = story.state.ToJson();
        story.state.LoadJson(storyState);
    }

    void ChooseStoryChoice(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        RefreshUI();  
    }

    void EraseUI()
    {
        for (int i = 0; i< this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
        storyText.text = "";
    }
    //load from binary file
    public void LoadStory(string filename) {
        hasLoadedButtons = false;
        SaveData data = SaveSystem.LoadData(filename);
        storyState = data.saveState;
        currentScene = data.currentScene;//get last scene
        currentCharacters = data.currentCharacters;//get last characters
        story.state.LoadJson(storyState);
        RefreshUI();
    }

    //save to binary file
    public string lastFilename;

    //save to binary file
    public void SaveStory() {
        string filename = GenerateFileName();
        SaveSystem.SaveSlotList(this);//update save slot list
        storyState = story.state.ToJson();//save story state
        SaveSystem.SaveData(this, filename);
    }
    //generate unique filename
    public string GenerateFileName()
    {
        string format = "yyyy-MM-dd HH,mm,ss";
        string filename = System.DateTime.Now.ToString(format);
        if (lastFilename != null)
        {
            while (lastFilename == filename)//while they are equal
            {
                filename = System.DateTime.Now.ToString(format);//reset filename
            }
        }
        lastFilename = filename;
        CreateSaveSlot(filename);
        saveSlots.Add(filename);//add new filename to list
        return filename;
    }
        

    
    //spawn visible save slot in scroll list
    public void CreateSaveSlot(string filename) {
        Button newButton = Instantiate(saveSlot) as Button;
        newButton.transform.SetParent(scrollList.transform, false);
        newButton.GetComponentInChildren<Text>().text = filename;
        newButton.onClick.AddListener(delegate {
            string file = newButton.GetComponentInChildren<Text>().text;
            LoadStory(file);
            RefreshUI();
            

        });
    }



  
}
