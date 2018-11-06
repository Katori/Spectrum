using UnityEngine;

namespace SpectrumFPSExampleGame.Sources
{
	[RequireComponent(typeof(PlayerMotor))]
	public class PlayerController : MonoBehaviour
	{

		[SerializeField]
		private float Speed = 5.0f;

		[SerializeField]
		private float XLookSensisitivity = 3.0f;

		[SerializeField]
		private float YLookSensisitivity = 3.0f;

		// component caching
		private PlayerMotor Motor;

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

			XMovementBuffer = Input.GetAxis("Horizontal");
			ZMovementBuffer = Input.GetAxis("Vertical");

			MovementHorizontal = transform.right * XMovementBuffer;
			MovementVertical = transform.forward * ZMovementBuffer;

			Velocity = (MovementHorizontal + MovementVertical) * Speed;

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
		}

	}
}