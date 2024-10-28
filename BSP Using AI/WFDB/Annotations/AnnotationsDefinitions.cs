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

            public int number;          // P-edge: [0], QRS-edge: [1], T-edge: [2], Double T-peak: [4]
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
            public static int NOTQRS = 0;       /* not-QRS (not a getann/putann code) */
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
            // Non-beat AnnotationCodes
            public static int NOISE = 14;       /* signal quality change */
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
            public static int PQ = -39;         /* PQ junction (beginning of QRS) */ // Obsolete (replaced by "WFON")
            public static int WFOFF = 40;       /* waveform end */
            public static int JPT = -40;        /* J point (end of QRS) */           // Obsolete (replaced by "WFOFF")
            // Control codes
            public static int ACMAX = 49;       /* value of largest valid annot code (must be < 50) */
            public static int SKIP = 59;        /* long null annotation */
            public static int NUM = 60;         /* change 'num' field */
            public static int SUB = 61;         /* subtype */
            public static int CHN = 62;         /* change 'chan' field */
            public static int AUX = 63;         /* auxiliary information */

            public static Dictionary<int, Annotation> AnnotationCodesDict = new Dictionary<int, Annotation>()
            {
                { NOTQRS, new Annotation() { codeValue = NOTQRS, CodeInShort = "NOTQRS", CodeSymbol = "_" } },              /* not-QRS (not a getann/putann code) */
                { NORMAL, new Annotation() { codeValue = NORMAL, CodeInShort = "NORMAL", CodeSymbol = "N" } },              /* normal beat */
                { LBBB, new Annotation() { codeValue = LBBB, CodeInShort = "LBBB", CodeSymbol = "L" } },                    /* left bundle branch block beat */
                { RBBB, new Annotation() { codeValue = RBBB, CodeInShort = "RBBB", CodeSymbol = "R" } },                    /* right bundle branch block beat */
                { ABERR, new Annotation() { codeValue = ABERR, CodeInShort = "ABERR", CodeSymbol = "a" } },                 /* aberrated atrial premature beat */
                { PVC, new Annotation() { codeValue = PVC, CodeInShort = "PVC", CodeSymbol = "V" } },                       /* premature ventricular contraction */
                { FUSION, new Annotation() { codeValue = FUSION, CodeInShort = "FUSION", CodeSymbol = "F" } },              /* fusion of ventricular and normal beat */
                { NPC, new Annotation() { codeValue = NPC, CodeInShort = "NPC", CodeSymbol = "J" } },                       /* nodal (junctional) premature beat */
                { APC, new Annotation() { codeValue = APC, CodeInShort = "APC", CodeSymbol = "A" } },                       /* atrial premature contraction */
                { SVPB, new Annotation() { codeValue = SVPB, CodeInShort = "SVPB", CodeSymbol = "S" } },                    /* premature or ectopic supraventricular beat */
                { VESC, new Annotation() { codeValue = VESC, CodeInShort = "VESC", CodeSymbol = "E" } },                    /* ventricular escape beat */
                { NESC, new Annotation() { codeValue = NESC, CodeInShort = "NESC", CodeSymbol = "j" } },                    /* nodal (junctional) escape beat */
                { PACE, new Annotation() { codeValue = PACE, CodeInShort = "PACE", CodeSymbol = "/" } },                    /* paced beat */
                { UNKNOWN, new Annotation() { codeValue = UNKNOWN, CodeInShort = "UNKNOWN", CodeSymbol = "Q" } },           /* unclassifiable beat */
                { BBB, new Annotation() { codeValue = BBB, CodeInShort = "BBB", CodeSymbol = "B" } },                       /* left or right bundle branch block */
                { LEARN, new Annotation() { codeValue = LEARN, CodeInShort = "LEARN", CodeSymbol = "?" } },                 /* beat not classified during learning */
                { AESC, new Annotation() { codeValue = AESC, CodeInShort = "AESC", CodeSymbol = "e" } },                    /* atrial escape beat */
                { SVESC, new Annotation() { codeValue = SVESC, CodeInShort = "SVESC", CodeSymbol = "n" } },                 /* supraventricular escape beat */
                { PFUS, new Annotation() { codeValue = PFUS, CodeInShort = "PFUS", CodeSymbol = "f" } },                    /* fusion of paced and normal beat */
                { RONT, new Annotation() { codeValue = RONT, CodeInShort = "RONT", CodeSymbol = "r" } },                    /* R-on-T premature ventricular contraction */

                { NOISE, new Annotation() { codeValue = NOISE, CodeInShort = "NOISE", CodeSymbol = "~" } },                 /* signal quality change */
                { ARFCT, new Annotation() { codeValue = ARFCT, CodeInShort = "ARFCT", CodeSymbol = "|" } },                 /* isolated QRS-like artifact */
                { STCH, new Annotation() { codeValue = STCH, CodeInShort = "STCH", CodeSymbol = "s" } },                    /* ST change */
                { TCH, new Annotation() { codeValue = TCH, CodeInShort = "TCH", CodeSymbol = "T" } },                       /* T-wave change */
                { SYSTOLE, new Annotation() { codeValue = SYSTOLE, CodeInShort = "SYSTOLE", CodeSymbol = "*" } },           /* systole */
                { DIASTOLE, new Annotation() { codeValue = DIASTOLE, CodeInShort = "DIASTOLE", CodeSymbol = "D" } },        /* diastole */
                { NOTE, new Annotation() { codeValue = NOTE, CodeInShort = "NOTE", CodeSymbol = "\"" } },                   /* comment annotation */
                { MEASURE, new Annotation() { codeValue = MEASURE, CodeInShort = "MEASURE", CodeSymbol = "=" } },           /* measurement annotation */
                { PWAVE, new Annotation() { codeValue = PWAVE, CodeInShort = "PWAVE", CodeSymbol = "p" } },                 /* P-wave peak */
                { PACESP, new Annotation() { codeValue = PACESP, CodeInShort = "PACESP", CodeSymbol = "^" } },              /* non-conducted pacer spike */
                { TWAVE, new Annotation() { codeValue = TWAVE, CodeInShort = "TWAVE", CodeSymbol = "t" } },                 /* T-wave peak */
                { RHYTHM, new Annotation() { codeValue = RHYTHM, CodeInShort = "RHYTHM", CodeSymbol = "+" } },              /* rhythm change */
                { UWAVE, new Annotation() { codeValue = UWAVE, CodeInShort = "UWAVE", CodeSymbol = "u" } },                 /* U-wave peak */
                { FLWAV, new Annotation() { codeValue = FLWAV, CodeInShort = "FLWAV", CodeSymbol = "!" } },                 /* ventricular flutter wave */
                { VFON, new Annotation() { codeValue = VFON, CodeInShort = "VFON", CodeSymbol = "[" } },                    /* start of ventricular flutter/fibrillation */
                { VFOFF, new Annotation() { codeValue = VFOFF, CodeInShort = "VFOFF", CodeSymbol = "]" } },                 /* end of ventricular flutter/fibrillation */
                { LINK, new Annotation() { codeValue = LINK, CodeInShort = "LINK", CodeSymbol = "@" } },                    /* link to external data (aux contains URL) */
                { NAPC, new Annotation() { codeValue = NAPC, CodeInShort = "NAPC", CodeSymbol = "x" } },                    /* non-conducted P-wave (blocked APB) */
                { WFON, new Annotation() { codeValue = WFON, CodeInShort = "WFON", CodeSymbol = "(" } },                    /* waveform onset */
                { PQ, new Annotation() { codeValue = WFON, CodeInShort = "PQ", CodeSymbol = "‘" } },                        /* PQ junction (beginning of QRS) */ // Obsolete (replaced by "WFON")
                { WFOFF, new Annotation() { codeValue = WFOFF, CodeInShort = "WFOFF", CodeSymbol = ")" } },                 /* waveform end */
                { JPT, new Annotation() { codeValue = WFOFF, CodeInShort = "JPT", CodeSymbol = "’" } }                      /* J point (end of QRS) */           // Obsolete (replaced by "WFOFF")
            };
        }
    }
}
