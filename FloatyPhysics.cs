using Landfall.TABS.GameState;
using Photon.Bolt;
using UnityEngine;

public class FloatyPhysics : MonoBehaviour
{
	public LayerMask mask;

	public AnimationCurve flightCurve;

	public float deadPowerDecay = 2f;

	public float heightVariance = 0.5f;

	public float variationSpeed = 0.5f;

	public float flightForce;

	public float legForceMultiplier = 1f;

	private DataHandler data;

	private RigidbodyHolder rigHolder;

	private float deadPower = 1f;

	private Rigidbody rightFootRig;

	private Rigidbody leftFootRig;

	private Rigidbody hipRig;

	private Rigidbody headRig;

	public float headM = 0.5f;

	private float time;

	public Animator animator1;

	public Animator animator2;

	private bool dead;

	public bool useWings = true;

	public bool useWingsInPlacement = true;

	[Tooltip("Enable if units move erratically on the client side of ProjectMars games. Only enable if you are sure Wings.cs is causing erratic movement.")]
	public bool setUnitMainRigKinematic;

	private GameStateManager m_gameStateManager;

	public float rotationTorque = 10f;

	private void Start()
	{
		data = base.transform.root.GetComponentInChildren<DataHandler>();
		rigHolder = data.GetComponent<RigidbodyHolder>();
		data.takeFallDamage = false;
		//data.canFall = false;
		if ((bool)data.footRight)
		{
			rightFootRig = data.footRight.GetComponent<Rigidbody>();
		}
		if ((bool)data.footLeft)
		{
			leftFootRig = data.footLeft.GetComponent<Rigidbody>();
		}
		hipRig = data.hip.GetComponent<Rigidbody>();
		if ((bool)data.head)
		{
			headRig = data.head.GetComponent<Rigidbody>();
		}
		AnimationHandler component = data.GetComponent<AnimationHandler>();
		if ((bool)component)
		{
			component.multiplier = 0.5f;
		}
		heightVariance *= Random.value;
		time = Random.Range(0f, 1000f);
		//Balance component2 = data.GetComponent<Balance>();
		//if ((bool)component2)
		//{
		//	component2.enabled = false;
		//}
		m_gameStateManager = ServiceLocator.GetService<GameStateManager>();
		if (setUnitMainRigKinematic && BoltNetwork.IsClient)
		{
			data.mainRig.isKinematic = true;
		}
	}

	private void FixedUpdate()
	{
		if ((!useWingsInPlacement && m_gameStateManager.GameState != GameState.BattleState) || !useWings)
		{
			return;
		}
		if ((bool)data && data.Dead)
		{
			if (data.allRigs.AllRigs[0] != null)
			{
				if (deadPower > 0f)
				{
					data.mainRig.AddForce(data.mainRig.transform.up * flightForce * deadPower * 1f, ForceMode.Acceleration);
					data.mainRig.AddForce(Vector3.up * flightForce * deadPower * 0.3f, ForceMode.Acceleration);
					deadPower -= Time.deltaTime * deadPowerDecay;
				}
				else
				{
					if (dead)
					{
						return;
					}
					dead = true;
					if ((bool)animator1)
					{
						animator1.speed = 0f;
						animator1.transform.SetParent(base.transform.root);
						Rigidbody rigidbody = animator1.gameObject.AddComponent<Rigidbody>();
						if ((bool)rigidbody && (bool)data.mainRig)
						{
							rigidbody.gameObject.AddComponent<SetInterpolation>();
							rigidbody.velocity = data.mainRig.velocity;
						}
						animator1.GetComponentInChildren<Collider>().enabled = true;
						animator1.gameObject.AddComponent<RemoveAfterSeconds>().shrink = true;
					}
					if ((bool)animator2)
					{
						animator2.speed = 0f;
						animator2.transform.SetParent(base.transform.root);
						Rigidbody rigidbody2 = animator2.gameObject.AddComponent<Rigidbody>();
						if ((bool)rigidbody2 && (bool)data.mainRig)
						{
							rigidbody2.gameObject.AddComponent<SetInterpolation>();
							rigidbody2.velocity = data.mainRig.velocity;
						}
						animator2.GetComponentInChildren<Collider>().enabled = true;
						animator2.gameObject.AddComponent<RemoveAfterSeconds>().shrink = true;
					}
				}
			}
			else if (!dead)
			{
				dead = true;
				if ((bool)animator1)
				{
					animator1.speed = 0f;
				}
				if ((bool)animator2)
				{
					animator2.speed = 0f;
				}
			}
			return;
		}
		bool value = data.unit.m_PreferedDistance > data.distanceToTarget;
		if ((bool)animator2)
		{
			animator1.SetBool("InRange", value);
		}
		if ((bool)animator2)
		{
			animator2.SetBool("InRange", value);
		}
		Physics.Raycast(new Ray(base.transform.position, Vector3.down), out var hitInfo, flightCurve.keys[flightCurve.keys.Length - 1].time, mask);
		if ((bool)hitInfo.transform && data.ragdollControl == 1f)
		{
			float num = hitInfo.distance + Mathf.Cos((Time.time + time) * variationSpeed) * heightVariance;
			data.mainRig.AddTorque(rotationTorque * Vector3.Angle(data.mainRig.transform.up, data.groundedMovementDirectionObject.forward) * Vector3.Cross(data.mainRig.transform.up, data.groundedMovementDirectionObject.forward), ForceMode.Acceleration);
			if ((bool)headRig)
			{
				headRig.AddForce(Vector3.up * flightForce * headM * flightCurve.Evaluate(num), ForceMode.Acceleration);
			}
			data.mainRig.AddForce(Vector3.up * flightForce * flightCurve.Evaluate(num), ForceMode.Acceleration);
			if ((bool)rightFootRig)
			{
				rightFootRig.AddForce(Vector3.up * flightForce * legForceMultiplier * 0.5f * flightCurve.Evaluate(num), ForceMode.Acceleration);
			}
			if ((bool)rightFootRig)
			{
				leftFootRig.AddForce(Vector3.up * flightForce * legForceMultiplier * 0.5f * flightCurve.Evaluate(num), ForceMode.Acceleration);
			}
			data.TouchGround(hitInfo.point, hitInfo.normal);
		}
	}

	public void EnableFlight()
	{
		useWings = true;
	}

	public void DiableFlight()
	{
		useWings = false;
	}
}
