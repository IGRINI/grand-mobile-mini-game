using UnityEngine;
using Zenject;

public class UpgradeEffectUpdater : MonoBehaviour
{
    private UpgradeEffectApplier _upgradeEffectApplier;
    
    [Inject]
    public void Construct(UpgradeEffectApplier upgradeEffectApplier)
    {
        _upgradeEffectApplier = upgradeEffectApplier;
    }
    
    private void Update()
    {
        _upgradeEffectApplier?.Update();
    }
} 