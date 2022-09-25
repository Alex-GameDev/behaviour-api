using System.Collections;
using System.Collections.Generic;
using System.Timers;

public class TimerPerception : Perception {

    #region variables

    public float Time { get; }

    private bool launched;
    private Timer timer;

    #endregion variables

    public TimerPerception(float time, BehaviourEngine behaviourEngine) : base()
    {
        this.Time = time;
        this.launched = false;
        this.timer = new Timer(time * 1000);

        base.behaviourEngine = behaviourEngine;
    }

    public override bool Check()
    {
        if(!this.launched) {
            this.timer.Enabled = true;
            this.timer.Elapsed += TimerEvent;
            this.timer.Start();
        }

        return launched;
    }

    private void TimerEvent(object sender, ElapsedEventArgs e)
    {
        this.launched = true;
    }

    public override void Reset()
    {
        this.timer.Enabled = false;
        this.launched = false;
        this.timer.Stop();
    }
}