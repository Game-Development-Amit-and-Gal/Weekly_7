using System;
using System.Collections.Generic;
using UnityEngine;

using State = UnityEngine.MonoBehaviour;
using Transition = System.Tuple<UnityEngine.MonoBehaviour, System.Func<bool>, UnityEngine.MonoBehaviour>;

/// <summary>
/// Generic finite state machine implementation.
/// Each state is represented by a MonoBehaviour component attached
/// to the same GameObject as this StateMachine.
/// 
/// The state machine:
/// - keeps track of registered states,
/// - evaluates transitions every frame,
/// - enables only the active state and disables all others.
/// 
/// The first state added is automatically set as the initial active state.
/// </summary>
public class StateMachine : MonoBehaviour
{
    /// <summary>
    /// List of all registered states.
    /// Each state is a MonoBehaviour component.
    /// </summary>
    private List<State> states = new List<State>();

    /// <summary>
    /// List of transitions between states.
    /// Each transition consists of:
    /// - a source state,
    /// - a condition function,
    /// - a destination state.
    /// </summary>
    private List<Transition> transitions = new List<Transition>();

    /// <summary>
    /// Currently active state.
    /// Only this state is enabled at runtime.
    /// </summary>
    private State activeState = null;

    /// <summary>
    /// Switches the active state of the state machine.
    /// Disables the previous state and enables the new one.
    /// </summary>
    /// <param name="newActiveState">State to activate.</param>
    public void GoToState(State newActiveState)
    {
        if (activeState == newActiveState) return;

        if (activeState != null)
            activeState.enabled = false;

        activeState = newActiveState;
        activeState.enabled = true;

        Debug.Log("Going to state " + activeState);
    }

    /// <summary>
    /// Registers a new state with the state machine.
    /// The first state added becomes the initial active state.
    /// </summary>
    /// <param name="newState">State component to add.</param>
    /// <returns>Reference to this StateMachine (for chaining).</returns>
    public StateMachine AddState(State newState)
    {
        states.Add(newState);
        return this;
    }

    /// <summary>
    /// Registers a transition between two states.
    /// The transition is evaluated only when the source state is active.
    /// </summary>
    /// <param name="fromState">State from which the transition starts.</param>
    /// <param name="condition">Condition that must return true to trigger the transition.</param>
    /// <param name="toState">State to activate when the condition is met.</param>
    /// <returns>Reference to this StateMachine (for chaining).</returns>
    public StateMachine AddTransition(State fromState, Func<bool> condition, State toState)
    {
        transitions.Add(new Transition(fromState, condition, toState));
        return this;
    }

    /// <summary>
    /// Initializes the state machine by disabling all states
    /// and activating the first registered state.
    /// </summary>
    private void Start()
    {
        foreach (State state in states)
        {
            state.enabled = false;
        }

        GoToState(states[0]);
    }

    /// <summary>
    /// Evaluates all transitions each frame.
    /// If a transition condition evaluates to true for the active state,
    /// the state machine switches to the target state.
    /// </summary>
    private void Update()
    {
        foreach (Transition transition in transitions)
        {
            if (transition.Item1 == activeState)
            {
                if (transition.Item2() == true)
                {
                    GoToState(transition.Item3);
                    break;
                }
            }
        }
    }
}
