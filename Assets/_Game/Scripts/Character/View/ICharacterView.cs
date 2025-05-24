using UnityEngine;

public interface ICharacterView
{
    void Initialize(Character model);
    Transform WeaponPivot { get; }
    Transform HitTarget { get; }
} 