using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SebCharCtrl
{
    [RequireComponent(typeof(SebController))]
    //[RequireComponent(typeof(SebPlayer))]
    public class StateMachine : MonoBehaviour
    {
        #region Properties

        public Dictionary<Type, State> AvailableStates;
        public State CurrentState { get; private set; }

        #endregion

        protected void Start()
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
}