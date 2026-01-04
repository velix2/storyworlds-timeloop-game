
using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This script manages the dialoguebox
/// It reads the ink files, converts them into a story object and allows for players to make choices
/// The typing effect is also handled in DialogueManager
/// The most important functions across the scripts are EnterDialogueMode and EnterDialogueModeSimple (for text over the player)
/// </summary>
public class DialogueManager : MonoBehaviour
{
    #region Variables
    public static DialogueManager Instance { get; private set; }

    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("Load Globals Json")]
    [SerializeField] private TextAsset loadGlobalsJSON;

    private GameObject dialogueUI;
    private TextMeshProUGUI dialogueText;
    private GameObject nameBox;
    private TextMeshProUGUI nameText;
    private GameObject imagePanel;
    private Image speakerImage;
    private GameObject continueIcon;
    private GameObject[] choices;

    private TextMeshProUGUI[] choicesText;


    private GameObject playerPanel;
    private TextMeshProUGUI playerText;
    private Transform playerTransform;


    public bool DialogueIsPlaying { get; private set; }         // Flag to signal if the dialogue is playing
                                                                // is accessed to freeze player movement/turn off other interactions    
    public bool ChoicesDisplayed {  get; private set; }         // Flag to signal if choice box is active
                                                                // Player should only be able to continue if they have made a choice
    public bool IsTyping { get; private set; }                  // Flag to signal if a dialogue line is still typing
                                                                // Important for skipping a dialogue line
    public bool SimpleDialogueIsPlaying { get; private set; }   // Flag to distinguish between simple and normal dialogue

    // Ink Tags (To add more adjust the tag handling function
    private const string SPEAKER_TAG = "speaker";
    private const string EMOTION_TAG = "emotion";
    //private const string AUDIO_TAG = "audio";


    [SerializeField] private SpeakerManager speakerManager;     // Grants access to all speaker data

    // Variables to process through the story
    private Story currentStory;
    private string currentLine = "";
    private Coroutine typingCoroutine;

    private VariableObserver variableObserver;                  // Allows access to ink global variables


    #endregion


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        variableObserver = new VariableObserver(loadGlobalsJSON);

        DialogueIsPlaying = false;
        IsTyping = false;
        ChoicesDisplayed = false;
        SimpleDialogueIsPlaying = false;
        
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        // Move this to start because it depends on SpeakerManager's Awake()
        if (SpeakerManager.Instance == null)
        {
            Debug.LogError("DialogManager requires a SpeakerManager in the scene, but none was found.");
            Destroy(gameObject);
            Instance = null;
            return;
        }

        // Subscribe to events
        InputManager.ContinueDialogue += ContinueStory;
        InputManager.ContinueDialogueSimple += ContinueStorySimple;
    }

    private void OnDisable()
    {
        InputManager.ContinueDialogue -= ContinueStory;
        InputManager.ContinueDialogueSimple -= ContinueStorySimple;
    }

    #region Dialogue Calling Functions
    
    public void EnterDialogueMode(TextAsset inkJson)
    {
        if(!DialogueIsPlaying)
        {
            currentStory = new Story(inkJson.text);
            DialogueIsPlaying = true;
            SimpleDialogueIsPlaying = false;
            dialogueUI.SetActive(true);

            variableObserver.StartListening(currentStory);

            ContinueStory();
        }
        
    }

    /// <summary>
    /// Displays dialogue above the player. Does not support choices
    /// </summary>
    /// <param name="inkJson">Ink json to be displayed</param>
    public void EnterDialogueModeSimple(TextAsset inkJson)
    {
        if (!DialogueIsPlaying)
        {
            currentStory = new Story(inkJson.text);
            DialogueIsPlaying = true;
            SimpleDialogueIsPlaying = true;
            playerPanel.SetActive(true);

            variableObserver.StartListening(currentStory);

            Vector3 screenPosition = Camera.main.WorldToScreenPoint(playerTransform.position + Vector3.up * 2);
            playerPanel.transform.position = screenPosition;

            ContinueStorySimple();
        }

    }

    #endregion

    #region Helper functions for processing the story
    private void ContinueStory()
    {
        if (ChoicesDisplayed) return;
        
        // Safety measure so that only one coroutine is running
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        // Skip Effect
        if (IsTyping)
        {
            IsTyping = false;
            dialogueText.text = currentLine;
            continueIcon.SetActive(true);
            if (currentStory.currentChoices.Count > 0)
            {
                DisplayChoices(currentStory.currentChoices);
            }
            return;
        }

        // Continue story if possible
        if (currentStory.canContinue)
        {
            IsTyping = true;
            string line = currentStory.Continue().Trim();
            typingCoroutine = StartCoroutine(DisplayLine(line, dialogueText));
            HandleTags(currentStory.currentTags);
            return;
        }
        else
        {
            ExitDialogueMode();
        }
        
    }
    private void ContinueStorySimple()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (IsTyping)
        {
            IsTyping = false;
            playerText.text = currentLine;
            return;
        }

        if (currentStory.canContinue)
        {
            IsTyping = true;
            string line = currentStory.Continue().Trim();
            typingCoroutine = StartCoroutine(DisplayLine(line, playerText));
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private IEnumerator DisplayLine(string line, TextMeshProUGUI textObject)
    {
        textObject.text = "";
        currentLine = line;

        continueIcon.SetActive(false);

        foreach (char letter in line.ToCharArray())
        {
            textObject.text += letter;
            yield return new WaitForSeconds(typingSpeed);

        }

        // Display continue icon when the text finished typing
        IsTyping = false;
        continueIcon.SetActive(true);

        // Display choices if there are any
        if (currentStory.currentChoices.Count > 0)
        {
            DisplayChoices(currentStory.currentChoices);
        }
    }

    /// <summary>
    /// Displays up to 3 choices
    /// </summary>
    /// <param name="currentChoices">Choices from the ink file</param>
    private void DisplayChoices(List<Choice> currentChoices)
    {
        if(currentChoices.Count > choices.Length)
        {
            Debug.LogError("It can be only displayed up to 3 choices.");
            return;
        }

        ChoicesDisplayed = true;
        int index = 0;
        // Show choice buttons
        foreach (Choice choice in currentChoices)
        {
            choices[index].SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        // Hide remaining choices
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].SetActive(false);
        }
    }

    private void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
        }
        ChoicesDisplayed = false;
    }
    #endregion

    #region Tag handling
    private void HandleTags(List<string> currentTags)
    {
        string speakerID = "";
        // Loop through each tag and handle it accordingly
        foreach (string tag in currentTags)
        {
            // Parse tag
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be appropriately parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            // Handle tag
            switch (tagKey)
            {
                case SPEAKER_TAG:
                    if (tagValue == "narrator")
                    {
                        nameText.text = "";
                        DeactivatePortraitPanels();
                    }
                    else
                    {
                        ActivatePortraitPanels();
                        speakerID = tagValue;
                        SetSpeakerName(speakerID, nameText);
                    }
                    break;
                case EMOTION_TAG:
                    if (Enum.TryParse(tagValue, out Emotion emotion))
                    {
                        ActivatePortraitPanels();
                        SetSpeakerEmotion(speakerID, emotion, speakerImage);
                    }
                    else
                    {
                        Debug.LogError("Invalid emotion tag was parsed!");
                    }
                    break;
                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }
    /// <summary>
    /// Function to set the speaker's name on the name box
    /// </summary>
    /// <param name="speakerID">ID of the current speaker</param>
    /// <param name="speakerNameText">UI element to be set</param>
    private void SetSpeakerName(string speakerID, TextMeshProUGUI speakerNameText)
    {
        Dictionary<string, SpeakerData> speakerDictionary = speakerManager.speakerDictionary;
        if (speakerDictionary.ContainsKey(speakerID))
        {
            SpeakerData speaker = speakerDictionary[speakerID];
            speakerNameText.text = speaker.speakerName;
            speakerNameText.color = speaker.color;
        }
        else
        {
            Debug.LogError("Speaker with ID " + speakerID + " could not be found!");
        }
    }

    /// <summary>
    /// Function to set the portrait of the speaker according to the emotion.
    /// If the image with given emotion cannot be found, the first image in the list is taken
    /// </summary>
    /// <param name="speakerID">ID of current speaker</param>
    /// <param name="emotion">Emotion to be set</param>
    /// <param name="portraitImage">UI element to be set</param>
    private void SetSpeakerEmotion(string speakerID, Emotion emotion, Image portraitImage)
    {
        Dictionary<string, SpeakerData> speakerDictionary = speakerManager.speakerDictionary;
        if (speakerDictionary.ContainsKey(speakerID))
        {
            SpeakerData speaker = speakerDictionary[speakerID];
            Sprite portrait = speakerManager.GetSpeakerPortraitByEmotion(speakerID, emotion);
            portraitImage.sprite = portrait;
        }
        else
        {
            Debug.LogError("Speaker with ID " + speakerID + " could not be found!");
        }
    }
    #endregion

    #region Button functions
    public void MakeChoice(int index)
    {
        currentStory.ChooseChoiceIndex(index);
        HideChoices();
        ContinueStory();
    }

    #endregion

    private void ExitDialogueMode()
    {
        // Reset flags
        DialogueIsPlaying = false;
        IsTyping = false;
        ChoicesDisplayed = false;
        SimpleDialogueIsPlaying = false;

        // Deactivate Panels
        dialogueUI.SetActive(false);
        playerPanel.SetActive(false);
        playerText.text = "";
        dialogueText.text = "";

        variableObserver.StopListening(currentStory);

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

    }

    private void DeactivatePortraitPanels()
    {
        nameBox.SetActive(false);
        imagePanel.SetActive(false);
    }

    private void ActivatePortraitPanels()
    {
        nameBox.SetActive(true);
        imagePanel.SetActive(true);
    }

    /// <summary>
    /// Function to get a variable state from the globals dictionary
    /// Use to access the dialogue's variable 
    /// </summary>
    /// <param name="variableName"></param>
    /// <returns></returns>
    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object variable = null;
        variableObserver.variables.TryGetValue(variableName, out variable);
        if(variable == null)
        {
            Debug.LogWarning("Variable with name " +  variableName + " could not be found!");
        }
        return variable;
    }

    public void RegisterDialogueUI(GameObject dialogueUI, TextMeshProUGUI dialogueText, GameObject nameBox,
        TextMeshProUGUI nameText, GameObject imagePanel, Image speakerImage, GameObject continueIcon, GameObject[] choices, 
        GameObject playerPanel, TextMeshProUGUI playerText, Transform playerTransform )
    {
        this.dialogueUI = dialogueUI;
        this.nameBox = nameBox;
        this.nameText = nameText;
        this.imagePanel = imagePanel;
        this.speakerImage = speakerImage;
        this.continueIcon = continueIcon;
        this.choices = choices;
        this.playerPanel = playerPanel;
        this.playerText = playerText;
        this.playerTransform = playerTransform;
        this.dialogueText = dialogueText;

        InitializeChoices(choices);

        Debug.Log("DialogueBox initiialized");
    }

    private void InitializeChoices(GameObject[] choices)
    {
        HideChoices();

        // Get all choices text components for easier access
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>(true);
            index++;
        }
    }

}
