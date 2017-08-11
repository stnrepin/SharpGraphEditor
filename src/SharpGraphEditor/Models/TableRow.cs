using System.Linq;

namespace SharpGraphEditor.Models
{
    public class TableRow
    {
        public string[] Values { get; private set; }

        public TableRow(params string[] values)
        {
            Values = values;
        }

        public override bool Equals(object obj)
        {
            if (obj is TableRow tableRow)
            {
                return Enumerable.SequenceEqual(Values, tableRow.Values);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Values.GetHashCode();
        }
    }
}
