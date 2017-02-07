using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGraphEditor.Models.Algorithms
{
    public static class AlgorithmManager
    {
        private static List<IAlgorithm> _algorithm = new List<IAlgorithm>();

        public static bool IsLoaded { get; set; } = false;

        public static void LoadAll()
        {
            _algorithm = new List<IAlgorithm>()
            {
                new EllipseLayouterAlgorithm(),
                new RandomLayouterAlgorithm(),
                new TestAlgorithm()
            };
            IsLoaded = true;
        }

        public static List<IAlgorithm> Algorithms => (IsLoaded ? _algorithm : null);
    }
}
