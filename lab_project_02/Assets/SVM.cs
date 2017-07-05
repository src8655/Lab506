using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using UnityEngine;
using Accord;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using Accord.Math.Optimization.Losses;
class SVM
{
    public MulticlassSupportVectorMachine<Gaussian> teach(string m_strPath, string filename)
    {
        MulticlassSupportVectorMachine<Gaussian> machine;
        List<int> labels = new List<int>();
        int VectorNum = 0;
        double[][] inputs;
        StreamReader sr = new StreamReader(m_strPath + filename);
        // 먼저 한줄을 읽는다. 
        string source = sr.ReadLine();
        while (source != null)
        {
            string[] values = source.Split(' ');  // 공백으로 구분한다. 저장시에 공백로 구분하여 저장하였다.
            labels.Add(Int32.Parse(values[0]));
            VectorNum = values.Length - 1;
            if (values.Length == 0)
            {
                sr.Close();
                break;
            }
            MonoBehaviour.print(values.Length);
            source = sr.ReadLine();    // 한줄 읽는다.

        }

        sr = new StreamReader(m_strPath + filename);
        source = sr.ReadLine();
        inputs = new double[labels.Count][];
        for (int x = 0; x < inputs.Length; x++)
            inputs[x] = new double[VectorNum]; //26*5=130//

        int i = 0;
        while (source != null)
        {
            string[] values = source.Split(' ');  // 공백으로 구분한다. 저장시에 공백로 구분하여 저장하였다.
            for (int j = 1; j < values.Length; j++)
            {
                //MonoBehaviour.print(j + ":" + double.Parse(values[j]));
                inputs[i][j - 1] = double.Parse(values[j]);  //error 
            }
            if (values.Length == 0)
            {
                sr.Close();
                break;
            }
            i++;
            source = sr.ReadLine();    // 한줄 읽는다.
        }
        var teacher = new MulticlassSupportVectorLearning<Gaussian>()
        {
            Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
            {
                UseKernelEstimation = true
            }
        };
        //teacher.ParallelOptions.MaxDegreeOfParallelism = 1;
        // Learn a machine
        machine = teacher.Learn(inputs, labels.ToArray());
        var calibration = new MulticlassSupportVectorLearning<Gaussian>()
        {
            Model = machine, // We will start with an existing machine

            // Configure the learning algorithm to use Platt's calibration
            Learner = (param) => new ProbabilisticOutputCalibration<Gaussian>()
            {
                Model = param.Model // Start with an existing machine
            }
        };


        // Configure parallel execution options
        calibration.ParallelOptions.MaxDegreeOfParallelism = 1;

        // Learn a machine
        calibration.Learn(inputs, labels.ToArray());
        return machine;
    }

}