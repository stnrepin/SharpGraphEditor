using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGraphEditor.Graph.Core.Algorithms
{
    public class AlgorithmProvider
    {
        private static readonly AlgorithmProvider _instance = new AlgorithmProvider();

        private List<IAlgorithm> _algorithms = new List<IAlgorithm>();

        public List<IAlgorithm> Algorithms => (_algorithms);

        public AlgorithmProvider()
        {
            _algorithms = new List<IAlgorithm>()
            {
                new EllipseLayouterAlgorithm(),
                new RandomLayouterAlgorithm(),
                new AestheticLayouterAlgorithm(),
                new BreadthFirstSearchAlgoritm(),
                new DepthFirstSearchAlgorithm(),
                new TestAlgorithm()
            };
        }

        public static AlgorithmProvider Instance
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
