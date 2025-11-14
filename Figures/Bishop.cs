using System;

namespace CourseWork;

public class Bishop : Figure
{
    public Bishop(char positionLetter, int positionNumber, bool isWhite, int value) : base(positionLetter, positionNumber, isWhite, value) { Type = FigureType.Bishop; }

    public override List<Move> GetPossibleMoves(ChessBoard board, bool includeAllies = false)
    {
        var moves = new List<Move>();
        int[] dx = { -1, 0, 1 };
        int[] dy = { -1, 0, 1 };

        foreach (var x in dx)
        {
            foreach (var y in dy)
            {
                if (Math.Abs(x) == Math.Abs(y) && x != 0)
                {
                    char newLetter = (char)(PositionLetter + x);
                    int newNumber = PositionNumber + y;
                    while (newLetter >= 'a' && newLetter <= 'h' && newNumber >= 1 && newNumber <= 8)
                    {
                        var target = board.GetFigureAt(newLetter, newNumber);
                        if (target == null)
                        {
                            moves.Add(new Move(this.Type, Position, (newLetter, newNumber)));
                        }
                        else
                        {
                            if (target.IsWhite != IsWhite || includeAllies)
                            {
                                moves.Add(new Move(this.Type, Position, (newLetter, newNumber), target.Type));
                            }
                            break;
                        }
                        newLetter = (char)(newLetter + x);
                        newNumber = newNumber + y;
                    }
                }
            }
        }
        return moves;
    }
}
