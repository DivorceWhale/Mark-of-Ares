using UnityEngine;

public class EnemyAnimationRelay : MonoBehaviour
{
    public Weapon weapon;

    public void EnableDamage()
    {
        if (weapon != null) weapon.EnableDamage();
    }

    public void DisableDamage()
    {
        if (weapon != null) weapon.DisableDamage();
    }
}
