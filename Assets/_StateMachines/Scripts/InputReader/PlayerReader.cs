﻿using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerInputReader", menuName = "Input Reader/Player", order = 0)]
public class PlayerReader : InputReader
{
    public bool Interact { get; protected set; }
    public bool Dash { get; protected set; }
    
    protected override void OnEnable()
    {
        base.OnEnable();

        Interact = false;
        Dash = false;
    }

    #region Custom Methods

    protected override void ReadMovement()
    {
        base.ReadMovement();

        if (Input.GetKeyDown(Controls.Instance.kJump)) Jump = true;
        Dash = Input.GetKeyDown(Controls.Instance.kDash);

        DirX = Input.GetAxisRaw(Controls.Instance.kHorizontal);
        DirY = Input.GetAxisRaw(Controls.Instance.kVertical);

        Direction.Set(DirX, DirY);
    }

    protected override void ReadAction()
    {
        base.ReadAction();

        Attack = Input.GetKeyDown(Controls.Instance.kAttack);
        Interact = Input.GetKeyDown(Controls.Instance.kInteract);
    }
    
    public override void ClearInput()
    {
        base.ClearInput();
        Interact = false;
        Dash = false;
    }

    #endregion
}
