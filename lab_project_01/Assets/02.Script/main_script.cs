using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using System.IO;
using UnityEngine.UI;
using Accord;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using Accord.Math.Optimization.Losses;

public class main_script : MonoBehaviour
{
    Controller controller;          //컨트롤러
    bool check;
    List<Vector> R_PVL; //오른 손바닥
    List<Vector> RTV0;  //오른 손가락
    List<Vector> RTV1;
    List<Vector> RTV2;
    List<Vector> RTV3;
    List<Vector> RTV4;

    List<double> R_Histo; 
    List<double> R_Histo0;
    List<double> R_Histo1;
    List<double> R_Histo2;
    List<double> R_Histo3;
    List<double> R_Histo4;


    List<Vector> L_PVL; //왼 손바닥
    List<Vector> LTV0;  //왼 손가락
    List<Vector> LTV1;
    List<Vector> LTV2;
    List<Vector> LTV3;
    List<Vector> LTV4;

    List<double> L_Histo;
    List<double> L_Histo0;
    List<double> L_Histo1;
    List<double> L_Histo2;
    List<double> L_Histo3;
    List<double> L_Histo4;

    List<double> Histo; //오,왼 둘다

    float power;
    public static int predicted = 10;
    string m_strPath = "Assets/";
    GetGesture g;
    SVM svm;
    InputField gstLabel;
    MulticlassSupportVectorMachine<Gaussian> machine;

    public static Hand R_Hand;
    public static Hand L_Hand;

