using UnityEngine;
using Zenject;

public class UpgradeInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        var availableUpgrades = CreateAllUpgrades();
        
        Container.Bind<UpgradeData[]>().FromInstance(availableUpgrades).AsSingle();
        Container.Bind<UpgradeEffectApplier>().AsSingle().NonLazy();
        Container.Bind<IUpgradeEffectProvider>().To<UpgradeEffectApplier>().FromResolve();
        Container.BindInterfacesAndSelfTo<UpgradeService>().AsSingle().NonLazy();
        Container.Bind<UpgradeEffectUpdater>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
    }
    
    private UpgradeData[] CreateAllUpgrades()
    {
        return new UpgradeData[]
        {
            // Лечение
            new UpgradeData(
                "Аптечка",
                "Восстанавливает здоровье всем персонажам",
                UpgradeType.HealthRestore,
                25f,
                false,
                3,
                10f,
                1,
                999,
                "Восстанавливает {value} здоровья всем персонажам",
                Color.green
            ),
            
            // Добавление пассажира
            new UpgradeData(
                "Новый союзник",
                "Добавляет нового пассажира в команду",
                UpgradeType.AddPassenger,
                1f,
                false,
                2,
                1f,
                3,
                999,
                "Добавляет нового пассажира",
                Color.blue
            ),
            
            // Защитные апгрейды
            new UpgradeData(
                "Броня",
                "Уменьшает входящий урон",
                UpgradeType.ReduceIncomingDamage,
                10f,
                true,
                5,
                5f,
                1,
                999,
                "Уменьшает входящий урон на {value}",
                Color.cyan
            ),
            
            new UpgradeData(
                "Защита команды",
                "Уменьшает урон по персонажам",
                UpgradeType.ReduceDamageToCharacters,
                15f,
                true,
                4,
                10f,
                2,
                999,
                "Уменьшает урон по персонажам на {value}",
                Color.cyan
            ),
            
            new UpgradeData(
                "Дополнительная броня",
                "Добавляет очки брони",
                UpgradeType.AddArmor,
                5f,
                false,
                5,
                3f,
                1,
                999,
                "Добавляет {value} брони",
                Color.gray
            ),
            
            // Боевые апгрейды
            new UpgradeData(
                "Мощное оружие",
                "Увеличивает урон персонажей",
                UpgradeType.IncreaseCharacterDamage,
                20f,
                true,
                5,
                15f,
                1,
                999,
                "Увеличивает урон персонажей на {value}",
                Color.red
            ),
            
            new UpgradeData(
                "Таран",
                "Увеличивает урон от сбивания",
                UpgradeType.IncreaseRamDamage,
                25f,
                true,
                4,
                20f,
                2,
                999,
                "Увеличивает урон от сбивания на {value}",
                Color.red
            ),
            
            new UpgradeData(
                "Скорострельность",
                "Увеличивает скорость стрельбы",
                UpgradeType.IncreaseFireRate,
                15f,
                true,
                5,
                10f,
                1,
                999,
                "Увеличивает скорость стрельбы на {value}",
                Color.orange
            ),
            
            new UpgradeData(
                "Дальнобойность",
                "Увеличивает дальность атаки",
                UpgradeType.IncreaseRange,
                20f,
                true,
                4,
                15f,
                2,
                999,
                "Увеличивает дальность атаки на {value}",
                Color.orange
            ),
            
            new UpgradeData(
                "Критический удар",
                "Увеличивает шанс критического удара",
                UpgradeType.IncreaseCriticalChance,
                5f,
                true,
                5,
                3f,
                3,
                999,
                "Увеличивает шанс критического удара на {value}",
                Color.yellow
            ),
            
            new UpgradeData(
                "Критический урон",
                "Увеличивает критический урон",
                UpgradeType.IncreaseCriticalDamage,
                25f,
                true,
                4,
                20f,
                4,
                999,
                "Увеличивает критический урон на {value}",
                Color.yellow,
                new UpgradeType[] { UpgradeType.IncreaseCriticalChance }
            ),
            
            // Скорость и мобильность
            new UpgradeData(
                "Турбо двигатель",
                "Увеличивает максимальную скорость",
                UpgradeType.IncreaseMaxSpeed,
                15f,
                true,
                5,
                10f,
                1,
                999,
                "Увеличивает максимальную скорость на {value}",
                Color.magenta
            ),
            
            new UpgradeData(
                "Быстрые ноги",
                "Увеличивает скорость движения персонажей",
                UpgradeType.IncreaseMovementSpeed,
                10f,
                true,
                4,
                8f,
                2,
                999,
                "Увеличивает скорость движения на {value}",
                Color.magenta
            ),
            
            // Утилитарные апгрейды
            new UpgradeData(
                "Опытный боец",
                "Увеличивает получение опыта",
                UpgradeType.IncreaseExperienceGain,
                20f,
                true,
                3,
                15f,
                1,
                999,
                "Увеличивает получение опыта на {value}",
                Color.white
            ),
            
            new UpgradeData(
                "Регенерация",
                "Увеличивает регенерацию здоровья",
                UpgradeType.IncreaseHealthRegeneration,
                2f,
                false,
                5,
                1f,
                3,
                999,
                "Увеличивает регенерацию здоровья на {value}/сек",
                Color.green
            ),
            
            new UpgradeData(
                "Быстрая перезарядка",
                "Уменьшает перезарядку оружия",
                UpgradeType.ReduceWeaponCooldown,
                15f,
                true,
                4,
                10f,
                2,
                999,
                "Уменьшает перезарядку оружия на {value}",
                Color.orange
            )
        };
    }
} 