using UnityEngine;

public abstract class InputReader : ScriptableObject
{
    #region Properties

    //interaction
    public bool Attack { get; protected set; }

    //movement
    public float DirX { get; protected set; }
    public float DirY { get; protected set; }
    public Vector2 Direction;
    public bool Jump { get; protected set; }
    
    #endregion
    
    protected virtual void OnEnable()
    {
        #region Initialize

        Attack = false;

        Jump = false;

        DirX = 0f;
        DirY = 0f;
        Direction = Vector2.zero;

        //Debug.Log(name + " initialized");

        #endregion
    }

    public virtual void Read()
    {
        ReadMovement();
        ReadAction();
    }

    protected virtual void ReadMovement()
    {
        //Debug.Log("Reading Movement from " + name);
    }

    protected virtual void ReadAction()
    {
        //Debug.Log("Reading Action from " + name);
    }

    public virtual void ClearInput()
    {
        Attack = false;
        Jump = false;
    }
}
