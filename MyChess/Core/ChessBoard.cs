using System;
using MyChess.Models;
using MyChess.Models.Pieces;

namespace MyChess.Core;

public class ChessBoard
{
    public BitBoard[] BitBoards = new BitBoard[12]; 
    
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
            clone.BitBoards[i] = new BitBoard(BitBoards[i]);
        }
    
        for (var i = 0; i < 2; i++)
        {
            clone.Occupancies[i] = new BitBoard(Occupancies[i]);
        }
    
        return clone;
    }

    public void MovePiece(int from, int to)
    {
        var piece = -1;
        for (var i = 0; i < 12; i++)
        {
            if (!BitBoards[i].PopBit(from)) continue;
            piece = i;
            Occupancies[piece / 6].PopBit(from);
            break;
        }
        if (piece == -1) return;
        
        BitBoards[piece].SetBit(to);
        Occupancies[piece / 6].SetBit(to);
        
        if (!Occupancies[1 - piece / 6].PopBit(to)) return;
        
        for (var i = 0; i < 12; i++)
        {
            if (i == piece || !BitBoards[i].PopBit(to)) continue;
            return;
        }
    }

    public void RemovePiece(int cell)
    {
        if (!((BitBoard)(Occupancies[0] | Occupancies[1])).GetBit(cell)) return;
    
        for (var i = 0; i < 12; i++)
        {
            if (!BitBoards[i].PopBit(cell)) continue;
            Occupancies[i / 6].PopBit(cell);
            return;
        }
    }

    public void SetPiece(int cell, IChessPiece? piece)
    {
        for (var i = 0; i < 12; i++)
        {
            if (!BitBoards[i].PopBit(cell)) continue;
            Occupancies[i / 6].PopBit(cell);
            break;
        }
        if (piece == null) return;
    
        BitBoards[piece.Index].SetBit(cell);
        Occupancies[piece.Index / 6].SetBit(cell);
    }

    public int FindKing(ChessColor color)
    {
        var bitBoard = color == ChessColor.White ? BitBoards[5] : BitBoards[11];
        return bitBoard.GetLeastSignificantBitIndex();
    }
}