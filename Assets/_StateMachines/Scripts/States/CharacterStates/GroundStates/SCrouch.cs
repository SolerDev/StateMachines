using System;
using UnityEngine;

public class SCrouch : SGround
{
    private readonly int crouchingHash = Animator.StringToHash("Crouching");

    public SCrouch(Character character) : base(character)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        anim.SetBool(crouchingHash, true);
    }

    public override void OnExit()
    {
        base.OnExit();
        anim.SetBool(crouchingHash, false);
    }

    public override Type FixedTick()
    {
        ft = base.FixedTick();
        if (ft != null)
        {
            return ft;
        }

        //todo: attack ifs
        if (inputReader.JumpPress)
        {
            if (Physics2D.Raycast(
                trans.position,
                Vector2.down,
                (character.Height() / 2) + controller.SkinWidth,
                StaticRefs.MASK_THROUGH))
            {
                controller.Collisions.fallingThroughPlatform = true;
                controller.StartCoroutine(controller.ResetFallingThroughPlatform());
                ft = typeof(SFall);
            }
        }
        else if (!inputReader.DirY.Equals(-1f))
        {
            ft = typeof(SIdle);
        }

        return ft;
    }
}
