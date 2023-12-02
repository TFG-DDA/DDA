using UnityEngine;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
	private struct PointInSpace
	{
		public Vector3 Position;
		public float Time;
	}

	[SerializeField]
	[Tooltip("The transform to follow")]
	private Transform target;

	[SerializeField]
	[Tooltip("The offset between the target and the camera")]
	private Vector3 offset;

	[Tooltip("The delay before the camera starts to follow the target")]
	[SerializeField]
	private float delay = 0.5f;

	[SerializeField]
	[Tooltip("The speed used in the lerp function when the camera follows the target")]
	private float speed = 5;

	///<summary>
	/// Contains the positions of the target for the last X seconds
	///</summary>
	private Queue<PointInSpace> pointsInSpace = new Queue<PointInSpace>();

	public void ResetPos()
	{
        target = PlayerInstance.instance.transform.GetChild(0).GetChild(0);
		transform.position = new Vector3(target.transform.position.x, target.position.y, transform.position.z); 
	}

	void Update()
	{
		if(target == null || GameManager.instance.IsPaused()) return;
		// Add the current target position to the list of positions
		pointsInSpace.Enqueue(new PointInSpace() { Position = target.position, Time = Time.time });
		Vector3 newPos = transform.position;

		// Move the camera to the position of the target X seconds ago 
		if(pointsInSpace.Count > 0 && pointsInSpace.Peek().Time <= Time.time - delay + Mathf.Epsilon)
		{
			newPos = Vector3.Lerp(newPos, pointsInSpace.Dequeue().Position + offset, Time.deltaTime * speed);
		}
		transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
	}
}
