using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MyGeometry;

public class ChainTower : Tower 
{
	[SerializeField] private int maxBindingCount;
	[SerializeField] private float idleRotationSpeed;
	// This should be an object with ParticleSystem
	[SerializeField] private GameObject laserPrefab;
	// This should be an objct with Button
	[SerializeField] private GameObject bindButtonPrefab;
	// This should be an objct with Button
	[SerializeField] private GameObject unbindButtonPrefab;
	// This should be an object with LineRenderer
	[SerializeField] private GameObject bindingPreviewPrefab;

	private List<Binding> bindings = new List<Binding>();
	private List<GameObject> previews = new List<GameObject>();

	public bool CanBind { get; set; } = true;

	protected override void PopUpSet()
	{
		base.PopUpSet();
		UpdateBindingButtons();
	}

	private void UpdateBindingButtons()
	{
		foreach (var i in previews) if (i != null) Destroy(i);
		previews.Clear();

		if (PopUpMenu)
		{
			if (CanBind)
			{ // Show previews for binding
				var chainTowers = FindObjectsOfType<ChainTower>();

				foreach (var other in chainTowers)
				{
					if (other == this) continue; // Skip self
					if (!other.CanBind) continue; // Skip unfree towers
					if (bindings.Any((x) => x.bindedTower == other)) continue; // Skip if already binded
					float distance = Vector2.Distance(transform.position, other.transform.position);
					if (distance > radius || distance > other.radius)
						continue; // Skip towers that too far
								  // Create binding preview
					var preview = Instantiate(bindingPreviewPrefab);
					var line = preview.GetComponent<LineRenderer>();
					line.SetPosition(0, transform.position);
					line.SetPosition(1, other.transform.position);
					var bindingButton = Instantiate(bindButtonPrefab, Controller.Instance.worldCanvas.transform);
					bindingButton.transform.position = Vector2.Lerp(transform.position, other.transform.position, (distance - 1) / distance);
					var button = bindingButton.GetComponent<Button>();
					button.onClick.AddListener(() => { BindTo(other); UpdateBindingButtons(); });
					// Add to the previews array for the following destruction
					previews.Add(preview);
					previews.Add(bindingButton);
				}
			}
			// Show buttons for unbinding
			foreach (var binding in bindings)
			{
				var unbindingButton = Instantiate(unbindButtonPrefab, Controller.Instance.worldCanvas.transform);
				float distance = Vector2.Distance(transform.position, binding.bindedTower.transform.position);
				unbindingButton.transform.position = Vector2.Lerp(transform.position, binding.bindedTower.transform.position, (distance - 1) / distance);
				var button = unbindingButton.GetComponent<Button>();
				button.onClick.AddListener(() => { UnbindFrom(binding); UpdateBindingButtons(); });
				previews.Add(unbindingButton);
			}
		}
	}

	private void BindTo(ChainTower other)
	{
		var damageLine = new Segment(transform.position, other.transform.position);
		Vector2 direction = transform.position - other.transform.position;
		var laser = Instantiate(laserPrefab, Vector2.Lerp(transform.position, other.transform.position, 0.5f),
			Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, direction)));
		var ps = laser.GetComponent<ParticleSystem>();
		var shape = ps.shape;
		shape.radius = Vector2.Distance(transform.position, other.transform.position) / 2 - 0.2f;
		//
		var gunRotation = Vector2.SignedAngle(Vector2.right, direction);
		gun.transform.rotation = Quaternion.Euler(0, 0, gunRotation + 180);
		//
		var binding = new Binding
		{
			bindedTower = other,
			damageLine = damageLine,
			laser = laser,
			gun = gun,
			idle = false
		};

		bindings.Add(binding);
		CanBind = maxBindingCount > bindings.Count;
		gun = Instantiate(gun, transform);

		if (!CanBind) gun.SetActive(false);

		other.Bind(this, damageLine, laser, gunRotation);
	}

	private void UnbindFrom(Binding other)
	{
		other.bindedTower.Unbind(this);
		bindings.Remove(other);

		Destroy(other.gun);

		CanBind = maxBindingCount > bindings.Count;

		if (gun != null)
			gun.SetActive(CanBind);
	}

	private void Unbind(ChainTower other)
	{
		for (int i = 0; i < bindings.Count; i++)
		{
			if (bindings[i].bindedTower == other)
			{
				if (bindings[i].laser != null)
				{
					var particleSystem = bindings[i].laser.GetComponent<ParticleSystem>();
					particleSystem.Stop();
					Destroy(bindings[i].laser, particleSystem.main.startLifetime.constantMax);
				}
				if (bindings[i].gun != null)
					Destroy(bindings[i].gun);

				bindings.RemoveAt(i);

				CanBind = maxBindingCount > bindings.Count;

				if (gun != null)
					gun.SetActive(CanBind);
			}
		}
	}

	private void Bind(ChainTower other, Segment damageLine, GameObject laser, float gunRotation)
	{
		gun.transform.rotation = Quaternion.Euler(0, 0, gunRotation);

		var binding = new Binding
		{
			bindedTower = other,
			damageLine = damageLine,
			laser = laser,
			gun = gun,
			idle = true
		};

		bindings.Add(binding);
		CanBind = maxBindingCount > bindings.Count;
		gun = Instantiate(gun, transform);

		if (!CanBind)
			gun.SetActive(false);
	}

	protected void Update()
	{
		if (bindings.Count != maxBindingCount)
		{
			gun.transform.rotation = Quaternion.Euler(0, 0, idleRotationSpeed * Time.time);
		}
		if (bindings.Count > 0)
		{
			foreach (var binding in bindings)
			{
				if (binding.idle) continue;

				var enumerator = Controller.Instance.SpawnedEnemies.First;
				if (enumerator == null) break;
				var next = enumerator.Next;
				do
				{
					var enemy = enumerator.Value;
					var translation = new Segment(enemy.LastPosition, enemy.transform.position);

					if (Geometry2D.SegmentIntersection(binding.damageLine, translation).x != float.NegativeInfinity)
					{
						InflictDamage(enemy);
						binding.bindedTower.InflictDamage(enemy);
					}
					enumerator = next;
					if (enumerator == null) break;
					next = enumerator.Next;
				} while (true);
			}
		}
	}

	private void InflictDamage(Enemy enemy)
	{
		enemy.TakeDamage(damage);
	}

	private void OnDestroy()
	{
		foreach (var binding in bindings)
		{
			if (binding.bindedTower != null)
				binding.bindedTower.Unbind(this);
		}
	}

	private class Binding
	{
		public GameObject laser;
		public ChainTower bindedTower;
		public Segment damageLine;
		public GameObject gun;
		public bool idle;
	}
}