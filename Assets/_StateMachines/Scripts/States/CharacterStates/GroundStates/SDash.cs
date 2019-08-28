using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDash : SGround
{
    private readonly int dashHash = Animator.StringToHash("Dash");

    private float timeInState;
    private Vector2 dashVector;

    LayerMask characterLayer;
    LayerMask enemyLayer;
    private float dashDuration;

    public SDash(PlayerCharacter character) : base(character)
    {
        dashVector = Vector2.right * character.Attributes.DashImpulse;

        characterLayer = Mathf.RoundToInt(Mathf.Log(StaticRefs.MASK_PLAYER, 2));
        enemyLayer = Mathf.RoundToInt(Mathf.Log(StaticRefs.MASK_ENEMY, 2)); 
    }

    public override void OnEnter()
    {
        base.OnEnter();

        anim.SetTrigger(dashHash);
        dashDuration = anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        Debug.Log(dashDuration);

        timeInState = 0f;

        Physics2D.IgnoreLayerCollision(characterLayer, enemyLayer, true);
        character.Controller.Velocity = dashVector * character.InputReader.DirX;
    }

    public override void OnExit()
    {
        base.OnExit();
        Physics2D.IgnoreLayerCollision(characterLayer, enemyLayer, false);
    }

    public override Type FixedTick()
    {
        ft = base.FixedTick();
        if (ft != null) { return ft; }


        timeInState += Time.fixedDeltaTime;
        

        if (timeInState> dashDuration)
        {
            ft = typeof(SIdle);
        }

        //else if (timeInState>=dashDuration)
        //{
        //    ft = typeof(SIdle);
        //}


        return ft;
    }
}
