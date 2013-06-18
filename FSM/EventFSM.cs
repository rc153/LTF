using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.FSM
{
    // Event-driven finite-state machine
    public class EventFSM<TState>
    {
        private Dictionary<TState, State<TState>> map = new Dictionary<TState, State<TState>>();
        private State<TState> currentState;

        public EventFSM(TState startingState)
        {
            foreach(TState state in Enum.GetValues(typeof(TState)))
            {
                map.Add(state, new State<TState>(this));
            }

            Goto(startingState);
        }

        public State<TState> In(TState state)
        {
            return map[state];
        }

        internal void Goto(TState state)
        {
            if (currentState != null)
                currentState.Active = false;

            currentState = map[state];
            currentState.Active = true;
        }
    }

    public class Transition<TState>
    {
        private EventFSM<TState> fsm;
        private Action todo;
        private TState target;

        internal Transition(EventFSM<TState> fsm)
        {
            this.fsm = fsm;
        }

        public Transition<TState> Execute(Action evt)
        {
            todo = evt;
            return this;
        }

        public Transition<TState> Goto(TState state)
        {
            target = state;
            return this;
        }

        internal void Execute()
        {
            if (todo != null)
                todo();

            fsm.Goto(target);
        }
    }

    public class State<TState>
    {
        private EventFSM<TState> fsm;
        internal bool Active { get; set; }

        internal State(EventFSM<TState> fsm)
        {
            this.fsm = fsm;
        }

        public Transition<TState> On(ref Action evt)
        {
            Transition<TState> trans = new Transition<TState>(fsm);
            evt += () => { if(Active) trans.Execute(); };
            return trans;
        }
    }
}
