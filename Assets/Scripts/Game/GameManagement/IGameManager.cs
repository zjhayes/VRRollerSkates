using UnityEngine;

public interface IGameManager : IService
{
    public XRManager XR { get; }
    public PlayerManager Player { get; }
}
