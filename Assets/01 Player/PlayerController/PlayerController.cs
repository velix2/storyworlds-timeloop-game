using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cameraTransform;

    [Header("Parameters")] 
    [SerializeField] private float speed = 10f;

    // Update is called once per frame
    void Update()
    {
        // Freeze player movement during dialogue
        if (DialogueManager.GetInstance().dialogueIsPlaying) return;

        Vector2 input = InputManager.GetPlayerMovement();
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0;
        move = Vector3.ClampMagnitude(move, 1f);
        Vector3 finalMove = move * speed;
        controller.Move(finalMove * Time.deltaTime);
    }
}
