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
    public IEnumerator KeepLoadingStory()
    {
        while (story.canContinue)
        {
            
            yield return new WaitForSecondsRealtime(0.01f);//stop error where multiple buttons appear
            string text = story.Continue();
            List<string> tags = story.currentTags;
            if (tags.Count > 0)
            {
                storyText.text = tags[0] + " - " + text;
                Debug.Log(tags[0] + " - " + text);
            }
            else
            {
                storyText.text = text;
            }
            yield return new WaitUntil(() => Input.GetMouseButtonDown(1));

        }
        if (story.currentChoices.Count!=0) {
            LoadButtons();
            yield return null;
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

    public void RefreshUI()
    {
        EraseUI();
        var savedState = story.state.ToJson();
        story.state.LoadJson(savedState);
        StartCoroutine(KeepLoadingStory());
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
        
        SaveData data = SaveSystem.LoadData(filename);
        storyState = data.saveState;
        RefreshUI();
    }

    //save to binary file
    public string lastFilename;
    public void SaveStory() {
        string filename = GenerateFileName();
        SaveSystem.SaveSlotList(this);//update save slot list
        storyState = story.state.ToJson();
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
