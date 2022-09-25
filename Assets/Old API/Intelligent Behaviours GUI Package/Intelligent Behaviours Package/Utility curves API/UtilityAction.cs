using System;
using System.Collections.Generic;
using System.Linq;

public class UtilityAction
{
    #region variables

    public State utilityState;
    public bool HasSubmachine;
    public BehaviourEngine subMachine;
    private Factor factor;
    private UtilitySystemEngine uCurvesEngine;

    private Func<ReturnValues> valueReturned;
    private BehaviourTreeEngine bt;

    #endregion

    //Accion normal
    public UtilityAction(string name, Action action, Factor factor, UtilitySystemEngine utilityCurvesEngine)
    {
        this.HasSubmachine = false;
        this.factor = factor;
        this.utilityState = new State(name, action, utilityCurvesEngine);
        this.uCurvesEngine = utilityCurvesEngine;
    }

    //Acción con submáquina
    public UtilityAction(State utilState, Factor factor, UtilitySystemEngine uSystemEngine, BehaviourEngine subMachine)
    {
        this.HasSubmachine = true;
        this.utilityState = utilState;
        this.factor = factor;
        this.uCurvesEngine = uSystemEngine;
        this.subMachine = subMachine;
    }

    
    //Acción de salida para árboles de comportamiento (sale a nodo hoja instantáneamente)
    public UtilityAction(string name, Factor factor, ReturnValues valueReturned, UtilitySystemEngine utilityCurvesEngine, BehaviourTreeEngine behaviourTreeEngine)
    {
        this.HasSubmachine = false;
        
        Action action = () =>
        {
            new Transition("Exit_Action_Transition", this.utilityState, new PushPerception(this.uCurvesEngine), this.uCurvesEngine.NodeToReturn,
                            valueReturned, behaviourTreeEngine, this.uCurvesEngine)
                            .FireTransition();
        };
        this.utilityState = new State(name, action, utilityCurvesEngine);
        this.factor = factor;
        this.uCurvesEngine = utilityCurvesEngine;
    }

    //Acción de salida para árboles de comportamiento (sale a nodo hoja) con una acción a ejecutar y esperando a que el valor devuelto sea diferente de Running
    public UtilityAction(string name, Factor factor, Action ac, Func<ReturnValues> valueReturned, UtilitySystemEngine utilityCurvesEngine, BehaviourTreeEngine behaviourTreeEngine)
    {
        this.HasSubmachine = false;
        this.utilityState = new State(name, ac, utilityCurvesEngine);
        this.factor = factor;
        this.uCurvesEngine = utilityCurvesEngine;
        this.valueReturned = valueReturned;
        this.bt = behaviourTreeEngine;
    }

    public float getUtility()
    {
        float utilityValue = factor.getValue();
        if (utilityValue > 1.0f) return 1.0f;
        if (utilityValue < 0.0f) return 0.0f;
        return utilityValue;
    }

    public void Update()
    {
        // Valor devuelto al árbol de comportamientos, una vez devuelva un valor distinto de Running
        if(this.valueReturned != null)
        {
            ReturnValues returnValue = this.valueReturned();
            if (returnValue != ReturnValues.Running)
            {
                new Transition("Exit_Action_Transition", this.utilityState, new PushPerception(this.uCurvesEngine), this.uCurvesEngine.NodeToReturn,
                                returnValue, this.bt, this.uCurvesEngine)
                                .FireTransition();
            }
        }
        
    }

}
