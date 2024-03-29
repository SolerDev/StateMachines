﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticRefs
{
    public static readonly string TAG_GROUND = "Ground";
    public static readonly string TAG_WATER = "Water";
    public static readonly string TAG_PLATFORM = "Platform";

    //MyLayerMask = 1 << LayerMask.NameToLayer("My Layer Name");

    public static readonly LayerMask MASK_GROUND = 1 << LayerMask.NameToLayer("Ground");
    public static readonly int MASK_WATER = 1 << LayerMask.NameToLayer("Water");
    public static readonly int MASK_PLATFORM = 1 << LayerMask.NameToLayer("Platform");
    public static readonly LayerMask MASK_PLAYER = 1 << LayerMask.NameToLayer("Player");
    public static readonly LayerMask MASK_ENEMY = 1 << LayerMask.NameToLayer("Enemy");

    //int catLyrMask = (1 << someCat.layer)

    [Tooltip("mask containing the GROUND and PLATFORM masks")]
    public static readonly int MASK_FLOOR = MASK_GROUND | MASK_PLATFORM;
    [Tooltip("mask containing the Player and Enemy masks")]
    public static readonly int MASK_ACTOR = MASK_PLAYER | MASK_ENEMY;
    [Tooltip("mask containing the FLOOR, WATER AND ACTOR masks")]
    public static readonly int MASK_PALPABLE = MASK_FLOOR | MASK_WATER | MASK_ACTOR;
}
