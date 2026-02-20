using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Text turnText;

    void Update()
    {
        BattleController bc = FindObjectOfType<BattleController>();
        turnText.text = "Turn: " + bc.currentTurn.ToString();
    }

    public void VisualizeMove()
    {
        
    }
}