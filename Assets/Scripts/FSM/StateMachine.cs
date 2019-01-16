using PlayerState;

public class StateMachine<T>
{
    private T owner;
    public State<T> currentState;

    public StateMachine(T owner)
    {
        this.owner = owner;
    }

    public void Update()
    {
        if (currentState != null) currentState.Execute(owner);
    }

    public void ChangeState(State<T> state)
    {
        currentState.Exit(owner);
        currentState = state;
        currentState.Enter(owner);
    }
}
