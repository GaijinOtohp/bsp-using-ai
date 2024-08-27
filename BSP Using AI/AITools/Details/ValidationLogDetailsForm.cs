using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;

namespace BSP_Using_AI.AITools.Details
{
    public partial class DetailsForm
    {
        private static class EvaluationTechnique
        {
            public static string Raw = "Raw";
            public static string AccSeSp = "Acc, Se, Sp/Rec";
            public static string AccPpvNpv = "Acc, PPV/Pre, NPV";
            public static string AccF1Score = "Acc, F1-Score";
        }

        private void refreshValidationData()
        {
            // Insert new vallidation data in validationFlowLayoutPanel
            this.Invoke(new MethodInvoker(delegate () {
                validationFlowLayoutPanel.Controls.Clear();
                validationTitlesPanel.Controls.Clear();
                overallMetricsPanel.Controls.Clear();
            }));

            // Compute the raw values of the evaluation accumulated for each model
            double regrModelsCount = _InnerObjectiveModels.Values.Where(baseModel => baseModel.Type == ObjectiveType.Regression).Count();

            Dictionary<CustomArchiBaseModel, (int accumTP, int accumTN, int accumFP, int accumFN, double maseAverage)> modelsEvalDict = new Dictionary<CustomArchiBaseModel, (int accumTP, int accumTN, int accumFP, int accumFN, double maseAverage)>(_InnerObjectiveModels.Count);
            double overallMASE = 0;
            foreach (CustomArchiBaseModel innerObjectiveModel in _InnerObjectiveModels.Values)
            {
                int accumTP = 0, accumTN = 0, accumFP = 0, accumFN = 0;
                double maseAverage = 0;
                if (innerObjectiveModel.Type == ObjectiveType.Regression)
                {
                    foreach (OutputMetrics outputMet in innerObjectiveModel.ValidationData._ModelOutputsValidMetrics)
                        maseAverage += outputMet._mase / innerObjectiveModel.ValidationData._ModelOutputsValidMetrics.Length;

                    overallMASE += maseAverage / regrModelsCount;
                }
                else
                {
                    foreach (OutputMetrics outputMet in innerObjectiveModel.ValidationData._ModelOutputsValidMetrics)
                    {
                        accumTP += outputMet._truePositive;
                        accumTN += outputMet._trueNegative;
                        accumFP += outputMet._falsePositive;
                        accumFN += outputMet._falseNegative;
                    }
                }

                modelsEvalDict.Add(innerObjectiveModel, (accumTP, accumTN, accumFP, accumFN, maseAverage));
            }

            // Check which evaluation metrics is selected
            string selectedEvaluation = "";
            this.Invoke(new MethodInvoker(delegate () { selectedEvaluation = (string)metricsComboBox.SelectedItem; }));
            if (selectedEvaluation.Equals(EvaluationTechnique.Raw))
                DisplayRawEvaluation(modelsEvalDict, overallMASE);
            else if (selectedEvaluation.Equals(EvaluationTechnique.AccSeSp))
                DisplayAccSeSpEvaluation(modelsEvalDict, overallMASE);
            else if (selectedEvaluation.Equals(EvaluationTechnique.AccPpvNpv))
                DisplayAccPpvNpvEvaluation(modelsEvalDict, overallMASE);
            else if (selectedEvaluation.Equals(EvaluationTechnique.AccF1Score))
                DisplayAccF1ScoreEvaluation(modelsEvalDict, overallMASE);
        }

