using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biological_Signal_Processing_Using_AI.Garage
{
    internal class MatrixTools
    {
        public static void MatLog(double[,] mat, string title)
        {
            Debug.WriteLine(title);
            for (int row = 0; row < mat.GetLength(0); row++)
            {
                for (int col = 0; col < mat.GetLength(1); col++)
                    Debug.Write(mat[row, col] + "   ");

                Debug.Write("\n");
            }
        }

        public static double[,] MatIdentity(int size)
        {
            double[,] result = new double[size, size];
            for (int diag = 0; diag < size; diag++)
                result[diag, diag] = 1;

            return result;
        }

        public static void MatSetRow(double[,] mat, int row, double[] values)
        {
            if (values.Length == mat.GetLength(1))
                for (int col = 0; col < mat.GetLength(1); col++)
                    mat[row, col] = values[col];
        }
        public static void MatSetCol(double[,] mat, int col, double[] values)
        {
            if (values.Length == mat.GetLength(0))
                for (int row = 0; row < mat.GetLength(0); row++)
                    mat[row, col] = values[row];
        }

        public static double[,] Vecs2Mat(double[][] vecs) // double[columns][rows] each column is a sample
        {
            // Get the length of the longest column in vecs
            int maxLength = vecs.Max(vector => vector.Length);

            // Initialize the matrix
            double[,] result = new double[maxLength, vecs.Length];

            // Each column in the matrix represents a sample
            for (int col = 0; col < result.GetLength(1); col++)
                for (int row = 0; row < vecs[col].Length; row++)
                    result[row, col] = vecs[col][row];

            return result;
        }

        public static double[][] Mat2Vecs(double[,] mat) // double[rows, columns] each column is a sample
        {
            // Initialize the vector of vectors
            // Each column of the mat represents a sample
            double[][] result = new double[mat.GetLength(1)][];

            for (int col = 0; col < mat.GetLength(1); col++)
            {
                result[col] = new double[mat.GetLength(0)];
                for (int row = 0; row < mat.GetLength(0); row++)
                    result[col][row] = mat[row, col];
            }

            return result;
        }

        public static double[,] MatTranspose(double[,] mat) // double[rows, columns] each column is a sample
        {
            double[,] result = new double[mat.GetLength(1), mat.GetLength(0)];

            // Copy each row from mat to be a column in result
            for (int row = 0; row < mat.GetLength(0); row++)
                for (int col = 0; col < mat.GetLength(1); col++)
                    result[col, row] = mat[row, col];

            return result;
        }

        public static double[][] VecsTranspose(double[][] vecs) // double[columns][rows] each column is a sample
        {
            // Get the length of the longest column in vecs
            int maxLength = vecs.Max(vector => vector.Length);
            double[][] result = new double[maxLength][];

            // Copy each row from vecs to be a column in result
            for (int row = 0; row < maxLength; row++)
            {
                result[row] = new double[vecs.Length];
                for (int col = 0; col < vecs.Length; col++)
                    if (vecs[col].Length > row)
                        result[row][col] = vecs[col][row];
            }

            return result;
        }

        public static double[,] MatMultiply(double[,] mat1, double[,] mat2) // double[rows, columns] each column is a sample
        {
            double[,] result = new double[mat1.GetLength(0), mat2.GetLength(1)];
            // Check if number of columns of mat1 is equal to number of rows in mat2
            if (mat1.GetLength(1) == mat2.GetLength(0))
            {
                // If yes then start multiplication
                double buffer = 0d;
                for (int row = 0; row < result.GetLength(0); row++)
                {
                    // Initialize a row
                    for (int col = 0; col < result.GetLength(1); col++)
                    {
                        // Compute the multiplication coefficient for the selected row and column
                        buffer = 0d;
                        for (int i = 0; i < mat1.GetLength(1); i++)
                            buffer += mat1[row, i] * mat2[i, col];
                        // Insert result
                        result[row, col] = buffer;
                    }
                }
            }

            return result;
        }

        public static double[][] VecsMultiply(double[][] vecs1, double[][] vecs2) // double[columns][rows] each column is a sample
        {
            // Check if any sample is different in size in each of vecs1 and vecs2
            int maxLengthVecs1 = vecs1.Max(vector => vector.Length);
            foreach (double[] column in vecs1)
                if (column.Length != maxLengthVecs1)
                    // If yes then just return null
                    return null;
            int maxLengthVecs2 = vecs2.Max(vector => vector.Length);
            foreach (double[] column in vecs2)
                if (column.Length != maxLengthVecs2)
                    // If yes then just return null
                    return null;

            // Check if the number of columns of vecs1 is equal to the number of rows in vecs2
            if (vecs1.Length == maxLengthVecs2)
            {
                // If yes then start multiplication
                double[][] result = new double[vecs2.Length][];

                double buffer;
                for (int col = 0; col < vecs2.Length; col++)
                {
                    // Initialize a row
                    result[col] = new double[maxLengthVecs1];
                    for (int row = 0; row < maxLengthVecs1; row++)
                    {
                        // Compute the multiplication coefficient for the selected row and column
                        buffer = 0d;
                        for (int i = 0; i < vecs1.Length; i++)
                            buffer += vecs1[i][row] * vecs2[col][i];
                        // Insert result
                        result[col][row] = buffer;
                    }
                }
                return result;
            }

            return null;
        }

        public static (double[,] result, double elementWiseResult) MatSubtract(double[,] mat1, double[,] mat2) // double[rows, columns] each column is a sample
        {
            double[,] result = new double[mat1.GetLength(0), mat1.GetLength(1)];
            double elementWiseResult = 0;
            double totalElementsNumb = mat1.GetLength(0) * mat1.GetLength(1);
            // Check if both of mat1 and mat2 have the same size of rows and columns
            if (mat1.GetLength(0) == mat2.GetLength(0) && mat1.GetLength(1) == mat2.GetLength(1))
                // If yes then start subtraction
                for (int row = 0; row < result.GetLength(0); row++)
                    for (int col = 0; col < result.GetLength(1); col++)
                    {
                        // Compute the subtraction of the selected row and column and insert it in result matrix and in elementWiseResult
                        result[row, col] = mat1[row, col] - mat2[row, col];
                        // Update totalElementsNumb
                        elementWiseResult +=  Math.Abs(result[row, col]) / totalElementsNumb;
                    }

            return (result, elementWiseResult);
        }

        public static (double[][] result, double elementWiseResult) VecsSubtract(double[][] vecs1, double[][] vecs2) // double[columns][rows] each column is a sample
        {
            double[][] result = new double[vecs1.Length][];
            double elementWiseResult = 0;
            double totalElementsNumb = 0;
            // Check if both of vecs1 and vecs2 have the same columns count
            if (vecs1.Length == vecs2.Length)
            {
                // If yes then start subtraction
                for (int col = 0; col < result.Length; col++)
                {
                    // Check if the column size selected from vecs1 is different than that in vecs2
                    if (vecs1[col].Length != vecs2[col].Length)
                        // If yes then return null
                        return (null, double.MaxValue);
                    // Initialize a column
                    result[col] = new double[vecs2[col].Length];
                    for (int row = 0; row < result[col].Length; row++)
                    {
                        // Compute the subtraction of the selected row and column and insert it in result matrix and in elementWiseResult
                        result[col][row] = vecs1[col][row] - vecs2[col][row];
                        // Update totalElementsNumb (rows might not be of the same size. That's why it's movingliy averaged)
                        elementWiseResult = ((elementWiseResult * totalElementsNumb) + Math.Abs(result[col][row])) / (totalElementsNumb + 1);
                        totalElementsNumb++;
                    }
                }
            }

            return (result, elementWiseResult);
        }

        /// <summary>
        /// mat: should be of double[rows, columns] each column is a sample and each row is a variable measurements.<br/>
        /// MatRowsStandardize computes the mean and standard deviation of each row (one variable measurements),<br/>
        /// then subtracts the mean from each element in the row and devides it by the standard deviation.
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static double[,] MatRowsStandardize(double[,] mat) // double[rows, columns] each column is a sample
        {
            // Compute the mean and standard deviation of each row (each row represents the measurements of a variable in the samples/columns)
            double[,] stdzedMat = new double[mat.GetLength(0), mat.GetLength(1)];
            for (int row = 0; row < mat.GetLength(0); row++)
            {
                // Compute the mean and standard deviation of the selected column
                double[] selectedRow = Enumerable.Range(0, mat.GetLength(1)).Select(col => mat[row, col]).ToArray();
                double mean = GeneralTools.MeanMinMax(selectedRow).mean;
                double stdDev = GeneralTools.stdDevCalc(selectedRow, mean);

                for (int col = 0; col < mat.GetLength(1); col++)
                {
                    stdzedMat[row, col] = (mat[row, col] - mean) / stdDev;
                    if (double.IsNaN(stdzedMat[row, col]))
                        stdzedMat[row, col] = 0d;
                }
            }

            return stdzedMat;
        }

        /// <summary>
        /// mat: should be of double[rows, columns] each column is a sample and each row is a variable measurements.<br/>
        /// MatRowsCoVarForStandardizedData assumes that the rows of mat are standardized, which allows it to ignore computing the mean of each row.<br/>
        /// MatRowsCoVarForStandardizedData computes the covariance of the rows.
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static double[,] MatRowsCoVarForStandardizedData(double[,] mat) // double[rows, columns] each column is a sample
        {
            // Compute the covariance between the variables (the rows of mat) and not the samples (the columns of mat)
            double[,] covarMat = new double[mat.GetLength(0), mat.GetLength(0)];
            // Compute covariance for each row
            int totalCols = mat.GetLength(1) -1;
            for (int covarRow = 0; covarRow < covarMat.GetLength(0); covarRow++)
            {
                for (int covarCol = 0; covarCol < covarMat.GetLength(1); covarCol++)
                    for (int col = 0; col < mat.GetLength(1); col++)
                        covarMat[covarRow, covarCol] += mat[covarRow, col] * mat[covarCol, col] / totalCols;

            }

            return covarMat;
        }

        /// <summary>
        /// mat: should be of double[rows, columns] each column is a sample and each row is a variable measurements.<br/>
        /// MatColsOrthogonalize creates columns orthogonal matrix qMat where:<br/>
        /// mat = qMat . rMat
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static (double[,] qMat, double[,] rMat) MatColsOrthogonalize(double[,] mat) // double[rows, columns] each column is a sample
        {
            // Q matrix is the orthogonalization of mat columns (eigen vectors)
            // Orthogonalize mat columns using the Gram Schmidt algorithm
            // Iterate through each column in mat and orthogonalize it with what is in qMat

            // The Q matrix is of same dimensions sizes as mat
            double[,] qMat = new double[mat.GetLength(0), mat.GetLength(1)];
            // The R matrix is a square matrix that takes the size of mat columns
            double[,] rMat = new double[mat.GetLength(1), mat.GetLength(1)];

            // Start the orthogonalization
            for (int col = 0; col < mat.GetLength(1); col++)
            {
                // Initialize the new Psi (Psi is an orthogonal column in the Q matrix)
                double[] newPsi = Enumerable.Range(0, mat.GetLength(0)).Select(x => mat[x, col]).ToArray();

                // Iterate through each column in qMat
                // and remove the projection of the selected vector on the qMat vectors from the seleced vector
                for (int qMatCol = 0; qMatCol < col; qMatCol++)
                {
                    double[] Psi = Enumerable.Range(0, qMat.GetLength(0)).Select(x => qMat[x, qMatCol]).ToArray();

                    // Calculate the new dotProductCoef between current vector and psi
                    double dotProductCoef = 0d;
                    for (int qMatRow = 0; qMatRow < newPsi.Length; qMatRow++)
                        dotProductCoef += newPsi[qMatRow] * Psi[qMatRow];

                    // Remove the projevtion from the vector
                    for (int qMatRow = 0; qMatRow < newPsi.Length; qMatRow++)
                        newPsi[qMatRow] -= dotProductCoef * Psi[qMatRow];

                    // Insert dotProductCoef in rMat
                    rMat[qMatCol, col] = dotProductCoef;
                }

                // Normalize newPsi and insert it in qMat (newPsi is the new eigenvector)
                (newPsi, double normalizationCoef) = GeneralTools.vectorNormalization(newPsi);
                for (int qMatRow = 0; qMatRow < newPsi.Length; qMatRow++)
                    qMat[qMatRow, col] = newPsi[qMatRow];

                // Insert normalizationCoef in rMat (the diagonal values are the eigenvalues)
                rMat[col, col] = normalizationCoef;
            }

            return (qMat, rMat);
        }

        /// <summary>
        /// mat: should be of double[rows, columns] each column is a sample and each row is a variable measurements.<br/>
        /// tol: sets the threshold to stop ameliorating the decomposition if the sum of both triangles in sMat are less than tol.<br/>
        /// MatSVD decomposes mat into three matrices where:<br/>
        /// mat = uMat . sMat . vMat_transpose<br/>
        /// . uMat and vMat are orthogonal matrices.<br/>
        /// . sMat is a diagonal matrix.
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="tol"></param>
        /// <returns></returns>
        public static (double[,] uMat, double[,] sMat, double[,] vMat) MatSVD(double[,] mat, double tol) // double[rows, columns] each column is a sample
        {
            double[,] rMat1 = mat;
            double[,] uMat = MatIdentity(mat.GetLength(0));
            double[,] vMat = MatIdentity(mat.GetLength(1));

            for (int i = 0; i < 100; i++)
            {
                (double[,] qMat1, rMat1) = MatColsOrthogonalize(rMat1);
                (double[,] qMat2, double[,] rMat2) = MatColsOrthogonalize(MatTranspose(rMat1));

                rMat1 = MatTranspose(rMat2);

                uMat = MatMultiply(uMat, qMat1);
                vMat = MatMultiply(vMat, qMat2);

                // Check if thhe sum of the upper and lower triangle are less than the convergence tolerance
                double triaSum = 0;
                for (int row = 0; row < rMat1.GetLength(0); row++)
                    for (int col = 0; col < rMat1.GetLength(1); col++)
                        if (col != row)
                            triaSum += Math.Abs(rMat1[row, col]);

                if (triaSum < tol)
                    break;
            }

            double[,] sMat = rMat1;

            return (uMat, sMat, vMat);
        }

        /// <summary>
        /// mat: should be of double[rows, columns] each column is a sample and each row is a variable measurements.<br/>
        /// tol: sets the threshold to stop ameliorating the decomposition if the sum of both triangles in sMat are less than tol.<br/>
        /// MatPCA finds the relationship between the variables for dimention reduction purpose.
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="tol"></param>
        /// <returns></returns>
        public static (double[,] eigVecMat, double[] eigVals) MatPCA(double[,] mat, double tol) // double[rows, columns] each column is a sample and each row
        {
            // Compute the variables covariance (rows covariance) matrix
            double[,] covMat = MatRowsCoVarForStandardizedData(MatRowsStandardize(mat));

            // You can either compute the eigVecMat and eigVals using the covMat where:
            // 1. covMat . eigVecMat = eigVecMat . eigValsMat
            // 2. eigVecMat is columns orthogonal matrix
            // 3. so just keep computing the QR decomosition of (covMat . prev_qMat) until prev_qMat ~= qMat
            // 4. finally, from the QR decomposition of (covMat . prev_qMat):
            // eigVecMat = qMat, and
            // eigValsMat = rMat

            // Or you can compute SVD of mat where:
            // 1. mat = uMat . sMat . vMat_transpose
            // 2. eigVecMat = vMat = qMat
            // 3. Now we have two options:
            // 3.1. from the QR decomposition of (covMat . qMat):
            // eigVecMat = qMat, and
            // eigValsMat = rMat
            // 3.2. try extracting rMat from the equation: covMat . qMat = qMat . rMat
            // -> qMat_transpose . covMat_transpose = rMat_transpose . qMat_transpose
            // since qMat is an orthogonal matrix then qMat_transpose . qMat = identity matrix
            // -> rMat_transpose = qMat_transpose . covMat_transpose . qMat
            // -> rMat = qMat_transpose . covMat . qMat

            // I prefer using the SVD with the second option
            // Since PCA is a way of reducing variables, then the orthogonalization should be for the variables (rows), not the samples (columns)
            // then pass the transpose of mat for the SVD
            (double[,] uMat, double[,] sMat, double[,] vMat) = MatSVD(MatTranspose(mat), tol);

            // vMat holds the eigenvectors, rMat holds the eigenvalues (the diagonal elements are the eigenvalues)
            double[,] rMat = MatMultiply(MatMultiply(MatTranspose(vMat), covMat), vMat);

            // Sort the eigenvectors according to their correspoding eigenvalues descendingly
            int[] DescEigValsIndx = Enumerable.Range(0, rMat.GetLength(0)).OrderByDescending(index => rMat[index, index]).ToArray();

            double[,] eigVecMat = new double[vMat.GetLength(0), vMat.GetLength(1)];
            double[] eigVals = new double[DescEigValsIndx.Length];

            for (int col = 0; col < DescEigValsIndx.Length; col++)
            {
                double[] eigVec = Enumerable.Range(0, vMat.GetLength(0)).Select(row => vMat[row, DescEigValsIndx[col]]).ToArray();
                MatSetCol(eigVecMat, col, eigVec);
                eigVals[col] = rMat[DescEigValsIndx[col], DescEigValsIndx[col]];
            }

            return (eigVecMat, eigVals);
        }
    }
}
