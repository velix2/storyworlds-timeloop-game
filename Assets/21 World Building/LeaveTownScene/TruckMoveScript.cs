using UnityEngine;

public class TruckMoveScript : MonoBehaviour
{
    [SerializeField] private float xOffsetToReset;
    [SerializeField] private float moveSpeed;
    

    private void Update()
    {
        transform.position += Vector3.right * (moveSpeed * Time.deltaTime);
        
        if (!(transform.position.x > xOffsetToReset)) return;
        var pos = transform.position;
        pos.x -= 200f;
        transform.position = pos;
    }
}
