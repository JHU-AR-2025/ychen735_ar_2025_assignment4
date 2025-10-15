using  UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float speed = 0f;
    public float jumpForce = 0f;
    public bool isGrounded;


    private InputAction moveAction;
    private InputAction jumpAction;
    private Rigidbody rb;

    private int collectibleCount = 0;
    public TextMeshProUGUI collectibleText;
    public GameObject powerUp;
    public TextMeshProUGUI powerUpText;
    public TextMeshProUGUI winText; 

    public Button startButton;
    private bool canMove = false;
    private bool powerUpActive = false;
    private float originalSpeed;
    public Material normalMaterial;
    public Material powerUpMaterial; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        rb = GetComponent<Rigidbody>();

        jumpAction.performed += ctx => TryJump();
        jumpAction.Enable();

        rb.useGravity = false;
        canMove = false;

        startButton.onClick.AddListener(StartGame);
        UpdateCollectibleText();

        originalSpeed = speed;
        powerUp.SetActive(false);
        powerUpText.gameObject.SetActive(false);
        winText.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Vector3 movement = new Vector3(moveValue.x, 0f, moveValue.y);
        rb.AddForce(movement * speed);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;

        if (collision.gameObject.CompareTag("BreakableWall") && powerUpActive)
        {
            Destroy(collision.gameObject);
            Debug.Log("Breakable Wall Destroyed!");
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }


    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    
    void TryJump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            other.gameObject.SetActive(false);
            collectibleCount++;
            Debug.Log("Collision with " + other.gameObject.name);
            UpdateCollectibleText();

            if (collectibleCount == 8 && !powerUpActive)
            {
                powerUp.SetActive(true);
                Debug.Log("Power-Up Appears!");
            }
        }

        if (other.CompareTag("PowerUp") && !powerUpActive)
        {
            ActivatePowerUp();
            other.gameObject.SetActive(false);
        }

        if (other.CompareTag("GoalZone") && collectibleCount == 8 && powerUpActive)
        {
            WinGame();
        }
    }

    private void UpdateCollectibleText()
    {
        if (collectibleText != null)
            collectibleText.text = "Collected: " + collectibleCount + "/8";
    }

    private void StartGame()
    {
        rb.useGravity = true;
        canMove = true;
        startButton.gameObject.SetActive(false);
        Debug.Log("Game Started!");
    }

    private void ActivatePowerUp()
    {
        if (powerUpActive) return;

        powerUpActive = true;
        speed = originalSpeed * 4f;
        powerUpText.text = "Power-Up: ACTIVE!";
        powerUpText.gameObject.SetActive(true);
        Debug.Log("Power-Up Activated!");

        var renderer = GetComponent<Renderer>();
        if (renderer != null && powerUpMaterial != null)
        {
            renderer.material = powerUpMaterial;
        }
    }

    private void WinGame()
    {
        Debug.Log("You Win!");
        winText.gameObject.SetActive(true);

        canMove = false;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        GetComponent<Renderer>().enabled = false;
        this.enabled = false;
    }
}
