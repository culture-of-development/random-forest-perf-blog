namespace rf
{
    public sealed class DecisionTreeNode_class
    {
        public int FeatureIndex { get; set; }
        public double Value { get; set; }
        public int TrueBranch { get; set; }
        public int FalseBranch { get; set; }
    }

    public struct DecisionTreeNode_struct
    {
        public int FeatureIndex { get; set; }
        public double Value { get; set; }
        public int TrueBranch { get; set; }
        public int FalseBranch { get; set; }
    }

    public static class ArrayOfThings
    {
        public static DecisionTreeNode_class[] arrayOfClasses;
        public static DecisionTreeNode_struct[] arrayOfStructs;

        public static double GetScoreClasses(double[] instance)
        {
            var sum = 0d;
            for(int i = 0; i < arrayOfClasses.Length; i++)
                sum += arrayOfClasses[i].Value;
            return sum;
        }

        public static double GetScoreStruct(double[] instance)
        {
            var sum = 0d;
            for(int i = 0; i < arrayOfStructs.Length; i++)
                sum += arrayOfStructs[i].Value;
            return sum;
        }
    }
}