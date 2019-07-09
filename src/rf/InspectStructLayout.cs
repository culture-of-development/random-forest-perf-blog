using System.Runtime.InteropServices;

namespace rf
{
    public struct DecisionTreeNodeBad
    {
        public short FeatureIndex;
        public float Value;
        public byte TrueBranch;
        public byte FalseBranch;
    }

    public struct DecisionTreeNodeBetter
    {
        public float Value;
        public short FeatureIndex;
        public byte TrueBranch;
        public byte FalseBranch;
    }

    [StructLayout(LayoutKind.Auto)]
    public struct DecisionTreeNodeAuto
    {
        public short FeatureIndex;
        public float Value;
        public byte TrueBranch;
        public byte FalseBranch;
    }
}