using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Biological_Signal_Processing_Using_AI.DetailsModify.Annotations
{
    public class AnnotationsStructures
    {
        public enum AnnotationType
        {
            Point,
            Interval
        }

        [DataContract(IsReference = true)]
        public class AnnotationData
        {
            [DataMember]
            public string Name { get; set; }

            [DataMember]
            private List<AnnotationECG> Annotations = new List<AnnotationECG>();

            public AnnotationData(string name)
            {
                Name = name;
            }

            public AnnotationECG InsertAnnotation(string name, AnnotationType type, int startingIndex, int endingIndex)
            {
                return new AnnotationECG(name, type, startingIndex, endingIndex, this);
            }

            public void InsertAnnotation(AnnotationECG anno)
            {
                // Annotations should be sorted in an ascending way
                // Get the index where to insert the new annotation
                int newAnnoIndex = Annotations.Where(annotation => annotation.GetIndexes().starting < anno.GetIndexes().starting).ToArray().Length;
                Annotations.Insert(newAnnoIndex, anno);
            }

            public void RemoveAnnotation(AnnotationECG anno)
            {
                Annotations.Remove(anno);
            }

            public List<AnnotationECG> GetAnnotations()
            {
                return Annotations;
            }

            public void Clear()
            {
                Annotations.Clear();
            }
        }

        [DataContract(IsReference = true)]
        public class AnnotationECG
        {
            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public AnnotationData ParentAnnoData { get; set; }

            [DataMember]
            private AnnotationType Type { get; set; }

            [DataMember]
            private int _startingIndex { get; set; }
            [DataMember]
            private int _endingIndex { get; set; }

            public AnnotationECG(string name, AnnotationType type, int startingIndex, int endingIndex, AnnotationData parentAnnoData)
            {
                Name = name;
                Type = type;
                _startingIndex = startingIndex;
                _endingIndex = endingIndex;
                ParentAnnoData = parentAnnoData;

                ParentAnnoData.InsertAnnotation(this);
            }

            public void SetNewVals(string name, int starting, int ending)
            {
                Name = name;
                if (starting != int.MaxValue)
                    _startingIndex = starting;
                if (ending != int.MaxValue)
                    _endingIndex = ending;
            }

            public (int starting, int ending) GetIndexes()
            {
                return (_startingIndex, _endingIndex);
            }

            public AnnotationType GetAnnotationType()
            {
                return Type;
            }

            public void Remove()
            {
                ParentAnnoData.RemoveAnnotation(this);
            }
        }
    }
}
