using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject nameBox;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject imagePanel;
    [SerializeField] private Image speakerImage;
    [SerializeField] private GameObject continueIcon;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;

    [Header("Player")]
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private Transform playerTransform;

    void Start()
    {
        if(playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if(playerTransform == null)
        {
            Debug.LogError("No player transform found!");
            return;
        }        

        DialogueManager.Instance.RegisterDialogueUI(dialogueUI, dialogueBox, dialogueText, nameBox, nameText, imagePanel, 
            speakerImage, continueIcon, choices, playerPanel, playerText, playerTransform);
        dialogueBox.SetActive(false);
        playerPanel.SetActive(false);
    }
}
