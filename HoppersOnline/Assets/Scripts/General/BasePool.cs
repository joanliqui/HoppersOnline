using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePool : MonoBehaviour
{
    public abstract void ReturnToPool(IPooleable objectToReturn);
}
