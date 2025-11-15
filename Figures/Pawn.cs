using System;

namespace CourseWork;

public class Pawn : Figure
{
    public Pawn(char positionLetter, int positionNumber, bool isWhite, int value) : base(positionLetter, positionNumber, isWhite, value) { Type = FigureType.Pawn; }
    public override List<Move> GetPossibleMoves(ChessBoard board, bool includeAllies = false)
    {
        var moves = new List<Move>();
        int direction = IsWhite ? 1 : -1;
        var nextPosition = board.GetFigureAt(PositionLetter, PositionNumber + direction);
        if (nextPosition == null)
            moves.Add(new Move(this.Type, Position, (PositionLetter, PositionNumber+1*direction)));

        if ((IsWhite && PositionNumber == 2 || !IsWhite && PositionNumber == 7) &&
            board.GetFigureAt(PositionLetter, PositionNumber + 2 * direction) == null)
            moves.Add(new Move(this.Type, Position, (PositionLetter, PositionNumber+2*direction)));

        foreach (var dx in new[] { -1, 1 })
        {
            char newLetter = (char)(PositionLetter + dx);
            int newNumber = PositionNumber + direction;
            (char, int) newPos = (newLetter, newNumber);
            if (newLetter < 'a' || newLetter > 'h' || newNumber < 1 || newNumber > 8)
                continue;

            var target = board.GetFigureAt(newLetter, newNumber);
            if (target != null && (target.IsWhite != IsWhite || includeAllies))
                moves.Add(new Move(this.Type, Position, newPos, target.Type));

            if (board.EnPassantAvailable && board.EnPassantTarget == (newLetter, newNumber))
            {
                moves.Add(new Move(this.Type, Position, newPos, board.EnPassantTarget));
            }
        }

        return moves;
    }
}
