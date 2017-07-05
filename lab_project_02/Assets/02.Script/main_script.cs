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

    //그랩
    List<float> R_Greb0;
    List<float> R_Greb1;
    List<float> R_Greb2;
    List<float> R_Greb3;
    List<float> R_Greb4;

    List<float> L_Greb0;
    List<float> L_Greb1;
    List<float> L_Greb2;
    List<float> L_Greb3;
    List<float> L_Greb4;

    //그랩결과
    List<int> R_Gr0;
    List<int> R_Gr1;
    List<int> R_Gr2;
    List<int> R_Gr3;
    List<int> R_Gr4;

    List<int> L_Gr0;
    List<int> L_Gr1;
    List<int> L_Gr2;
    List<int> L_Gr3;
    List<int> L_Gr4;

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

        //그랩
        R_Greb0 = new List<float>();
        R_Greb1 = new List<float>();
        R_Greb2 = new List<float>();
        R_Greb3 = new List<float>();
        R_Greb4 = new List<float>();

        L_Greb0 = new List<float>();
        L_Greb1 = new List<float>();
        L_Greb2 = new List<float>();
        L_Greb3 = new List<float>();
        L_Greb4 = new List<float>();

        //그랩 결과
        R_Gr0 = new List<int>();
        R_Gr1 = new List<int>();
        R_Gr2 = new List<int>();
        R_Gr3 = new List<int>();
        R_Gr4 = new List<int>();

        L_Gr0 = new List<int>();
        L_Gr1 = new List<int>();
        L_Gr2 = new List<int>();
        L_Gr3 = new List<int>();
        L_Gr4 = new List<int>();

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

        //R_Hand.IsPinching
        //R_Hand.GrabAngle

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

        if (R_Hand != null)
        {
            print(R_Hand.Id);
        }
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

            //그랩초기화
            R_Gr0.Clear();
            R_Gr1.Clear();
            R_Gr2.Clear();
            R_Gr3.Clear();
            R_Gr4.Clear();

            R_Greb0.Clear();
            R_Greb1.Clear();
            R_Greb2.Clear();
            R_Greb3.Clear();
            R_Greb4.Clear();

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

                List<Vector> R, R0, R1, R2, R3, R4;

                R = g.resampling(R_PVL);
                R0 = g.resampling(RTV0);
                R1 = g.resampling(RTV1);
                R2 = g.resampling(RTV2);
                R3 = g.resampling(RTV3);
                R4 = g.resampling(RTV4);

                R_Histo = g.histograming(g.chainCode(g.normalize(R))); //g.histograming(g.chainCode(g.normalize(g.resampling(R_PVL))));

                R_Histo0 = g.histograming(g.chainCode(g.normalize(R0)));
                R_Histo1 = g.histograming(g.chainCode(g.normalize(R1)));
                R_Histo2 = g.histograming(g.chainCode(g.normalize(R2)));
                R_Histo3 = g.histograming(g.chainCode(g.normalize(R3)));
                R_Histo4 = g.histograming(g.chainCode(g.normalize(R4)));

                float fta0_max = 0;
                float fta1_max = 0;
                float fta2_max = 0;
                float fta3_max = 0;
                float fta4_max = 0;

                //손가락-손바닥 거리
                for (int i = 0; i < R2.Count; i++)
                {
                    R_Greb0.Add(R0[i].DistanceTo(R[i]));
                    R_Greb1.Add(R1[i].DistanceTo(R[i]));
                    R_Greb2.Add(R2[i].DistanceTo(R[i]));
                    R_Greb3.Add(R3[i].DistanceTo(R[i]));
                    R_Greb4.Add(R4[i].DistanceTo(R[i]));
                }
                for (int i = 0; i < R_Greb0.Count; i++)
                {
                    fta0_max = R_Greb0[i] > fta0_max ? R_Greb0[i] : fta0_max;
                    fta1_max = R_Greb1[i] > fta1_max ? R_Greb1[i] : fta1_max;
                    fta2_max = R_Greb2[i] > fta2_max ? R_Greb2[i] : fta2_max;
                    fta3_max = R_Greb3[i] > fta3_max ? R_Greb3[i] : fta3_max;
                    fta4_max = R_Greb4[i] > fta4_max ? R_Greb4[i] : fta4_max;
                }
                for (int i = 0; i < R_Greb0.Count; i++)
                {
                    if ((R_Greb0[i] / fta0_max) >= 0.90) R_Gr0.Add(1);
                    else if ((R_Greb0[i] / fta0_max) >= 0.55) R_Gr0.Add(2);
                    else R_Gr0.Add(3);

                    if ((R_Greb1[i] / fta1_max) >= 0.90) R_Gr1.Add(1);
                    else if ((R_Greb1[i] / fta1_max) >= 0.55) R_Gr1.Add(2);
                    else R_Gr1.Add(3);

                    if ((R_Greb2[i] / fta2_max) >= 0.90) R_Gr2.Add(1);
                    else if ((R_Greb2[i] / fta2_max) >= 0.55) R_Gr2.Add(2);
                    else R_Gr2.Add(3);

                    if ((R_Greb3[i] / fta3_max) >= 0.90) R_Gr3.Add(1);
                    else if ((R_Greb3[i] / fta3_max) >= 0.55) R_Gr3.Add(2);
                    else R_Gr3.Add(3);

                    if ((R_Greb4[i] / fta4_max) >= 0.90) R_Gr4.Add(1);
                    else if ((R_Greb4[i] / fta4_max) >= 0.55) R_Gr4.Add(2);
                    else R_Gr4.Add(3);
                }

                //그랩 리셈플링
                R_Gr0 = g.grab_resampling(R_Gr0);
                R_Gr1 = g.grab_resampling(R_Gr1);
                R_Gr2 = g.grab_resampling(R_Gr2);
                R_Gr3 = g.grab_resampling(R_Gr3);
                R_Gr4 = g.grab_resampling(R_Gr4);

                for (int i = 0; i < R_Histo.Count; i++) Histo.Add(R_Histo[i]);
                for (int i = 0; i < R_Histo0.Count; i++) Histo.Add(R_Histo0[i]);
                for (int i = 0; i < R_Histo1.Count; i++) Histo.Add(R_Histo1[i]);
                for (int i = 0; i < R_Histo2.Count; i++) Histo.Add(R_Histo2[i]);
                for (int i = 0; i < R_Histo3.Count; i++) Histo.Add(R_Histo3[i]);
                for (int i = 0; i < R_Histo4.Count; i++) Histo.Add(R_Histo4[i]);


                for (int i = 0; i < R_Gr0.Count; i++) Histo.Add(R_Gr0[i]);
                for (int i = 0; i < R_Gr1.Count; i++) Histo.Add(R_Gr1[i]);
                for (int i = 0; i < R_Gr2.Count; i++) Histo.Add(R_Gr2[i]);
                for (int i = 0; i < R_Gr3.Count; i++) Histo.Add(R_Gr3[i]);
                for (int i = 0; i < R_Gr4.Count; i++) Histo.Add(R_Gr4[i]);
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

            //그랩초기화
            R_Gr0.Clear();
            R_Gr1.Clear();
            R_Gr2.Clear();
            R_Gr3.Clear();
            R_Gr4.Clear();

            R_Greb0.Clear();
            R_Greb1.Clear();
            R_Greb2.Clear();
            R_Greb3.Clear();
            R_Greb4.Clear();

            //히스토그램초기화
            Histo.Clear();
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            check = false;





            //오른손
            if (R_PVL.Count != 0)
            {

                List<Vector> R, R0, R1, R2, R3, R4;

                R = g.resampling(R_PVL);
                R0 = g.resampling(RTV0);
                R1 = g.resampling(RTV1);
                R2 = g.resampling(RTV2);
                R3 = g.resampling(RTV3);
                R4 = g.resampling(RTV4);

                R_Histo = g.histograming(g.chainCode(g.normalize(R))); //g.histograming(g.chainCode(g.normalize(g.resampling(R_PVL))));

                R_Histo0 = g.histograming(g.chainCode(g.normalize(R0)));
                R_Histo1 = g.histograming(g.chainCode(g.normalize(R1)));
                R_Histo2 = g.histograming(g.chainCode(g.normalize(R2)));
                R_Histo3 = g.histograming(g.chainCode(g.normalize(R3)));
                R_Histo4 = g.histograming(g.chainCode(g.normalize(R4)));

                float fta0_max = 0;
                float fta1_max = 0;
                float fta2_max = 0;
                float fta3_max = 0;
                float fta4_max = 0;

                //손가락-손바닥 거리
                for (int i = 0; i < R2.Count; i++)
                {
                    R_Greb0.Add(R0[i].DistanceTo(R[i]));
                    R_Greb1.Add(R1[i].DistanceTo(R[i]));
                    R_Greb2.Add(R2[i].DistanceTo(R[i]));
                    R_Greb3.Add(R3[i].DistanceTo(R[i]));
                    R_Greb4.Add(R4[i].DistanceTo(R[i]));
                }
                for (int i = 0; i < R_Greb0.Count; i++)
                {
                    fta0_max = R_Greb0[i] > fta0_max ? R_Greb0[i] : fta0_max;
                    fta1_max = R_Greb1[i] > fta1_max ? R_Greb1[i] : fta1_max;
                    fta2_max = R_Greb2[i] > fta2_max ? R_Greb2[i] : fta2_max;
                    fta3_max = R_Greb3[i] > fta3_max ? R_Greb3[i] : fta3_max;
                    fta4_max = R_Greb4[i] > fta4_max ? R_Greb4[i] : fta4_max;
                }
                for (int i = 0; i < R_Greb0.Count; i++)
                {
                    if ((R_Greb0[i] / fta0_max) >= 0.90) R_Gr0.Add(1);
                    else if ((R_Greb0[i] / fta0_max) >= 0.55) R_Gr0.Add(2);
                    else R_Gr0.Add(3);

                    if ((R_Greb1[i] / fta1_max) >= 0.90) R_Gr1.Add(1);
                    else if ((R_Greb1[i] / fta1_max) >= 0.55) R_Gr1.Add(2);
                    else R_Gr1.Add(3);

                    if ((R_Greb2[i] / fta2_max) >= 0.90) R_Gr2.Add(1);
                    else if ((R_Greb2[i] / fta2_max) >= 0.55) R_Gr2.Add(2);
                    else R_Gr2.Add(3);

                    if ((R_Greb3[i] / fta3_max) >= 0.90) R_Gr3.Add(1);
                    else if ((R_Greb3[i] / fta3_max) >= 0.55) R_Gr3.Add(2);
                    else R_Gr3.Add(3);

                    if ((R_Greb4[i] / fta4_max) >= 0.90) R_Gr4.Add(1);
                    else if ((R_Greb4[i] / fta4_max) >= 0.55) R_Gr4.Add(2);
                    else R_Gr4.Add(3);
                }

                //그랩 리셈플링
                R_Gr0 = g.grab_resampling(R_Gr0);
                R_Gr1 = g.grab_resampling(R_Gr1);
                R_Gr2 = g.grab_resampling(R_Gr2);
                R_Gr3 = g.grab_resampling(R_Gr3);
                R_Gr4 = g.grab_resampling(R_Gr4);

                for (int i = 0; i < R_Histo.Count; i++) Histo.Add(R_Histo[i]);
                for (int i = 0; i < R_Histo0.Count; i++) Histo.Add(R_Histo0[i]);
                for (int i = 0; i < R_Histo1.Count; i++) Histo.Add(R_Histo1[i]);
                for (int i = 0; i < R_Histo2.Count; i++) Histo.Add(R_Histo2[i]);
                for (int i = 0; i < R_Histo3.Count; i++) Histo.Add(R_Histo3[i]);
                for (int i = 0; i < R_Histo4.Count; i++) Histo.Add(R_Histo4[i]);


                for (int i = 0; i < R_Gr0.Count; i++) Histo.Add(R_Gr0[i]);
                for (int i = 0; i < R_Gr1.Count; i++) Histo.Add(R_Gr1[i]);
                for (int i = 0; i < R_Gr2.Count; i++) Histo.Add(R_Gr2[i]);
                for (int i = 0; i < R_Gr3.Count; i++) Histo.Add(R_Gr3[i]);
                for (int i = 0; i < R_Gr4.Count; i++) Histo.Add(R_Gr4[i]);
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
                GetComponent<Rigidbody>().AddForce(velocity);
           
        }
        if (predicted == 1 && L_Hand == null && R_Hand != null)
        {
            velocity = velocity * -power;
            GetComponent<Rigidbody>().AddForce(velocity);

        }

        if (predicted == 2 && L_Hand == null && R_Hand != null)
        {
            velocity2 = velocity2 * -power;
            GetComponent<Rigidbody>().AddForce(velocity2);
        }
        if (predicted == 3 && L_Hand == null && R_Hand != null)
        {
            velocity2 = velocity2 * power;
            GetComponent<Rigidbody>().AddForce(velocity2);
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