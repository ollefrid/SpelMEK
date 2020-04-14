using System;
using System.Collections.Generic;

public class StateMachine
{
    private State currentState;
    private State queuedState;

    private Stack<State> automaton;
    private Dictionary<Type, State> stateDictionary = new Dictionary<Type, State>();

    public StateMachine(object controller, State[] states)
    {
        foreach (State state in states)
        {
            State instance = UnityEngine.Object.Instantiate(state);
            instance.owner = controller;
            instance.stateMachine = this;
            stateDictionary.Add(instance.GetType(), instance);

            // TODO(Fors): Kolla de här, fungerar förmodligen inte 
            if (currentState == null)
                currentState = instance;
        }
        currentState?.Enter();
    }

    public void TransitionTo<T>() where T : State
    {
        queuedState = stateDictionary[typeof(T)];
    }

    public void TransitionBack()
    {
        // NOTE(Fors): Pushdown automaton
        /*
        if (automaton.Count != 0)
            queuedState = automaton.Pop();
            */
    }

    public void Run()
    {
        UpdateState();
        currentState.Run();
    }

    private void UpdateState()
    {
        if (queuedState != currentState)
        {
            currentState?.Exit();
            // NOTE(Fors): Pushdown automaton
            //automaton.Push(currentState);
            currentState = queuedState;
            currentState.Enter();
        }
    }
}
