using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController player;

    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private GameObject groundTrigger;
    private bool isGrounded = false;

    void Start()
    {
        player = GetComponent<CharacterController>();
    }

    void Update()
    {
        Move();
        Gravity();
    }

    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        Quaternion rotation = Aim();

        Vector3 rotatedDirection = rotation * direction;

        if (Input.GetKey(KeyCode.LeftShift))
            player.Move((rotatedDirection * sprintSpeed) * Time.deltaTime);
        else
            player.Move((rotatedDirection * movementSpeed) * Time.deltaTime);
    }
    private Quaternion Aim()
    {
        var (success, position) = GetMousePosition();
        if (success)
        {
            // Calculate the direction
            var direction = position - transform.position;

            // Ignore the height difference.
            direction.y = 0;

            // Make the transform look in the direction.
            transform.forward = direction;

            // Return the rotation
            return Quaternion.LookRotation(direction);
        }

        // Return the current rotation if no aim direction is found
        return transform.rotation;
    }

    public (bool success, Vector3 position) GetMousePosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask.value))
            return (success: true, position: hitInfo.point);
        else
            return (success: false, position: Vector3.zero);
    }


    void Gravity()
    {
        if (!isGrounded)
            player.Move((Vector3.down * gravity) * Time.deltaTime);
        else player.Move(Vector3.zero);
        
    }

    void OnTriggerEnter(Collider other)
    {   
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            groundTrigger.SetActive(false);
        } else {
            isGrounded = false;
            groundTrigger.SetActive(true);
        }
    }
}
