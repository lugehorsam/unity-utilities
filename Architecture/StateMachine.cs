﻿using System;

namespace Utilities
{
 
    public class StateMachine<T>
    {
        public event Action<T, T> OnStateChanged = (arg1, arg2) => { };

        public T State
        {
            get { return _state; }
            set
            {
                if (value.Equals(_state))
                {
                    return;
                }

                T oldProperty = _state;
                _state = value;

                IState oldState = oldProperty as IState;
                IState newState = value as IState;
        
                if (oldState != null)
                    oldState.HandleTransitionFrom();
        
                if (newState != null)
                    newState.HandleTransitionTo();
        
                OnStateChanged(oldProperty, value);
            }
        }

        private T _state;

        public StateMachine(T initialState)
        {
            State = initialState;
        }
    
        public StateMachine() {}

        public static implicit operator T(StateMachine<T> thisStateMachine)
        {
            return thisStateMachine._state;
        }
    }   
}