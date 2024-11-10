using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    private float timeToDestroyDelta;
    private bool destroyAfterTime = true;

    private void Update()
    {
        if (!destroyAfterTime)
            return;

        if (timeToDestroyDelta <= 0f)
        {
            Destroy(gameObject);
            return;
        }
        timeToDestroyDelta -= Time.deltaTime;
    }

    public void SetDestroyTimer(float time)
    {
        destroyAfterTime = true;
        timeToDestroyDelta = time;
    }
}
