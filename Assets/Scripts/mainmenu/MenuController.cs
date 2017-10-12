using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    GameObject mainPanel;
    Button start;
    Button settings;
    Button quit;

    GameObject startMenu;
    Button newGame;
    Button loadGame;
    Button back1;

    GameObject settingsMenu;
    Button back2;


    

    // Use this for initialization
    void Start () {
        mainPanel = GameObject.Find("MainPanel");
        start = GameObject.Find ("startButton").GetComponent<Button>();
        settings = GameObject.Find("settingsButton").GetComponent<Button>();
        quit = GameObject.Find("quitButton").GetComponent<Button>();

        startMenu = GameObject.Find ("StartMenu");
        newGame = GameObject.Find ("NewGameBTN").GetComponent<Button>();
        loadGame = GameObject.Find ("LoadGameBTN").GetComponent<Button>();
        back1 = GameObject.Find("BackButton1").GetComponent<Button>();

        settingsMenu = GameObject.Find("SettingsMenu");
        back2 = GameObject.Find("BackButton2").GetComponent<Button>();


        start.onClick.AddListener(Play);
        settings.onClick.AddListener(Settings);
        quit.onClick.AddListener(Quit);

        newGame.onClick.AddListener(NewGame);
        loadGame.onClick.AddListener(LoadGame);
        back1.onClick.AddListener(BackToMain);

        back2.onClick.AddListener(BackToMain);

        startMenu.SetActive(false);
        settingsMenu.SetActive(false);


    }
	
    void Play()
    {
        Debug.Log("Started");
        mainPanel.SetActive(false);
        startMenu.SetActive(true);
    }
    void Settings()
    {
        mainPanel.SetActive(false);
        settingsMenu.SetActive(true);
    }
    void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
    }


    void NewGame()
    {
        Debug.Log("new game");
        SceneManager.LoadScene(1);
    }

    void LoadGame()
    {
        Debug.Log("Tried To load game");
    }

    void BackToMain()
    {
        settingsMenu.SetActive(false);
        startMenu.SetActive(false);
        mainPanel.SetActive(true);
    }
}
