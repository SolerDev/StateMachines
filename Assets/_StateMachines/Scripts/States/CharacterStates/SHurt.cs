using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHurt : State
{
    private readonly int hurtHash = Animator.StringToHash("Hurt");

    public SHurt(Character character) : base(character)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        anim.SetTrigger(hurtHash);
    }
}
