using UnityEngine;

public class RotatableProjectile : Projectile
{
    protected override void UpdateRotation()
    {
        float angle = this.transform.rotation.eulerAngles.z;

        angle += 720 * Time.deltaTime;

        this.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}