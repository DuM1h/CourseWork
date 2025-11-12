using System;

namespace CourseWork;

public class Pawn : Figure
{
    public Pawn(char positionLetter, int positionNumber, bool isWhite, int value) : base(positionLetter, positionNumber, isWhite, value) { }
    public override List<(char, int)> GetPossibleMoves(ChessBoard board, bool includeAllies = false)
    {
        var moves = new List<(char, int)>();

        int direction = IsWhite ? 1 : -1;

        if (board.GetFigureAt(PositionLetter, PositionNumber + direction) == null)
            moves.Add((PositionLetter, PositionNumber + direction));

        if ((IsWhite && PositionNumber == 2 || !IsWhite && PositionNumber == 7) &&
            board.GetFigureAt(PositionLetter, PositionNumber + 2 * direction) == null)
            moves.Add((PositionLetter, PositionNumber + 2 * direction));

        foreach (var dx in new[] { -1, 1 })
        {
            char newLetter = (char)(PositionLetter + dx);
            int newNumber = PositionNumber + direction;

            if (newLetter < 'a' || newLetter > 'h' || newNumber < 1 || newNumber > 8)
                continue;

            var target = board.GetFigureAt(newLetter, newNumber);
            if (target != null && (target.IsWhite != IsWhite || includeAllies))
                moves.Add((newLetter, newNumber));
        }

        return moves;
    }
}
