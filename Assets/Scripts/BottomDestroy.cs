using UnityEngine;

public class BottomDestroy : ServerOnlyMonobehavior
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // TODO: Respawn
            return;
        }

        Destroy(other.gameObject);
    }
}