    Vector3 velocity3 = new Vector3(0.0f, 400.0f, 0.0f);
    void Start()
    {
        controller = new Controller();

        //오른손
        R_PVL = new List<Vector>();
        RTV0 = new List<Vector>();
        RTV1 = new List<Vector>();
        RTV2 = new List<Vector>();
        RTV3 = new List<Vector>();
        RTV4 = new List<Vector>();

        R_Histo = new List<double>();
        R_Histo0 = new List<double>();
        R_Histo1 = new List<double>();
        R_Histo2 = new List<double>();
        R_Histo3 = new List<double>();
        R_Histo4 = new List<double>();

        //왼손
        L_PVL = new List<Vector>();
        LTV0 = new List<Vector>();
        LTV1 = new List<Vector>();
        LTV2 = new List<Vector>();
        LTV3 = new List<Vector>();
        LTV4 = new List<Vector>();

        L_Histo = new List<double>();
        L_Histo0 = new List<double>();
        L_Histo1 = new List<double>();
        L_Histo2 = new List<double>();
        L_Histo3 = new List<double>();
        L_Histo4 = new List<double>();

        //오,왼 둘다
        Histo = new List<double>();

        g = new GetGesture();
        svm = new SVM();
        gstLabel = GameObject.Find("InputField").GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    {

        Frame frame1 = controller.Frame();

        R_Hand = null;
        L_Hand = null;

        //왼손,오른손찾기
        if (frame1.Hands.Count > 0)
        {
            if (frame1.Hands[0].IsRight) R_Hand = frame1.Hands[0];
            else L_Hand = frame1.Hands[0];
        }
        if (frame1.Hands.Count > 1)
        {
            if (frame1.Hands[1].IsRight) R_Hand = frame1.Hands[1];
            else L_Hand = frame1.Hands[1];
        }

        if (R_Hand != null) print(R_Hand.Id);
        if (L_Hand != null) print(L_Hand.Id);


        if (Input.GetKeyDown(KeyCode.S))
        {
            print("start-------------------------------------------");
            check = true;
            //오른
            R_PVL.Clear();
            RTV0.Clear();
            RTV1.Clear();
            RTV2.Clear();
            RTV3.Clear();
            RTV4.Clear();

            //왼
            L_PVL.Clear();
            LTV0.Clear();
            LTV1.Clear();
            LTV2.Clear();
            LTV3.Clear();
            LTV4.Clear();

            //히스토그램초기화
            Histo.Clear();
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            print("stop---------------------------------------");
            check = false;



            //오른손
            if (R_PVL.Count != 0)
            {
                R_Histo = g.histograming(g.chainCode(g.normalize(g.resampling(R_PVL)))); //g.histograming(g.chainCode(g.normalize(g.resampling(R_PVL))));

                R_Histo0 = g.histograming(g.chainCode(g.normalize(g.resampling(RTV0))));
                R_Histo1 = g.histograming(g.chainCode(g.normalize(g.resampling(RTV1))));
                R_Histo2 = g.histograming(g.chainCode(g.normalize(g.resampling(RTV2))));
                R_Histo3 = g.histograming(g.chainCode(g.normalize(g.resampling(RTV3))));
                R_Histo4 = g.histograming(g.chainCode(g.normalize(g.resampling(RTV4))));

                for (int i = 0; i < R_Histo.Count; i++) Histo.Add(R_Histo[i]);
                for (int i = 0; i < R_Histo0.Count; i++) Histo.Add(R_Histo0[i]);
                for (int i = 0; i < R_Histo1.Count; i++) Histo.Add(R_Histo1[i]);
                for (int i = 0; i < R_Histo2.Count; i++) Histo.Add(R_Histo2[i]);
                for (int i = 0; i < R_Histo3.Count; i++) Histo.Add(R_Histo3[i]);
                for (int i = 0; i < R_Histo4.Count; i++) Histo.Add(R_Histo4[i]);
            }


            //왼손
            if (L_PVL.Count != 0)
            {
                L_Histo = g.histograming(g.chainCode(g.normalize(g.resampling(L_PVL)))); //g.histograming(g.chainCode(g.normalize(g.resampling(R_PVL))));

                L_Histo0 = g.histograming(g.chainCode(g.normalize(g.resampling(LTV0))));
                L_Histo1 = g.histograming(g.chainCode(g.normalize(g.resampling(LTV1))));
                L_Histo2 = g.histograming(g.chainCode(g.normalize(g.resampling(LTV2))));
                L_Histo3 = g.histograming(g.chainCode(g.normalize(g.resampling(LTV3))));
                L_Histo4 = g.histograming(g.chainCode(g.normalize(g.resampling(LTV4))));

                for (int i = 0; i < L_Histo.Count; i++) Histo.Add(L_Histo[i]);
                for (int i = 0; i < L_Histo0.Count; i++) Histo.Add(L_Histo0[i]);
                for (int i = 0; i < L_Histo1.Count; i++) Histo.Add(L_Histo1[i]);
                for (int i = 0; i < L_Histo2.Count; i++) Histo.Add(L_Histo2[i]);
                for (int i = 0; i < L_Histo3.Count; i++) Histo.Add(L_Histo3[i]);
                for (int i = 0; i < L_Histo4.Count; i++) Histo.Add(L_Histo4[i]);
            }

            WriteData();
        }
        if (check)
        {
            if (R_Hand != null)
            {
                R_PVL.Add(R_Hand.PalmPosition);
                RTV0.Add(R_Hand.Fingers[0].TipPosition);
                RTV1.Add(R_Hand.Fingers[1].TipPosition);
                RTV2.Add(R_Hand.Fingers[2].TipPosition);
                RTV3.Add(R_Hand.Fingers[3].TipPosition);
                RTV4.Add(R_Hand.Fingers[4].TipPosition);
            }
            if (L_Hand != null)
            {
                L_PVL.Add(L_Hand.PalmPosition);
                LTV0.Add(L_Hand.Fingers[0].TipPosition);
                LTV1.Add(L_Hand.Fingers[1].TipPosition);
                LTV2.Add(L_Hand.Fingers[2].TipPosition);
                LTV3.Add(L_Hand.Fingers[3].TipPosition);
                LTV4.Add(L_Hand.Fingers[4].TipPosition);
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            //양손이면
            if ((R_PVL.Count != 0) && (L_PVL.Count != 0)) machine = svm.teach(m_strPath, "Data3.txt");
            //오른손이면
            else if (R_PVL.Count != 0) machine = svm.teach(m_strPath, "Data1.txt");
            //왼손이면
            else if (L_PVL.Count != 0) machine =  svm.teach(m_strPath, "Data2.txt");

            print("start-------------------------------------------");
            check = true;
            //오른
            R_PVL.Clear();
            RTV0.Clear();
            RTV1.Clear();
            RTV2.Clear();
            RTV3.Clear();
            RTV4.Clear();

            //왼
            L_PVL.Clear();
            LTV0.Clear();
            LTV1.Clear();
            LTV2.Clear();
            LTV3.Clear();
            LTV4.Clear();

            //히스토그램초기화
            Histo.Clear();
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            check = false;




            //오른손
            if (R_PVL.Count != 0)
            {
                R_Histo = g.histograming(g.chainCode(g.normalize(g.resampling(R_PVL)))); //g.histograming(g.chainCode(g.normalize(g.resampling(R_PVL))));

                R_Histo0 = g.histograming(g.chainCode(g.normalize(g.resampling(RTV0))));
                R_Histo1 = g.histograming(g.chainCode(g.normalize(g.resampling(RTV1))));
                R_Histo2 = g.histograming(g.chainCode(g.normalize(g.resampling(RTV2))));
                R_Histo3 = g.histograming(g.chainCode(g.normalize(g.resampling(RTV3))));
                R_Histo4 = g.histograming(g.chainCode(g.normalize(g.resampling(RTV4))));

                for (int i = 0; i < R_Histo.Count; i++) Histo.Add(R_Histo[i]);
                for (int i = 0; i < R_Histo0.Count; i++) Histo.Add(R_Histo0[i]);
                for (int i = 0; i < R_Histo1.Count; i++) Histo.Add(R_Histo1[i]);
                for (int i = 0; i < R_Histo2.Count; i++) Histo.Add(R_Histo2[i]);
                for (int i = 0; i < R_Histo3.Count; i++) Histo.Add(R_Histo3[i]);
                for (int i = 0; i < R_Histo4.Count; i++) Histo.Add(R_Histo4[i]);
            }


            //왼손
            if (L_PVL.Count != 0)
            {
                L_Histo = g.histograming(g.chainCode(g.normalize(g.resampling(L_PVL)))); //g.histograming(g.chainCode(g.normalize(g.resampling(R_PVL))));

                L_Histo0 = g.histograming(g.chainCode(g.normalize(g.resampling(LTV0))));
                L_Histo1 = g.histograming(g.chainCode(g.normalize(g.resampling(LTV1))));
                L_Histo2 = g.histograming(g.chainCode(g.normalize(g.resampling(LTV2))));
                L_Histo3 = g.histograming(g.chainCode(g.normalize(g.resampling(LTV3))));
                L_Histo4 = g.histograming(g.chainCode(g.normalize(g.resampling(LTV4))));

                for (int i = 0; i < L_Histo.Count; i++) Histo.Add(L_Histo[i]);
                for (int i = 0; i < L_Histo0.Count; i++) Histo.Add(L_Histo0[i]);
                for (int i = 0; i < L_Histo1.Count; i++) Histo.Add(L_Histo1[i]);
                for (int i = 0; i < L_Histo2.Count; i++) Histo.Add(L_Histo2[i]);
                for (int i = 0; i < L_Histo3.Count; i++) Histo.Add(L_Histo3[i]);
                for (int i = 0; i < L_Histo4.Count; i++) Histo.Add(L_Histo4[i]);
            }



            predicted = machine.Decide(Histo.ToArray());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = new Vector3(0.39f, 0.55f, -1.41f);
            predicted = 10;

        }

        //int predicted = machine.Decide(예측할 제스처);
       //print(predicted
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");
    }
    void FixedUpdate()
    {
         power = -20.0f;
        Vector3 velocity = new Vector3(0.5f, 0.0f, 0.0f);
        Vector3 velocity2 = new Vector3(0.0f, 0.0f, 0.5f);
        if (predicted == 0 && L_Hand == null && R_Hand != null)
        {
            velocity = velocity * power;
            if (transform.position.x > -4.5)
                GetComponent<Rigidbody>().AddForce(velocity*100);
            predicted = 10;

        }
        if (predicted == 1 && L_Hand == null && R_Hand != null)
        {
            velocity = velocity * -power;
            GetComponent<Rigidbody>().AddForce(velocity * 100);
            predicted = 10;

        }

        if (predicted == 2 && L_Hand == null && R_Hand != null)
        {
            velocity2 = velocity2 * -power;
            GetComponent<Rigidbody>().AddForce(velocity2 * 100);
            predicted = 10;
        }
        if (predicted == 3 && L_Hand == null && R_Hand != null)
        {
            velocity2 = velocity2 * power;
            GetComponent<Rigidbody>().AddForce(velocity2 * 100);
            predicted = 10;
        }
        if (predicted == 0 && L_Hand != null && R_Hand == null)  //위로
        {
            predicted = 10;
            if (transform.position.y < 0.57)
                GetComponent<Rigidbody>().AddForce(velocity3);
            if (transform.position.y > 3.53)
                GetComponent<Rigidbody>().AddForce(-velocity3);
        }
        if (predicted == 1 && L_Hand != null && R_Hand == null)  //리셋
        {
            transform.position = new Vector3(0.39f, 0.55f, -1.41f);
            predicted = 10;

        }

    }
    void OnDisable()
    {
        controller.StopConnection(); //error 
    }

    public void WriteData()
    {
        FileStream f = new FileStream(m_strPath + "default.txt", FileMode.Append, FileAccess.Write);
        StreamWriter writer;


        if ((R_PVL.Count != 0) && (L_PVL.Count != 0)) f = new FileStream(m_strPath + "Data3.txt", FileMode.Append, FileAccess.Write);
        else if (R_PVL.Count != 0) f = new FileStream(m_strPath + "Data1.txt", FileMode.Append, FileAccess.Write);
        else if (L_PVL.Count != 0) f = new FileStream(m_strPath + "Data2.txt", FileMode.Append, FileAccess.Write);


        writer = new StreamWriter(f, System.Text.Encoding.Unicode);


        if (gstLabel.text != null)
            writer.Write(gstLabel.text);
        else
            writer.Write("1 ");

        for (int i = 0; i < Histo.Count; i++)
        {
            writer.Write(" " + Histo[i]);
        }
        writer.WriteLine("");
        writer.Close();
        f.Close();
        
    }
}