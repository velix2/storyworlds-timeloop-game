using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject namePanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject imagePanel;
    [SerializeField] private Image speakerImage;
    [SerializeField] private GameObject continueIcon;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;

    [Header("PLayer")]
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private Transform playerTransform;

    void Start()
    {
        DialogueManager.Instance.RegisterDialogueUI(dialogueBox, dialogueText, namePanel, nameText, imagePanel, 
            speakerImage, continueIcon, choices, playerPanel, playerText, playerTransform);
        dialogueBox.SetActive(false);
    }
}
