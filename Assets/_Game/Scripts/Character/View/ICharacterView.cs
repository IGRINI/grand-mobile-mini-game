using UnityEngine;

public interface ICharacterView
{
    void Initialize(Character model);
    void SetSeatIndex(int seatIndex);
    Transform WeaponPivot { get; }
    Transform HitTarget { get; }
    Animator Animator { get; }
    Transform ModelTransform { get; }
} 