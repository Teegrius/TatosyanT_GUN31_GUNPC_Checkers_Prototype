using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Team team;
    public PieceType type = PieceType.Checker;
    public Cell cell;

    private Material originalMaterial;
    private new Renderer renderer;

    void Awake()
    {
        renderer = GetComponent<Renderer>();
        originalMaterial = renderer.material;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cell != null) cell.HighlightHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cell != null) cell.HighlightHover(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (cell != null) cell.OnPointerClick(eventData);
    }

    public void PromoteToKing()
    {
        type = PieceType.King;
        transform.localScale = new Vector3(1, 2, 1);
    }

    public void Highlight(bool enable)
    {
        renderer.material.color = enable ? Color.yellow : originalMaterial.color;
    }
}