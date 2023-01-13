using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public Dictionary<string, State> states;

    public StateMachine()
    {
        //states = new Dictionary<string, State>();
    }

    public void SetCallbacks(string StateName, System.Action UpdateMethod, System.Action Coroutine, System.Action BeginMethod, System.Action EndMethod)
    {
        State newState = new State(StateName, UpdateMethod, Coroutine, BeginMethod, EndMethod);
        states[StateName] = newState;
    }

    public class State
    {
        string StateName;
        System.Action UpdateMethod;
        System.Action Coroutine;
        System.Action BeginMethod;
        System.Action EndMethod;

        public State(string StateName, System.Action UpdateMethod, System.Action Coroutine, System.Action BeginMethod, System.Action EndMethod)
        {
            this.StateName = StateName;
            this.UpdateMethod = UpdateMethod;
            this.Coroutine = Coroutine;
            this.BeginMethod = BeginMethod;
            this.EndMethod = EndMethod;

            //public string getStateName()
            //{
            //    return this.StateName;
            //}

            //public System.Action getUpdateMethod()
            //{
            //    return this.UpdateMethod;
            //}

            //public System.Action getCoroutine()
            //{
            //    return this.Coroutine;
            //}

            //public System.Action getBeginMethod()
            //{
            //    return this.BeginMethod;
            //}

            //public System.Action getEndMethod()
            //{
            //    return this.EndMethod;
            //}
        }
    }
}
