﻿using AIFH_Vol3.Core;
using AIFH_Vol3.Examples.Learning;
using AIFH_Vol3_Core.Core.ANN;
using AIFH_Vol3_Core.Core.ANN.Activation;
using AIFH_Vol3_Core.Core.ANN.Train;
using AIFH_Vol3_Core.Core.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIFH_Vol3.Examples.ANN
{
    public class LearnDigitsDropout : SimpleLearn
    {
        /// <summary>
        /// The name of this example.
        /// </summary>
        public static string ExampleName = "MNIST Digits Dropout Neural Network.";

        /// <summary>
        /// The chapter this example is from.
        /// </summary>
        public static int ExampleChapter = 12;

        public static readonly int MNIST_DEPTH = 1;

        public void Process()
        {
            Console.WriteLine("Please wait, reading MNIST training data.");
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            MNISTReader trainingReader = LearnDigitsBackprop.LoadMNIST(dir, true, MNIST_DEPTH);
            MNISTReader validationReader = LearnDigitsBackprop.LoadMNIST(dir, false, MNIST_DEPTH);

            Console.WriteLine("Training set size: " + trainingReader.NumImages);
            Console.WriteLine("Validation set size: " + validationReader.NumImages);

            int inputCount = trainingReader.Data[0].Input.Length;
            int outputCount = trainingReader.Data[0].Ideal.Length;

            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, inputCount));
            network.AddLayer(new BasicLayer(new ActivationReLU(), true, 50));
            network.AddLayer(new DropoutLayer(new ActivationReLU(), true, 25, 0.5));
            network.AddLayer(new BasicLayer(new ActivationReLU(), true, 5));
            network.AddLayer(new BasicLayer(new ActivationSoftMax(), false, outputCount));
            network.FinalizeStructure();
            network.Reset();

            // train the neural network
            Console.WriteLine("Training neural network.");
            BackPropagation train = new BackPropagation(network, trainingReader.Data, 1e-4, 0.9);
            train.L1 = 0;
            train.L2 = 1e-11;

            this.PerformIterationsClassifyEarlyStop(train, network, trainingReader.Data, 5);
            
        }

        /// <summary>
        ///     The entry point for this example.  If you would like to make this example
        ///     stand alone, then add to its own project and rename to Main.
        /// </summary>
        /// <param name="args">Not used.</param>
        public static void ExampleMain(string[] args)
        {
            var prg = new LearnDigitsDropout();
            prg.Process();
        }
    }
}
