using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
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
    private static DialogueManager _instance;

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

    public bool dialoguePanelActivated { get; private set; }
    public bool isChoice {  get; private set; }
    public bool isTyping { get; private set; }

    // Ink Tags
    private const string SPEAKER_TAG = "speaker";
    private const string EMOTION_TAG = "emotion";
    //private const string AUDIO_TAG = "audio";

    [SerializeField] private SpeakerManager speakerManager;

    private Story currentStory;
    private string currentLine = "";
    private Coroutine displayLineCoroutine;
    private bool canContinueToNextLine = true;

    private VariableObserver variableObserver;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene!");
        }
        _instance = this;
        variableObserver = new VariableObserver(loadGlobalsJSON);
    }

    private void Start()
    {
        dialoguePanelActivated = false;
        dialoguePanel.SetActive(false);
        playerPanel.SetActive(false);

        isTyping = false;
        isChoice = false;
        canContinueToNextLine = true;
        dialoguePanelActivated = false;

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

    public static DialogueManager GetInstance()
    {
        return _instance;
    }

    public void EnterDialogueMode(TextAsset inkJson)
    {
        currentStory = new Story(inkJson.text);
        dialoguePanelActivated = true;
        dialoguePanel.SetActive(true);

        variableObserver.StartListening(currentStory);

        ContinueStory();
    }

    public void EnterDialogueModeSimple(TextAsset inkJson)
    {
        currentStory = new Story(inkJson.text);
        dialoguePanelActivated = true;
        playerPanel.SetActive(true);

        variableObserver.StartListening(currentStory);

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(playerTransform.position + Vector3.up * 2);
        playerPanel.transform.position = screenPosition;

        ContinueStorySimple();
    }


    private void ExitDialogueMode()
    {
        isTyping = false;
        dialoguePanelActivated = false;
        dialoguePanel.SetActive(false);
        playerPanel.SetActive(false);
        playerText.text = "";
        dialogueText.text = "";

        variableObserver.StopListening(currentStory);

        if (displayLineCoroutine != null)
        {
            StopCoroutine(displayLineCoroutine);
            displayLineCoroutine = null;
        }
    }

    private void ContinueStorySimple()
    {
        if (isTyping || isChoice) return;

        if (currentStory.canContinue)
        {
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
                displayLineCoroutine = null;
            }
            isTyping = true;
            StartCoroutine(DisplayLine(currentStory.Continue(), playerText));
        }
        else
        {
            ExitDialogueMode();
        }
    }
    private void ContinueStory()
    {
        if (isTyping)
        {
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
                displayLineCoroutine = null;
            }
            dialogueText.text = currentLine;
            isTyping = false;
            canContinueToNextLine = true;
            continueIcon.SetActive(true);
            if (!isChoice)
            {
                DisplayChoices();
            }
        }
        else if (!isChoice && canContinueToNextLine)
        {
            if (currentStory.canContinue)
            {
                isTyping = true;
                if (displayLineCoroutine != null)
                {
                    StopCoroutine(displayLineCoroutine);
                    displayLineCoroutine = null;
                }
                displayLineCoroutine = StartCoroutine(DisplayLine(currentStory.Continue(), dialogueText));

                HandleTags(currentStory.currentTags);
            }
            else
            {
                ExitDialogueMode();
            }
        }


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
                    if(tagValue == "narrator")
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
                    if(Enum.TryParse(tagValue, out Emotion emotion))
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
        Dictionary<string, SpeakerData> speakerDictionary = speakerManager.GetSpeakerDirectionary();
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
        Dictionary<string, SpeakerData> speakerDictionary = speakerManager.GetSpeakerDirectionary();
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



    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > 0)
        {
            isChoice = true;
        }

        // UI can support up to 3 choices
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can support. Number of choices given: "
                + currentChoices.Count);
        }

        int index = 0;
        // Initialize choices
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        // Hide remaining choices
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }
    }

    private void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
        }
    }

    private IEnumerator DisplayLine(string line, TextMeshProUGUI textObject)
    {
        currentLine = line;
        textObject.text = "";

        HideChoices();
        canContinueToNextLine = false;
        continueIcon.SetActive(false);

        // Display each letter one at a time
        foreach (char letter in line.ToCharArray())
        {
            textObject.text += letter;
            yield return new WaitForSeconds(typingSpeed);

        }
        isTyping = false;
        canContinueToNextLine = true;
        continueIcon.SetActive(true);
        if (!isChoice)
        {
            DisplayChoices();
        }

    }

    public void MakeChoice(int index)
    {
        currentStory.ChooseChoiceIndex(index);
        isChoice = false;
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

    
}
