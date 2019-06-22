using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace rf.naive
{
    public sealed class DecisionTree
    {
        public class DecisionTreeNode
        {
            public int FeatureIndex { get; set; }
            public double Value { get; set; }
            public int TrueBranch { get; set; }
            public int FalseBranch { get; set; }
        }

        const int LeafIndex = -1;

        private readonly DecisionTreeNode[] nodes;

        public DecisionTree(DecisionTreeNode[] tree)
        {
            nodes = tree;
        }

        public double Evaluate(double[] features)
        {
            var node = nodes[0];
            while(node.FeatureIndex != LeafIndex)
            {
                int nodeIndex = features[node.FeatureIndex] < node.Value ? node.TrueBranch : node.FalseBranch;
                node = nodes[nodeIndex];
            }
            return node.Value;
        }
    }

    public sealed class RandomForest
    {
        private readonly DecisionTree[] trees;

        public RandomForest(DecisionTree[] trees)
        {
            this.trees = trees;
        }

        public double EvaluateProbability(double[] instance)
        {
            var sum = 0d;
            for(int i = 0; i < trees.Length; i++)
            {
                sum += trees[i].Evaluate(instance);
            }
            var result = Logit(sum);
            return result;
        }

        // this converts the output to a probability
        private double Logit(double value)
        {
            return 1f / (1f + Math.Exp(-value));
        }
    }

    public static class ModelBuilder
    {
        private static readonly Regex treeSplit = new Regex(@"^booster\[\d+\]\r?\n", RegexOptions.Compiled | RegexOptions.Multiline);
        public static RandomForest CreateXGBoost(string allTrees)
        {
            var treeStrings = treeSplit.Split(allTrees);
            var trees = treeStrings
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Select(CreateDecisionTree)
                .ToArray();
            var model = new RandomForest(trees);
            return model;
        }

        private static DecisionTree CreateDecisionTree(string definition)
        {
            var lines = definition.Split('\n');
            var nodes = lines
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Select(ParseLine)
                .OrderBy(m => m.index)
                .Select(m => m.node)
                .ToArray();
            var dt = new DecisionTree(nodes);
            return dt;
        }
        private static (int index, DecisionTree.DecisionTreeNode node) ParseLine(string line)
        {
            var parts = line.Split(':');
            var index = int.Parse(parts[0]);
            var nodeInfo = parts[1];
            var node = nodeInfo.StartsWith("leaf=") ? ParseLeaf(nodeInfo) : ParseDecision(nodeInfo);
            return (index, node);
        }
        // leaf example: "leaf=-0.199992761,cover=27584.75"
        private static readonly Regex leafParser = new Regex(@"^leaf=([^,]+),cover=(.*)$", RegexOptions.Compiled);
        public const int LeafIndex = -1;
        private static DecisionTree.DecisionTreeNode ParseLeaf(string nodeInfo)
        {
            var parts = leafParser.Match(nodeInfo);
            return new DecisionTree.DecisionTreeNode
            { 
                FeatureIndex = LeafIndex, 
                Value = float.Parse(parts.Groups[1].Value),
            };
        }
        // decision example: "[f0<0.99992311] yes=1,no=2,missing=1,gain=97812.25,cover=218986"
        private static readonly Regex decisionParser = 
            new Regex(@"^\[f(\d+)\<([^\]]+)\] yes=(\d+),no=(\d+),missing=\d+,gain=[^,]+,cover=(.*)$", RegexOptions.Compiled);
        private static DecisionTree.DecisionTreeNode ParseDecision(string nodeInfo)
        {
            var parts = decisionParser.Match(nodeInfo);
            return new DecisionTree.DecisionTreeNode
            {
                FeatureIndex = short.Parse(parts.Groups[1].Value),
                Value = float.Parse(parts.Groups[2].Value),
                TrueBranch = byte.Parse(parts.Groups[3].Value),
                FalseBranch = byte.Parse(parts.Groups[4].Value),
            };
        }
    }
}