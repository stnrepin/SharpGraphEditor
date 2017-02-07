using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGraphEditor.Graph.Core.Algorithms
{
    public class AlgorithmManager
    {
        private static readonly AlgorithmManager _instance = new AlgorithmManager();

        private List<IAlgorithm> _algorithms = new List<IAlgorithm>();

        public List<IAlgorithm> Algorithms => (_algorithms);

        public AlgorithmManager()
        {
            _algorithms = new List<IAlgorithm>()
            {
                new EllipseLayouterAlgorithm(),
                new RandomLayouterAlgorithm(),
                new AestheticLayouterAlgorithm(),
                new TestAlgorithm()
            };
        }

        public static AlgorithmManager Instance
        {
            get
            {
                return _instance;
            }
        }

        public IAlgorithm FindAlgorithmByName(string name)
        {
            if (String.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("Algorithm name should not be null");

            return Algorithms.FirstOrDefault(x => x.Name == name);
        }

        public IAlgorithm FindAlgorithmByType<T>() where T : IAlgorithm
        {
            return Algorithms.FirstOrDefault(x => x is T);
        }
    }
}
