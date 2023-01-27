using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPooleable 
{
    public GameObject GameObject { get;}
    public Pool Pool { get; set; }

}
