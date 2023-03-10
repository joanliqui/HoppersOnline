using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Biome", menuName = "HoppersMenu/Biome")]
public class Biome : ScriptableObject
{
    public BiomeObject[] zones;
    public NetBiomeObject[] netZones;
    public BackgroundController background;
    public NetBackgroundController netBackground;
}

