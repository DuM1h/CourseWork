using System;

namespace CourseWork
{
    public class Knight : Figure
    {
        public Knight(char positionLetter, int positionNumber, bool isWhite, int value) : base(positionLetter, positionNumber, isWhite, value) { }

        public override List<(char, int)> GetPossibleMoves(ChessBoard board, bool includeAllies = false)
        {
            var moves = new List<(char, int)>();
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
                            if (target==null || (target != null && (target.IsWhite != IsWhite || includeAllies)))
                            {
                                moves.Add((newLetter, newNumber));
                            }
                        }
                    }
                }
            }
            return moves;
        }
    }
}
