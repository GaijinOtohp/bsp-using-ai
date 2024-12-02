using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biological_Signal_Processing_Using_AI.WFDB.Annotations
{
    public class AnnotationsDefinitions
    {
        //::::::::::::::::::::::::::::::More information source:::::::::::::::::::::::::::::://
        //___________________________________________________________________________________//
        // For more information visite the following websites:
        // https://physionet.org/physiotools/wag/annot-5.htm
        // https://physionet.org/physiotools/wpg/wpg_36.htm#Annotation-Codes
        // https://www.physionet.org/content/wfdb/10.6.2/lib/ecgcodes.h

        public class Annotation
        {
            public int index;

            public int codeValue;
            public string CodeInShort;
            public string CodeSymbol;

            public int number;          // P-edge: [0], QRS-edge: [1], T-edge: [2], U-edge: [3], Double T-peak: [4]
                                        // ecgpuwave uses "number" also to classify the T waves as normal (0), inverted (1), only upwards (2), only downwards (3), biphasic negative-positive (4), or biphasic positive-negative (5)
            public int channel;         // Defines the signal which the annotation refers to
            public int subType;         // The marked class/category of each annotation (annotation in a noisy segment on the signal)

            public Annotation Clone()
            {
                Annotation annotationClone = new Annotation();

                annotationClone.index = index;
                annotationClone.codeValue = codeValue;
                annotationClone.CodeInShort = CodeInShort;
                annotationClone.CodeSymbol = CodeSymbol;
                annotationClone.number = number;
                annotationClone.channel = channel;
                annotationClone.subType = subType;

                return annotationClone;
            }

            public Annotation() { }
        }

        public static class AnnotationCodes
        {
            // Beat AnnotationCodes
            public class Beat
            {
                public static int NORMAL = 1;       /* normal beat */
                public static int LBBB = 2;         /* left bundle branch block beat */
                public static int RBBB = 3;         /* right bundle branch block beat */
                public static int ABERR = 4;        /* aberrated atrial premature beat */
                public static int PVC = 5;          /* premature ventricular contraction */
                public static int FUSION = 6;       /* fusion of ventricular and normal beat */
                public static int NPC = 7;          /* nodal (junctional) premature beat */
                public static int APC = 8;          /* atrial premature contraction */
                public static int SVPB = 9;         /* premature or ectopic supraventricular beat */
                public static int VESC = 10;        /* ventricular escape beat */
                public static int NESC = 11;        /* nodal (junctional) escape beat */
                public static int PACE = 12;        /* paced beat */
                public static int UNKNOWN = 13;     /* unclassifiable beat */
                public static int BBB = 25;         /* left or right bundle branch block */
                public static int LEARN = 30;       /* beat not classified during learning */
                public static int AESC = 34;        /* atrial escape beat */
                public static int SVESC = 35;       /* supraventricular escape beat */
                public static int PFUS = 38;        /* fusion of paced and normal beat */
                public static int RONT = 41;        /* R-on-T premature ventricular contraction */

                public static int[] GetBeatCodes()
                {
                    return typeof(Beat).GetFields().Select(field => (int)field.GetValue(null)).ToArray();
                }
            }
            // Non-beat AnnotationCodes
            public class NonBeat
            {
                public static int NOTQRS = 0;       /* not-QRS (not a getann/putann code) */
                public static int ABERR = 14;       /* signal quality change */
                public static int ARFCT = 16;       /* isolated QRS-like artifact */
                public static int STCH = 18;        /* ST change */
                public static int TCH = 19;         /* T-wave change */
                public static int SYSTOLE = 20;     /* systole */
                public static int DIASTOLE = 21;    /* diastole */
                public static int NOTE = 22;        /* comment annotation */
                public static int MEASURE = 23;     /* measurement annotation */
                public static int PWAVE = 24;       /* P-wave peak */
                public static int PACESP = 26;      /* non-conducted pacer spike */
                public static int TWAVE = 27;       /* T-wave peak */
                public static int RHYTHM = 28;      /* rhythm change */
                public static int UWAVE = 29;       /* U-wave peak */
                public static int FLWAV = 31;       /* ventricular flutter wave */
                public static int VFON = 32;        /* start of ventricular flutter/fibrillation */
                public static int VFOFF = 33;       /* end of ventricular flutter/fibrillation */
                public static int LINK = 36;        /* link to external data (aux contains URL) */
                public static int NAPC = 37;        /* non-conducted P-wave (blocked APB) */
                public static int WFON = 39;        /* waveform onset */
                public static int PQ = -39;         /* NonBeat.PQ junction (beginning of QRS) */ // Obsolete (replaced by "NonBeat.WFON")
                public static int WFOFF = 40;       /* waveform end */
                public static int JPT = -40;        /* J point (end of QRS) */           // Obsolete (replaced by "NonBeat.WFOFF")

                public static int[] GetNonBeatCodes()
                {
                    return typeof(NonBeat).GetFields().Select(field => (int)field.GetValue(null)).ToArray();
                }
            }
            // Control codes
            public class Control
            {

                public static int ACMAX = 49;       /* value of largest valid annot code (must be < 50) */
                public static int SKIP = 59;        /* long null annotation */
                public static int NUM = 60;         /* change 'num' field */
                public static int SUB = 61;         /* subtype */
                public static int CHN = 62;         /* change 'chan' field */
                public static int AUX = 63;         /* auxiliary information */

                public static int[] GetControlCodes()
                {
                    return typeof(Control).GetFields().Select(field => (int)field.GetValue(null)).ToArray();
                }
            }

            public static Dictionary<int, Annotation> AnnotationCodesDict = new Dictionary<int, Annotation>()
            {
                { Beat.NORMAL, new Annotation() { codeValue = Beat.NORMAL, CodeInShort = "NORMAL", CodeSymbol = "N" } },                    /* normal beat */
                { Beat.LBBB, new Annotation() { codeValue = Beat.LBBB, CodeInShort = "LBBB", CodeSymbol = "L" } },                          /* left bundle branch block beat */
                { Beat.RBBB, new Annotation() { codeValue = Beat.RBBB, CodeInShort = "RBBB", CodeSymbol = "R" } },                          /* right bundle branch block beat */
                { Beat.ABERR, new Annotation() { codeValue = Beat.ABERR, CodeInShort = "ABERR", CodeSymbol = "a" } },                       /* aberrated atrial premature beat */
                { Beat.PVC, new Annotation() { codeValue = Beat.PVC, CodeInShort = "PVC", CodeSymbol = "V" } },                             /* premature ventricular contraction */
                { Beat.FUSION, new Annotation() { codeValue = Beat.FUSION, CodeInShort = "FUSION", CodeSymbol = "F" } },                    /* fusion of ventricular and normal beat */
                { Beat.NPC, new Annotation() { codeValue = Beat.NPC, CodeInShort = "NPC", CodeSymbol = "J" } },                             /* nodal (junctional) premature beat */
                { Beat.APC, new Annotation() { codeValue = Beat.APC, CodeInShort = "APC", CodeSymbol = "A" } },                             /* atrial premature contraction */
                { Beat.SVPB, new Annotation() { codeValue = Beat.SVPB, CodeInShort = "SVPB", CodeSymbol = "S" } },                          /* premature or ectopic supraventricular beat */
                { Beat.VESC, new Annotation() { codeValue = Beat.VESC, CodeInShort = "VESC", CodeSymbol = "E" } },                          /* ventricular escape beat */
                { Beat.NESC, new Annotation() { codeValue = Beat.NESC, CodeInShort = "NESC", CodeSymbol = "j" } },                          /* nodal (junctional) escape beat */
                { Beat.PACE, new Annotation() { codeValue = Beat.PACE, CodeInShort = "PACE", CodeSymbol = "/" } },                          /* paced beat */
                { Beat.UNKNOWN, new Annotation() { codeValue = Beat.UNKNOWN, CodeInShort = "UNKNOWN", CodeSymbol = "Q" } },                 /* unclassifiable beat */
                { Beat.BBB, new Annotation() { codeValue = Beat.BBB, CodeInShort = "BBB", CodeSymbol = "B" } },                             /* left or right bundle branch block */
                { Beat.LEARN, new Annotation() { codeValue = Beat.LEARN, CodeInShort = "LEARN", CodeSymbol = "?" } },                       /* beat not classified during learning */
                { Beat.AESC, new Annotation() { codeValue = Beat.AESC, CodeInShort = "AESC", CodeSymbol = "e" } },                          /* atrial escape beat */
                { Beat.SVESC, new Annotation() { codeValue = Beat.SVESC, CodeInShort = "SVESC", CodeSymbol = "n" } },                       /* supraventricular escape beat */
                { Beat.PFUS, new Annotation() { codeValue = Beat.PFUS, CodeInShort = "PFUS", CodeSymbol = "f" } },                          /* fusion of paced and normal beat */
                { Beat.RONT, new Annotation() { codeValue = Beat.RONT, CodeInShort = "RONT", CodeSymbol = "r" } },                          /* R-on-T premature ventricular contraction */

                { NonBeat.NOTQRS, new Annotation() { codeValue = NonBeat.NOTQRS, CodeInShort = "NOTQRS", CodeSymbol = "_" } },                    /* not-QRS (not a getann/putann code) */
                { NonBeat.ABERR, new Annotation() { codeValue = NonBeat.ABERR, CodeInShort = "ABERR", CodeSymbol = "~" } },                 /* signal quality change */
                { NonBeat.ARFCT, new Annotation() { codeValue = NonBeat.ARFCT, CodeInShort = "ARFCT", CodeSymbol = "|" } },                 /* isolated QRS-like artifact */
                { NonBeat.STCH, new Annotation() { codeValue = NonBeat.STCH, CodeInShort = "STCH", CodeSymbol = "s" } },                    /* ST change */
                { NonBeat.TCH, new Annotation() { codeValue = NonBeat.TCH, CodeInShort = "TCH", CodeSymbol = "T" } },                       /* T-wave change */
                { NonBeat.SYSTOLE, new Annotation() { codeValue = NonBeat.SYSTOLE, CodeInShort = "SYSTOLE", CodeSymbol = "*" } },           /* systole */
                { NonBeat.DIASTOLE, new Annotation() { codeValue = NonBeat.DIASTOLE, CodeInShort = "DIASTOLE", CodeSymbol = "D" } },        /* diastole */
                { NonBeat.NOTE, new Annotation() { codeValue = NonBeat.NOTE, CodeInShort = "NOTE", CodeSymbol = "\"" } },                   /* comment annotation */
                { NonBeat.MEASURE, new Annotation() { codeValue = NonBeat.MEASURE, CodeInShort = "MEASURE", CodeSymbol = "=" } },           /* measurement annotation */
                { NonBeat.PWAVE, new Annotation() { codeValue = NonBeat.PWAVE, CodeInShort = "PWAVE", CodeSymbol = "p" } },                 /* P-wave peak */
                { NonBeat.PACESP, new Annotation() { codeValue = NonBeat.PACESP, CodeInShort = "PACESP", CodeSymbol = "^" } },              /* non-conducted pacer spike */
                { NonBeat.TWAVE, new Annotation() { codeValue = NonBeat.TWAVE, CodeInShort = "TWAVE", CodeSymbol = "t" } },                 /* T-wave peak */
                { NonBeat.RHYTHM, new Annotation() { codeValue = NonBeat.RHYTHM, CodeInShort = "RHYTHM", CodeSymbol = "+" } },              /* rhythm change */
                { NonBeat.UWAVE, new Annotation() { codeValue = NonBeat.UWAVE, CodeInShort = "UWAVE", CodeSymbol = "u" } },                 /* U-wave peak */
                { NonBeat.FLWAV, new Annotation() { codeValue = NonBeat.FLWAV, CodeInShort = "FLWAV", CodeSymbol = "!" } },                 /* ventricular flutter wave */
                { NonBeat.VFON, new Annotation() { codeValue = NonBeat.VFON, CodeInShort = "VFON", CodeSymbol = "[" } },                    /* start of ventricular flutter/fibrillation */
                { NonBeat.VFOFF, new Annotation() { codeValue = NonBeat.VFOFF, CodeInShort = "VFOFF", CodeSymbol = "]" } },                 /* end of ventricular flutter/fibrillation */
                { NonBeat.LINK, new Annotation() { codeValue = NonBeat.LINK, CodeInShort = "LINK", CodeSymbol = "@" } },                    /* link to external data (aux contains URL) */
                { NonBeat.NAPC, new Annotation() { codeValue = NonBeat.NAPC, CodeInShort = "NAPC", CodeSymbol = "x" } },                    /* non-conducted P-wave (blocked APB) */
                { NonBeat.WFON, new Annotation() { codeValue = NonBeat.WFON, CodeInShort = "WFON", CodeSymbol = "(" } },                    /* waveform onset */
                { NonBeat.PQ, new Annotation() { codeValue = NonBeat.WFON, CodeInShort = "PQ", CodeSymbol = "‘" } },                        /* NonBeat.PQ junction (beginning of QRS) */ // Obsolete (replaced by "NonBeat.WFON")
                { NonBeat.WFOFF, new Annotation() { codeValue = NonBeat.WFOFF, CodeInShort = "WFOFF", CodeSymbol = ")" } },                 /* waveform end */
                { NonBeat.JPT, new Annotation() { codeValue = NonBeat.WFOFF, CodeInShort = "JPT", CodeSymbol = "’" } }                      /* J point (end of QRS) */           // Obsolete (replaced by "NonBeat.WFOFF")
            };
        }
    }
}
