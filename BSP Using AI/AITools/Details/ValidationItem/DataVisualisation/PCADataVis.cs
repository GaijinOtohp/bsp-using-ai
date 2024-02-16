using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.Garage;
using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Forms;
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
            GeneralTools.saveChartAsImage(pcaChart);
        }

        private void pcaChart_MouseMove(object sender, MouseEventArgs e)
        {
            // If yes then color the column where the mouse cursor is above
            // Check which column is the mouse cursor above to

            // Get value of x position of the mouse's cursor
            Plot plot = pcaChart.Plot;
            IPlottable[] plottable = plot.GetPlottables();
            (double x, double y) = plot.GetCoordinate(e.X, e.Y);

            // Check which column is the mouse above
            // (each column is centered in a normal number (1, 2, 3,...) with a width of 0.8)
            int selCol = (int)x;
            double offset = x % 1;
            // Check if the offset is inside current selected column or next column
            if (offset < 0.4d && offset > -0.4d)
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
            if (selCol > -1 && selCol < ((BarPlot)plottable[0]).Positions.Length)
            {
                // Change color of the selected column
                ((BarPlot)plottable[1]).Values = new double[] { ((BarPlot)plottable[0]).Values[selCol] };
                ((BarPlot)plottable[1]).Positions = new double[] { ((BarPlot)plottable[0]).Positions[selCol] };
                _selectedColumn = selCol;
            }
            else
            {
                // Change color of all unselected columns
                ((BarPlot)plottable[1]).Values = new double[] { 0 };
                _selectedColumn = -1;
            }
            pcaChart.Refresh();
        }

        private void pcaChart_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Update mouse X and Y indexes
            _firstMouseX = e.X;
            _firstMouseY = e.Y;
        }
        private void pcaChart_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
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
            _arthtModelsDic[_ModelName + _ProblemName].ARTHTModelsDic[_stepName].PCA.Clear();
            // Save selected eigenvectors in _pcLoadingScoresArray
            foreach (PCAitem pcaItem in PCA)
                _arthtModelsDic[_ModelName + _ProblemName].ARTHTModelsDic[_stepName].PCA.Add(pcaItem.Clone());

            // Update model in models table with the new eigenvectors
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.Update("models", new string[] { "the_model" },
                new Object[] { GeneralTools.ObjectToByteArray(_arthtModelsDic[_ModelName + _ProblemName].Clone()) }, _modelId, "PCADataVis");

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
            if (_ModelName.Equals(NeuralNetworkModel.ModelName))
            {
                // This is for neural network
                aIToolsForm._tFBackThread._queue.Enqueue(new QueueSignalInfo()
                {
                    TargetFunc = "fit",
                    CallingClass = "PCADataVis",
                    ModelsName = _ModelName + _ProblemName,
                    DataLists = dataLists,
                    _datasetSize = -1,
                    _modelId = _modelId,
                    StepName = _stepName
                });
                aIToolsForm._tFBackThread._signal.Set();
            }
            else if (_ModelName.Equals(KNNModel.ModelName))
            {
                // This is for knn
                KNNBackThread kNNBackThread = new KNNBackThread(_arthtModelsDic, aIToolsForm);
                Thread knnThread = new Thread(() => kNNBackThread.fit(_ModelName + _ProblemName, dataLists, -1, _modelId, _stepName));
                knnThread.Start();
            }
            else if (_ModelName.Equals(NaiveBayesModel.ModelName))
            {
                // This is for naive bayes TF_NET_KERAS_NN
                NaiveBayesBackThread naiveBayesBackThread = new NaiveBayesBackThread(_arthtModelsDic, aIToolsForm);
                Thread nbThread = new Thread(() => naiveBayesBackThread.fit(_ModelName + _ProblemName, dataLists, -1, _modelId, _stepName));
                nbThread.Start();
            }
            else if (_ModelName.Equals(TFNETNeuralNetworkModel.ModelName))
            {
                // This is for Tensorflow.Net Neural Networks models
                TF_NET_NN tf_NET_NN = new TF_NET_NN(_arthtModelsDic, aIToolsForm);
                Thread tfNetThread = new Thread(() => tf_NET_NN.fit(_ModelName + _ProblemName, dataLists, -1, _modelId, _stepName));
                tfNetThread.Start();
            }
            else if (_ModelName.Equals(TFKerasNeuralNetworkModel.ModelName))
            {
                // This is for Tensorflow.Keras Neural Networks models
                TF_NET_KERAS_NN tf_Keras_NN = new TF_NET_KERAS_NN(_arthtModelsDic, aIToolsForm);
                Thread tfKerasThread = new Thread(() => tf_Keras_NN.fit(_ModelName + _ProblemName, dataLists, -1, _modelId, _stepName));
                tfKerasThread.Start();
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
            double[] values = new double[PCA.Count];
            double[] positions = new double[PCA.Count];
            string[] labels = new string[PCA.Count];
            for (int i = 0; i < PCA.Count; i++)
            {
                values[i] = Math.Round(PCA[i]._eigenValue, 4);
                positions[i] = i;
                labels[i] = "PC" + (i + 1);
                CheckBox pcaCheckBox = createCheckBox(labels[i], i, pcaCheckBox_CheckedChanged);
                pcFlowLayoutPanel.Controls.Add(pcaCheckBox);
            }
            BarPlot barPlot = pcaChart.Plot.AddBar(values, positions);
            barPlot.ShowValuesAboveBars = true;
            pcaChart.Plot.SetAxisLimits(yMin: 0);
            pcaChart.Plot.AddBar(new double[] { 0 }, new double[] { 0 }, System.Drawing.Color.Green);
            pcaChart.Plot.XTicks(labels);
            pcaChart.Refresh();

            // Check the previously selected PCs
            for (int i = 0; i < _arthtModelsDic[_ModelName + _ProblemName].ARTHTModelsDic[_stepName].PCA.Count; i++)
            {
                PCAitem pcaItem = _arthtModelsDic[_ModelName + _ProblemName].ARTHTModelsDic[_stepName].PCA[i];
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

            // Get eigenvectors and eigenvalues using PCA
            // Set the convergence tolerance of eigVecMat
            double tol = 0.00001;
            (double[,] eigVecMat, double[] eigVals) = MatrixTools.MatPCA(MatrixTools.Vecs2Mat(data), tol);

            // Copy eigenvectors in _eigenVectors
            pca = new List<PCAitem>(eigVals.Length);
            for (int col = 0; col < eigVals.Length; col++)
            {
                EigenVectorItem[] eigVecItem = Enumerable.Range(0, eigVecMat.GetLength(0)).Select(row =>
                                           new EigenVectorItem { FeatureLabel = featuresLabels[row], loadingScore = eigVecMat[row, col] }).ToArray();
                pca.Add(new PCAitem() { _eigenValue = eigVals[col], EigenVector = eigVecItem });
            }

            return pca;
        }
    }
}
