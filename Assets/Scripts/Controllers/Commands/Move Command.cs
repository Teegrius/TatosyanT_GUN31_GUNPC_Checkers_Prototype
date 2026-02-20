using UnityEngine;

public class MoveCommand : IGameplayCommand
{
    public void Interact(Cell cell)
    {
        UnityEngine.Object.FindObjectOfType<BattleController>().HandleCellClick(cell);
    }
}