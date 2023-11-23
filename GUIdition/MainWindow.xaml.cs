using ConsiditionLib2023;
using ConsiditionLib2023.Genetics;
using GAF;
using GAF.Operators;
using GUIdition.Genetics;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GUIdition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Properties and Fields

        private CancellationTokenSource _CancellationTokenSource = new();

        private readonly SeriesCollection _SeriesCollection = new()
        {
            new LineSeries()
            {
                Values = new ChartValues<double> { 0d }
            }
        };

        public SeriesCollection SeriesCollection
        {
            get
            {
                return _SeriesCollection;
            }
        }

        private Maps _KartorSelectedItem = Maps.Vasteras;

        public Maps KartorSelectedItem
        {
            get
            {
                return _KartorSelectedItem;
            }
            set
            {
                _KartorSelectedItem = value;
                _CurrentMap = value;
            }
        }

        private OptimizeFor _OptimizeForSelectedItem = OptimizeFor.Score;

        public OptimizeFor OptimizeForSelectedItem
        {
            get
            {
                return _OptimizeForSelectedItem;
            }
            set
            {
                _OptimizeForSelectedItem = value;
                _OptimizeFor = value;
            }
        }

        private string _MaxScoreText = "0";
        public string MaxScoreText
        {
            get
            {
                return _MaxScoreText;
            }
            set
            {
                if (_MaxScoreText != value)
                {
                    _MaxScoreText = value;
                    OnPropertyChanged(nameof(MaxScoreText));
                }
            }
        }
        //OptimizeSelectedItem
        private static GameData? HighScore;
        private static SubmitSolution? HighScoreSolution;
        private static Maps _CurrentMap;
        private static OptimizeFor _OptimizeFor;
        private static MapData? _CurrentMapData = null;
        private static CompleteMapData _CompleteMapData;
        private static Chromosome BestChromosome;
        private const int POPULATION_SIZE = 200;//200;
        private static RandomMutationOperator _RandomMutate = new(0.01d);
        //private static ReverseMutationOperator _ReverseMutate = new(0.01d);
        private static SwapMutate _SwapMutate = new(0.1d);
        private static CustomCrossover _Crossover = new(0.85d, true, CrossoverType.DoublePoint, ReplacementMethod.GenerationalReplacement);
        private static SwapPairMutationOperator _SwapPairMutate = new(0.1d);

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            ComboBoxSelectedKarta.ItemsSource = Enum.GetValues(typeof(Maps)).Cast<Maps>();
            ComboBoxSelectedOptimize.ItemsSource = Enum.GetValues(typeof(OptimizeFor)).Cast<OptimizeFor>();
            KartorSelectedItem = Maps.Vasteras;
            OptimizeForSelectedItem = OptimizeFor.Score;
        }

        #endregion

        #region gammel randomlösning

        /*StoreLocation location = locationKeyPair.Value;
        double salesVolume = location.SalesVolume;

        config.Locations[location.LocationName] = new PlacedLocations()
        {
            Freestyle3100Count = new Random().Next(1, 2),
            Freestyle9100Count = new Random().Next(0, 2)
        };

        if (config.Locations[location.LocationName].Freestyle9100Count == 0 &&
            config.Locations[location.LocationName].Freestyle3100Count == 0)
        {
            config.Locations[location.LocationName].Freestyle3100Count = new Random().Next(1, 6);
        }*/

        #endregion

        private async void ButtonRunSolution_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonRun.IsEnabled = false;

            _CurrentMapData = await Core.GetMapDataAsync(_CurrentMap);
            _CompleteMapData = await Core.GetCompleteMapDataAsync(_CurrentMap);

            if (_CurrentMapData == null)
            {
                throw new Exception("No mapdata");
            }

            List<Gene> firstGeneration = await Generations.GetFirstGenerationGenes(_CurrentMap);

            await Task.Run(() =>
            {
                if (_CurrentMapData == null)
                {
                    return;
                }

                /*Population population = new();
                Elite elite = new(10);
                Crossover crossover = new(0.85d, false, CrossoverType.DoublePoint, ReplacementMethod.GenerationalReplacement);
                BinaryMutate binaryMutate = new(0.01d, false);
                SwapMutate swapMutate = new(0.1d);

                highscore på gbg
                 * 
                 */

                Population population = new(0, 0, false, true, ParentSelectionMethod.FitnessProportionateSelection, true);
                Elite elite = new(8); // 8 bra

                Random random = new();

                for (int i = 0; i != POPULATION_SIZE; ++i)
                {
                    Chromosome chromosome = new();

                    //276 = antalet apparater * antalet locations * 3 (bitar)
                    /*for (int j = 0; j != 2 * _CurrentMapData.Locations.Count * 3; ++j)
                    {
                        if (i % 3 == 0)
                        {
                            chromosome.Genes.Add(new(j % 2 == 0));
                        }
                        else if (i % 2 == 0)
                        {
                            chromosome.Genes.Add(new(true));
                        }
                        else
                        {
                            chromosome.Genes.Add(new(false));
                        }
                    }*/

                    /*for (int j = 0; j != 2 * _CurrentMapData.Locations.Count; ++j)
                    {
                        chromosome.Genes.Add(new(random.Next(0, 3)));
                    }*/
                    
                    foreach (Gene gene in firstGeneration)
                    {
                        chromosome.Genes.Add(gene);
                    }
                    
                    population.Solutions.Add(chromosome);
                }

                GeneticAlgorithm geneticAlgorithm = new(population, CalculateFitness);

                geneticAlgorithm.OnGenerationComplete += OnGenerationComplete;

                geneticAlgorithm.Operators.Add(elite);
                geneticAlgorithm.Operators.Add(_Crossover);
                geneticAlgorithm.Operators.Add(_RandomMutate);
                geneticAlgorithm.Operators.Add(_SwapMutate);
                geneticAlgorithm.Operators.Add(_SwapPairMutate);
                //geneticAlgorithm.Operators.Add(_ReverseMutate);

                geneticAlgorithm.Run(TerminateAlgorithm);
            });
        }

        public bool TerminateAlgorithm(Population population,
            int currentGeneration, long currentEvaluation)
        {
            if (_CancellationTokenSource.Token.IsCancellationRequested)
            {
                _CancellationTokenSource = new CancellationTokenSource();
                InvokeOnGUIThread(() => { ButtonRun.IsEnabled = true; });

                return true;
            }

            return false;
        }

        public static IEnumerable<string> SplitBinaryString(string str, int count)
        {
            int index = 0;

            while (index < str.Length)
            {
                if (str.Length - index >= count)
                {
                    yield return str.Substring(index, count);
                }
                else
                {
                    yield return str[index..];
                }

                index += count;
            }
        }

        public static double CalculateFitness(Chromosome chromosome)
        {
            //Splitta genen i två. Högsta värdet med tre bitar är 7
            //000|000
            //^ ena värdet
            //    ^andra värdet
            //List<string> binaryStrings = new(SplitBinaryString(chromosome.ToBinaryString(), 3));
            //Debug.WriteLine(String.Join(" ", binaryStrings));
            //List<int> convertedBinaryStrings = binaryStrings.Select(str => Convert.ToInt32(str, 2)).ToList();
            SolutionConfiguration config = new(_CurrentMap, new Dictionary<string, PlacedLocations>());

            if (_CurrentMapData == null)
            {
                return 0;
            }

            int index = 0;

            //Skapa en lösning att värdera
            foreach (KeyValuePair<string, StoreLocation> locationKeyPair in _CurrentMapData.Locations)
            {
                /*int binaryValue1 = convertedBinaryStrings[index];
                int binaryValue2 = convertedBinaryStrings[index + 1];*/
                int binaryValue1 = (int)chromosome.Genes[index].ObjectValue;
                int binaryValue2 = (int)chromosome.Genes[index + 1].ObjectValue;

                if (binaryValue1 != 0 || binaryValue2 != 0)
                {
                    config.Locations.Add(locationKeyPair.Key, new PlacedLocations()
                    {
                        Freestyle3100Count = System.Math.Abs(binaryValue1),
                        Freestyle9100Count = System.Math.Abs(binaryValue2)
                    });
                }
                else
                {
                    //chromosome.Genes[index] = new Gene(1);
                    //TODO TA BORT ELSE
                    /*config.Locations.Add(locationKeyPair.Key, new PlacedLocations()
                    {
                        Freestyle3100Count = 1,
                        Freestyle9100Count = 0
                    });*/
                }

                index++;
            }

            if (config.Locations.Count == 0)
            {
                return 0;
            }

            GameData result = Core.Run(config, _CompleteMapData);

            if (result.GameScore == null)
            {
                return 0;
            }

            if (HighScore == null || HighScore.GameScore == null || GetOptimizeForHighscore() < GetOptimizeForResult(result.GameScore))
            {
                HighScore = result;
                HighScoreSolution = new SubmitSolution { Locations = config.Locations };
                BestChromosome = chromosome;
            }

            if (GetOptimizeForResult(result.GameScore) < 0)
            {
                return 0;
            }

            int fitnessDivisor = GetFitnessDivisor(_CurrentMap);
            double fitnessPunishment = (fitnessDivisor * 4d) / _CurrentMapData.Locations.Count; //(fitnessDivisor * 2) / _CurrentMapData.Locations.Count;
            int numberOfNonProfitableLocations = result.Locations.Where(pair => !pair.Value.IsProfitable).Count();
            int numberOfNonCO2SavingLocations = result.Locations.Where(pair => !pair.Value.IsCo2Saving).Count();

            //Ändra efter vilken optimering som ska exekvera
            double fitness = (GetOptimizeForResult(result.GameScore) - (numberOfNonProfitableLocations * fitnessPunishment) - (numberOfNonCO2SavingLocations * fitnessPunishment)) / GetFitnessDivisor(_CurrentMap);
            //double fitness = GetOptimizeForResult(result.GameScore) / fitnessDivisor;
            //double fitness = result.GameScore.TotalFootfall / fitnessDivisor;
            //double fitness = result.GameScore.Earnings;
            //double fitness = result.GameScore.KgCo2Savings / GetFitnessDivisor(_CurrentMap);

            if (fitness > 1)
            {
                return 1;
            }
            else if (fitness <= 0)
            {
                return 0;
            }
            
            return fitness;
            //return (result.GameScore.Total - (numberOfNonProfitableLocations * fitnessModifier)) / GetFitnessDivisor(_CurrentMap);
        }

        private static int GetFitnessDivisor(Maps map)
        {
            switch (map)
            {
                case Maps.Stockholm:
                    break;
                case Maps.Goteborg:
                    return 6200;
                case Maps.Malmo:
                    break;
                case Maps.Uppsala:
                    return 2450;
                case Maps.Vasteras:
                    return 1500;
                case Maps.Orebro:
                    break;
                case Maps.London:
                    break;
                case Maps.Linkoping:
                    return 700;
                case Maps.Berlin:
                    break;
            }
            return 1000000;
        }

        private static double GetOptimizeForHighscore()
        {
            if (_OptimizeFor == OptimizeFor.Score)
            {
                return HighScore.GameScore.Total;
            }
            else if (_OptimizeFor == OptimizeFor.CO2)
            {
                return HighScore.GameScore.KgCo2Savings;
            }

            throw new Exception("Unknown case");
        }

        private static double GetOptimizeForResult(Score score)
        {
            if (_OptimizeFor == OptimizeFor.Score)
            {
                return score.Total;
            }
            else if (_OptimizeFor == OptimizeFor.CO2)
            {
                return score.KgCo2Savings;
            }

            throw new Exception("Unknown case");
        }

        private void OnGenerationComplete(object sender, GaEventArgs e)
        {
            //Chromosome bestSolutionThisGeneration = e.Population.GetTop(1)[0];
            double avgFitness = e.Population.Solutions.Select(solution => solution.Fitness).Average();

            if (e.Generation % 301 == 0)
            {
                InvokeOnGUIThread(() =>
                {
                    SeriesCollection[0].Values = new ChartValues<double>() { avgFitness };
                });
            }

            MaxScoreText = HighScore.ToString();

            //_RandomMutate.MutationProbability = System.Math.Min(1d - avgFitness, 0.01d);
            //_SwapMutate.MutationProbability = System.Math.Min(1d - avgFitness, 0.1d);
            //_SwapPairMutate.MutationProbability = System.Math.Min(1d - avgFitness, 0.1d);
            //_ReverseMutate.MutationProbability = 1d - avgFitness;
            _Crossover.CrossoverProbability = System.Math.Max(avgFitness + 0.1d, 0.85d);

            PlotValue(avgFitness);
            //PlotValue(HighScore.GameScore.Total);
        }

        private void PlotValue(double value)
        {
            SeriesCollection[0].Values.Add(value);
        }

        private async void ButtonSubmit_OnClick(object sender, RoutedEventArgs e)
        {
            if (HighScore == null || HighScoreSolution == null)
            {
                return;
            }

            GameData? result = await Core.SubmitAsync(HighScore.MapName, HighScoreSolution);
        }

        private async void ButtonOptimize_OnClick(object sender, RoutedEventArgs e)
        {
            if (HighScore != null && HighScoreSolution != null)
            {
                GameData? result = await Core.SubmitAsync(HighScore.MapName, HighScoreSolution);

                if (result != null)
                {
                    GameData? optimizedResult = await SolutionOptimizer.OptimizeAndSubmitAsync(HighScoreSolution, result);
                    //optimizedResult = await SolutionOptimizer.OptimizeAndSubmitv2Async(HighScoreSolution, result, _CompleteMapData.GeneralData);
                }
            }
        }

        private async void ButtonRunTest_OnClick(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                if (SeriesCollection[0].Values == null)
                {
                    //Init
                    InvokeOnGUIThread(() => { SeriesCollection[0].Values = new ChartValues<double>() { 0d }; });
                }

                while (true)
                {
                    Score result = await Core.CoreTest();

                    InvokeOnGUIThread(() =>
                    {
                        SeriesCollection[0].Values.Add(result.Total);
                    });

                    await Task.Delay(100);

                    if (_CancellationTokenSource.IsCancellationRequested)
                    {
                        _CancellationTokenSource = new();
                        break;
                    }
                }
            });
        }

        private void ButtonStop_OnClick(object sender, RoutedEventArgs e)
        {
            _CancellationTokenSource.Cancel();
        }
        private static void InvokeOnGUIThread(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }
        private void ButtonReset_OnClick(object sender, RoutedEventArgs e)
        {
            InvokeOnGUIThread(() => { SeriesCollection[0].Values = new ChartValues<double>() { 0d }; });
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (propertyName != null)
            {
                InvokeOnGUIThread(() =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                });
            }
        }
    }
}
