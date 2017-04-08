using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpGraphEditor.Graph.Core.Elements;
using SharpGraphEditor.Graph.Core.Exceptions;
using SharpGraphEditor.Graph.Core.Extentions;

namespace SharpGraphEditor.Graph.Core.FormatStorage
{
    public class HierarchicalRtfFormatStorage : BaseFormatStorage
    {
        public override void Open(TextReader stream, IGraph graph)
        {
            throw new NotImplementedException("this format supports only saving");
        }

        public override void Save(TextWriter stream, IGraph graph)
        {
            if (!graph.IsDirected)
            {
                throw new InvalidGraphFormatException("graph must be directed");
            }

            var hasVertexParent = new bool[graph.Vertices.Count()];
            foreach (var edge in graph.Edges)
            {
                hasVertexParent[edge.Target.Index - 1] = true;
            }

            var verticesWithoutParent = hasVertexParent.AllIndexesOf(x => !x);
            if (verticesWithoutParent.Count() == 0)
            {
                throw new InvalidGraphFormatException("graph must have some vertices without parent");
            }

            IVertex mainVertex = graph.Vertices.ElementAt(Array.IndexOf(hasVertexParent, false));

            var tree = new List<TreeNode>();
            var dfs = new Algorithms.Helpers.DepthFirstSearch(graph)
            {
                ProcessEdge = (p, v) =>
                {
                    tree.Add(new TreeNode(v.Index, p.Index, v, null));
                }
            };
            dfs.Run(mainVertex);
            var rootNode = new TreeNode(mainVertex.Index, 0, mainVertex, null);
            tree.Add(rootNode);
            tree = tree.OrderBy(x => x.Id).ToList();
            tree.Add(new TreeNode(-1, -1, null, null));

            var treeHash = tree.ToLookup(cat => cat.ParentId);

            foreach (var node in tree)
            {
                node.Children = treeHash[node.Id].ToList();
            }
            rootNode.PrintTreeAsRtf(stream);
        }

        private class TreeNode
        {
            private const string ColorTable = "{\\colortbl ;" +
                                            "\\red0\\green0\\blue0;" +
                                            "\\red0\\green0\\blue255;" +
                                            "\\red0\\green255\\blue0;" +
                                            "\\red255\\green0\\blue0;" +
                                            "\\red128\\green128\\blue128;}";
            private readonly Dictionary<VertexColor, string> ColorPositionInTable = new Dictionary<VertexColor, string>()
            {
                [VertexColor.Black] = "0",
                [VertexColor.White] = "1",
                [VertexColor.Blue] = "2",
                [VertexColor.Green] = "3",
                [VertexColor.Red] = "4",
                [VertexColor.Gray] = "5"
            };

            public int Id { get; private set; }
            public int ParentId { get; private set; }
            public IVertex Value { get; private set; }
            public IEnumerable<TreeNode> Children { get; set; }

            public TreeNode(int id, int parentId, IVertex value, IEnumerable<TreeNode> children)
            {
                Id = id;
                ParentId = parentId;
                Value = value;
                Children = children;
            }

            public void PrintTreeAsRtf(TextWriter textWriter)
            {
                textWriter.WriteLine("{\\rtf1 " + ColorTable + Environment.NewLine);
                var firstStack = new List<TreeNode> { this };

                var childListStack = new List<List<TreeNode>> { firstStack };

                while (childListStack.Count > 0)
                {
                    var childStack = childListStack[childListStack.Count - 1];

                    if (childStack.Count == 0)
                    {
                        childListStack.RemoveAt(childListStack.Count - 1);
                    }
                    else
                    {
                        var root = childStack[0];
                        childStack.RemoveAt(0);

                        var indent = String.Empty;
                        for (int i = 0; i < childListStack.Count - 1; i++)
                        {
                            indent += "  ";
                        }

                        var rootStr = String.Join("", root.Value.Name.Select(x => "\\u" + ((int)x).ToString() + "?"));
                        textWriter.WriteLine("{\\cf" + ColorPositionInTable[root.Value.Color] + " " + indent + rootStr + "\\cf0" + "}");
                        textWriter.WriteLine("\\par");

                        if (root.Children.Count() > 0)
                        {
                            childListStack.Add(new List<TreeNode>(root.Children));
                        }
                    }
                }
                textWriter.WriteLine("}");
            }
        }
    }
}
