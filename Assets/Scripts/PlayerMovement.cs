using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController player;

    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float gravity = 9.8f;

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

        Quaternion rotation = Rotation();

        Vector3 rotatedDirection = rotation * direction;

        if (Input.GetKey(KeyCode.LeftShift))
            player.Move((rotatedDirection * sprintSpeed) * Time.deltaTime);
        else
            player.Move((rotatedDirection * movementSpeed) * Time.deltaTime);
    }
    Quaternion Rotation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        float distance;

        if (ground.Raycast(ray, out distance))
        {
            Vector3 lookAt = ray.GetPoint(distance);
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(lookAt.x, transform.position.y, lookAt.z) - transform.position);
            transform.LookAt(new Vector3(lookAt.x, transform.position.y, lookAt.z));
            return targetRotation;
        }

        return transform.rotation; 
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
