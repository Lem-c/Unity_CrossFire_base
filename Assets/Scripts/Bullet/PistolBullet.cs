// A specific bullet type: pistol
using UnityEngine;

public class PistolBullet : Bullet
{

    protected override void AssignData()
    {
        InitiBullet(500f, "pistol", 5000f);
    }
    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        HandleRaycastHit();
    }

    protected override void OnTriggerEnter(Collider other){}
}