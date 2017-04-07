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
        private static string _colorTable = "{\\colortbl ;" +
                                            "\\red0\\green0\\blue0;" +
                                            "\\red0\\green0\\blue255;" +
                                            "\\red0\\green255\\blue0;" + 
                                            "\\red255\\green0\\blue0;" +
                                            "\\red128\\green128\\blue128;}";

        private static Dictionary<VertexColor, string> _rtfColor = new Dictionary<VertexColor, string>()
        {
            [VertexColor.Black] = "0",
            [VertexColor.White] = "1",
            [VertexColor.Blue] = "2",
            [VertexColor.Green] = "3",
            [VertexColor.Red] = "4",
            [VertexColor.Gray] = "15"
        };

        public static void PrintTreeAsRtf(this TreeNode root, System.IO.TextWriter textWriter)
        {
            textWriter.WriteLine("{\\rtf1 " + _colorTable + "\n");
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

                    var rootStr = String.Join("", root.Value.Name.Select(x => "\\u" + ((int)x).ToString() + "?"));
                    textWriter.WriteLine("{\\cf" + _rtfColor[root.Value.Color] + " " + indent  + rootStr + "\\cf0" + "}");
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
