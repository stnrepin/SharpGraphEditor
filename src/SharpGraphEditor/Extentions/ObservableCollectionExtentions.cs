using System.Collections.ObjectModel;

namespace SharpGraphEditor.Extentions
{
    public static class ObservableCollectionExtentions
    {
        public static void RemoveLast<T>(this ObservableCollection<T> coll, T item)
        {
            for (int i = coll.Count - 1; i >= 0; i--)
            {
                if (coll[i] != null && coll[i].Equals(item))
                {
                    coll.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
