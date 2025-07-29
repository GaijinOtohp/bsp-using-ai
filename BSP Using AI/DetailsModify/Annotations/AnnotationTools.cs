using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation.CWDNamigs;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;

namespace Biological_Signal_Processing_Using_AI.DetailsModify.Annotations
{
    public class AnnotationTools
    {
        public static AnnotationData FixBordersAnnotation(AnnotationData annotationData)
        {
            AnnotationData fixedAnnotationData = annotationData.Clone();

            List<AnnotationECG> annotations = fixedAnnotationData.GetAnnotations();
            Stack<AnnotationECG> latestOnsets = new Stack<AnnotationECG>();
            Stack<AnnotationECG> latestPeaks = new Stack<AnnotationECG>();

            for (int iAnno = 0; iAnno < annotations.Count; iAnno++)
            {
                AnnotationECG currentAnno = annotations[iAnno];

                if (currentAnno.Name == PeaksLabelsOutputs.POnset)
                    latestOnsets.Push(currentAnno);
                else if (currentAnno.Name == PeaksLabelsOutputs.PPeak)
                {
                    latestPeaks.Push(currentAnno);
                    if (latestOnsets.Count > 0)
                        latestOnsets.Pop().Name = PeaksLabelsOutputs.POnset;
                }
                else if (currentAnno.Name == PeaksLabelsOutputs.RPeak)
                {
                    latestPeaks.Push(currentAnno);
                    if (latestOnsets.Count > 0)
                        latestOnsets.Pop().Name = PeaksLabelsOutputs.QPeak;
                }
                else if (currentAnno.Name == PeaksLabelsOutputs.TPeak)
                {
                    latestPeaks.Push(currentAnno);
                    if (latestOnsets.Count > 0)
                        latestOnsets.Pop().Name = PeaksLabelsOutputs.TOnset;
                }
                else
                {
                    if (latestPeaks.TryPop(out AnnotationECG latestPeak))
                        if (latestPeak.Name == PeaksLabelsOutputs.PPeak)
                            currentAnno.Name = PeaksLabelsOutputs.PEnd;
                        else if (latestPeak.Name == PeaksLabelsOutputs.RPeak)
                            currentAnno.Name = PeaksLabelsOutputs.SPeak;
                        else if (latestPeak.Name == PeaksLabelsOutputs.TPeak)
                            currentAnno.Name = PeaksLabelsOutputs.TEnd;
                }
            }

            return fixedAnnotationData;
        }
    }
}
