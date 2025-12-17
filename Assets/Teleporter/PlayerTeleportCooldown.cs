using UnityEngine;

public class PlayerTeleportCooldown : MonoBehaviour
{
    public bool canTeleport = true;
    public float cooldownTime = 0.75f;

    public void TriggerCooldown()
    {
        canTeleport = false;
        StartCoroutine(ReenableTeleport());
    }

    private System.Collections.IEnumerator ReenableTeleport()
    {
        yield return new WaitForSeconds(cooldownTime);
        canTeleport = true;
    }
}
