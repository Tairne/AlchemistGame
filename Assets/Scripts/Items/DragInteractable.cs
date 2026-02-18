using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DragInteractable : MonoBehaviour
{
    [Header("Drag Settings")]
    public float maxDragSeconds = 10f;
    public float holdDistance = 2.0f;      // на каком рассто€нии держим от камеры
    public float spring = 4000f;           // Усила удержани€Ф
    public float damper = 80f;             // демпфер
    public float maxDistance = 0.01f;       // УрезинкаФ
    public float breakForce = Mathf.Infinity;
    public float breakTorque = Mathf.Infinity;

    public bool CanDrag => enabled && gameObject.activeInHierarchy;

    public void PrepareRigidbodyForDrag(Rigidbody rb)
    {
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 10f;
        rb.maxAngularVelocity = 2f;
        rb.useGravity = true;
    }
}
