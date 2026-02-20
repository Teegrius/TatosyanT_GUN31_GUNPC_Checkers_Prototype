using System;

public enum Team
{
    White,
    Black
}

public enum PieceType
{
    Checker,
    King
}

public enum GameState
{
    SelectPiece,
    SelectMove,
    Visualizing,
    Idle
}

public enum PlayerTurn
{
    White,
    Black
}