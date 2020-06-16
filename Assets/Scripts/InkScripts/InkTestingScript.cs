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
    public static string loadedState="";
    public string storyState = "";
    public GameObject scrollList;
    public Button saveSlot;
    public bool hasLoadedButtons = false;
    public string currentScene;//name of scene
    public List<string> currentCharacters;//list of character names in scene
    public List<string> storyLog;//log of all previous text
    public static bool justLoaded = false;
    public GameObject saveMenu;
    public GameObject textLogBox;
    public GameObject textLogList;
    public float delay = 0.001f;
    public bool finishedTyping = true;
    // Start is called before the first frame update
    void Start()
    {
        if (justLoaded == false)
        {
            story = new Story(inkJSON.text);
           // storyState = story.state.ToJson();//save story state
        }
        else {
            story = new Story(inkJSON.text);
            storyState = loadedState;
            story.state.LoadJson(storyState);
            justLoaded = false;
        }
       

    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !saveMenu.activeSelf && finishedTyping)
        {
            KeepLoadingStory();
        }
    }

    /// <summary>
    /// Get scene name and characters from tags
    /// </summary>
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
    /// <summary>
    /// Continue loading story until a choice is available
    /// </summary>
    public void KeepLoadingStory()
    {
        if (story.canContinue)
        {
            
            string text = story.Continue();
            //storyText.text = text;
            StartCoroutine(WriteText(text));
            AddToTextLog(text);//log all text
            UpadateSceneAndCharacters();
            storyState = story.state.ToJson();
        }
        else
        {

            if ((story.currentChoices.Count != 0) && !hasLoadedButtons)
            {
                hasLoadedButtons = true;
                LoadButtons();

            }
        }
    }
    /// <summary>
    /// Create 'typewriter' effect when writing out passage
    /// </summary>
    /// <param name="passage"></param>
    /// <returns></returns>
    IEnumerator WriteText(string passage) {
        finishedTyping = false;
        for (int i =0; i< passage.Length; i++) {
            storyText.text = passage.Substring(0,i);
            yield return new WaitForSeconds(delay);
        }
        finishedTyping = true;
    }
    /// <summary>
    /// Add story dialogue to scroll list log
    /// </summary>
    /// <param name="text"></param>
    public void AddToTextLog(string text) {
        storyLog.Add(text);//add current string to save list
        GameObject txt = Instantiate(textLogBox) as GameObject;
        txt.transform.SetParent(textLogList.transform, false);
        txt.GetComponent<Text>().text = text;
    }

    /// <summary>
    /// Load buttons instantiates a save slot to a list and gives it a listener
    /// </summary>
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
    /// <summary>
    /// Refresh the UI after a load or button press
    /// </summary>
    public void RefreshUI()
    {
        hasLoadedButtons = false;
        EraseUI();
    }
    /// <summary>
    /// Select the corresponding choice in ink as the button pressed
    /// </summary>
    /// <param name="choice"></param>
    void ChooseStoryChoice(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        RefreshUI();  
    }
    /// <summary>
    /// Destroy buttons that are children of the choices object
    /// </summary>
    void EraseUI()
    {
        for (int i = 0; i< this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
        storyText.text = "";
    }
    /// <summary>
    /// Load an ink save state json string from a binary file 
    /// </summary>
    /// 

/*
 * Saving methods
 */
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
    /// <summary>
    /// Save to binary file
    /// </summary>
    public void SaveStory() {
        string filename = GenerateFileName();
        storyState = story.state.ToJson();//save story state
        SaveSystem.SaveData(this, filename);//save data to file on system
        CreateSaveSlot(filename);//create save slot to load file
    }
    /// <summary>
    /// Generate unique filename
    /// </summary>
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
        return filename;
    }

    /// <summary>
    /// Spawn new visible save slot in scroll list
    /// </summary>
    public void CreateSaveSlot(string filename)
    {
        Button newButton = Instantiate(saveSlot) as Button;
        newButton.transform.SetParent(scrollList.transform, false);
        newButton.GetComponentInChildren<Text>().text = filename;
        newButton.onClick.AddListener(delegate
        {
            string file = newButton.GetComponentInChildren<Text>().text;
            LoadStory(file);
            saveMenu.SetActive(false);//get out of save menu
            RefreshUI();
        });
    }




}
