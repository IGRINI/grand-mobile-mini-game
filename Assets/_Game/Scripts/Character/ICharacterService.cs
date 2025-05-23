using System.Collections.Generic;

public interface ICharacterService
{
    IReadOnlyList<Character> Characters { get; }
    Character Selected { get; }
    void Select(Character character);
} 