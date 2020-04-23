using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InkTestingScript : MonoBehaviour
{
    public TextAsset inkJSON;
    private Story story;
    public TextMeshProUGUI storyText;
    public Button buttonPrefab;

    // Start is called before the first frame update
    void Start()
    {
        story = new Story(inkJSON.text);
        if (PlayerPrefs.HasKey("inkSaveState"))
        {
            var savedState = PlayerPrefs.GetString("inkSaveState");
            story.state.LoadJson(savedState);
        }
        RefreshUI();
    }
    public void ResetState()
    {
        story.ResetState();
    }

    public List<string> GetLines() {
        List<string> lines = new List<string>();
        // Get the current tags (if any)
        // If there are tags, use the first one.
        // Otherwise, just show the text.
        while (story.canContinue)
        {
            string text = story.Continue();
            List<string> tags = story.currentTags;
            if (tags.Count > 0)
            {
                lines.Add(tags[0] + " - " + text);
            }
            else
            {
                lines.Add(text);
            }

        }

        return lines;
    }
    public IEnumerator KeepLoadingStory()
    {
        while (story.canContinue)
        {
            yield return new WaitForSecondsRealtime(0.01f);
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
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
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
                var savedState = story.state.ToJson();
                PlayerPrefs.SetString("inkSaveState", savedState);
            });
        }
    }

    public void RefreshUI()
    {
        EraseUI();
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



  
}
