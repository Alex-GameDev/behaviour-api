using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class KeyPerception : Perception
{
    #region variables
    private bool pressed;
    private ConsoleKey key;
    #endregion variables

    public KeyPerception(ConsoleKey key, BehaviourEngine behaviourEngine) : base()
    {
        this.pressed = false;
        this.key = key;
    }

    public override bool Check()
    {
        if (Console.ReadKey().Key.Equals(this.key))
        {
            this.pressed = true;
        }
        return this.pressed;
    }

    public override void Reset()
    {
        this.pressed = false;
    }

}

