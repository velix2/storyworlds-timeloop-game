
using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    private GameObject dialogueUI;
    private GameObject dialogueBox;
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
    private const string PAUSE_TAG = "pause";


    [SerializeField] private SpeakerManager speakerManager;     // Grants access to all speaker data

    // Variables to process through the story
    private Story currentStory;
    private string currentLine = "";
    private Coroutine typingCoroutine;

    private VariableObserver variableObserver;                  // Allows access to ink global variables

    private bool DialoguePaused = false;

    private bool LineIsFinished = false;

    public static event Action DialogueModeChanged;


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
    }

    private void OnEnable()
    {
        // Subscribe to events
        DialogueInputManager.ContinueDialogue += ContinueStory;
        DialogueInputManager.ContinueDialogueSimple += ContinueStorySimple;
    }

    private void OnDisable()
    {
        DialogueInputManager.ContinueDialogue -= ContinueStory;
        DialogueInputManager.ContinueDialogueSimple -= ContinueStorySimple;
    }

    #region Dialogue Normal
    
    public void EnterDialogueMode(TextAsset inkJson)
    {
        if(!DialogueIsPlaying)
        {
            currentStory = new Story(inkJson.text);
            DialogueIsPlaying = true;
            SimpleDialogueIsPlaying = false;

            dialogueText.text = "";
            nameText.text = "";
            DialogueModeChanged?.Invoke();
            dialogueBox.SetActive(true);

            variableObserver.StartListening(currentStory);
            CutsceneManager.Instance.PauseCutscene();

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

        if (DialoguePaused) return;

        // Continue story if possible
        if (currentStory.canContinue)
        {
            IsTyping = true;
            string line = currentStory.Continue().Trim();
            line = line.Replace("[nl]", "\n");
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

        // Display continue icon when the text finished typing
        IsTyping = false;
        continueIcon.SetActive(true);

        // Display choices if there are any
        if (currentStory.currentChoices.Count > 0)
        {
            DisplayChoices(currentStory.currentChoices);
        }
    }

    private void ExitDialogueMode()
    {
        // Reset flags
        DialogueIsPlaying = false;
        if(!CutsceneManager.Instance.CutsceneIsPlaying)
            DialogueModeChanged?.Invoke();
        IsTyping = false;
        ChoicesDisplayed = false;
        SimpleDialogueIsPlaying = false;

        // Deactivate Panels
        dialogueBox.SetActive(false);
        dialogueText.text = "";

        variableObserver.StopListening(currentStory);

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (CutsceneManager.Instance.CutsceneIsPlaying)
        {
            CutsceneManager.Instance.ContinueCutscene();
        }

        Debug.Log("Dialogue is finished");

    }

    /// <summary>
    /// Displays up to 3 choices
    /// </summary>
    /// <param name="currentChoices">Choices from the ink file</param>
    private void DisplayChoices(List<Choice> currentChoices)
    {
        if (currentChoices.Count > choices.Length)
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

            // Initialize Buttons
            Button button = choices[index].GetComponent<Button>();
            button.onClick.RemoveAllListeners();

            int cachedIndex = index;

            button.onClick.AddListener(() => MakeChoice(cachedIndex));

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

    #region Dialogue Simple

    /// <summary>
    /// Displays dialogue above the player. Does not support choices
    /// </summary>
    /// <param name="dialogueLine">Line to be displayed</param>
    public void EnterDialogueModeSimple(string dialogueLine)
    {
        if (!DialogueIsPlaying)
        {
            Debug.Log("Entering Simple Dialogue Mode");
            DialogueIsPlaying = true;
            DialogueModeChanged?.Invoke();
            SimpleDialogueIsPlaying = true;
            playerText.text = "";
            playerPanel.SetActive(true);

            Vector3 screenPosition = Camera.main.WorldToScreenPoint(playerTransform.position + Vector3.up * 2);
            playerPanel.transform.position = screenPosition;

            currentLine = dialogueLine;

            typingCoroutine = StartCoroutine(DisplayLineSimple(currentLine));


        }

    }
    private void ContinueStorySimple()
    {
        if(LineIsFinished)
        {
            ExitDialogueModeSimple();
        }

        if (IsTyping)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            IsTyping = false;
            playerText.text = currentLine;
            LineIsFinished = true;
            return;
        }   

    }

    

    private IEnumerator DisplayLineSimple(string line)
    {
        playerText.text = "";
        IsTyping = true;

        foreach (char letter in line.ToCharArray())
        {
            playerText.text += letter;
            yield return new WaitForSeconds(typingSpeed);

        }

        IsTyping = false;
        LineIsFinished = true;

    }

    private void ExitDialogueModeSimple()
    {
        // Reset flags
        DialogueIsPlaying = false;
        DialogueModeChanged?.Invoke();
        IsTyping = false;
        SimpleDialogueIsPlaying = false;
        LineIsFinished = false;

        // Deactivate Panels
        playerPanel.SetActive(false);
        playerText.text = "";

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        Debug.Log("Dialogue is finished");
    }


    #endregion

    #region Tag handling
    private void HandleTags(List<string> currentTags)
    {
        string speakerID = "";
        DeactivateSpeakerPanels();

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
                        DeactivateSpeakerPanels();
                    }
                    else
                    {
                        speakerID = tagValue;
                        SetSpeakerName(speakerID, nameText);
                        nameBox.SetActive(true);
                    }
                    break;
                case EMOTION_TAG:
                    if (Enum.TryParse(tagValue, out Emotion emotion))
                    {
                        imagePanel.SetActive(true);
                        SetSpeakerEmotion(speakerID, emotion, speakerImage);
                    }
                    else
                    {
                        Debug.LogWarning("Invalid emotion tag for speaker " + speakerID + " was parsed!");
                    }
                    break;
                case PAUSE_TAG:
                    PauseDialogue();
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
            //speakerNameText.color = speaker.color;
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

   

    

    private void DeactivateSpeakerPanels()
    {
        nameBox.SetActive(false);
        imagePanel.SetActive(false);
    }

    private void PauseDialogue()
    {
        dialogueBox.SetActive(false);
        CutsceneManager.Instance.ContinueCutscene();
        DialoguePaused = true;
    }

    public void ResumeDialogueInCutscene()
    {
        dialogueBox.SetActive(true);
        DialoguePaused = false;
        ContinueStory();

        CutsceneManager.Instance.PauseCutscene();
        
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

    public void RegisterDialogueUI(GameObject dialogueUI, GameObject dialogueBox, TextMeshProUGUI dialogueText, GameObject nameBox,
        TextMeshProUGUI nameText, GameObject imagePanel, Image speakerImage, GameObject continueIcon, GameObject[] choices, 
        GameObject playerPanel, TextMeshProUGUI playerText, Transform playerTransform )
    {
        this.dialogueUI = dialogueUI;
        this.dialogueBox = dialogueBox;
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
