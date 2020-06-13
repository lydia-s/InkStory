using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSystem : MonoBehaviour
{
    public Button saveSlot;
    public GameObject scrollList;
    public GameObject saveSlotMenu;
    public void LoadNewGame() {
        SceneManager.LoadScene("Game");
    }
    public void LoadSaves()
    {
        saveSlotMenu.SetActive(true);
    }
    public void LoadOutOfSaves() {
        saveSlotMenu.SetActive(false);
    }
    public void LoadOptions()
    {
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    /// <summary>
    /// Populate a list with save slot buttons
    /// *loads existing files*
    /// </summary>
    public void PopulateScrollList()
    {
        string folderpath = Application.persistentDataPath + "/" + "saves";
        foreach (string filepath in Directory.GetFiles(folderpath))
        {
            string filename = filepath.Replace(folderpath, "").Replace(@"\", "").Replace(".story", "");//just get filename
            CreateSaveSlot(filename);
        }
    }
    /// <summary>
    /// Spawn visible save slot in scroll list
    /// *loads existing files*
    /// </summary>
    public void CreateSaveSlot(string filename)
    {
        Button newButton = Instantiate(saveSlot) as Button;
        newButton.transform.SetParent(scrollList.transform, false);
        newButton.GetComponentInChildren<Text>().text = filename;
        newButton.onClick.AddListener(delegate {
            string file = newButton.GetComponentInChildren<Text>().text;
            SaveData data = SaveSystem.LoadData(filename);
            string savedState = data.saveState;
            InkTestingScript.loadedState = savedState;
            InkTestingScript.justLoaded = true;
            SceneManager.LoadScene("Game");
            saveSlotMenu.SetActive(false);
        });
    }

    private void Start()
    {
        PopulateScrollList();
    }


}
