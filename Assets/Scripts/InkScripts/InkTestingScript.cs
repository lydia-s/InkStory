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

    //On Load Variables
    public static string loadedState;
    public static string loadedScene;
    public static List<string> loadedChars;
    public static List<string> loadedTextLog;
    public bool hasLoadedButtons = false;
    public static bool justLoaded = false;
    //On Load Variables

    //Current Variables(these get saved)
    public List<string> currentCharacters;//list of character names in scene
    public List<string> storyLog;//log of all previous text
    public string currentScene;//name of scene
    public string storyState;
    //Current Variables

    //objects
    public TextMeshProUGUI storyText;
    public Button buttonPrefab;
    public GameObject scrollList;
    public Button saveSlot;
    public GameObject saveMenu;
    public GameObject textLogBox;
    public GameObject textLogList;
    public GameObject background;
    public GameObject stage;
    public GameObject characterBox;
    //objects
    public TextAsset inkJSON;
    private Story story;
    public float delay = 0.001f;
    public bool finishedTyping = true;

    // Start is called before the first frame update
    void Start()
    {
        story = new Story(inkJSON.text);
        if (justLoaded == true)
        {
            storyState = loadedState;
            story.state.LoadJson(storyState);
            LoadSceneAndCharacters();//load scene and characters
            LoadTextLogContent();//load text log
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

    #region Characters and scene
    /// <summary>
    /// Set stage and background on load
    /// </summary>
    public void LoadSceneAndCharacters()
    {
        currentCharacters.Clear();
        ClearStage();
        ChangeScene(loadedScene);
        for (int i = 0; i < loadedChars.Count; i++)
        {
            currentCharacters.Add(loadedChars[i]);
        }
        RefreshCharacters();
    }
    /// <summary>
    /// Get scene name and characters from tags
    /// </summary>
    public void UpdateSceneAndCharacters() {
        List<string> tags = story.currentTags;
        if (tags.Count > 0)
        {
            ChangeScene(tags[0]);//change the scene
            //if tags is more than 1, there is also a character change
            if (tags.Count > 1) {
                currentCharacters.Clear();
                for (int i = 1; i < tags.Count; i++)
                {
                    currentCharacters.Add(tags[i]);
                }
                RefreshCharacters();
            }
        }
    }
    /// <summary>
    /// Remove all the characters from the stage
    /// </summary>
    public void ClearStage() {
        for (int i = 0; i < stage.transform.childCount; i++)
        {
            Destroy(stage.transform.GetChild(i).gameObject);
        }
    }
   
   
    /// <summary>
    /// Change scene image
    /// </summary>
    /// <param name="image"></param>
    public void ChangeScene(string image) {
        currentScene = image;
        background.GetComponent<Image>().sprite = Resources.Load<Sprite>(image);//load image from resources
    }
    /// <summary>
    /// Change scene image
    /// </summary>
    /// <param name="image"></param>
    public void ChangeCharacter(string character)
    {
        GameObject charPlaceholder = Instantiate(characterBox) as GameObject;
        charPlaceholder.transform.SetParent(stage.transform, false);
        charPlaceholder.GetComponent<Image>().sprite = Resources.Load<Sprite>(character);//load image from resources
    }
    /// <summary>
    /// Clear the stage and then add all current characters
    /// </summary>
    public void RefreshCharacters() {
        ClearStage();
        for (int i =0; i<currentCharacters.Count; i++) {
            ChangeCharacter(currentCharacters[i]);
        }
    }

    #endregion

    #region Dialogue methods
    public void LoadTextLogContent() {
        foreach(string s in loadedTextLog) {
            AddToTextLog(s);
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
            UpdateSceneAndCharacters();
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
    IEnumerator WriteText(string passage)
    {
        finishedTyping = false;
        for (int i = 0; i < passage.Length; i++)
        {
            storyText.text = passage.Substring(0, i);
            yield return new WaitForSeconds(delay);
        }
        finishedTyping = true;
    }
    /// <summary>
    /// Add story dialogue to scroll list log
    /// </summary>
    /// <param name="text"></param>
    public void AddToTextLog(string text)
    {
        storyLog.Add(text);//add current string to save list
        GameObject txt = Instantiate(textLogBox) as GameObject;
        txt.transform.SetParent(textLogList.transform, false);
        txt.GetComponent<Text>().text = text;
    }
    /// <summary>
    /// Load buttons instantiates a save slot to a list and gives it a listener
    /// </summary>
    public void LoadButtons()
    {
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
    /// Select the corresponding choice in ink as the button pressed
    /// </summary>
    /// <param name="choice"></param>
    void ChooseStoryChoice(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        RefreshUI();
    }
    #endregion

    #region UI methods
    /// <summary>
    /// Destroy buttons that are children of the choices object
    /// </summary>
    void EraseUI()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
        storyText.text = "";
    }
    /// <summary>
    /// Refresh the UI after a load or button press
    /// </summary>
    public void RefreshUI()
    {
        hasLoadedButtons = false;
        EraseUI();
    }

    #endregion

    #region Saving methods
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

    #endregion


}
