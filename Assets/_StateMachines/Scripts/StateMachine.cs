using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMachine: MonoBehaviour
{
    #region Properties

    public Dictionary<Type, State> AvailableStates;
    public State CurrentState { get; private set; }
    
    #endregion

    protected void Awake()
    {
        //enter first state
        CurrentState = AvailableStates.Values.First();
        SwitchToState(CurrentState.GetType());
    }

    private void Update()
    {
        var nextState = CurrentState?.Tick();

        if (nextState != null &&
            nextState != CurrentState.GetType())
        {
            SwitchToState(nextState);
        }
    }

    private void FixedUpdate()
    {
        var nextState = CurrentState?.FixedTick();

        if (nextState != null &&
            nextState != CurrentState.GetType())
        {
            SwitchToState(nextState);
        }
    }

    public void SwitchToState(Type nextState)
    {

        CurrentState.OnExit();
        CurrentState = AvailableStates[nextState];
        CurrentState.OnEnter();

    }
}
