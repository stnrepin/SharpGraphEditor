using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Graph.Core
{
    public interface IAlgorithmHost
    {
        IAlgorithmOutput Output { get; }

        double MinElementX { get; }
        double MinElementY { get; }
        double MaxElementX { get; }
        double MaxElementY { get; }

        IVertex GetSelectedVertex();

        void ShowComment();
        void ShowComment(string text);
        void ShowCommentForLastAction(string text);
        void HideComment();
        void ClearComment();

        void ShowTable();
        void AddToTable(string row);
        void AddToTableForLastAction(string row);
        void AddToTable(string[] row);
        void AddToTableForLastAction(string[] row);
        void RemoveRowFromTable(string row);
        void RemoveRowFromTableForLastAction(string row);
        void RemoveRowFromTable(string[] row);
        void RemoveRowFromTableForLastAction(string[] row);
        void HideTable();
        void ClearTable();
    }
}
