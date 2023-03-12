using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation
{
    // The fastest way of computing PCA using "eigenvalues" and "eigenvectors"
    // is done with the QR algorithm
    // more information can be found here
    // http://madrury.github.io/jekyll/update/statistics/2017/10/04/qr-algorithm.html#:~:text=The%20standard%20algorithm%20for%20computing,process%20for%20orthonormalizing%20a%20basis).
    // https://www.youtube.com/watch?v=FAnNBw7d0vg
    // and here is the overall operation
    // https://medium.com/analytics-vidhya/understanding-principle-component-analysis-pca-step-by-step-e7a4bb4031d9

    [Serializable]
    [DataContract(IsReference = true)]
    public class PCAitem
    {
        [DataMember]
        public double _eigenValue;
        [DataMember]
        public EigenVectorItem[] EigenVector; // PC loading scores

        [DataMember]
        public bool _selected = true;

        public PCAitem Clone()
        {
            PCAitem pcaitem = new PCAitem();
            pcaitem._eigenValue = _eigenValue;
            pcaitem.EigenVector = new EigenVectorItem[EigenVector.Length];
            for (int i = 0; i < EigenVector.Length; i++)
                pcaitem.EigenVector[i] = new EigenVectorItem { FeatureLabel = EigenVector[i].FeatureLabel, loadingScore = EigenVector[i].loadingScore };
            pcaitem._selected = _selected;

            return pcaitem;
        }
    }
    [Serializable]
    [DataContract(IsReference = true)]
    public class EigenVectorItem
    {
        [DataMember]
        public string FeatureLabel;
        [DataMember]
        public double loadingScore;
    }

    partial class DataVisualisationForm
    {
        List<PCAitem> PCA;

        int _selectedColumn;

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void saveAsImageButton_Click(object sender, EventArgs e)
        {
            Garage.saveChartAsImage(pcaChart);
        }

        private void pcaChart_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown)
            {
                EventHandlers.signalExhibitor_MouseMove(sender, e, _previousMouseX, _previousMouseY);
                _previousMouseX = e.X;
                _previousMouseY = e.Y;
            }
            else
            {
                // If yes then color the column where the mouse cursor is above
                // Check which column is the mouse cursor above to

                // Get value of x position of the mouse's cursor
                double xValue = ((sender as Chart).ChartAreas[0].AxisX.PixelPositionToValue(e.X));

                // Check which column is the mouse above
                // (each column is centered in a normal number (1, 2, 3,...) with a width of 0.8)
                int selCol = (int)xValue - 1;
                double offset = xValue % 1;
                // Check if the offset is inside current selected column or next column
                if (offset < 0.4d)
                {
                    // If yes then the cursor is inside current selected column
                }
                else if (offset > 0.6d)
                {
                    // If yes then the next column is the selected one
                    selCol++;
                }
                else
                {
                    // If yes then unselect the previous selected column
                    selCol = -1;
                }

                // Check which column is being selected
                if (selCol > -1 && selCol < pcaChart.Series[0].Points.Count)
                {
                    // Change color of the selected column
                    pcaChart.Series[0].Points[selCol].Color = System.Drawing.Color.Green;
                    _selectedColumn = selCol;
                }
                else
                {
                    // Change color of all unselected columns
                    foreach (DataPoint point in pcaChart.Series[0].Points)
                        point.Color = System.Drawing.Color.DodgerBlue;
                    _selectedColumn = -1;
                }
            }
        }

        private void pcaChart_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Check if the click didn't move
            if (_firstMouseX == e.X && _firstMouseY == e.Y)
                // If yes then check if there is a selected column from the chart
                if (_selectedColumn > -1)
                {
                    // If yes then show eigenvector detail of the selected column
                    string eigenvector = "";
                    foreach (EigenVectorItem EigenVecItem in PCA[_selectedColumn].EigenVector)
                        eigenvector += ". " + EigenVecItem.FeatureLabel + ": " + EigenVecItem.loadingScore + "\n";
                    MessageBox.Show(eigenvector, "eigenvector of PC" + (_selectedColumn + 1) + " (loading scores)", MessageBoxButtons.OK);
                }
        }

        private void saveChangesButton_Click(object sender, EventArgs e)
        {
            // Remove previous selection of eigenvectors
            _arthtModelsDic[_modelName].ARTHTModelsDic[_stepName].PCA.Clear();
            // Save selected eigenvectors in _pcLoadingScoresArray
            foreach (PCAitem pcaItem in PCA)
                _arthtModelsDic[_modelName].ARTHTModelsDic[_stepName].PCA.Add(pcaItem.Clone());

            // Update model in models table with the new eigenvectors
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.Update("models", new string[] { "the_model" },
                new Object[] { Garage.ObjectToByteArray(_arthtModelsDic[_modelName].Clone()) }, _modelId, "PCADataVis");

            // Update the model
            // Initialize features lists
            Dictionary<string, List<Sample>> dataLists = new Dictionary<string, List<Sample>>();
            dataLists.Add(_stepName, DataList);

            // Send features for fitting
            // Search for AIToolsForm
            AIToolsForm aIToolsForm = null;
            if (Application.OpenForms.OfType<AIToolsForm>().Count() == 1)
                aIToolsForm = Application.OpenForms.OfType<AIToolsForm>().First();
            // Check which model is selected
            if (_modelName.Contains("Neural network"))
            {
                // This is for neural network
                aIToolsForm._tFBackThread._queue.Enqueue(new QueueSignalInfo()
                {
                    TargetFunc = "fit",
                    CallingClass = "PCADataVis",
                    ModelsName = _modelName,
                    DataLists = dataLists,
                    _datasetSize = -1,
                    _modelId = _modelId,
                    StepName = _stepName
                });
                aIToolsForm._tFBackThread._signal.Set();
            }
            else if (_modelName.Contains("K-Nearest neighbor"))
            {
                // This is for knn
                KNNBackThread kNNBackThread = new KNNBackThread(_arthtModelsDic, aIToolsForm);
                Thread knnThread = new Thread(() => kNNBackThread.fit(_modelName, dataLists, -1, _modelId, _stepName));
                knnThread.Start();
            }
            else if (_modelName.Contains("Naive bayes"))
            {
                // This is for naive bayes
                NaiveBayesBackThread naiveBayesBackThread = new NaiveBayesBackThread(_arthtModelsDic, aIToolsForm);
                Thread nbThread = new Thread(() => naiveBayesBackThread.fit(_modelName, dataLists, -1, _modelId, _stepName));
                nbThread.Start();
            }
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        private void setPCAVisTab()
        {
            // Check if PCA is already computed
            if (PCA != null)
                if (PCA.Count > 0)
                    // If yes then just return
                    return;

            PCA = getPCA(DataList);

            // Insert eigenvalues in pcaChart
            for (int i = 0; i < PCA.Count; i++)
            {
                pcaChart.Series[0].Points.AddXY("PC" + (i + 1), Math.Round(PCA[i]._eigenValue, 4));
                CheckBox pcaCheckBox = createCheckBox(pcaChart.Series[0].Points[i].AxisLabel, i, pcaCheckBox_CheckedChanged);
                pcFlowLayoutPanel.Controls.Add(pcaCheckBox);
            }

            // Check the previously selected PCs
            for (int i = 0; i < _arthtModelsDic[_modelName].ARTHTModelsDic[_stepName].PCA.Count; i++)
            {
                PCAitem pcaItem = _arthtModelsDic[_modelName].ARTHTModelsDic[_stepName].PCA[i];
                if (pcaItem._selected)
                    ((CheckBox)pcFlowLayoutPanel.Controls[i]).Checked = true;
            }
        }

        private void pcaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox pcaCheckBox = (sender as CheckBox);
            // Change the selection property of its corresponding PCAitem
            PCA[(int)pcaCheckBox.Tag]._selected = pcaCheckBox.Checked;
        }

        public static List<PCAitem> getPCA(List<Sample> dataList)
        {
            List<PCAitem> pca;

            // Initialise input data
            double[][] data = new double[dataList.Count][];
            for (int i = 0; i < dataList.Count; i++)
                data[i] = dataList[i].getFeatures();
            // Get features labels and sort them by index
            string[] featuresLabels = null;
            if (dataList.Count > 0)
                featuresLabels = dataList[0].DataParent.FeaturesLabelsIndx.OrderBy(feature => feature.Value).Select(feature => feature.Key).ToArray();

            // Standardize data and compute covariance matrix
            double[][] covMat = coVarMatForStandardizedData(standardizeData(data));

            // Compute PCA (QR algorithm of covMat)
            // where the columns of qMat are the eigenvectors, and the diagonal of rMat is the eigenvalues
            double[][] qMat, rMat, prevQMat;
            (qMat, rMat) = getQRMat(covMat);
            // Set the convergence tolerance of qMat
            double tol = 0.00001;
            // Converge the qMat
            for (int i = 0; i < 100; i++)
            {
                prevQMat = qMat;
                (qMat, rMat) = getQRMat(matMultiply(covMat, qMat));

                if (matSubtract(prevQMat, qMat).Item2 < tol)
                    break;
            }

            // Set a list for eigenvalue and its corresponding eigenvector
            List<(double, EigenVectorItem[])> PCList = new List<(double, EigenVectorItem[])>();
            EigenVectorItem[] eigenVector;
            for (int col = 0; col < qMat[0].Length; col++)
            {
                eigenVector = new EigenVectorItem[qMat.GetLength(0)];
                for (int row = 0; row < eigenVector.Length; row++)
                    eigenVector[row] = new EigenVectorItem { FeatureLabel = featuresLabels[row], loadingScore = qMat[row][col] };
                PCList.Add((rMat[col][col], eigenVector));
            }
            // Sort the eigenvectors according to eigenvalues in a descending way
            PCList.Sort((e1, e2) => { return e2.Item1.CompareTo(e1.Item1); });

            // Copy eigenvectors in _eigenVectors
            pca = new List<PCAitem>(PCList.Count);
            for (int i = 0; i < PCList.Count; i++)
            {
                pca.Add(new PCAitem() { _eigenValue = PCList[i].Item1, EigenVector = PCList[i].Item2 });
            }

            return pca;
        }

        private static double[][] standardizeData(double[][] data)
        {
            // Calculate mean and standard deviation of each column (mean row, and standard deviation row)
            double[] means = new double[data[0].Length], stdDevs = new double[data[0].Length];
            double[] colVals;
            for (int col = 0; col < means.Length; col++)
            {
                // Copy the column of attributes
                colVals = new double[data.GetLength(0)];
                for (int row = 0; row < colVals.Length; row++)
                    colVals[row] = data[row][col];
                // Compute the mean
                means[col] = (Garage.MeanMinMax(colVals)).mean;
                // Compute standard deviation
                stdDevs[col] = Garage.stdDevCalc(colVals, means[col]);
            }
            // Standardize data
            double[][] stdzedData = new double[data.GetLength(0)][];
            for (int row = 0; row < data.GetLength(0); row++)
                stdzedData[row] = new double[data[row].Length];
            for (int col = 0; col < data[0].Length; col++)
                for (int row = 0; row < data.GetLength(0); row++)
                {
                    stdzedData[row][col] = (data[row][col] - means[col]) / stdDevs[col];
                    if (double.IsNaN(stdzedData[row][col]))
                        stdzedData[row][col] = 0d;
                }

            return stdzedData;
        }

        private static double[][] coVarMatForStandardizedData(double[][] data)
        {
            // Initialize coVar matrix
            double[][] covarMat = new double[data[0].Length][];
            // Compute covariance for each row
            int totalCols = data.GetLength(0);
            for (int covarRow = 0; covarRow < covarMat.GetLength(0); covarRow++)
            {
                double[] coVarRow = new double[covarMat.GetLength(0)];
                for (int covarCol = 0; covarCol < covarMat.GetLength(0); covarCol++)
                {
                    foreach (double[] dataRow in data)
                        coVarRow[covarCol] += dataRow[covarCol] * dataRow[covarRow] / totalCols;
                }
                // Add the new coVar row in covarMat
                covarMat[covarRow] = coVarRow;
            }

            return covarMat;
        }

        /**
         * qMat columns are the eigenvectors
         * diagonal elements of rMat are the eigenvalues
         */
        private static (double[][], double[][]) getQRMat(double[][] data)
        {
            // Initialize Q matrix which has the same number of rows as data
            double[][] qMat = new double[data.GetLength(0)][];
            for (int row = 0; row < qMat.GetLength(0); row++)
                qMat[row] = new double[data[0].Length];
            // Initialize R matrix which has the number of qMat columns as the number of rows
            double[][] rMat = new double[data[0].Length][];
            for (int row = 0; row < rMat.GetLength(0); row++)
                rMat[row] = new double[data.GetLength(0)];

            // Q matrix is the orthogonalization of data rows
            // Orthogonalize data column using the Gram Schmidt algorithm
            // Iterate through each column in data and orthogonalize it with what is in qMat
            double[] newPsi;
            double dotProductCoef;
            double normalizationCoef;
            for (int col = 0; col < data[0].Length; col++)
            {
                newPsi = new double[data.GetLength(0)];
                for (int row = 0; row < newPsi.Length; row++)
                    newPsi[row] = data[row][col];

                // Iterate through each column in qMat
                // and remove the projection of the selected vector on the qMat vectors from the seleced vector
                for (int qMatCol = 0; qMatCol < col; qMatCol++)
                {
                    double[] Psi = Enumerable.Range(0, qMat.GetLength(0)).Select(x => qMat[x][qMatCol]).ToArray();

                    // Calculate the new dotProductCoef between current vector and psi
                    dotProductCoef = 0D;
                    for (int row = 0; row < newPsi.Length; row++)
                        dotProductCoef += newPsi[row] * Psi[row];

                    // Remove the projevtion from the vector
                    for (int row = 0; row < newPsi.Length; row++)
                        newPsi[row] -= dotProductCoef * Psi[row];

                    // Insert dotProductCoef in rMat
                    rMat[qMatCol][col] = dotProductCoef;
                }

                // Compute the magnitude of the new vector for normalization
                normalizationCoef = 0D;
                foreach (double sample in newPsi)
                    normalizationCoef += sample * sample;
                normalizationCoef = Math.Sqrt(normalizationCoef);
                // Normalize the vector and insert it in qMat
                for (int row = 0; row < newPsi.Length; row++)
                {
                    newPsi[row] = newPsi[row] / normalizationCoef;
                    if (double.IsNaN(newPsi[row]))
                        newPsi[row] = 0d;
                    qMat[row][col] = newPsi[row];
                }

                // Insert normalizationCoef in rMat
                rMat[col][col] = normalizationCoef;
            }

            return (qMat, rMat);
        }

        private static double[][] matMultiply(double[][] mat1, double[][] mat2)
        {
            double[][] result = new double[mat1.GetLength(0)][];
            // Check if number of columns of mat1 is equal to number of rows in mat2
            if (mat1.GetLength(0) == mat2[0].Length)
            {
                // If yes then start multiplication
                double buffer = 0d;
                for (int row = 0; row < result.GetLength(0); row++)
                {
                    // Initialize a row
                    result[row] = new double[mat2[0].Length];
                    for (int col = 0; col < result[row].Length; col++)
                    {
                        // Compute the multiplication coefficient for the selected row and column
                        buffer = 0d;
                        for (int i = 0; i < result.GetLength(0); i++)
                            buffer += mat1[row][i] * mat2[i][col];
                        // Insert result
                        result[row][col] = buffer;
                    }
                }
            }

            return result;
        }

        private static (double[][], double) matSubtract(double[][] mat1, double[][] mat2)
        {
            double[][] result = new double[mat1.GetLength(0)][];
            double elementWiseResult = 0;
            double totalElementsNumb = mat1.GetLength(0) * mat1[0].Length;
            // Check if both matrices have the same row and column numbers
            if (mat1.GetLength(0) == mat2.GetLength(0) && mat1[0].Length == mat2[0].Length)
            {
                // If yes then start subtraction
                for (int row = 0; row < result.GetLength(0); row++)
                {
                    // Initialize a row
                    result[row] = new double[mat2[0].Length];
                    for (int col = 0; col < result[row].Length; col++)
                    {
                        // Compute the subtraction of the selected row and column and insert it in result matrix and in elementWiseResult
                        result[row][col] = mat1[row][col] - mat2[row][col];
                        // Update totalElementsNumb
                        elementWiseResult += Math.Abs(result[row][col]) / totalElementsNumb;
                    }
                }
            }

            return (result, elementWiseResult);
        }
    }
}