        private void DisplayRawEvaluation(Dictionary<CustomArchiBaseModel, (int accumTP, int accumTN, int accumFP, int accumFN, double maseAverage)> modelsEvalDict, double overallMASE)
        {
            // Insert titles
            this.Invoke(new MethodInvoker(delegate () { validationTitlesPanel.Controls.Add(new ValidationRawMetricsTitles()); }));

            double overallTP = 0, overAllTN = 0, overallFP = 0, overallFN = 0;
            foreach(CustomArchiBaseModel innerObjectiveModel in modelsEvalDict.Keys)
            {
                ValidationRawMetrics validationRawMetricsUserControl = new ValidationRawMetrics(_objectivesModelsDic, _ObjectiveModel, innerObjectiveModel);
                ValidationData validationData = innerObjectiveModel.ValidationData;
                string TP = "/", TN = "/", FP = "/", FN = "/", MASE = "/";

                (int accumTP, int accumTN, int accumFP, int accumFN, double maseAverage) = modelsEvalDict[innerObjectiveModel];

                if (innerObjectiveModel.Type == ObjectiveType.Regression)
                {
                    MASE = Math.Round(maseAverage, 4).ToString();
                }
                else
                {
                    TP = accumTP.ToString();
                    TN = accumTN.ToString();
                    FP = accumFP.ToString();
                    FN = accumFN.ToString();

                    overallTP += accumTP;
                    overAllTN += accumTN;
                    overallFP += accumFP;
                    overallFN += accumFN;
                }

                validationRawMetricsUserControl.Name = innerObjectiveModel.Name;
                validationRawMetricsUserControl.modelTargetLabel.Text = innerObjectiveModel.Name;
                validationRawMetricsUserControl.algorithmTypeLabel.Text = innerObjectiveModel.Type.ToString();
                validationRawMetricsUserControl.datasetSizeLabel.Text = validationData._datasetSize.ToString();
                validationRawMetricsUserControl.trainingDatasetLabel.Text = Math.Round(validationData._trainingDatasetSize, 2).ToString();
                validationRawMetricsUserControl.validationDatasetLabel.Text = Math.Round(validationData._validationDatasetSize, 2).ToString();

                validationRawMetricsUserControl.truePositiveLabel.Text = TP;
                validationRawMetricsUserControl.trueNegativeLabel.Text = TN;
                validationRawMetricsUserControl.falsePositiveLabel.Text = FP;
                validationRawMetricsUserControl.falseNegativeLabel.Text = FN;
                validationRawMetricsUserControl.maseLabel.Text = MASE;

                this.Invoke(new MethodInvoker(delegate () { validationFlowLayoutPanel.Controls.Add(validationRawMetricsUserControl); }));
            }

            // Insert overall accuracy, sensitivity, specificity, and mase in their controls
            ValidationOverallRawMetrics validationOverallRawMetricsUserControl = new ValidationOverallRawMetrics();
            validationOverallRawMetricsUserControl.overallTPLabel.Text = "Overall true positives: " + overallTP.ToString();
            validationOverallRawMetricsUserControl.overallTNLabel.Text = "Overall true negatives: " + overAllTN.ToString();
            validationOverallRawMetricsUserControl.overallFPLabel.Text = "Overall false positives: " + overallFP.ToString();
            validationOverallRawMetricsUserControl.overallFNLabel.Text = "Overall false negatives: " + overallFN.ToString();
            validationOverallRawMetricsUserControl.overallMASELabel.Text = "Overall MASE: " + Math.Round(overallMASE, 2).ToString();
            this.Invoke(new MethodInvoker(delegate () { overallMetricsPanel.Controls.Add(validationOverallRawMetricsUserControl); }));
        }

        private void DisplayAccSeSpEvaluation(Dictionary<CustomArchiBaseModel, (int accumTP, int accumTN, int accumFP, int accumFN, double maseAverage)> modelsEvalDict, double overallMASE)
        {
            // Insert titles
            this.Invoke(new MethodInvoker(delegate () { validationTitlesPanel.Controls.Add(new ValidationAccSeSpTitles()); }));

            double overallAcc = 0, overAllSe = 0, overallSp = 0;
            foreach (CustomArchiBaseModel innerObjectiveModel in modelsEvalDict.Keys)
            {
                ValidationAccSeSp validationAccSeSpUserControl = new ValidationAccSeSp(_objectivesModelsDic, _ObjectiveModel, innerObjectiveModel);
                ValidationData validationData = innerObjectiveModel.ValidationData;
                string AccuracyText = "/", SensitivityText = "/", SpecificityText = "/", MASEText = "/";

                (int accumTP, int accumTN, int accumFP, int accumFN, double maseAverage) = modelsEvalDict[innerObjectiveModel];

                if (innerObjectiveModel.Type == ObjectiveType.Regression)
                {
                    MASEText = Math.Round(maseAverage, 4).ToString();
                }
                else
                {
                    double accuracy = (double)(accumTP + accumTN) / (accumTP + accumTN + accumFP + accumFN);
                    double sensitivity = (double)accumTP / (accumTP + accumFN);
                    double specificity = (double)accumTN / (accumTN + accumFP);

                    AccuracyText = Math.Round(accuracy, 4).ToString();
                    SensitivityText = Math.Round(sensitivity, 4).ToString();
                    SpecificityText = Math.Round(specificity, 4).ToString();

                    overallAcc += accuracy;
                    overAllSe += sensitivity;
                    overallSp += specificity;
                }

                validationAccSeSpUserControl.Name = innerObjectiveModel.Name;
                validationAccSeSpUserControl.modelTargetLabel.Text = innerObjectiveModel.Name;
                validationAccSeSpUserControl.algorithmTypeLabel.Text = innerObjectiveModel.Type.ToString();
                validationAccSeSpUserControl.datasetSizeLabel.Text = validationData._datasetSize.ToString();
                validationAccSeSpUserControl.trainingDatasetLabel.Text = Math.Round(validationData._trainingDatasetSize, 2).ToString();
                validationAccSeSpUserControl.validationDatasetLabel.Text = Math.Round(validationData._validationDatasetSize, 2).ToString();

                validationAccSeSpUserControl.accuracyLabel.Text = AccuracyText;
                validationAccSeSpUserControl.sensitivityLabel.Text = SensitivityText;
                validationAccSeSpUserControl.specificityLabel.Text = SpecificityText;
                validationAccSeSpUserControl.maseLabel.Text = MASEText;

                this.Invoke(new MethodInvoker(delegate () { validationFlowLayoutPanel.Controls.Add(validationAccSeSpUserControl); }));
            }

            // Insert overall accuracy, sensitivity, specificity, and mase in their controls
            ValidationOverallAccSeSpMetrics validationOverallAccSeSpMetricsUserControl = new ValidationOverallAccSeSpMetrics();
            validationOverallAccSeSpMetricsUserControl.overallAccuracyLabel.Text = "Overall accuracy: " + Math.Round(overallAcc, 2).ToString();
            validationOverallAccSeSpMetricsUserControl.overallSensitivityLabel.Text = "Overall sensitivity: " + Math.Round(overAllSe, 2).ToString();
            validationOverallAccSeSpMetricsUserControl.overallSpecificityLabel.Text = "Overall specificity (recall): " + Math.Round(overallSp, 2).ToString();
            validationOverallAccSeSpMetricsUserControl.overallMASELabel.Text = "Overall MASE: " + Math.Round(overallMASE, 2).ToString();
            this.Invoke(new MethodInvoker(delegate () { overallMetricsPanel.Controls.Add(validationOverallAccSeSpMetricsUserControl); }));
        }

