using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class BattleController : MonoBehaviour
{
    private InputActions inputActions;
    private Battlefield battlefield;
    public PlayerTurn currentTurn = PlayerTurn.White;
    private GameState state = GameState.SelectPiece;
    private Unit selectedUnit;
    private List<Cell> possibleMoves = new List<Cell>();
    private bool mustCapture;

    void Awake()
    {
        inputActions = new InputActions();
        inputActions.Gameplay.Enable();
        inputActions.Gameplay.Cancel.performed += OnCancel;
        inputActions.Gameplay.Confirm.performed += OnConfirm;
        inputActions.Gameplay.Restart.started += OnRestartStart;
        inputActions.Gameplay.Restart.canceled += OnRestartCancel;
        battlefield = FindObjectOfType<Battlefield>();
    }

    private float restartHoldTime = 0f;
    private const float restartDuration = 2f;
    private bool isRestarting;

    private void OnRestartStart(InputAction.CallbackContext ctx)
    {
        isRestarting = true;
    }

    private void OnRestartCancel(InputAction.CallbackContext ctx)
    {
        isRestarting = false;
        restartHoldTime = 0f;
    }

    void Update()
    {
        if (isRestarting)
        {
            restartHoldTime += Time.deltaTime;
            if (restartHoldTime >= restartDuration)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        }
    }

    public void HandleCellClick(Cell cell)
    {
        if (state == GameState.Visualizing) return;

        if (state == GameState.SelectPiece)
        {
            if (cell.unit != null && cell.unit.team == (Team)currentTurn)
            {
                if (mustCapture && !CanCapture(cell.unit)) return;

                selectedUnit = cell.unit;
                cell.HighlightSelected(true);
                CalculatePossibleMoves(selectedUnit);
                state = GameState.SelectMove;
            }
        }
        else if (state == GameState.SelectMove)
        {
            if (possibleMoves.Contains(cell))
            {
                ExecuteMove(selectedUnit, cell);
            }
            else
            {
                ResetSelection();
            }
        }
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (state != GameState.Visualizing && !mustCapture)
        {
            ResetSelection();
        }
    }

    private void OnConfirm(InputAction.CallbackContext ctx)
    {
        
    }

    private void ResetSelection()
    {
        if (selectedUnit != null && selectedUnit.cell != null)
        {
            selectedUnit.cell.HighlightSelected(false);
            selectedUnit = null;
        }
        foreach (var move in possibleMoves)
        {
            move.HighlightPossibleMove(false);
        }
        possibleMoves.Clear();
        state = GameState.SelectPiece;
    }

    private void CalculatePossibleMoves(Unit unit)
    {
        possibleMoves.Clear();

        Vector2Int pos = unit.cell.position;
        int dir = unit.team == Team.White ? -1 : 1;
        bool isKing = unit.type == PieceType.King;

        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(1, dir),
            new Vector2Int(-1, dir)
        };
        if (isKing)
        {
            directions.Add(new Vector2Int(1, -dir));
            directions.Add(new Vector2Int(-1, -dir));
        }

        foreach (var d in directions)
        {
            AddMovesInDirection(unit, pos, d, isKing);
        }

        foreach (var move in possibleMoves)
        {
            move.HighlightPossibleMove(true);
        }
    }

    private void AddMovesInDirection(Unit unit, Vector2Int pos, Vector2Int dir, bool isKing)
    {
        Vector2Int nextPos = pos + dir;
        Cell nextCell = battlefield.GetCell(nextPos);
        if (nextCell == null) return;

        if (nextCell.unit == null)
        {
            possibleMoves.Add(nextCell);
            if (isKing) AddMovesInDirection(unit, nextPos, dir, true);
        }
        else if (nextCell.unit.team != unit.team)
        {
            Vector2Int jumpPos = nextPos + dir;
            Cell jumpCell = battlefield.GetCell(jumpPos);
            if (jumpCell != null && jumpCell.unit == null)
            {
                possibleMoves.Add(jumpCell);
                if (isKing) AddMovesInDirection(unit, jumpPos, dir, true);
            }
        }
    }

    private void ExecuteMove(Unit unit, Cell target)
    {
        state = GameState.Visualizing;
        ResetSelection();

        Vector2Int start = unit.cell.position;
        Vector2Int end = target.position;
        Vector2Int diff = end - start;
        bool isCapture = Mathf.Abs(diff.x) > 1 || Mathf.Abs(diff.y) > 1;

        if (isCapture)
        {
            Vector2Int mid = start + diff / 2;
            Cell midCell = battlefield.GetCell(mid);
            if (midCell.unit != null)
            {
                Destroy(midCell.unit.gameObject);
                midCell.unit = null;
            }
        }

        unit.cell.unit = null;
        unit.cell = target;
        target.unit = unit;
        unit.transform.position = target.transform.position + Vector3.up * 0.5f;

        CheckPromotion(unit);

        // Проверяем цепной ход ТОЛЬКО если был взятие
        bool canChain = isCapture && CheckForChainCaptures(unit);

        if (canChain)
        {
            mustCapture = true;
            selectedUnit = unit;
            CalculatePossibleMoves(unit);
            state = GameState.SelectMove;
        }
        else
        {
            mustCapture = false;
            SwitchTurn();
            state = GameState.SelectPiece;
        }
    }

    private void CheckPromotion(Unit unit)
    {
        int endRow = unit.team == Team.White ? 0 : 7;
        if (unit.cell.position.y == endRow && unit.type == PieceType.Checker)
        {
            unit.PromoteToKing();
        }
    }

    private bool CheckForChainCaptures(Unit unit)
    {
        CalculatePossibleMoves(unit);
        bool hasCapture = possibleMoves.Exists(m => Mathf.Abs(m.position.x - unit.cell.position.x) > 1);
        possibleMoves.Clear();
        return hasCapture;
    }

    private void SwitchTurn()
    {
        currentTurn = currentTurn == PlayerTurn.White ? PlayerTurn.Black : PlayerTurn.White;
    }

    private bool CanCapture(Unit unit)
    {
        CalculatePossibleMoves(unit);
        bool can = possibleMoves.Exists(m => Mathf.Abs(m.position.x - unit.cell.position.x) > 1);
        possibleMoves.Clear();
        return can;
    }
}