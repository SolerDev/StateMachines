using System;
using UnityEngine;

public class SWalkPlayer : SGround
{
    #region Properties

    protected new PlayerCharacter character { get => (PlayerCharacter)base.character; set => base.character = value; }

    private Vector2 moveVector = Vector2.zero;
    private Vector2 facingDirection = Vector2.one;

    private readonly int walkingHash = Animator.StringToHash("Moving");
    private readonly int jumpHash = Animator.StringToHash("Jump");

    public SWalkPlayer(PlayerCharacter character) : base(character)
    {
    }

    #endregion


    public override void OnEnter()
    {
        base.OnEnter();
        anim.SetBool(walkingHash, true);
    }

    public override void OnExit()
    {
        base.OnExit();
        anim.SetBool(walkingHash, false);
        //anim.SetFloat(velXHash, 0f);
    }

    public override Type FixedTick()
    {
        ft = base.FixedTick();
        if (ft != null)
        {
            return ft;
        }

        Vector2 direction = character.InputReader.Direction;

        if (direction.x.Equals(0f))
        {
            ft = typeof(SIdle);
        }
        else if (character.InputReader.Dash)
        {
            ft = typeof(SDash);
        }
        else if (character.InputReader.Jump && character.CanJump())
        {
            character.Jump(Vector2.up, jumpHash);
        }
        else if (character.IsEdgeFromSide(direction.x))
        {
            ft = typeof(SWallClimb);
        }
        else if (character.IsStepFromSide(direction.x))
        {
            character.Step(direction.x);
        }
        else
        {
            character.Walk(direction.x);
            //anim.SetFloat(velXHash, Mathf.Abs(character.Controller.Velocity.x));
        }
        return ft;
    }
}
