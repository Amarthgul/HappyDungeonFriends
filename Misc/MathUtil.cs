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
        /// Rotates an N×N matrix 90° clockwise.
        /// </summary>
        public static double[,] Rotate90Clockwise(double[,] matrix)
        {
            int n = matrix.GetLength(0);
            if (n != matrix.GetLength(1))
                throw new ArgumentException("Matrix must be square.");

            var result = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    // New row i, col j comes from old row (n-1-j), col i
                    result[i, j] = matrix[n - 1 - j, i];
                }
            }
            return result;
        }


        /// <summary>
        /// Returns a new 3×3 kernel whose off-center entries are random doubles in [minValue, maxValue),
        /// the center is 0, and all entries sum to 1.
        /// The original kernel is not modified.
        /// </summary>
        /// <param name="kernel">A 3×3 double[,] array (only used for dimension check).</param>
        /// <param name="minValue">Inclusive lower bound for random values.</param>
        /// <param name="maxValue">Exclusive upper bound for random values.</param>
        /// <returns>A newly allocated, normalized 3×3 kernel.</returns>
        public static double[,] RandomizeKernel(double[,] kernel, double minValue = 0.0, double maxValue = 1.0)
        {
            if (kernel == null)
                throw new ArgumentNullException(nameof(kernel));
            if (kernel.GetLength(0) != 3 || kernel.GetLength(1) != 3)
                throw new ArgumentException("Kernel must be 3×3.", nameof(kernel));
            if (minValue >= maxValue)
                throw new ArgumentException("minValue must be less than maxValue.");

            // Allocate the new kernel (copying dimensions from input)
            double[,] newKernel = new double[3, 3];

            // 1) Fill with random values, center forced to 0
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    newKernel[i, j] = (i == 1 && j == 1)
                        ? 0.0
                        : Globals.RND.NextDouble() * (maxValue - minValue) + minValue;
                }
            }

            // 2) Compute sum of all entries (center is zero so it's safe)
            double sum = 0.0;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    sum += newKernel[i, j];

            if (sum == 0.0)
                throw new InvalidOperationException("Sum of random values is zero; cannot normalize.");

            // 3) Normalize so that total sum == 1
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    newKernel[i, j] /= sum;

            return newKernel;
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


        /// <summary>
        /// Returns a new 2-D array in which every element of <paramref name="source"/> is
        /// multiplied by <paramref name="scalar"/>.  The original array is left unchanged.
        /// </summary>
        public static double[,] ArrayMultiply(double[,] source, double scalar)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            int rows = source.GetLength(0);
            int cols = source.GetLength(1);

            var result = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = source[i, j] * scalar;
                }
            }

            return result;
        }


        /// <summary>
        /// Generates a new 2D array of the given dimensions filled with random doubles
        /// in the half-open range [minValue, maxValue).
        /// </summary>
        /// <param name="rows">Number of rows (must be ≥ 1).</param>
        /// <param name="cols">Number of columns (must be ≥ 1).</param>
        /// <param name="minValue">Inclusive lower bound for generated values.</param>
        /// <param name="maxValue">Exclusive upper bound for generated values.</param>
        /// <returns>A newly-allocated double[rows, cols] array.</returns>
        public static double[,] RandomDoubleArray(int rows, int cols, double minValue = 0.0, double maxValue = 1.0)
        {
            if (rows <= 0) throw new ArgumentOutOfRangeException(nameof(rows), "rows must be ≥ 1");
            if (cols <= 0) throw new ArgumentOutOfRangeException(nameof(cols), "cols must be ≥ 1");
            if (minValue >= maxValue) throw new ArgumentException("minValue must be less than maxValue");

            var result = new double[rows, cols];
            double range = maxValue - minValue;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = Globals.RND.NextDouble() * range + minValue;
                }
            }
            return result;
        }


        public static double[,] ArrayClamp(double[,] arr,
                                   double minValue = 0,
                                   double maxValue = 1)
        {
            int rows = arr.GetLength(0);
            int cols = arr.GetLength(1);

            var result = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    double v = arr[i, j];                    // take the source value
                    result[i, j] = v < minValue ? minValue
                                 : v > maxValue ? maxValue
                                 : v;                        // store the clamped value
                }
            }
            return result;
        }


        /// <summary>
        /// Creates a new kernel whose values are the source kernel divided by their total,
        /// so that the returned kernel’s entries sum to exactly 1.0.
        /// </summary>
        /// <param name="kernel">The 2-D kernel to normalise.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the kernel is empty or its total is 0 (normalisation impossible).
        /// </exception>
        public static double[,] NormalizeKernel(double[,] kernel)
        {
            if (kernel is null) throw new ArgumentNullException(nameof(kernel));

            int rows = kernel.GetLength(0);
            int cols = kernel.GetLength(1);
            if (rows == 0 || cols == 0)
                throw new ArgumentException("Kernel must contain at least one element.", nameof(kernel));

            // 1. Sum all values
            double total = 0;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    total += kernel[i, j];

            if (Math.Abs(total) < double.Epsilon)
                throw new ArgumentException("Kernel total is zero – cannot normalise.", nameof(kernel));

            // 2. Divide every entry by the total, writing into a new array
            var normalised = new double[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    normalised[i, j] = kernel[i, j] / total;

            return normalised;
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


        public static double[,] BoolToDoubleInvert(bool[,] input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            int rows = input.GetLength(0);
            int cols = input.GetLength(1);
            var output = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // false → 1.0, true → 0.0
                    output[i, j] = input[i, j] ? 0.0 : 1.0;
                }
            }

            return output;
        }


        /// <summary>
        /// For each element x in the input 2D array, compute (1 - x) and clamp the result to [0,1].
        /// Returns a newly allocated 2D array; the input is not modified.
        /// </summary>
        public static double[,] OneMinusClamp(double[,] input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            int rows = input.GetLength(0);
            int cols = input.GetLength(1);
            var output = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    double v = 1.0 - input[i, j];
                    // clamp to [0,1]
                    if (v < 0.0) v = 0.0;
                    else if (v > 1.0) v = 1.0;
                    output[i, j] = v;
                }
            }

            return output;
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


        /// <summary>
        /// Greyscale erosion (min filter) on a 2-D array in [0,1].
        /// Each pass replaces every pixel by the minimum in its
        /// (2·radius+1) × (2·radius+1) neighbourhood.
        ///
        /// The image borders are padded with <paramref name="padValue"/>
        /// (0 by default), so blobs touching the border will shrink too.
        /// </summary>
        public static double[,] Erode(
            double[,] src,
            int radius = 1,
            int iterations = 1,
            double padValue = 0.0)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));
            if (radius < 1) throw new ArgumentOutOfRangeException(nameof(radius));
            if (iterations < 1) throw new ArgumentOutOfRangeException(nameof(iterations));

            int rows = src.GetLength(0);
            int cols = src.GetLength(1);

            // two work buffers — we ping-pong between them
            double[,] a = (double[,])src.Clone();
            double[,] b = new double[rows, cols];

            for (int it = 0; it < iterations; it++)
            {
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        double m = double.MaxValue;

                        for (int ky = -radius; ky <= radius; ky++)
                        {
                            int yy = y + ky;

                            for (int kx = -radius; kx <= radius; kx++)
                            {
                                int xx = x + kx;
                                double v = (yy < 0 || yy >= rows || xx < 0 || xx >= cols)
                                             ? padValue           // outside ⇒ pad value
                                             : a[yy, xx];          // inside  ⇒ real value

                                if (v < m) m = v;
                                if (m <= padValue) goto done;      // can't get lower
                            }
                        }
                    done: b[y, x] = m;
                    }
                }
                // swap for next pass (or for the return)
                var tmp = a; a = b; b = tmp;
            }
            return a;
        }


        /// <summary>
        /// Returns a new array that contains a Gaussian-weighted “distance from non-zero”
        /// measure.  Values are in [0,1] (≈1 deep inside a large zero region, ≈0 elsewhere).
        /// 
        /// If <paramref name="threshold"/> is &lt; 1, every output pixel is then set to
        ///   1.0 when its blurred value ≥ threshold, otherwise 0.0 ––
        /// so you get a *small* sprinkled mask of ones.  Pass threshold = 1.0 to keep the
        /// continuous field.
        /// </summary>
        public static double[,] ConvolveZeroMaskGaussian(
            double[,] src,
            double sigma = 3.0,   // controls spread of the Gaussian
            double zeroTol = 1e-9,  // what counts as exactly zero
            double threshold = 1.0)   // 1.0  → keep continuous values
        {
            if (src == null) throw new ArgumentNullException(nameof(src));
            if (sigma <= 0) throw new ArgumentOutOfRangeException(nameof(sigma));

            int rows = src.GetLength(0), cols = src.GetLength(1);

            //---------------------- 1. build 1-D Gaussian kernel ----------------------
            int radius = (int)Math.Ceiling(3 * sigma);      // ±3 σ captures > 99 %
            int size = 2 * radius + 1;
            double[] g = new double[size];
            double sum = 0.0;
            double twoSigma2 = 2 * sigma * sigma;

            for (int i = 0; i < size; i++)
            {
                int d = i - radius;
                g[i] = Math.Exp(-(d * d) / twoSigma2);
                sum += g[i];
            }
            // normalise so Σg = 1
            for (int i = 0; i < size; i++) g[i] /= sum;

            //---------------------- 2. build binary “zero” mask ----------------------
            double[,] mask = new double[rows, cols];
            for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                    mask[y, x] = (src[y, x] <= zeroTol) ? 1.0 : 0.0;

            //---------------------- 3. separable Gaussian blur -----------------------
            // first horizontal
            double[,] tmp = new double[rows, cols];
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    double acc = 0.0;
                    for (int k = -radius; k <= radius; k++)
                    {
                        int xx = Clamp(x + k, 0, cols - 1);
                        acc += mask[y, xx] * g[k + radius];
                    }
                    tmp[y, x] = acc;
                }
            }

            // then vertical
            double[,] dst = new double[rows, cols];
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    double acc = 0.0;
                    for (int k = -radius; k <= radius; k++)
                    {
                        int yy = Clamp(y + k, 0, rows - 1);
                        acc += tmp[yy, x] * g[k + radius];
                    }
                    double v = acc;                         // already in [0,1]
                    dst[y, x] = (threshold < 1.0)
                                 ? (v >= threshold ? 1.0 : 0.0)
                                 : v;
                }
            }
            return dst;
        }

        private static int Clamp(int v, int lo, int hi) => (v < lo) ? lo : (v > hi) ? hi : v;





        // A ramp of increasing “density” – you can tweak or extend this.
        private static readonly char[] Shades = new char[] { ' ', '.', ':', '-', '=', '+', '*', '#', '▇' };

        /// <summary>
        /// Prints the given 2D array to the console, mapping lower values to light characters
        /// and higher values to darker/denser ones (▇ at the high end).
        /// </summary>
        public static void PrintShaded(double[,] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            int rows = data.GetLength(0), cols = data.GetLength(1);
            // Find min & max
            double min = double.MaxValue, max = double.MinValue;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    var v = data[i, j];
                    if (v < min) min = v;
                    if (v > max) max = v;
                }
            double range = max - min;
            if (range == 0) range = 1;  // avoid div0 if all same

            // Print each row
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // normalize to [0,1]
                    double t = (data[i, j] - min) / range;
                    // map to shade index
                    int idx = (int)(t * (Shades.Length - 1));
                    Console.Write(Shades[idx]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("============================================================");
        }

    }
}
