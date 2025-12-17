using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Header("Teleport Destination")]
    public Transform Destination;

    private void OnTriggerEnter(Collider other)
    {
        // Always use the ROOT transform of whatever entered
        Transform root = other.transform.root;

        // We only teleport the player
        if (!root.CompareTag("Player"))
            return;

        // Check the player's global teleport cooldown
        PlayerTeleportCooldown cooldown = root.GetComponent<PlayerTeleportCooldown>();
        if (cooldown == null || !cooldown.canTeleport)
            return;

        // Temporarily disable CharacterController to avoid Move() errors
        CharacterController cc = root.GetComponent<CharacterController>();
        if (cc != null)
            cc.enabled = false;

        // Teleport player to the destination
        root.position = Destination.position;

        // Re-enable CharacterController
        if (cc != null)
            cc.enabled = true;

        // Start player cooldown (prevents immediate re-trigger)
        cooldown.TriggerCooldown();
    }
}
