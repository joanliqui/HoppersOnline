using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void Damaged();
    public void Damaged(Vector2 dir, float impulseForce);
    public void Damaged(float x, float y, float impulseForce, int id);
}
