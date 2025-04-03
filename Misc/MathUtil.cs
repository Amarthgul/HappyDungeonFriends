using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyDungeon
{
    class MathUtil
    {

        public static double[,] Convolve2D(double[,] input, double[,] kernel, bool padInput=false)
        {
            if (padInput)
                return Convolve2DInputPadding(input, kernel);
            else
                return Convolve2DOutputPadding(input, kernel);
        }


        public static double[,] Convolve2DInputPadding(double[,] input, double[,] kernel)
        {
            // Ensure the kernel is 3x3.
            if (kernel.GetLength(0) != 3 || kernel.GetLength(1) != 3)
                throw new ArgumentException("Kernel must be 3x3.");

            int rows = input.GetLength(0);
            int cols = input.GetLength(1);
            double[,] output = new double[rows, cols];

            // Loop over each element of the input array.
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    double sum = 0.0;
                    // Apply the 3x3 kernel.
                    for (int ki = -1; ki <= 1; ki++)
                    {
                        for (int kj = -1; kj <= 1; kj++)
                        {
                            int rowIndex = i + ki;
                            int colIndex = j + kj;
                            // Check if neighbor is within bounds; if not, treat as 0.
                            if (rowIndex >= 0 && rowIndex < rows && colIndex >= 0 && colIndex < cols)
                            {
                                // Kernel index is shifted by +1 since ki and kj range from -1 to 1.
                                sum += input[rowIndex, colIndex] * kernel[ki + 1, kj + 1];
                            }
                        }
                    }
                    output[i, j] = sum;
                }
            }

            return output;
        }


        public static double[,] Convolve2DOutputPadding(double[,] input, double[,] kernel)
        {
            // Validate that the kernel is 3x3.
            if (kernel.GetLength(0) != 3 || kernel.GetLength(1) != 3)
                throw new ArgumentException("Kernel must be 3x3.");

            int inputRows = input.GetLength(0);
            int inputCols = input.GetLength(1);
            double[,] output = new double[inputRows, inputCols];

            // Only compute convolution where the kernel fully overlaps the input.
            // This valid region is from row 1 to inputRows - 2 and column 1 to inputCols - 2.
            for (int i = 1; i < inputRows - 1; i++)
            {
                for (int j = 1; j < inputCols - 1; j++)
                {
                    double sum = 0.0;

                    // Loop over the kernel indices.
                    for (int ki = 0; ki < 3; ki++)
                    {
                        for (int kj = 0; kj < 3; kj++)
                        {
                            // Calculate the corresponding input indices.
                            int inputRow = i + ki - 1;
                            int inputCol = j + kj - 1;
                            sum += input[inputRow, inputCol] * kernel[ki, kj];
                        }
                    }

                    // Assign the computed sum to the output at the same location.
                    output[i, j] = sum;
                }
            }

            // The border cells of the output remain zero (or can be assigned any desired default)
            // which effectively pads the output to match the dimensions of the input.
            return output;
        }


        /// <summary>
        /// Performs element-wise addition of two 2D int arrays and clamps each result to the specified range.
        /// </summary>
        /// <param name="arr1">The first 2D array.</param>
        /// <param name="arr2">The second 2D array (must be the same dimensions as arr1).</param>
        /// <param name="minValue">The minimum allowed value in the result.</param>
        /// <param name="maxValue">The maximum allowed value in the result.</param>
        /// <returns>A new 2D int array containing the clamped sums.</returns>
        /// <exception cref="ArgumentException">Thrown if the input arrays do not have the same dimensions.</exception>
        public static double[,] ArrayAdd(double[,] arr1, double[,] arr2)
        {
            int rows = arr1.GetLength(0);
            int cols = arr1.GetLength(1);

            if (rows != arr2.GetLength(0) || cols != arr2.GetLength(1))
                throw new ArgumentException("Both arrays must have the same dimensions.");

            double[,] result = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    double sum = arr1[i, j] + arr2[i, j];
                    result[i, j] = sum; 
                }
            }

            return result;
        }

        public static double[,] ArrayClamp(double[,] arr, double minValue = 0, double maxValue = 1)
        {
            int rows = arr.GetLength(0);
            int cols = arr.GetLength(1);

            double[,] result = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
 
                    result[i, j] = (result[i, j] < minValue) ? minValue : (result[i, j] > maxValue ? maxValue : result[i, j]);
                }
            }

            return result;
        }


        /// <summary>
        /// Converts a 2D boolean array into a 2D double array.
        /// True is mapped to 1.0 and false to 0.0.
        /// </summary>
        /// <param name="boolArray">The input 2D boolean array.</param>
        /// <returns>A 2D double array with corresponding values.</returns>
        public static double[,] BoolToDouble(bool[,] boolArray)
        {
            int rows = boolArray.GetLength(0);
            int cols = boolArray.GetLength(1);
            double[,] doubleArray = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    doubleArray[i, j] = boolArray[i, j] ? 1.0 : 0.0;
                }
            }

            return doubleArray;
        }


        /// <summary>
        /// Converts a 2D double array into a 2D boolean array.
        /// A value greater than or equal to the threshold is considered true.
        /// </summary>
        /// <param name="doubleArray">The input 2D double array.</param>
        /// <param name="threshold">Threshold value to decide true/false (default 0.5).</param>
        /// <returns>A 2D boolean array with corresponding values.</returns>
        public static bool[,] DoubleToBool(double[,] doubleArray, double threshold = 0.5)
        {
            int rows = doubleArray.GetLength(0);
            int cols = doubleArray.GetLength(1);
            bool[,] boolArray = new bool[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    boolArray[i, j] = doubleArray[i, j] >= threshold;
                }
            }

            return boolArray;
        }


        public static List<Point> GetPointsFromMatrix(bool[,] matrix)
        {
            List<Point> points = new List<Point>();
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (matrix[i, j])
                    {
                        // Note: X is the column index and Y is the row index.
                        points.Add(new Point(j, i));
                    }
                }
            }
            return points;
        }


        public static bool[,] PointsToMatrix(List<Point> points, int rows, int cols)
        {
            bool[,] matrix = new bool[rows, cols];
            foreach (var p in points)
            {
                if (p.Y >= 0 && p.Y < rows && p.X >= 0 && p.X < cols)
                {
                    matrix[p.Y, p.X] = true;
                }
            }
            return matrix;
        }



        /// <summary>
        /// This method is supposed to smooth the path but it makes things extremely slow 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="sigma"></param>
        /// <param name="edgeBias"></param>
        /// <returns></returns>
        public static List<Point> GaussianSmoothPoints(List<Point> points, double sigma = 2, double edgeBias = 3)
        {
            List<Point> smoothedPoints = new List<Point>();
            double twoSigmaSq = 2 * sigma * sigma;

            // Determine the boundaries of the point set.
            int minX = int.MaxValue, maxX = int.MinValue;
            int minY = int.MaxValue, maxY = int.MinValue;
            foreach (var pt in points)
            {
                if (pt.X < minX) minX = pt.X;
                if (pt.X > maxX) maxX = pt.X;
                if (pt.Y < minY) minY = pt.Y;
                if (pt.Y > maxY) maxY = pt.Y;
            }

            // For each point, compute the Gaussian-smoothed new position.
            foreach (var p in points)
            {
                // Check if the point is on the edge of the set.
                bool isEdge = (p.X == minX || p.X == maxX || p.Y == minY || p.Y == maxY);

                double weightSum = 0.0;
                double xSum = 0.0;
                double ySum = 0.0;

                foreach (var q in points)
                {
                    double dx = p.X - q.X;
                    double dy = p.Y - q.Y;
                    double distanceSq = dx * dx + dy * dy;

                    // Gaussian weight: exp(-distance^2 / (2*sigma^2))
                    double weight = Math.Exp(-distanceSq / twoSigmaSq);

                    // If q is the same as p and p is an edge point, apply extra bias.
                    if (p.Equals(q) && isEdge)
                    {
                        weight *= edgeBias;
                    }

                    weightSum += weight;
                    xSum += q.X * weight;
                    ySum += q.Y * weight;
                }

                int newX = (int)Math.Round(xSum / weightSum);
                int newY = (int)Math.Round(ySum / weightSum);
                smoothedPoints.Add(new Point(newX, newY));
            }

            return smoothedPoints;
        }




    }
}
