public class WarpArgs
{
    public int Times = 1;
    public bool OnPlayer;
    public bool Middle;
    public bool CornerOnly;
    public bool Spawn;
    public bool SkipNextState;
}

public class ConeArgs
{
    public int Count = 1;
    public float Delay = 0.3f;
}

public class ParabolicMissileArgs
{
    public int Count = 1;
    public float Delay = 1f;
}

public class LaserArgs
{
    public float WarningTime = 1f;
    public float LockDelay = 0.2f;
    public float FireDuration = 1.5f;
}

public class IdleArgs
{
    public float Duration = 1f;
}
