using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI;
using BSP_Using_AI.AITools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tensorflow;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.ReinforcementLearning.Environment;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.AITools.AIBackThreadReportHolder;
using static Tensorflow.Binding;

namespace Biological_Signal_Processing_Using_AI.AITools.RL_Objectives
{
    public class CWD_RL_TFNET
    {
        private AIBackThreadReportHolder _aiBackThreadReportHolderForAIToolsForm;

        private Dictionary<string, ObjectiveBaseModel> _objectivesModelsDic = null;

        string _SelectedModelName;
        int _currentFitStep;
        int _maxFitSteps;

        public CWD_RL_TFNET(Dictionary<string, ObjectiveBaseModel> objectivesModelsDic, AIBackThreadReportHolder aiBackThreadReportHolderForAIToolsForm)
        {
            _objectivesModelsDic = objectivesModelsDic;
            _aiBackThreadReportHolderForAIToolsForm = aiBackThreadReportHolderForAIToolsForm;
        }

        private void UpdateFittingProgress(int currentProgress, int maxProgress)
        {
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new FittingProgAIReport()
                {
                    ReportType = AIReportType.FittingProgress,
                    ModelName = _SelectedModelName,
                    fitProgress = _currentFitStep * maxProgress + currentProgress,
                    fitMaxProgress = _maxFitSteps * maxProgress
                }, "AIToolsForm");
        }

