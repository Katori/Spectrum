using UnityEngine;

namespace ExampleGame.Sources
{
	//[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(PlayerMotor))]
	public class PlayerController : MonoBehaviour
	{

		[SerializeField]
		private float Speed = 5.0f;

		[SerializeField]
		private float XLookSensisitivity = 3.0f;

		[SerializeField]
		private float YLookSensisitivity = 3.0f;

		//[SerializeField]
		//private float ThrusterForce = 1000f;

		//[SerializeField]
		//private float ThrusterFuelBurnSpeed = 1f;

		//[SerializeField]
		//private float ThrusterFuelRegenSpeed = 0.3f;

		//private float ThrusterFuelAmount = 1f;

		//public float GetThrusterFuelAmount()
		//{
		//	return ThrusterFuelAmount;
		//}

		[SerializeField]
		LayerMask EnvironmentMask;

		[Header("Spring Settings")]
		[SerializeField]
		private float JointSpring;
		[SerializeField]
		private float JointMaxForce = 40f;

		// component caching
		private PlayerMotor Motor;
		private ConfigurableJoint Joint;
		//private Animator animator;

		private float XMovementBuffer;
		private float ZMovementBuffer;

		private Vector3 MovementHorizontal;
		private Vector3 MovementVertical;

		private Vector3 Velocity;

		[SerializeField]
		private Vector3 JumpForce;

		// Use this for initialization
		void Start()
		{
			Motor = GetComponent<PlayerMotor>();
			Joint = GetComponent<ConfigurableJoint>();
		}

		private void Awake()
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void OnEnable()
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void OnDisable()
		{
			Cursor.lockState = CursorLockMode.None;
		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (Cursor.lockState != CursorLockMode.Locked)
				{
					Cursor.lockState = CursorLockMode.Locked;
				}
			}

			// setting target position for spring
			// (works better with flying over objects)
			RaycastHit _hit;
			if(Physics.Raycast(transform.position, Vector3.down, out _hit, 100f, EnvironmentMask))
			{
				//Joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
			}
			else
			{
				//Joint.targetPosition = new Vector3(0f, 0f, 0f);
			}

			XMovementBuffer = Input.GetAxis("Horizontal");
			ZMovementBuffer = Input.GetAxis("Vertical");

			MovementHorizontal = transform.right * XMovementBuffer;
			MovementVertical = transform.forward * ZMovementBuffer;

			Velocity = (MovementHorizontal + MovementVertical) * Speed;

			//animator.SetFloat("ForwardVelocity", ZMovementBuffer);

			Motor.Move(Velocity);

			// turning player around
			float yRot = Input.GetAxisRaw("Mouse X");

			// generate rotation Vector
			Vector3 rotation = new Vector3(0f, yRot, 0f) * XLookSensisitivity;

			// apply rotation to player
			Motor.Rotate(rotation);

			// get Y for looking up and down
			float XRot = Input.GetAxisRaw("Mouse Y");

			// generate rotation Vector
			float CameraRotationX = XRot *  YLookSensisitivity;

			// apply rotation to player
			Motor.RotateCamera(CameraRotationX);

			Vector3 _ThrusterForce = Vector3.zero;

			if (Input.GetButtonDown("Jump"))
			{
				Motor.JumpForce(JumpForce);
			}

			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Cursor.lockState = CursorLockMode.None;
			}

			//if (Input.GetButtonDown("Dash"))
			//{
			//	if (Input.GetKey(KeyCode.A))
			//	{
			//		Motor.Dash(Vector3.left);
			//	}
			//	else if (Input.GetKey(KeyCode.S))
			//	{
			//		Motor.Dash(Vector3.back);
			//	}
			//	else if (Input.GetKey(KeyCode.D))
			//	{
			//		Motor.Dash(Vector3.right);
			//	}
			//	else
			//	{
			//		Motor.Dash(Vector3.forward);
			//	}
			//}

			//// apply thruster force
			//if (Input.GetButton("Jump") && ThrusterFuelAmount > 0f)
			//{
			//	ThrusterFuelAmount -= ThrusterFuelBurnSpeed * Time.fixedDeltaTime;
			//	if (ThrusterFuelAmount >= 0.01f)
			//	{
			//		_ThrusterForce = Vector3.up * ThrusterForce;
			//		SetJointSettings(0f);
			//	}
			//}
			//else
			//{
			//	ThrusterFuelAmount += ThrusterFuelRegenSpeed * Time.fixedDeltaTime;
			//	SetJointSettings(JointSpring);
			//}
			//ThrusterFuelAmount = Mathf.Clamp(ThrusterFuelAmount, 0f, 1f);
			//Motor.ApplyThrust(_ThrusterForce);
			//SetJointSettings(JointSpring);

			//if (Input.GetButtonDown("Use"))
			//{
			//	GameManager.instance.CmdTryUseCurrentObject(gameObject.name);
			//}
		}

	}
}