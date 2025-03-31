using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HappyDungeon.General
{
    /// <summary>
    /// During v0.11, it seems necessary to be able to have a tile position 
    /// having both block and item/enemy recorded. Previous method, however, 
    /// can only have either block or item/enemy, and whenever a position is 
    /// given for item/enemy, the block underneath would become the default 
    /// block. 
    /// 
    /// This coder is designed to combine both indexes into one. 
    /// 
    /// The way it does this is by concatenating the two indexes. 
    /// For example, if blood bead (index -257) is standing on top on block 7, 
    /// then the result would be:
    ///     - (257 * 1000 + 7) = -257007
    /// Where:
    ///     -     (257    *      1000     +    7     )  =  -257007
    ///   Sign  Item/Enemy     THOUSAND      Block       Coded Result   
    ///   
    /// </summary>

    class IndexCoder
    {
        private const int THOUSAND = 1000; 

        /// <summary>
        /// Given the blcok and item/enemy on it, generate the coded index.
        /// </summary>
        /// <param name="BlockIndex">Index of the block</param>
        /// <param name="AuxIndex">Index of item or enemy</param>
        /// <returns>Coded index</returns>
        public static int GenerateIndex(int BlockIndex, int AuxIndex)
        {
            return Math.Sign(AuxIndex) * (
                    Math.Abs(AuxIndex) * THOUSAND +
                    BlockIndex
                );
        }

        /// <summary>
        /// Extract the block index from the coded index
        /// </summary>
        /// <param name="CodedIndex">Input coded index</param>
        /// <returns>Block part of the coded index</returns>
        public static int GetBlockIndex(int CodedIndex)
        {
            return Math.Abs(CodedIndex % THOUSAND);
        }

        /// <summary>
        /// Extract the item/enemy index from the coded index
        /// </summary>
        /// <param name="CodedIndex">Input coded index</param>
        /// <returns>Item/enemy part of the coded index</returns>
        public static int GetAuxIndex(int CodedIndex)
        {
            return (int)(CodedIndex / THOUSAND);
        }

        /// <summary>
        /// Given a coded index and new index of the block, replace
        /// the block part with the new block index. 
        /// </summary>
        /// <param name="OriginalIndex">Original coded index</param>
        /// <param name="NewBlockIndex">Index of the block and block only</param>
        /// <returns>Updated coded index</returns>
        public static int OverrideBlockIndex(int OriginalIndex, int NewBlockIndex)
        {
            int AuxIndex = GetAuxIndex(OriginalIndex);

            // In the case that no item/enemy is in this position
            // mark it as 1 to avoid having a 0 as result
            if (AuxIndex == 0)
                AuxIndex = 1; 

            return Math.Sign(AuxIndex) * (
                    Math.Abs(AuxIndex) * THOUSAND +
                    NewBlockIndex
                );
        }

        /// <summary>
        /// Given a coded index and new index of the item/enemy, replace
        /// the item/enemy part with the new block index. 
        /// </summary>
        /// <param name="OriginalIndex">Original coded index</param>
        /// <param name="NewAuxIndex">Index of the item/enemy and item/enemy only</param>
        /// <returns>Updated coded index</returns>
        public static int OverrideAuxIndex(int OriginalIndex, int NewAuxIndex)
        {
            int BlockIndex = GetBlockIndex(OriginalIndex);

            return Math.Sign(NewAuxIndex) * (
                    Math.Abs(NewAuxIndex) * THOUSAND +
                    BlockIndex
                );
        }

        /// <summary>
        /// Flood both parts of a coded index with same value  
        /// </summary>
        /// <param name="Index">Index value to flood</param>
        /// <returns>Coded index with both parts ahving same value</returns>
        public static int FloodIndex(int Index)
        {
            return Math.Sign(Index) * (
                    Math.Abs(Index) * THOUSAND +
                    Index
                );
        }

        public static int[,] OverrideMatrix(int[,] OriginalMat, int[,] TargetMat, bool IsBlock)
        {
            Debug.Assert(OriginalMat.GetLength(1) == TargetMat.GetLength(1)
                && OriginalMat.GetLength(0) == TargetMat.GetLength(0));

            int[,] Result = new int[OriginalMat.GetLength(0), OriginalMat.GetLength(1)]; 

            for (int i = 0; i < Result.GetLength(0); i++)
            {
                for (int j = 0; j < Result.GetLength(1); j++)
                {
                    if (IsBlock)
                        Result[i, j] = OverrideBlockIndex(OriginalMat[i, j], TargetMat[i, j]);
                    else
                        Result[i, j] = OverrideAuxIndex(OriginalMat[i, j], TargetMat[i, j]);
                }
            }

            return Result; 
        }

        public static int[,] OverrideMatrixAutoFill(int AutoFillValue,  int[,] TargetMat, bool IsBlock)
        {
            int[,] Result = new int[TargetMat.GetLength(0), TargetMat.GetLength(1)];

            for (int i = 0; i < Result.GetLength(0); i++)
            {
                for (int j = 0; j < Result.GetLength(1); j++)
                {
                    if (IsBlock)
                        Result[i, j] = OverrideBlockIndex(AutoFillValue, TargetMat[i, j]);
                    else
                        Result[i, j] = OverrideAuxIndex(AutoFillValue, TargetMat[i, j]);
                }
            }

            return Result;
        }


    }
}