        public void Fit(string modelName, List<Sample> trainingSamplesList, long datasetSize, long modelId)
        {
            _SelectedModelName = modelName;
            _currentFitStep = 0;
            _maxFitSteps = 1;

            CWDReinforcementL cwdReinforcementL = (CWDReinforcementL)_objectivesModelsDic[modelName];
            // Fit features in model
            TF_NET_NN.fit(cwdReinforcementL.CWDReinforcementLModel, cwdReinforcementL.CWDReinforcementLModel.BaseModel, trainingSamplesList, UpdateFittingProgress, saveModel: true, earlyStopThreshold: 0.00001f);

            // Update model in models table
            DbStimulator dbStimulator = new DbStimulator();
            if (cwdReinforcementL.DataIdsIntervalsList.Count > 0)
            {
                Thread dbStimulatorThread = new Thread(() => dbStimulator.Update("models", new string[] { "the_model", "dataset_size" },
                    new object[] { GeneralTools.ObjectToByteArray(cwdReinforcementL.Clone()), datasetSize }, modelId, "CWD_RL"));
                dbStimulatorThread.Start();
            }

            // Send report about fitting is finished and models table should be updated
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new FittingCompAIReport()
                                                                    {
                                                                        ReportType = AIReportType.FittingComplete,
                                                                        ModelName = modelName,
                                                                        datasetSize = datasetSize,
                                                                    }, "AIToolsForm");
        }

        public void createTFNETRLModelForCWD()
        {
            // Set models in name and path
            int modelIndx = 0;
            while (_objectivesModelsDic.ContainsKey(TFNETReinforcementL.ModelName + " for " + CharacteristicWavesDelineation.ObjectiveName + modelIndx))
                modelIndx++;
            string modelPath = System.IO.Directory.GetCurrentDirectory() + @"/AIModels/CWD/TFNETModels/RL" + modelIndx;
            string crazyModelPath = System.IO.Directory.GetCurrentDirectory() + @"/AIModels/CWD/TFNETModels/CrazyRL" + modelIndx;

            // Create the model object
            CWDReinforcementL cwdReinforcementL = new CWDReinforcementL();
            cwdReinforcementL.CWDReinforcementLModel = createTFNETRLModel(CWDNamigs.RLCornersScanData, modelPath, inputDim: 8, outputDim: 2);
            cwdReinforcementL.CWDCrazyReinforcementLModel = createTFNETRLModel(CWDNamigs.RLCornersScanData, crazyModelPath, inputDim: 8, outputDim: 2);

            cwdReinforcementL.ModelName = TFNETReinforcementL.ModelName;
            cwdReinforcementL.ObjectiveName = " for " + CharacteristicWavesDelineation.ObjectiveName + modelIndx;

            _objectivesModelsDic.Add(cwdReinforcementL.ModelName + cwdReinforcementL.ObjectiveName, cwdReinforcementL);

            // Save models in models table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.Insert("models", new string[] { "type_name", "model_target", "the_model", "dataset_size" },
                new object[] { TFNETReinforcementL.ModelName, CharacteristicWavesDelineation.ObjectiveName, GeneralTools.ObjectToByteArray(cwdReinforcementL.Clone()), 0 }, "CWD_RL");

            // Refresh modelsFlowLayoutPanel
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new AIReport() { ReportType = AIReportType.CreateModel }, "AIToolsForm");
        }

        public static TFNETReinforcementL createTFNETRLModel(string name, string path, int inputDim, int outputDim)
        {
            List<RLDimension> dimensionsList = new List<RLDimension>(2);
            dimensionsList.Add(new RLDimension(name: CWDNamigs.AT, size: 60, min: 1, max: 40));
            dimensionsList.Add(new RLDimension(name: CWDNamigs.ART, size: 60, min: 0, max: 0.3d));

            TFNETReinforcementL model = new TFNETReinforcementL(path, inputDim, outputDim, dimensionsList) { Name = name, Type = ObjectiveType.Regression };

            model.BaseModel.Session = createTFNETNeuralNetModelSession(inputDim, outputDim);

            // Save the model
            if (path.Length > 0)
                TF_NET_NN.SaveModelVariables(model.BaseModel.Session, path, new string[] { "output" });

            // Set initial thresholds for output decisions
            model.OutputsThresholds = new OutputThresholdItem[outputDim];
            for (int i = 0; i < outputDim; i++)
                model.OutputsThresholds[i] = new OutputThresholdItem();

            // Get the parameters
            return model;
        }
        public static Session createTFNETNeuralNetModelSession(int inputDim, int outputDim)
        {
            // Disable eager mode to enable storing new nodes to the default graph automatically
            tf.compat.v1.disable_eager_execution();

            // The model operations and variables are organized in a graph
            // The graph is automatically built in the default graph
            Graph graph = new Graph().as_default();
            // Create the list of variables (wieghts, and biases) assignments
            List<Operation>  assignmentsList = new List<Operation>();
            // Define the input and output variables
            Tensor input = tf.placeholder(tf.float32, (-1, inputDim), name: "input_place_holder");
            Tensor outputPlaceholder = tf.placeholder(tf.float32, (-1, outputDim), name: "output_place_holder");
            // Start from the first hidden layer, since the input is not actually a layer   
            // but inform the shape of the input, with "input" elements.
            int hidLayerDim = (int)((float)inputDim * (2f / 3f) + outputDim);
            // Define the model function
            Tensor hiddenLayer1 = tf.nn.tanh(TF_NET_NN.Layer(input, hidLayerDim, assignmentsList)); // The input layer connected to the 1st hidden layer. The 1st layer has "Tanh" as activation function
            Tensor hiddenLayer2 = TF_NET_NN.Layer(hiddenLayer1, hidLayerDim, assignmentsList); // The 1st hidden layer connected to the 2nd hidden layer. The 2nd layer has "linear" as activation function (default)
            Tensor output = TF_NET_NN.HardSigmoid(TF_NET_NN.Layer(hiddenLayer2, outputDim, assignmentsList), "output"); // The 2nd hidden layer connected to the output layer. The output layer has "hard sigmoid" as activation function
            Session session = new Session(graph);
            // Run the assignments of the variables
            session.run(assignmentsList.ToArray());
            // Insert the cost function operation to the graph of the model
            Tensor costFunc = tf.reduce_mean(tf.square(tf.sub(output, outputPlaceholder)), name: "cost_function");
            // Insert the optimizer operation to the graph of the model with a default learning rate of 0.01
            Tensor learning_rate = tf.placeholder(tf.float32, Shape.Scalar, name: "learning_rate");
            Operation optimizer = tf.train.GradientDescentOptimizer(learning_rate).minimize(costFunc, name: "optimizer");

            return session;
        }

        public void initializeRLModelForCWD(CWDReinforcementL cwdReinforcementL)
        {
            TFNETReinforcementL model = (TFNETReinforcementL)cwdReinforcementL.CWDReinforcementLModel.Clone();
            model.BaseModel.Session = TF_NET_NN.LoadModelVariables(model.BaseModel.ModelPath, model.BaseModel._inputDim, model.BaseModel._outputDim, createTFNETNeuralNetModelSession);
            cwdReinforcementL.CWDReinforcementLModel = model;

            TFNETReinforcementL crazyModel = (TFNETReinforcementL)cwdReinforcementL.CWDCrazyReinforcementLModel.Clone();
            crazyModel.BaseModel.Session = TF_NET_NN.LoadModelVariables(crazyModel.BaseModel.ModelPath, crazyModel.BaseModel._inputDim, crazyModel.BaseModel._outputDim, createTFNETNeuralNetModelSession);
            cwdReinforcementL.CWDCrazyReinforcementLModel = crazyModel;

            _objectivesModelsDic.Add(cwdReinforcementL.ModelName + cwdReinforcementL.ObjectiveName, cwdReinforcementL);
        }
    }
}
