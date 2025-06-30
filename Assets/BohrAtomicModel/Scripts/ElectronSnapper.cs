using Lean.Touch;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ElectronSnapper : MonoBehaviour
{
  public float suctionDistance = 0.135f;
  public float detachDistanceThreshold = 0.085f;

  private Rigidbody2D rb;
  private LeanSelectableByFinger lsbf;

  private bool snapped = false;
  private ShellZone snappedZone;

  void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
    lsbf = GetComponent<LeanSelectableByFinger>();
  }

  void Update()
  {
    if (!BohrAtomPhysicsController.isGameReady) return;

    if (!snapped)
    {
      TrySnapToShell();
    }
    else if (snapped && snappedZone != null)
    {
      float distanceFromOrbit = Mathf.Abs(Vector3.Distance(transform.position, snappedZone.transform.position) - snappedZone.shellRadius);

      if (lsbf.IsSelected && distanceFromOrbit > detachDistanceThreshold)
      {
        Unsnap();
        return;
      }

      ConstrainToOrbit();
    }
  }

  void TrySnapToShell()
  {
    ShellZone[] zones = FindObjectsByType<ShellZone>(FindObjectsSortMode.None);

    ShellZone bestZone = null;
    float bestDelta = float.MaxValue;

    foreach (var zone in zones)
    {
      float distToCenter = Vector3.Distance(transform.position, zone.transform.position);
      float radialDelta = Mathf.Abs(distToCenter - zone.shellRadius);

      if (radialDelta <= zone.snapRadius && radialDelta < bestDelta)
      {
        bestDelta = radialDelta;
        bestZone = zone;
      }
    }

    if (bestZone != null)
    {
      Vector3 snappedPos = bestZone.GetSnappedPosition(transform.position);

      rb.linearVelocity = Vector2.zero;
      rb.gravityScale = 0f;
      rb.bodyType = RigidbodyType2D.Dynamic;

      transform.position = snappedPos;
      snapped = true;
      snappedZone = bestZone;

      lsbf.Deselect();

      snappedZone.RegisterElectron(transform);

    }
  }

  void ConstrainToOrbit()
  {
    Vector3 center = snappedZone.transform.position;
    Vector3 direction = (transform.position - center).normalized;
    Vector3 orbitPos = center + direction * snappedZone.shellRadius;

    transform.position = orbitPos;
  }

  public void Unsnap()
  {
    if (snapped && snappedZone != null)
    {
      snappedZone.UnregisterElectron(transform);
      snappedZone = null;
      snapped = false;
      rb.gravityScale = 1f;
    }
  }

  public bool IsSnapped() => snapped && snappedZone != null;
}
