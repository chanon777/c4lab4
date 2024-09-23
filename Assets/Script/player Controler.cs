using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharMovement1 : MonoBehaviour
{
    
    public float speed = 10.0f;          
    public float jumpForce = 8.0f;       
    public float gravity = 20.0f;         
    public float rotationSpeed = 100.0f;  

   
    public bool isGrounded = false;       
    public bool isDef = false;            
    public bool isDancing = false;         
    public bool isWalking = false;         

    private Animator animator;             
    private CharacterController characterController; 
    private Vector3 inputVector = Vector3.zero; 
    private Vector3 targetDirection = Vector3.zero;
    private Vector3 moveDirection = Vector3.zero; 
    private Vector3 velocity = Vector3.zero;       

    void Awake()
    {
        // เริ่มต้นคอมโพเนนต์
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        Time.timeScale = 1; // ตั้งค่าสเกลเวลาในเกม
        isGrounded = characterController.isGrounded; // ตรวจสอบว่าตัวละครอยู่บนพื้น
    }

    void Update()
    {
        // รับค่าการเคลื่อนที่จากผู้เล่น
        float z = Input.GetAxis("Horizontal");
        float x = -(Input.GetAxis("Vertical"));

        // อัปเดตพารามิเตอร์อนิเมชัน
        animator.SetFloat("inputX", -(x));
        animator.SetFloat("InputZ", z);

        // ตรวจสอบว่าตัวละครกำลังเดินหรือไม่
        if (x != 0 || z != 0)
        {
            isWalking = true;
            animator.SetBool("isWalking", isWalking);
        }
        else
        {
            isWalking = false;
            animator.SetBool("isWalking", isWalking);
        }

        // ตรวจสอบว่าตัวละครอยู่บนพื้น
        isGrounded = characterController.isGrounded;
        if (isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection *= speed;

            //เปิดใช้งานการกระโดดหากผู้เล่นกดปุ่มกระโดด (ยังไม่เปิดใช้งาน)
            if (Input.GetButton("Jump"))
             {
                moveDirection.y = jumpForce;
             }
        }

     
        moveDirection.y -= gravity * Time.deltaTime;

        
        characterController.Move(moveDirection * Time.deltaTime);

 
        inputVector = new Vector3(x, 0, z);
        updateMovement();
    }

    void updateMovement()
    {
        Vector3 motion = inputVector;
     
        motion = ((Mathf.Abs(motion.x) > 1) || (Mathf.Abs(motion.z) > 1)) ? motion.normalized : motion;

     
        rotatTowardMovement();
        viewRelativeMovement();
    }

    void rotatTowardMovement()
    {
      
        if (inputVector != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void viewRelativeMovement()
    {
       
        Transform cameraTransform = Camera.main.transform;
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0.0f;
        forward = forward.normalized;
        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
        targetDirection = (Input.GetAxis("Horizontal") * right) + (Input.GetAxis("Vertical") * forward);
    }
}