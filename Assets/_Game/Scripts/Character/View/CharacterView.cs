using UnityEngine;

public class CharacterView : MonoBehaviour, ICharacterView
{
    private Character _model;
    [SerializeField] private Transform weaponPivot;
    public Transform WeaponPivot => weaponPivot;

    public void Initialize(Character model)
    {
        _model = model;
        gameObject.name = model.Name;
        // Здесь можно добавить логику отображения: анимации, эффекты и т.д.
    }
} 