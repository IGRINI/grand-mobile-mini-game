using UnityEngine;

public class WeaponView : MonoBehaviour, IWeaponView
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform secondHandPoint;

    public Transform SpawnPoint => spawnPoint;
    public Transform SecondHandPoint => secondHandPoint;
} 