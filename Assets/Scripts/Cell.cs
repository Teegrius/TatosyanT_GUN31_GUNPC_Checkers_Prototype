using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Vector2Int position;
    public Unit unit;
    public bool isBlack;

    private static readonly int COLOR_ID = Shader.PropertyToID("_Color");

    private Color originalColor;
    private Renderer renderer;

    private bool isHovered;
    private bool isSelected;
    private bool isPossibleMove;

    public void Init()
    {
        renderer = GetComponent<Renderer>();
        originalColor = renderer.material.color;
    }

    public void HighlightHover(bool enable)
    {
        isHovered = enable;
        UpdateHighlight();
    }

    public void HighlightSelected(bool enable)
    {
        isSelected = enable;
        UpdateHighlight();
    }

    public void HighlightPossibleMove(bool enable)
    {
        isPossibleMove = enable;
        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        if (isSelected)
        {
            renderer.material.SetColor(COLOR_ID, Color.yellow);
        }
        else if (isPossibleMove)
        {
            renderer.material.SetColor(COLOR_ID, Color.green);
        }
        else if (isHovered)
        {
            renderer.material.SetColor(COLOR_ID, Color.red);
        }
        else
        {
            renderer.material.SetColor(COLOR_ID, originalColor);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HighlightHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HighlightHover(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<BattleController>().HandleCellClick(this);
    }
}