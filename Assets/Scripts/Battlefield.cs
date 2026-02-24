using UnityEngine;

public class Battlefield : MonoBehaviour
{
    public Cell[,] cells = new Cell[8, 8];
    public Material blackMaterial;
    public Material whiteMaterial;
    public GameObject cellPrefab;
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;

    void Start()
    {
        GenerateBoard();
        PlacePieces();
    }

    private void GenerateBoard()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                GameObject cellObj = Instantiate(cellPrefab, new Vector3(x * 2, 0, y * 2), Quaternion.identity, transform);
                cellObj.transform.localScale = new Vector3(2, 0.1f, 2);
                Cell cell = cellObj.AddComponent<Cell>();
                cell.position = new Vector2Int(x, y);
                cell.isBlack = (x + y) % 2 == 1;
                cell.GetComponent<Renderer>().material = cell.isBlack ? blackMaterial : whiteMaterial;
                
                cell.Init();

                cells[x, y] = cell;
            }
        }
    }

    private void PlacePieces()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if ((x + y) % 2 == 1)
                {
                    CreatePiece(Team.Black, new Vector2Int(x, y));
                }
            }
        }

        for (int y = 5; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if ((x + y) % 2 == 1)
                {
                    CreatePiece(Team.White, new Vector2Int(x, y));
                }
            }
        }
    }

    private void CreatePiece(Team team, Vector2Int pos)
    {
        Cell cell = cells[pos.x, pos.y];
        GameObject prefab = team == Team.White ? whitePiecePrefab : blackPiecePrefab;
        GameObject pieceObj = Instantiate(prefab, cell.transform.position + Vector3.up * 0.5f, Quaternion.identity);
        Unit unit = pieceObj.AddComponent<Unit>();
        unit.team = team;
        unit.cell = cell;
        cell.unit = unit;
    }

    public Cell GetCell(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8)
        {
            return cells[pos.x, pos.y];
        }
        return null;
    }
}