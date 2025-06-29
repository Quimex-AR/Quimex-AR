using System.Collections.Generic;
using UnityEngine;

public class ShellZone : MonoBehaviour
{
  public int maxElectrons = 2;
  public float snapRadius = 0.185f;

  [HideInInspector]
  public float shellRadius;

  private List<Transform> snappedElectrons = new();

  public bool HasRoom() => snappedElectrons.Count < maxElectrons;

  public void RegisterElectron(Transform electron)
  {
    if (!snappedElectrons.Contains(electron))
    {
      snappedElectrons.Add(electron);
    }
  }

  public void UnregisterElectron(Transform electron)
  {
    if (snappedElectrons.Contains(electron))
    {
      snappedElectrons.Remove(electron);
    }
  }

  public Vector3 GetSnappedPosition(Vector3 worldPosition)
  {
    Vector3 local = transform.InverseTransformPoint(worldPosition);
    Vector3 clamped = local.normalized * shellRadius;

    return transform.TransformPoint(clamped);
  }

  public bool IsSnappedElectron(Transform electron) => snappedElectrons.Contains(electron);

  public int GetSnappedCount() => snappedElectrons.Count;

  public List<Transform> GetSnappedElectrons() => snappedElectrons;
}
