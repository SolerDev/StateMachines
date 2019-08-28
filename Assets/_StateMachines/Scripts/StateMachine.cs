using System;
using System.Collections.Generic;
using System.Linq;

public class StateMachine
{
    #region Properties

    public Dictionary<Type, State> AvailableStates;
    public State CurrentState { get; private set; }
    
    #endregion

    public void OnStart()
    {
        //enter first state
        CurrentState = AvailableStates.Values.First();
        SwitchToState(CurrentState.GetType());
    }

    public void StateTick()
    {
        var nextState = CurrentState?.Tick();

        if (nextState != null &&
            nextState != CurrentState.GetType())
        {
            SwitchToState(nextState);
        }
    }

    public void StateFixedTick()
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
