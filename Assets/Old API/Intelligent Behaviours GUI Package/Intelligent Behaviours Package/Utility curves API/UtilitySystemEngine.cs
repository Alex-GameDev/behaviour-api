using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UtilitySystemEngine : BehaviourEngine
{
    #region variables

    public UtilityAction ActiveAction;
    public List<UtilityAction> actions;

    private float inertia;
    private bool DEBUG = false;
    #endregion

    #region constructors

    /// <summary>
    /// Creates a UtilityCurvesEngine that CANNOT be a submachine
    /// </summary>
    /// <param name="inertia">Value provided to avoid inertia problem when switching between actions.
    /// Recommended value: between 1.2 and 1.3</param>
    public UtilitySystemEngine(float inertia = 1.3f)
    {
        base.transitions = new Dictionary<string, Transition>();
        base.states = new Dictionary<string, State>();
        this.actions = new List<UtilityAction>();
        base.IsSubMachine = false;

        entryState = new State("Entry_Machine", this);
        this.actualState = entryState;
        states.Add(entryState.Name, entryState);

        this.inertia = inertia;

        Active = true;
    }

    /// <summary>
    /// Creates a UtilityCurvesEngine
    /// </summary>
    /// <param name="isSubmachine">Is this a submachine?</param>
    /// <param name="inertia">Value provided to avoid inertia problem when switching between actions.
    /// Recommended value: between 1.2 and 1.3</param>
    public UtilitySystemEngine(bool isSubmachine, float inertia = 1.3f)
    {
        base.transitions = new Dictionary<string, Transition>();
        base.states = new Dictionary<string, State>();
        this.actions = new List<UtilityAction>();
        base.IsSubMachine = isSubmachine;

        entryState = new State("Entry_Machine", this);
        this.actualState = entryState;
        states.Add(entryState.Name, entryState);

        this.inertia = inertia;

        Active = (isSubmachine) ? false : true;
    }

    #endregion

    #region methods

    public void Update()
    {
        if(actions.Count != 0)
        {
            if (ActiveAction != null)
            {
                if (!Active && !ActiveAction.HasSubmachine) return;

                // Para evitar problemas, si el estado actual es distinto a la acción, se realiza
                // un reseteo de la máquina. Esto ocurre al volver de un BT
                if (ActiveAction.utilityState != this.actualState)
                {
                    this.Reset();
                }

                int maxIndex = getMaxUtilityIndex();
                int activeActionIndex = this.actions.IndexOf(ActiveAction);

                if (maxIndex != activeActionIndex)
                {
                    ExitTransition(this.actions[maxIndex]);
                }
                ActiveAction.Update(); 
            } else if(Active && ActiveAction == null) {
                int maxIndex = getMaxUtilityIndex();
                ExitTransition(this.actions[maxIndex]);
                ActiveAction.Update(); 
            }
        }
        
        
    }

    public override void Reset()
    {
        this.ActiveAction = null;
    }

    private int getMaxUtilityIndex()
    {
        int actionsSize = this.actions.Count;
        List<float> utilities = new List<float>(actionsSize);

        for (int i = 0; i < actionsSize; i++)
        {
            if(this.actions[i] == ActiveAction)
            {
                // INERCIA
                if (this.actions[i].getUtility() * inertia > 1.0f) { utilities.Add(1.0f); }
                else { utilities.Add(this.actions[i].getUtility() * inertia); }
                
            } else
            {
                utilities.Add(this.actions[i].getUtility());
            }
            
        }

        // Si hay dos utilidades máximas iguales, se queda con la primera que encuentre
        return utilities.IndexOf(utilities.Max());
    }

    #endregion

    #region transitions
    /// <summary>
    /// Exits from the actual utilityAction to go to another one
    /// </summary>
    public void ExitTransition(UtilityAction action)
    {
        string last = this.actualState.Name;
        if (ActiveAction != null)
        {
            if (ActiveAction.HasSubmachine)
            {
                this.ActiveAction.subMachine.ResetPerceptionsActiveState();
                new Transition("Max_Utility_Transition", this.ActiveAction.subMachine.actualState, new PushPerception(this),
                    this.GetEntryState(), this, this.ActiveAction.subMachine)
                    .FireTransition();
            } else
            {
                new Transition("Max_Utility_Transition", this.actualState, new PushPerception(this), action.utilityState, this)
                    .FireTransition();
            }
        } else
        {
            new Transition("Max_Utility_Transition", this.actualState, new PushPerception(this), action.utilityState, this)
                    .FireTransition();
        }

        this.ActiveAction = action;

        //DEBUGGING
        if (DEBUG)
        {
            //EXIT TRANSITION
            Console.WriteLine("[DEBUG] ExitTransition - New active action: " + action.utilityState.Name +
            ". Active State: " + this.actualState.Name + ". Last active action: " + last);
            Console.WriteLine("-------------------------");

            //UTILITIES
            Console.WriteLine("[DEBUG] Utilities: ");
            foreach(UtilityAction a in this.actions)
            {
                if(a == ActiveAction)
                {
                    Console.Write(a.utilityState.Name + " active action utility " + a.getUtility());
                    if (a.getUtility() * inertia > 1.0f) { Console.WriteLine(" inertia 1.0f"); }
                    else { Console.WriteLine(" inertia " + a.getUtility() * inertia); }
                } else
                {
                    Console.WriteLine(a.utilityState.Name + " utility " + a.getUtility());
                }
                
            }
            Console.WriteLine("[DEBUG] FINISHED UTILITIES DEBUG");
            Console.WriteLine("--------------------------------");
        }
    }

    #endregion

    #region create actions

    /// <summary>
    /// Creates a new basic <see cref="UtilityAction"/> in the utility curves engine
    /// </summary>
    /// <param name="name">The name of the utility action</param>
    /// <param name="action">The action the utility action will execute</param>
    /// <param name="factor">The factor that will have the Utility Action</param>
    public UtilityAction CreateUtilityAction(string name, Action action, Factor factor)
    {
        if (!states.ContainsKey(name))
        {
            UtilityAction uAction = new UtilityAction(name, action, factor, this);
            actions.Add(uAction);
            states.Add(name, uAction.utilityState);

            return uAction;
        }
        else
        {
            throw new DuplicateWaitObjectException(name, "The utility action already exists in the utility engine");
        }
    }

    /// <summary>
    /// Creates a new specific <see cref="UtilityAction"/> in the utility curves engine that exits to the
    /// <see cref="LeafNode"/> that contains the <see cref="UtilitySystemEngine"/> with the ReturnValues
    /// provided in the constructor.
    /// </summary>
    /// <param name="name">The name of the utility action</param>
    /// <param name="factor">The factor that will have the Utility Action</param>
    /// <param name="valueReturned">The <see cref="ReturnValues"/> returned to the <see cref="LeafNode"/>.</param>
    /// <param name="behaviourTreeEngine">The <see cref="BehaviourTreeEngine"/> that contains the <see cref="UtilitySystemEngine"/>.</param>
    public UtilityAction CreateUtilityAction(string name, Factor factor, ReturnValues valueReturned, BehaviourTreeEngine behaviourTreeEngine)
    {
        if (!states.ContainsKey(name))
        {
            UtilityAction uAction = new UtilityAction(name, factor, valueReturned, this, behaviourTreeEngine);
            actions.Add(uAction);
            states.Add(name, uAction.utilityState);

            return uAction;
        }
        else
        {
            throw new DuplicateWaitObjectException(name, "The utility action already exists in the utility engine");
        }
    }

    /// <summary>
    /// Creates a new specific <see cref="UtilityAction"/> in the utility curves engine that exits to the
    /// <see cref="LeafNode"/> that contains the <see cref="UtilitySystemEngine"/> when the function valueReturned
    /// returns something different than "Running".
    /// </summary>
    /// <param name="name">The name of the utility action</param>
    /// <param name="factor">The factor that will have the Utility Action</param>
    /// <param name="ac">The action that the <see cref="UtilityAction"/> will execute.</param>
    /// <param name="valueReturned">The <see cref="ReturnValues"/> returned to the <see cref="LeafNode"/>. It's used to wait for a 
    /// "Succeed" or "Failed".</param>
    /// <param name="behaviourTreeEngine">The <see cref="BehaviourTreeEngine"/> that contains the <see cref="UtilitySystemEngine"/>.</param>
    public UtilityAction CreateUtilityAction(string name, Factor factor, Action ac, Func<ReturnValues> valueReturned, BehaviourTreeEngine behaviourTreeEngine)
    {
        if (!states.ContainsKey(name))
        {
            UtilityAction uAction = new UtilityAction(name, factor, ac, valueReturned, this, behaviourTreeEngine);
            actions.Add(uAction);
            states.Add(name, uAction.utilityState);

            return uAction;
        }
        else
        {
            throw new DuplicateWaitObjectException(name, "The utility action already exists in the utility engine");
        }
    }

    #endregion

    #region create sub-state machines

    /// <summary>
    /// Adds a type of <see cref="UtilityAction"/> with a sub-behaviour engine in it and its transition to the entry state
    /// </summary>
    /// <param name="actionName">The name of the action</param>
    /// <param name="factor">The factor that gives the utility value to the action</param>
    /// <param name="subBehaviourEngine">The sub-behaviour tree inside the </param>
    public UtilityAction CreateSubBehaviour(string actionName, Factor factor, BehaviourEngine subBehaviourEngine)
    {
        if (!states.ContainsKey(actionName))
        {
            State stateTo = subBehaviourEngine.GetEntryState();
            State state = new State(actionName, subBehaviourEngine.GetState("Entry_Machine"), stateTo, subBehaviourEngine, this);
            UtilityAction utilAction = new UtilityAction(state, factor, this, subBehaviourEngine);
            states.Add(utilAction.utilityState.Name, utilAction.utilityState);
            actions.Add(utilAction);

            return utilAction;
        } else
        {
            throw new DuplicateWaitObjectException(actionName, "The utility action already exists in the utility engine");
        }  
    }

    /// <summary>
    /// Adds a type of <see cref="UtilityAction"/> with a sub-behaviour engine in it and its transition to the specified state
    /// </summary>
    /// <param name="actionName">The name of the action</param>
    /// <param name="factor">The factor that gives the utility value to the action</param>
    /// <param name="subBehaviourEngine">The sub-behaviour tree inside the </param>
    /// <param name="stateTo">The name of the state where the sub-state machine will enter</param>
    public UtilityAction CreateSubBehaviour(string actionName, Factor factor, BehaviourEngine subBehaviourEngine, State stateTo)
    {
        if (!states.ContainsKey(actionName))
        {
            State state = new State(actionName, subBehaviourEngine.GetState("Entry_Machine"), stateTo, subBehaviourEngine, this);
            UtilityAction utilAction = new UtilityAction(state, factor, this, subBehaviourEngine);
            states.Add(utilAction.utilityState.Name, utilAction.utilityState);
            actions.Add(utilAction);

            return utilAction;
        }
        else
        {
            throw new DuplicateWaitObjectException(actionName, "The utility action already exists in the utility engine");
        }
    }

    #endregion create sub-state machines

}

