using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Bullet Factory Interface
public interface IBulletFactory
{
    Bullet CreateBullet();
}

// Concrete Bullet Factories for different bullet types
public class PistolBulletFactory : IBulletFactory
{
    public Bullet CreateBullet()
    {
        GameObject bulletObject = Resources.Load<GameObject>("Bullet_Pistol");
        GameObject instantiatedBullet = Object.Instantiate(bulletObject);
        return instantiatedBullet.GetComponent<Bullet>();
    }
}

public class RifleBulletFactory : IBulletFactory
{
    public Bullet CreateBullet()
    {
        GameObject bulletObject = Resources.Load<GameObject>("Bullet_Rifle");
        GameObject instantiatedBullet = Object.Instantiate(bulletObject);
        return instantiatedBullet.GetComponent<Bullet>();
    }
}
