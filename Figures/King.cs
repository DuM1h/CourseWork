using System;

namespace CourseWork
{
    public class King : Figure
    {
        public King(char positionLetter, int positionNumber, bool isWhite) : base(positionLetter, positionNumber, isWhite) { }

        public override List<(char, int)> GetPossibleMoves(ChessBoard board, bool includeAllies = false)
        {
            var moves = new List<(char, int)>();
            int[] dx = { -1, 0, 1 };
            int[] dy = { -1, 0, 1 };

            ChessBoard boardCopy;
            foreach (var x in dx)
            {
                foreach (var y in dy)
                {
                    boardCopy = new ChessBoard(board);
                    King kingCopy = new King(PositionLetter, PositionNumber, IsWhite);
                    if (x == 0 && y == 0) continue;
                    char newLetter = (char)(PositionLetter + x);
                    int newNumber = PositionNumber + y;
                    if (newLetter >= 'a' && newLetter <= 'h' && newNumber >= 1 && newNumber <= 8)
                    {
                        var target = board.GetFigureAt(newLetter, newNumber);
                        if (target==null || (target != null && (target.IsWhite != IsWhite || includeAllies )))
                        {
                            kingCopy.Move(newLetter, newNumber, boardCopy);
                            if (!kingCopy.IsChecking(boardCopy))
                                moves.Add((newLetter, newNumber));                            
                        }
                    }
                }
            }
            int[] kdx = {0, 1, 2};
            int[] qdx = {0, -1, -2};
            switch (IsWhite)
            {
                case true:
                    if (board.CanWhiteCastleKingside)
                    {
                        foreach (var k in kdx)
                        {
                            var target = board.GetFigureAt((char)(PositionLetter + k), PositionNumber);
                            if (target != null)
                                break;
                            boardCopy = new ChessBoard(board);
                            King kingCopy = new King(PositionLetter, PositionNumber, IsWhite);
                            char newLetter = (char)(PositionLetter + k);
                            kingCopy.Move(newLetter, PositionNumber, boardCopy);
                            if (kingCopy.IsChecking(boardCopy))
                                break;
                            if (k == 2)
                                moves.Add((newLetter, PositionNumber));
                        }
                    }
                    if (board.CanWhiteCastleQueenside)
                    {
                        foreach (var q in qdx)
                        {
                            var target = board.GetFigureAt((char)(PositionLetter + q), PositionNumber);
                            if (target != null)
                                break;
                            boardCopy = new ChessBoard(board);
                            King kingCopy = new King(PositionLetter, PositionNumber, IsWhite);
                            char newLetter = (char)(PositionLetter + q);
                            kingCopy.Move(newLetter, PositionNumber, boardCopy);
                            if (kingCopy.IsChecking(boardCopy))
                                break;
                            if (q == -2)
                                moves.Add((newLetter, PositionNumber));
                        }
                    }
                break;
                case false:
                if (board.CanBlackCastleKingside)
                {
                    foreach (var k in kdx)
                    {
                        var target = board.GetFigureAt((char)(PositionLetter + k), PositionNumber);
                        if (target != null)
                            break;
                        boardCopy = new ChessBoard(board);
                        King kingCopy = new King(PositionLetter, PositionNumber, IsWhite);
                        char newLetter = (char)(PositionLetter + k);
                        kingCopy.Move(newLetter, PositionNumber, boardCopy);
                        if (kingCopy.IsChecking(boardCopy))
                            break;
                        if (k == 2)
                            moves.Add((newLetter, PositionNumber));
                    }
                }
                if (board.CanBlackCastleQueenside)
                {
                    foreach (var q in qdx)
                    {
                        var target = board.GetFigureAt((char)(PositionLetter + q), PositionNumber);
                        if (target != null)
                            break;
                        boardCopy = new ChessBoard(board);
                        King kingCopy = new King(PositionLetter, PositionNumber, IsWhite);
                        char newLetter = (char)(PositionLetter + q);
                        kingCopy.Move(newLetter, PositionNumber, boardCopy);
                        if (kingCopy.IsChecking(boardCopy))
                            break;
                        if (q == -2)
                            moves.Add((newLetter, PositionNumber));
                    }
                }
                break;
            }
            return moves;
        }

        public bool IsChecking(ChessBoard chessBoard)
        {
            bool isChecked = false;
            AttackingFigures = new List<Figure>();
            Figure[,] opponentsFigures;
            if (this.IsWhite)
                opponentsFigures = chessBoard.GetBlackFigures();
            else
                opponentsFigures = chessBoard.GetWhiteFigures();
            foreach (Figure figure in opponentsFigures)
            {
                if (figure == null) continue;
                if (figure is King) continue;
                foreach (var (toLetter, toNumber) in figure.GetPossibleMoves(chessBoard))
                {
                    if (figure is Pawn)
                    {
                        if (Math.Abs((char)(this.PositionLetter - figure.PositionLetter)) == 1)
                        {
                            if ((toLetter == this.PositionLetter) && (toNumber == this.PositionNumber))
                            {
                                isChecked = true;
                                this.AttackingFigures.Add(figure);
                            }
                        }
                    } 
                    else if ((toLetter == PositionLetter) && (toNumber == PositionNumber))
                    {
                        isChecked = true;
                        this.AttackingFigures.Add(figure);
                    }
                }
            }
            return isChecked;
        }

        public bool IsMated(ChessBoard board)
        {
            if (this.GetPossibleMoves(board).Count == 0 && this.IsChecking(board))
                return true;
            return false;
        }
    }
}
