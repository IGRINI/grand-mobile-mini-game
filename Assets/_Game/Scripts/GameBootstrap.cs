using System.Linq;
using UnityEngine;
using Zenject;

public class GameBootstrap : MonoBehaviour
{
    [Inject] private ICharacterService _characterService;

    private void Start()
    {
        if (_characterService.Characters != null && _characterService.Characters.Count > 0)
        {
            // Сажаем первого персонажа в первый слот
            _characterService.Select(_characterService.Characters.First());
        }
    }
} 