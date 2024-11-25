using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleBullet : Bullet
{
    protected override void AssignData()
    {
        InitiBullet(700f, "rifle", 10000f);
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
