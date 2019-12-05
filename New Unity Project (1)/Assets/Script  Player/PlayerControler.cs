using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    [Header("Speed Settings")]
    public float walkSpeed;
    public float runSpeed;
    private float speed;
    [Header("Jump Settings")]
    public float jumpForce;
    public ForceMode forceType;

    bool isFalling;
    [Header("Animatios")]
    public Animator anim;//Animator
    public float transitionTime;//Time to move to new pose
    float curLightAttack; // Current Attack Phase


    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent < Rigidbody>();
    }

    private void Update()
    {
        #region Inputs
        //Sprint
        if (Input.GetButton("Sprint"))
            speed = runSpeed;
        else if (speed != walkSpeed)
            speed = walkSpeed;
        
        
        //Jump
        if (Input.GetButton("Jump") && !isFalling)
            Jump(jumpForce, forceType);

        #endregion

        #region Animations
        if (speed == runSpeed)
        {
            AxisAnimationUpdate("Horizontal", "Direction", 1);
            AxisAnimationUpdate("Vertical", "Speed", 1);
        }
        else
        {
            AxisAnimationUpdate("Horizontal", "Direction", 0.5f);
            AxisAnimationUpdate("Vertical", "Speed", 0.5f);
        }

        anim.SetBool("isFalling", isFalling);
        
        if (!anim.GetCurrentAnimatorStateInfo(1).IsName("Default"))
        {
            FindObjectOfType<CameraController>().overideCamera = true;
        }
        else
        {
            FindObjectOfType<CameraController>().overideCamera = false;
        }

      if (Input.GetButtonDown("Fire1")&& anim.GetCurrentAnimatorStateInfo(1).IsName("Default"))
        {
           
             Attacking();
           
        }
        #endregion
    }


    #region

    private void Attacking()
    {
        anim.SetTrigger("Attack");
    }

    #endregion

    private void FixedUpdate()
    {

        #region Motion
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");
        rb.MovePosition(transform.position + transform.TransformDirection(x, 0, z) * Time.deltaTime * speed);
        #endregion
    }

    #region Completed Function

    void Jump(float force,ForceMode type)
    {
        isFalling = true;
        rb.velocity = Vector3.zero;
        rb.AddForce(transform.up * force, type);
    }

    void AxisAnimationUpdate(string AxisName,string motionName, float motionSpeed)
    {
        if (Input.GetAxis(AxisName) > 0)
            anim.SetFloat(motionName, Mathf.Lerp(anim.GetFloat(motionName), motionSpeed, transitionTime * Time.deltaTime));
        else if (Input.GetAxis(AxisName) < 0)
            anim.SetFloat(motionName, Mathf.Lerp(anim.GetFloat(motionName), -motionSpeed, transitionTime * Time.deltaTime));
        else
            anim.SetFloat(motionName, Mathf.Lerp(anim.GetFloat(motionName), 0, transitionTime * Time.deltaTime));




    }


    #endregion

    #region Collisions And Triggers
    private void OnTriggerStay(Collider other)
    {
        
        if (isFalling && other.tag!="Player")
            isFalling = false;
    }
    #endregion

}