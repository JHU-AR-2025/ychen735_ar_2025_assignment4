using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject playerGameObject; // Reference to the player object

    private Vector3 offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = transform.position - playerGameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerGameObject.transform.position + offset;
    }
}
