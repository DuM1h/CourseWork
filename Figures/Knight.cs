using System;

namespace CourseWork;

public class Knight : Figure
{
    public Knight(char positionLetter, int positionNumber, bool isWhite, int value) : base(positionLetter, positionNumber, isWhite, value) { Type = FigureType.Knight; }

    public override List<Move> GetPossibleMoves(ChessBoard board, bool includeAllies = false)
    {
        var moves = new List<Move>();
        int[] dx = { -2, -1, 1, 2 };
        int[] dy = { -2, -1, 1, 2 };
        
        foreach (var x in dx)
        {
            foreach (var y in dy)
            {
                if (Math.Abs(x) != Math.Abs(y))
                {
                    char newLetter = (char)(PositionLetter + x);
                    int newNumber = PositionNumber + y;
                    if (newLetter >= 'a' && newLetter <= 'h' && newNumber >= 1 && newNumber <= 8)
                    {
                        var target = board.GetFigureAt(newLetter, newNumber);
                        if (target==null)
                            moves.Add(new Move(this.Type, Position, (newLetter, newNumber)));
                        if (target != null && (target.IsWhite != IsWhite || includeAllies))
                            moves.Add(new Move(this.Type, Position, (newLetter, newNumber), target.Type));
                    }
                }
            }
        }
        return moves;
    }
}
