using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Vector2Int position;
    public Unit unit;
    public bool isBlack;

    private Material originalMaterial;
    public new Renderer renderer;
    private bool isHovered;
    private bool isSelected;
    private bool isPossibleMove;

    void Awake()
    {
        renderer = GetComponent<Renderer>();
        originalMaterial = renderer.material;
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
            renderer.material.color = Color.yellow;
        }
        else if (isPossibleMove)
        {
            renderer.material.color = Color.green;
        }
        else if (isHovered)
        {
            renderer.material.color = Color.red;
        }
        else
        {
            renderer.material = originalMaterial;
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