using UnityEngine;

public class PlayerPoints : MonoBehaviour
{
    public int points = 0;

    public void AddPoints(int amount)
    {
        if (amount <= 0) return;
        points += amount;
    }

    public bool SpendPoints(int cost)
    {
        if (cost <= 0) return true;
        if (points < cost) return false;

        points -= cost;
        return true;
    }
}
