using System.Collections.Generic;

public interface ICharacterService
{
    IReadOnlyList<Character> Characters { get; }
    Character Selected { get; }
    void Select(Character character);
    IReadOnlyList<Character> GetAllCharacters();
    ICharacterView GetCharacterView(Character character);
    bool IsDriver(Character character);
} 