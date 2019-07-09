using UnityEngine;
using System.Collections;

public class CamRotation : MonoBehaviour
{
  public float MouseSpeedHorizontal = 250;
  public float MouseSpeedVertical = 250;

	public void Start ()
	{
	  _movement = transform.parent.GetComponent<CharacterMovement>();
	  _ownerTransform = transform.parent;
	  _previousRotation = transform.eulerAngles.y;
	}

  public void Update()
  {
    var rotation = transform.eulerAngles;
//    var rotationDiff = rotation.y - _previousRotation;
//    _previousRotation = rotation.y;
//    rotation.y += rotationDiff;

    // UP/Down
    if (_movement.IsMoving)
    {
      rotation.x = Mathf.Lerp(0, rotation.x, Mathf.Max(1, rotation.x));
    }
    if (Input.GetMouseButton(1))
    {
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
      rotation.x +=
          Input.GetAxis("Mouse Y") * MouseSpeedVertical * Time.deltaTime;
      if (rotation.x > 30 && rotation.x < 180) rotation.x = 30;
      if (rotation.x < 300 && rotation.x > 180) rotation.x = 300;
      //rotation.x = Mathf.Clamp(rotation.x, 0, 100);

//      rotation.y +=
//          Input.GetAxis("Mouse X") * MouseSpeedHorizontal * Time.deltaTime;
    }
    else
    {
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
    }

//    if(rotation.y > 0)
//    {
//      _movement.TurningLeft = true;
//      _movement.TurningRight = false;
//    }
//    else if (rotation.y < 0)
//    {
//      _movement.TurningLeft = false;
//      _movement.TurningRight = true;
//    }
//    else
//    {
//      _movement.TurningLeft = false;
//      _movement.TurningRight = false;
//    }

    transform.eulerAngles = rotation;
	}

  private CharacterMovement _movement;
  private Transform _ownerTransform;
  private float _previousRotation;
}
