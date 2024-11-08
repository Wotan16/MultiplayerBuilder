using UnityEngine;
using Unity.Netcode;

public class Test : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            float speed = 3f;
            transform.position = Vector3.zero;
        }
    }
}
