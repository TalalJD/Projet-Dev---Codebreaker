public abstract class StateBaseArgs
{
    public float nextStateDelay = 0.3f;
}

public class WarpArgs : StateBaseArgs
{
    public int Times = 1;
    public bool OnPlayer;
    public bool Middle;
    public bool CornerOnly;
    public bool Spawn;
    public bool SkipNextState;
   
}

public class ConeArgs : StateBaseArgs
{
  
    public float Speed = 10f;
}

public class ParabolicMissileArgs : StateBaseArgs
{
    
}

public class HomingMissileArgs : StateBaseArgs
{
   
}

public class LaserArgs : StateBaseArgs
{
    public float WarningTime = 1f;
    public float LockDelay = 0.2f;
    public float FireDuration = 1.5f;

}

public class IdleArgs : StateBaseArgs
{
    public float Duration = 1f;
}

public class ExplosionArgs : StateBaseArgs
{
  
}