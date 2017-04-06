using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Graph.Core.Utils
{
    public class TreeNode
    {
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
    }

    public static class TreeNodeExtentions
    {
        private static Dictionary<VertexColor, string> _rtfHighlightForColor = new Dictionary<VertexColor, string>()
        {
            [VertexColor.Black] = "1",
            [VertexColor.Blue] = "2",
            [VertexColor.Green] = "4",
            [VertexColor.Red] = "6",
            [VertexColor.White] = "8",
            [VertexColor.Gray] = "16"
        };

        public static void PrintTreeAsRtf(this TreeNode root, System.IO.TextWriter textWriter)
        {
            textWriter.WriteLine("{\\rtf1\n");
            var firstStack = new List<TreeNode> { root };

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
                    root = childStack[0];
                    childStack.RemoveAt(0);

                    var indent = String.Empty;
                    for (int i = 0; i < childListStack.Count - 1; i++)
                    {
                        indent += "  ";
                    }

                    var rootStr = root.Value.Color != VertexColor.White ? $"*{root.Value.Name}*" : root.Value.Name;
                    textWriter.WriteLine("{\\highlight" + _rtfHighlightForColor[root.Value.Color] + " " + indent  + rootStr + "}");
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
