using UnityEngine;

namespace ExampleGame
{
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerMotor : MonoBehaviour
	{

		[SerializeField]
		private Camera Cam;

		private Vector3 Velocity = Vector3.zero;
		private Vector3 Rotation = Vector3.zero;
		private float CameraRotationX = 0f;
		private float CurrentCameraRotationX = 0f;
		//private Vector3 ThrusterForce = Vector3.zero;

		[SerializeField]
		private float CameraRotationLimit = 85f;

		private Rigidbody Rb;

		[SerializeField]
		private float DashForce = 100;


		private void Start()
		{
			Rb = GetComponent<Rigidbody>();
		}

		public void Move(Vector3 _Velocity)
		{
			Velocity = _Velocity;
		}

		private void FixedUpdate()
		{
			PerformMovement();
			PerformRotation();
		}

		private void PerformMovement()
		{
			if (Velocity != Vector3.zero)
			{
				Rb.MovePosition(Rb.position + Velocity * Time.fixedDeltaTime);
			}
			//if (ThrusterForce != Vector3.zero)
			//{
			//	Rb.AddForce(ThrusterForce * Time.deltaTime, ForceMode.Acceleration);
			//}
		}

		private void PerformRotation()
		{
			Rb.MoveRotation(Rb.rotation * Quaternion.Euler(Rotation));
			if (Cam != null)
			{
				CurrentCameraRotationX -= CameraRotationX;
				CurrentCameraRotationX = Mathf.Clamp(CurrentCameraRotationX, -CameraRotationLimit, CameraRotationLimit);
				Cam.transform.localEulerAngles = new Vector3(CurrentCameraRotationX, 0f, 0f);
			}
		}

		public void Rotate(Vector3 _Rotation)
		{
			Rotation = _Rotation;
		}

		public void RotateCamera(float _CameraRotationX)
		{
			CameraRotationX = _CameraRotationX;
		}

		internal void JumpForce(Vector3 Force)
		{
			Rb.AddRelativeForce(Force, ForceMode.Impulse);
		}

		internal void Dash(Vector3 Direction)
		{
			Rb.AddRelativeForce(Direction * DashForce, ForceMode.Impulse);
		}

		//// get a force vector for our thrutsers
		//public void ApplyThrust(Vector3 _ThrusterForce)
		//{
		//	ThrusterForce = _ThrusterForce;
		//}
	}
}