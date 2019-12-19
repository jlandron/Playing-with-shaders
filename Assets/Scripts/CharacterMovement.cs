using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    public float speed = 6f;
    public float lookSpeed = 6f;
    public float turnSpeed = 20f;
    public float jumpSpeed = 8f;
    public float gravity = 20f;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;
    private Camera camera;
    private int jumps;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        camera = Camera.main;
    }

    
    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * Time.fixedDeltaTime * turnSpeed );
        camera.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), 0, 0) * Time.fixedDeltaTime * lookSpeed);
        if (characterController.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = jumpSpeed;
            }
        }
        else
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), moveDirection.y, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection.x *= speed;
            moveDirection.z *= speed;
        }
        moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.fixedDeltaTime);
    }
}
