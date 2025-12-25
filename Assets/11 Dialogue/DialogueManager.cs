using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager _instance;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    //[SerializeField] private TextMeshProUGUI nameText;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    private Story currentStory;

    public bool dialogueIsPlaying { get; private set; }

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene!");
        }
        _instance = this;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
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

    private void Update()
    {
        if (!dialogueIsPlaying) return;

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
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (!dialogueIsPlaying) return;

        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();

            DisplayChoices();
        }
        else
        {
            ExitDialogueMode();
        }

    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

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
        Debug.Log("currentChoices: " + currentChoices.Count);
        // Hide remaining choices
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }
    }

    public void MakeChoice(int index)
    {
        currentStory.ChooseChoiceIndex(index);
        ContinueStory();
    }


}
