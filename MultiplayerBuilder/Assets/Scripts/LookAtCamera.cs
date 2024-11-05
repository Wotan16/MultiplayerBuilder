using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform;

    private void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        transform.LookAt(cameraTransform);
    }
}
