using System;
using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{
  public float Speed_Forward = 1;
  public float Speed_Backward = 1;
  public float Speed_Strafe = 1;
  public float Speed_Rotate = 1;
  
  public bool MovingForward = false;
  public bool MovingBackward = false;
  public bool MovingLeft = false;
  public bool MovingRight = false;
  public bool TurningLeft = false;
  public bool TurningRight = false;

  public bool IsMoving
  {
    get { return MovingForward || MovingBackward || MovingLeft || MovingRight; }
  }
  
  public void Start ()
  {
    _animation = GetComponent<BbSpriteAnimation>();
    _rigidBody = GetComponent<Rigidbody>();
  }
	
	public void FixedUpdate()
  {
	  if (IsMoving)
	  {
	    _animation.Play("walk");
    }
	  else
	  {
      _animation.Play("idle");
    }

	  if (MovingForward)
	  {
	    _rigidBody.AddForce(transform.forward * Speed_Forward);
	  }
    if (MovingBackward)
    {
      _rigidBody.AddForce(-transform.forward * Speed_Backward);
    }
    if (MovingLeft)
    {
      _rigidBody.AddForce(-transform.right * Speed_Strafe);
    }
    if (MovingRight)
    {
      _rigidBody.AddForce(transform.right * Speed_Strafe);
    }
	  if (TurningLeft)
	  {
      _rigidBody.AddTorque(-transform.up * Speed_Rotate);
	  }
    if (TurningRight)
    {
      _rigidBody.AddTorque(transform.up * Speed_Rotate);
    }
  }

  public void OnCollisionEnter(Collision c)
  {
    if (c.collider.tag == "Ground")
    {
      grounded = true;
    }
  }

  private BbSpriteAnimation _animation;
  private Rigidbody _rigidBody;
  private bool grounded = false;
}
