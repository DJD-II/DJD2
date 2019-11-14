using UnityEngine;

sealed public class MoveForward : MonoBehaviour
{
    [SerializeField]
    private float speed = 20f;
    [SerializeField]
    private Vector3 forwardVector = Vector3.right;

    private void FixedUpdate()
    {
        transform.position += forwardVector.normalized * speed * Time.fixedDeltaTime;
    }
}
