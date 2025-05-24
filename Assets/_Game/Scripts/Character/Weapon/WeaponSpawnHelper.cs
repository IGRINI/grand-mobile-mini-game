using UnityEngine;

public static class WeaponSpawnHelper
{
    public static IWeaponView SpawnWeapon(Transform pivot, WeaponData data)
    {
        if (pivot == null || data == null || data.WeaponPrefab == null) return null;

        for (int i = pivot.childCount - 1; i >= 0; i--)
            Object.Destroy(pivot.GetChild(i).gameObject);

        var instance = Object.Instantiate(data.WeaponPrefab, pivot);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;

        return instance.GetComponent<IWeaponView>();
    }
} 