        private void DisplayAccPpvNpvEvaluation(Dictionary<CustomArchiBaseModel, (int accumTP, int accumTN, int accumFP, int accumFN, double maseAverage)> modelsEvalDict, double overallMASE)
        {
            // Insert titles
            this.Invoke(new MethodInvoker(delegate () { validationTitlesPanel.Controls.Add(new ValidationAccPpvNpvTitles()); }));

            double overallAcc = 0, overAllPPV = 0, overallNPV = 0;
            foreach (CustomArchiBaseModel innerObjectiveModel in modelsEvalDict.Keys)
            {
                ValidationAccPpvNpv validationAccSeSpUserControl = new ValidationAccPpvNpv(_objectivesModelsDic, _ObjectiveModel, innerObjectiveModel);
                ValidationData validationData = innerObjectiveModel.ValidationData;
                string AccuracyText = "/", PPVText = "/", NPVText = "/", MASEText = "/";

                (int accumTP, int accumTN, int accumFP, int accumFN, double maseAverage) = modelsEvalDict[innerObjectiveModel];

                if (innerObjectiveModel.Type == ObjectiveType.Regression)
                {
                    MASEText = Math.Round(maseAverage, 4).ToString();
                }
                else
                {
                    double accuracy = (double)(accumTP + accumTN) / (accumTP + accumTN + accumFP + accumFN);
                    double ppv = (double)accumTP / (accumTP + accumFP);
                    double npv = (double)accumTN / (accumTN + accumFN);

                    AccuracyText = Math.Round(accuracy, 4).ToString();
                    PPVText = Math.Round(ppv, 4).ToString();
                    NPVText = Math.Round(npv, 4).ToString();

                    overallAcc += accuracy;
                    overAllPPV += ppv;
                    overallNPV += npv;
                }

                validationAccSeSpUserControl.Name = innerObjectiveModel.Name;
                validationAccSeSpUserControl.modelTargetLabel.Text = innerObjectiveModel.Name;
                validationAccSeSpUserControl.algorithmTypeLabel.Text = innerObjectiveModel.Type.ToString();
                validationAccSeSpUserControl.datasetSizeLabel.Text = validationData._datasetSize.ToString();
                validationAccSeSpUserControl.trainingDatasetLabel.Text = Math.Round(validationData._trainingDatasetSize, 2).ToString();
                validationAccSeSpUserControl.validationDatasetLabel.Text = Math.Round(validationData._validationDatasetSize, 2).ToString();

                validationAccSeSpUserControl.accuracyLabel.Text = AccuracyText;
                validationAccSeSpUserControl.ppvLabel.Text = PPVText;
                validationAccSeSpUserControl.npvLabel.Text = NPVText;
                validationAccSeSpUserControl.maseLabel.Text = MASEText;

                this.Invoke(new MethodInvoker(delegate () { validationFlowLayoutPanel.Controls.Add(validationAccSeSpUserControl); }));
            }

            // Insert overall accuracy, sensitivity, specificity, and mase in their controls
            ValidationOverallAccPpvNpvMetrics validationOverallAccPpvNpvMetricsUserControl = new ValidationOverallAccPpvNpvMetrics();
            validationOverallAccPpvNpvMetricsUserControl.overallAccuracyLabel.Text = "Overall accuracy: " + Math.Round(overallAcc, 2).ToString();
            validationOverallAccPpvNpvMetricsUserControl.overallPPVLabel.Text = "Overall positive predictive value (precision): " + Math.Round(overAllPPV, 2).ToString();
            validationOverallAccPpvNpvMetricsUserControl.overallNPVLabel.Text = "Overall negative predictive value: " + Math.Round(overallNPV, 2).ToString();
            validationOverallAccPpvNpvMetricsUserControl.overallMASELabel.Text = "Overall MASE: " + Math.Round(overallMASE, 2).ToString();
            this.Invoke(new MethodInvoker(delegate () { overallMetricsPanel.Controls.Add(validationOverallAccPpvNpvMetricsUserControl); }));
        }

