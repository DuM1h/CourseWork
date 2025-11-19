using System;

namespace CourseWork;

static public class BoardEvaluator
{
    private const float CenterPawnBonus = 0.1f;
    private const float CenterKnightBonus = 0.05f;
    private const float CenterBishopBonus = 0.05f;
    private const float ControlCenterKnightBonus = 0.03f;

    private const float HangingPawnPenalty = 0.025f;
    private const float HangingMinorPenalty = 0.075f;
    private const float HangingRookPenalty = 0.125f;
    private const float HangingQueenPenalty = 0.250f;
    static public float CalculatedAdvantage(ChessBoard board)
    {
        int value = 0;
        int opponentValue = 0;
        float coefficient = 1.0f;
        float opponentCoefficient = 1.0f;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Figure fig = board.board[i, j];
                if (fig == null)
                    continue;

                bool isMyFigure = (fig.IsWhite == board.IsWhiteTurn);

                if (fig is King)
                    continue;

                if (isMyFigure)
                    value += fig.Value;
                else
                    opponentValue += fig.Value;

                int numMove = board.FullmoveNumber;
                float centerBonus = GetCenterControlBonus(fig, numMove);
                if (isMyFigure)
                    coefficient += centerBonus;
                else
                    opponentCoefficient += centerBonus;

                fig.FindAttackers(board);
                if ((fig.AttackingFigures.Count>0 && fig.ProtectingFigures.Count==0) ||
                    (fig.AttackingFigures.Count>0 && fig.ProtectingFigures.Count>0 && (fig.ProtectingFigures.Count < fig.AttackingFigures.Count)))
                {
                    if (isMyFigure)
                        coefficient -= GetHangingPiecePenalty(fig);
                    else
                        opponentCoefficient -= GetHangingPiecePenalty(fig);
                }
            }
        }
        King myKing = board.IsWhiteTurn ? board.GetWhiteKing() : board.GetBlackKing();
        King opponentKing = board.IsWhiteTurn ? board.GetBlackKing() : board.GetWhiteKing();

        coefficient += GetKingSafetyBonus(myKing, board);
        opponentCoefficient += GetKingSafetyBonus(opponentKing, board);

        coefficient -= GetKingSafetyPenalty(myKing, board);
        opponentCoefficient -= GetKingSafetyPenalty(opponentKing, board);


        return (value * coefficient) - (opponentValue * opponentCoefficient);
    }

    static private float GetKingSafetyBonus(King king, ChessBoard board)
    {
        float safetyBonus = 0.0f;
        int kingLetter = king.PositionLetter;
        int kingNumber = king.PositionNumber;
        bool isWhite = king.IsWhite;

        int shieldRank = isWhite ? kingNumber + 1 : kingNumber - 1;

        for (int i = -1; i <= 1; i++)
        {
            char shieldLetter = (char)(kingLetter + i);

            if (shieldLetter < 'a' || shieldLetter > 'h' || shieldRank < 1 || shieldRank > 8)
                continue;

            Figure shieldPiece = board.GetFigureAt(shieldLetter, shieldRank);

            if (shieldPiece != null && shieldPiece.IsWhite == isWhite && shieldPiece is Pawn)
            {
                safetyBonus += 0.005f;
            }
        }
        return safetyBonus;
    }
    private static float GetCenterControlBonus(Figure fig, int n)
    {
        if (n > 10)
            return 0.0f;
        char letter = fig.PositionLetter;
        int number = fig.PositionNumber;

        if ((letter == 'd' || letter == 'e') && (number == 4 || number == 5))
        {
            if (fig is Pawn) return CenterPawnBonus;
            if (fig is Knight) return CenterKnightBonus;
            if (fig is Bishop) return CenterBishopBonus;
        }
        if ((letter == 'c' || letter == 'f') && (number == 3 || number == 6))
        {
            if (fig is Knight) return ControlCenterKnightBonus;
        }

        return 0.0f;
    }

    private static float GetHangingPiecePenalty(Figure fig)
    {
        if (fig is Pawn) return HangingPawnPenalty;
        if (fig is Knight || fig is Bishop) return HangingMinorPenalty;
        if (fig is Rook) return HangingRookPenalty;
        if (fig is Queen) return HangingQueenPenalty;
        return 0.0f;
    }
    static private float GetKingSafetyPenalty(King king, ChessBoard board)
    {
        float safetyPenalty = 0.0f;
        int kingLetter = king.PositionLetter;
        int kingNumber = king.PositionNumber;
        bool isWhite = king.IsWhite;
        int rankDirection = isWhite ? 1 : -1;
        for (int i = -2; i <= 2; i++)
        {
            for (int j = 1; j <= 2; j++)
            {
                char attackLetter = (char)(kingLetter + i);
                int attackNumber = kingNumber + (j * rankDirection);
                if (attackLetter < 'a' || attackLetter > 'h' || attackNumber < 1 || attackNumber > 8)
                    continue;
                Figure attackingPiece = board.GetFigureAt(attackLetter, attackNumber);
                if (attackingPiece != null && attackingPiece.IsWhite != isWhite)
                    safetyPenalty += 0.03f;
                if (attackingPiece == null)
                    safetyPenalty += 0.001f;
            }
        }
        return safetyPenalty;
    }
}
