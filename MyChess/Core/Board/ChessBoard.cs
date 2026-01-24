using MyChess.Models;
using MyChess.Models.Pieces;

namespace MyChess.Core.Board;

public class ChessBoard
{
    public BitBoard[] BitBoards = new BitBoard[12]; 
    
    public BitBoard WhitePawns => BitBoards[0];
    public BitBoard WhiteKnights => BitBoards[1];
    public BitBoard WhiteBishops => BitBoards[2];
    public BitBoard WhiteRooks => BitBoards[3];
    public BitBoard WhiteQueens => BitBoards[4];
    public BitBoard WhiteKing => BitBoards[5];
    
    public BitBoard BlackPawns => BitBoards[6];
    public BitBoard BlackKnights => BitBoards[7];
    public BitBoard BlackBishops => BitBoards[8];
    public BitBoard BlackRooks => BitBoards[9];
    public BitBoard BlackQueens => BitBoards[10];
    public BitBoard BlackKing => BitBoards[11];
    
    public BitBoard WhiteOccupancies => Occupancies[0];
    public BitBoard BlackOccupancies => Occupancies[1];
    
    public readonly BitBoard[] Occupancies = new BitBoard[2];
    
    public ChessBoard()
    {
        for (var i = 0; i < 12; i++) BitBoards[i] = new BitBoard();
        for (var i = 0; i < 2; i++) Occupancies[i] = new BitBoard();
    }

    public void UpdateWhiteOccupancies()
    {
        Occupancies[0] = new BitBoard();

        for (var i = 0; i < 6; i++)
        {
            Occupancies[0] |= BitBoards[i];
        }
    }
    
    public void UpdateBlackOccupancies()
    {
        Occupancies[1] = new BitBoard();

        for (var i = 6; i < 12; i++)
        {
            Occupancies[1] |= BitBoards[i];
        }
    }
    
    public IChessPiece? GetPiece(int cell)
    {
        for (var pieceType = 0; pieceType < 12; pieceType++)
            if (BitBoards[pieceType].GetBit(cell)) 
                return CreatePieceFromIndex(pieceType);
    
        return null;
    }
    
    private IChessPiece CreatePieceFromIndex(int pieceIndex)
    {
        return pieceIndex switch
        {
            0 => Pawn.White,
            1 => Knight.White,
            2 => Bishop.White,
            3 => Rook.White,
            4 => Queen.White,
            5 => King.White,
            6 => Pawn.Black,
            7 => Knight.Black,
            8 => Bishop.Black,
            9 => Rook.Black,
            10 =>  Queen.Black,
            11 => King.Black,
            _ => throw new ArgumentException("Invalid piece index")
        };
    }
    
    public ChessBoard Clone()
    {
        var clone = new ChessBoard();
    
        for (var i = 0; i < 12; i++)
        {
            clone.BitBoards[i] = BitBoards[i];
        }
    
        for (var i = 0; i < 2; i++)
        {
            clone.Occupancies[i] = Occupancies[i];
        }
    
        return clone;
    }

    public void MovePiece(int from, int to)
    {
        var piece = -1;
        for (var i = 0; i < 12; i++)
        {
            if (!BitBoards[i].GetBit(from)) continue;
            piece = i;
            BitBoards[i] = BitBoards[i].ClearBit(from);
            Occupancies[piece / 6] = Occupancies[piece / 6].ClearBit(from);
            break;
        }
        if (piece == -1) return;
        
        BitBoards[piece] = BitBoards[piece].SetBit(to);
        Occupancies[piece / 6] = Occupancies[piece / 6].SetBit(to);
        
        var opponentOccupancy = Occupancies[1 - piece / 6];
        if (opponentOccupancy.GetBit(to))
        {
            Occupancies[1 - piece / 6] = opponentOccupancy.ClearBit(to);
            
            for (var i = 0; i < 12; i++)
            {
                if (i == piece || !BitBoards[i].GetBit(to)) continue;
                BitBoards[i] = BitBoards[i].ClearBit(to);
                return;
            }
        }
    }

    public void RemovePiece(int cell)
    {
        var allPieces = Occupancies[0] | Occupancies[1];
        if (!allPieces.GetBit(cell)) return;
    
        for (var i = 0; i < 12; i++)
        {
            if (!BitBoards[i].GetBit(cell)) continue;
            BitBoards[i] = BitBoards[i].ClearBit(cell);
            Occupancies[i / 6] = Occupancies[i / 6].ClearBit(cell);
            return;
        }
    }

    public void SetPiece(int cell, IChessPiece? piece)
    {
        for (var i = 0; i < 12; i++)
        {
            if (!BitBoards[i].GetBit(cell)) continue;
            BitBoards[i] = BitBoards[i].ClearBit(cell);
            Occupancies[i / 6] = Occupancies[i / 6].ClearBit(cell);
            break;
        }
        if (piece == null) return;
    
        BitBoards[piece.Index] = BitBoards[piece.Index].SetBit(cell);
        Occupancies[piece.Index / 6] = Occupancies[piece.Index / 6].SetBit(cell);
    }

    public int FindKing(ChessColor color)
    {
        var bitBoard = color == ChessColor.White ? WhiteKing : BlackKing;
        return bitBoard.GetLeastSignificantBitIndex();
    }
}