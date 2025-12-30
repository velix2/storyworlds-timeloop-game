
using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
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

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject namePanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject imagePanel;
    [SerializeField] private Image speakerImage;
    [SerializeField] private GameObject continueIcon;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;

    private TextMeshProUGUI[] choicesText;

    [Header("Player")]
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private Transform playerTransform;


    public bool DialoguePanelActivated { get; private set; }    // Flag to signal if the dialogue is playing
                                                                // is accessed to freeze player movement/turn off other interactions    
    public bool ChoicesDisplayed {  get; private set; }         // Flag to signal if choice box is active
                                                                // Player should only be able to continue if they have made a choice
    public bool IsTyping { get; private set; }                  // Flag to signal if a dialogue line is still typing
                                                                // Important for skipping a dialogue line

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

        if (SpeakerManager.Instance == null)
        {
            Debug.LogError("DialogManager requires a SpeakerManager in the scene, but none was found.");
            enabled = false;
        }

        variableObserver = new VariableObserver(loadGlobalsJSON);
    }

    private void Start()
    {
        DialoguePanelActivated = false;
        dialoguePanel.SetActive(false);
        playerPanel.SetActive(false);

        IsTyping = false;
        ChoicesDisplayed = false;
        DialoguePanelActivated = false;

        HideChoices();

        InputManager.ContinueDialogue += ContinueStory;

        // Get all choices text
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach(GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>(true);
            index++;
        }
    }

    private void OnDisable()
    {
        InputManager.ContinueDialogue -= ContinueStory;
    }


    public void EnterDialogueMode(TextAsset inkJson)
    {
        if(!DialoguePanelActivated)
        {
            currentStory = new Story(inkJson.text);
            DialoguePanelActivated = true;
            dialoguePanel.SetActive(true);

            variableObserver.StartListening(currentStory);

            ContinueStory();
        }
        
    }

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
        typingCoroutine = null;
        IsTyping = false;
        continueIcon.SetActive(true);

        if (currentStory.currentChoices.Count > 0)
        {
            DisplayChoices(currentStory.currentChoices);
        }
    }

    private void DisplayChoices(List<Choice> currentChoices)
    {
        ChoicesDisplayed = true;
        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            choices[index].SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].SetActive(false);
        }
    }

    public void EnterDialogueModeSimple(TextAsset inkJson)
    {
        if(DialoguePanelActivated)
        {
            currentStory = new Story(inkJson.text);
            DialoguePanelActivated = true;
            playerPanel.SetActive(true);

            variableObserver.StartListening(currentStory);

            Vector3 screenPosition = Camera.main.WorldToScreenPoint(playerTransform.position + Vector3.up * 2);
            playerPanel.transform.position = screenPosition;

            ContinueStorySimple();
        }
        
    }


    private void ExitDialogueMode()
    {
        // Reset flags
        DialoguePanelActivated = false;
        IsTyping = false;

        // Deactivate Panels
        dialoguePanel.SetActive(false);
        playerPanel.SetActive(false);
        playerText.text = "";
        dialogueText.text = "";

        variableObserver.StopListening(currentStory);

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        Debug.Log("Exit dialogue");
    }

    private void ContinueStorySimple()
    {
        if (IsTyping || ChoicesDisplayed) return;

        if (currentStory.canContinue)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            IsTyping = true;
            StartCoroutine(DisplayLine(currentStory.Continue(), playerText));
        }
        else
        {
            ExitDialogueMode();
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

    public void MakeChoice(int index)
    {
        currentStory.ChooseChoiceIndex(index);
        HideChoices();
        ContinueStory();
    }

    private void DeactivatePanels()
    {
        namePanel.SetActive(false);
        imagePanel.SetActive(false);
    }

    private void ActivatePanels()
    {
        namePanel.SetActive(true);
        imagePanel.SetActive(true);
    }

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
                        DeactivatePanels();
                    }
                    else
                    {
                        ActivatePanels();
                        speakerID = tagValue;
                        SetSpeakerName(speakerID, nameText);
                    }
                    break;
                case EMOTION_TAG:
                    if (Enum.TryParse(tagValue, out Emotion emotion))
                    {
                        ActivatePanels();
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



}
