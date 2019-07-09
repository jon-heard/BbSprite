using UnityEngine;
using System.Collections;

public class PcInput : MonoBehaviour
{

  private CharacterMovement _movement;

	void Start ()
	{
	  _movement = GetComponent<CharacterMovement>();
	}
	
	void Update ()
	{
    _movement.MovingForward = Input.GetKey(KeyCode.W);
    _movement.MovingBackward = Input.GetKey(KeyCode.S);

    _movement.MovingLeft = false;
    _movement.MovingRight = false;
    if (Input.GetMouseButton(1))
	  {
      _movement.MovingLeft = Input.GetKey(KeyCode.A);
      _movement.MovingRight = Input.GetKey(KeyCode.D);
    }
	  else
	  {
      _movement.TurningLeft = Input.GetKey(KeyCode.A);
      _movement.TurningRight = Input.GetKey(KeyCode.D);
    }
	}
}