        private void DisplayAccF1ScoreEvaluation(Dictionary<CustomArchiBaseModel, (int accumTP, int accumTN, int accumFP, int accumFN, double maseAverage)> modelsEvalDict, double overallMASE)
        {
            // Insert titles
            this.Invoke(new MethodInvoker(delegate () { validationTitlesPanel.Controls.Add(new ValidationAccF1ScoreTitles()); }));

            double overallAcc = 0, overAllF1Score = 0;
            foreach (CustomArchiBaseModel innerObjectiveModel in modelsEvalDict.Keys)
            {
                ValidationAccF1Score validationAccSeSpUserControl = new ValidationAccF1Score(_objectivesModelsDic, _ObjectiveModel, innerObjectiveModel);
                ValidationData validationData = innerObjectiveModel.ValidationData;
                string AccuracyText = "/", F1ScoreText = "/", MASEText = "/";

                (int accumTP, int accumTN, int accumFP, int accumFN, double maseAverage) = modelsEvalDict[innerObjectiveModel];

                if (innerObjectiveModel.Type == ObjectiveType.Regression)
                {
                    MASEText = Math.Round(maseAverage, 4).ToString();
                }
                else
                {
                    double accuracy = (double)(accumTP + accumTN) / (accumTP + accumTN + accumFP + accumFN);

                    double precision = (double)accumTP / (accumTP + accumFP);
                    double recall = (double)accumTN / (accumTN + accumFP);
                    double f1Score = 2 * (precision * recall) / (precision + recall);

                    AccuracyText = Math.Round(accuracy, 4).ToString();
                    F1ScoreText = Math.Round(f1Score, 4).ToString();

                    overallAcc += accuracy;
                    overAllF1Score += f1Score;
                }

                validationAccSeSpUserControl.Name = innerObjectiveModel.Name;
                validationAccSeSpUserControl.modelTargetLabel.Text = innerObjectiveModel.Name;
                validationAccSeSpUserControl.algorithmTypeLabel.Text = innerObjectiveModel.Type.ToString();
                validationAccSeSpUserControl.datasetSizeLabel.Text = validationData._datasetSize.ToString();
                validationAccSeSpUserControl.trainingDatasetLabel.Text = Math.Round(validationData._trainingDatasetSize, 2).ToString();
                validationAccSeSpUserControl.validationDatasetLabel.Text = Math.Round(validationData._validationDatasetSize, 2).ToString();

                validationAccSeSpUserControl.accuracyLabel.Text = AccuracyText;
                validationAccSeSpUserControl.f1ScoreLabel.Text = F1ScoreText;
                validationAccSeSpUserControl.maseLabel.Text = MASEText;

                this.Invoke(new MethodInvoker(delegate () { validationFlowLayoutPanel.Controls.Add(validationAccSeSpUserControl); }));
            }

            // Insert overall accuracy, sensitivity, specificity, and mase in their controls
            ValidationOverallAccF1ScoreMetrics validationOverallAccF1ScoreMetricsUserControl = new ValidationOverallAccF1ScoreMetrics();
            validationOverallAccF1ScoreMetricsUserControl.overallAccuracyLabel.Text = "Overall accuracy: " + Math.Round(overallAcc, 2).ToString();
            validationOverallAccF1ScoreMetricsUserControl.overallF1ScoreLabel.Text = "Overall F1-Score: " + Math.Round(overAllF1Score, 2).ToString();
            validationOverallAccF1ScoreMetricsUserControl.overallMASELabel.Text = "Overall MASE: " + Math.Round(overallMASE, 2).ToString();
            this.Invoke(new MethodInvoker(delegate () { overallMetricsPanel.Controls.Add(validationOverallAccF1ScoreMetricsUserControl); }));
        }
    }
}
