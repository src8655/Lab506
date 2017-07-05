using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Leap;

class GetGesture
{
    static public double pi = 3.1415926535897932384626433832795;
    static public double raTode = 180 / pi;
    public float distance(Vector v1, Vector v2)
    {
        return (float)Math.Sqrt(Math.Pow(v1.x - v2.x, 2) + Math.Pow(v1.y - v2.y, 2) + Math.Pow(v1.z - v2.z, 2));
    }

    /////////////////////////////////////////////////////////////

    public List<int> grab_resampling(List<int> inputs)
    {

        List<int> list1;
        List<int> list2;
        List<int> results;

        int resampling;     //리셈플링갯수
        int total;
        int last;
        int count;


        list1 = new List<int>();
        list2 = new List<int>();
        results = new List<int>();

        resampling = 32;
        total = inputs.Count;

        last = inputs[0];
        list1.Add(inputs[0]);
        count = 0;

        for (int i = 0; i < total; i++)
        {
            if (last == inputs[i]) count++;
            else
            {
                list2.Add(count);
                count = 1;
                last = inputs[i];
                list1.Add(inputs[i]);
            }
        }

        list2.Add(count);

        int list_total = 0;

        //비율구해서 list2에다가 복사
        for (int i = 0; i < list2.Count; i++)
        {
            list2[i] = (int)((float)list2[i] / (float)total * (float)resampling);
            list_total += list2[i];
        }

        count = 0;
        //리셈플링갯수보다 작을때
        while (list_total < resampling)
        {
            if (count > list2.Count - 1) count = 0;
            else
            {
                list2[count] = list2[count] + 1;
                count++;
                list_total++;
            }
        }

        //리셈플링결과 만들기
        for (int i = 0; i < list1.Count; i++)
        {
            for (int j = 0; j < list2[i]; j++) results.Add(list1[i]);
        }

        return results;
    }


    /////////////////////////////////////////////////////



    public List<Vector> resampling(List<Vector> VL)
    {
        Vector tempV;
        List<Vector> rHL = new List<Vector>(); ;
        float lengthG = 0;
        float lengthS = 0;
        float D = 0;
        float lengthA = 0;
        float x0 = 0;
        float x1 = 0;
        float w = 0;
        int numsample = 64;


        for (int i = 1; i < VL.Count; i++)
            lengthG += distance(VL[i-1], VL[i]);
        lengthS = lengthG / (numsample - 1);

        for (int i = 1; i < VL.Count; i++)
        {
            D = distance(VL[i], VL[i - 1]);
            lengthA += D;
            if (lengthA >= lengthS)
            {
                x0 = lengthS - (lengthA - D);
                x1 = D - x0;
                w = x0 + x1;
                x0 /= w;
                x1 /= w;
                tempV = VL[i - 1] * x1 + VL[i] * x0;


                rHL.Add(tempV);
                VL[i - 1] = tempV;
                i--;
                lengthA = 0;
            }
        }
        while (rHL.Count < numsample)
        {
            rHL.Add(VL[VL.Count - 1]);
        }
        return rHL;
    }
    public List<Vector> normalize(List<Vector> VL)
    {
        Vector cnt = new Vector(0, 0, 0);  //중심 벡터
        Vector min, max;
        Vector ratio;
        float MAX;
        List<Vector> nVL = new List<Vector>();
        min = max = VL[0];                   //min, max 초기화
        for (int i = 1; i < VL.Count; i++)
        {
            cnt = cnt + VL[i];
            if (VL[i].x < min.x)
                min.x = VL[i].x;
            if (VL[i].y < min.y)
                min.y = VL[i].y;
            if (VL[i].z < min.z)
                min.z = VL[i].z;
            if (VL[i].x > max.x)
                max.x = VL[i].x;
            if (VL[i].y > max.y)
                max.y = VL[i].y;
            if (VL[i].z > max.z)
                max.z = VL[i].z;
        }
        cnt = new Vector(cnt.x * (1.0f / VL.Count), cnt.y * (1.0f / VL.Count), cnt.z * (1.0f / VL.Count));
        MAX = (Math.Abs(max.x - min.x) > Math.Abs(max.z - min.z) ? Math.Abs(max.x - min.x) : Math.Abs(max.z - min.z)) > Math.Abs(max.y - min.y) ? (Math.Abs(max.x - min.x) > Math.Abs(max.z - min.z) ? Math.Abs(max.x - min.x) : Math.Abs(max.z - min.z)) : Math.Abs(max.y - min.y);
        ratio = new Vector(1.0f / MAX, 1.0f / MAX, 1.0f / MAX);
        for (int i = 0; i < VL.Count; i++)
        {
            nVL.Add(new Vector((VL[i].x - cnt.x) * ratio.x, (VL[i].y - cnt.y) * ratio.y, (VL[i].z - cnt.z) * ratio.z));

        }
        return nVL;
    }
    public List<int> chainCode(List<Vector> VL)
    {
        ///////
        List<int> chainL = new List<int>();
        float dx, dy, dz;
        float criticalPoint = 0.01f;
        double degree;
        Boolean transCheck_x, transCheck_y, transCheck_z;
        for (int i = 0; i < VL.Count - 1; i++)
        {
            dx = (VL[i + 1].x - VL[i].x);
            dy = (VL[i + 1].y - VL[i].y);
            dz = -(VL[i + 1].z - VL[i].z);
            degree = Math.Atan(dz / dx) * raTode;
            if (Math.Abs(dx) > criticalPoint)                     //임계치 이상 이동하는지 체크.
                transCheck_x = true;
            else
                transCheck_x = false;
            if (Math.Abs(dy) > criticalPoint)
                transCheck_y = true;
            else
                transCheck_y = false;
            if (Math.Abs(dz) > criticalPoint)
                transCheck_z = true;
            else
                transCheck_z = false;
            if (!transCheck_x && !transCheck_z && transCheck_y) // 체인코드 8(상),17(하) 
            {
                if (dy > 0)
                    chainL.Add(8);
                else
                    chainL.Add(17);
            }
            else if ((transCheck_x || transCheck_z) && !transCheck_y)//xz평면상 체인코드 0~7
            {
                if (dx > 0)
                {
                    if (90.0 * 3.0 / 4.0 < degree && degree <= 90.0)
                        chainL.Add(4);
                    else if (90.0 * 1.0 / 4.0 < degree && degree <= 90.0 * 3.0 / 4.0)
                        chainL.Add(5);
                    else if (-90.0 * 1.0 / 4.0 < degree && degree <= 90.0 * 1.0 / 4.0)
                        chainL.Add(6);
                    else if (-90.0 * 3.0 / 4.0 < degree && degree <= -90.0 * 1.0 / 4.0)
                        chainL.Add(7);
                    else if (-90.0 < degree && degree <= -90.0 * 3.0 / 4.0)
                        chainL.Add(0);
                }
                else
                {
                    if (90.0 * 3.0 / 4.0 < degree && degree <= 90.0)
                        chainL.Add(0);
                    else if (90.0 * 1.0 / 4.0 < degree && degree <= 90.0 * 3.0 / 4.0)
                        chainL.Add(1);
                    else if (-90.0 * 1.0 / 4.0 < degree && degree <= 90.0 * 1.0 / 4.0)
                        chainL.Add(2);
                    else if (-90.0 * 3.0 / 4.0 < degree && degree <= -90.0 * 1.0 / 4.0)
                        chainL.Add(3);
                    else if (-90.0 < degree && degree <= -90.0 * 3.0 / 4.0)
                        chainL.Add(4);
                }
            }

            else if ((transCheck_x || transCheck_z) && transCheck_y)
            {
                if (dy > 0)//위로 향하는 체인코드 9~16
                {
                    if (dx > 0)
                    {
                        if (90.0 * 3.0 / 4.0 < degree && degree <= 90.0)
                            chainL.Add(13);
                        else if (90.0 * 1.0 / 4.0 < degree && degree <= 90.0 * 3.0 / 4.0)
                            chainL.Add(14);
                        else if (-90.0 * 1.0 / 4.0 < degree && degree <= 90.0 * 1.0 / 4.0)
                            chainL.Add(15);
                        else if (-90.0 * 3.0 / 4.0 < degree && degree <= -90.0 * 1.0 / 4.0)
                            chainL.Add(16);
                        else if (-90.0 < degree && degree <= -90.0 * 3.0 / 4.0)
                            chainL.Add(9);
                    }
                    else
                    {
                        if (90.0 * 3.0 / 4.0 < degree && degree <= 90.0)
                            chainL.Add(9);
                        else if (90.0 * 1.0 / 4.0 < degree && degree <= 90.0 * 3.0 / 4.0)
                            chainL.Add(10);
                        else if (-90.0 * 1.0 / 4.0 < degree && degree <= 90.0 * 1.0 / 4.0)
                            chainL.Add(11);
                        else if (-90.0 * 3.0 / 4.0 < degree && degree <= -90.0 * 1.0 / 4.0)
                            chainL.Add(12);
                        else if (-90.0 < degree && degree <= -90.0 * 3.0 / 4.0)
                            chainL.Add(13);
                    }
                }

                else//아래로 향하는 체인코드 18~25
                {
                    if (dx > 0)
                    {
                        if (90.0 * 3.0 / 4.0 < degree && degree <= 90.0)
                            chainL.Add(22);
                        else if (90.0 * 1.0 / 4.0 < degree && degree <= 90.0 * 3.0 / 4.0)
                            chainL.Add(23);
                        else if (-90.0 * 1.0 / 4.0 < degree && degree <= 90.0 * 1.0 / 4.0)
                            chainL.Add(24);
                        else if (-90.0 * 3.0 / 4.0 < degree && degree <= -90.0 * 1.0 / 4.0)
                            chainL.Add(25);
                        else if (-90.0 < degree && degree <= -90.0 * 3.0 / 4.0)
                            chainL.Add(18);
                    }
                    else
                    {
                        if (90.0 * 3.0 / 4.0 < degree && degree <= 90.0)
                            chainL.Add(18);
                        else if (90.0 * 1.0 / 4.0 < degree && degree <= 90.0 * 3.0 / 4.0)
                            chainL.Add(19);
                        else if (-90.0 * 1.0 / 4.0 < degree && degree <= 90.0 * 1.0 / 4.0)
                            chainL.Add(20);
                        else if (-90.0 * 3.0 / 4.0 < degree && degree <= -90.0 * 1.0 / 4.0)
                            chainL.Add(21);
                        else if (-90.0 < degree && degree <= -90.0 * 3.0 / 4.0)
                            chainL.Add(22);
                    }
                }

            }

        }
        return chainL;
    }
    public List<double> histograming(List<int> chain)
    {
        List<double> histograme = new List<double>();
        List<double> chaincount = new List<double>();
        int n = 5;    //히스토그램 구간 개수
        int iNums = chain.Count / n;
        for (int i = 1; i <= n; i++)
        {
            chaincount.Clear();
            for (int j = 0; j < 26; j++)
                chaincount.Add(0);
            for (int j = iNums * (i - 1); j < iNums * i; j++)
            {
                switch (chain[j])
                {
                    case 0: chaincount[0]++; break;
                    case 1: chaincount[1]++; break;
                    case 2: chaincount[2]++; break;
                    case 3: chaincount[3]++; break;
                    case 4: chaincount[4]++; break;
                    case 5: chaincount[5]++; break;
                    case 6: chaincount[6]++; break;
                    case 7: chaincount[7]++; break;
                    case 8: chaincount[8]++; break;
                    case 9: chaincount[9]++; break;
                    case 10: chaincount[10]++; break;
                    case 11: chaincount[11]++; break;
                    case 12: chaincount[12]++; break;
                    case 13: chaincount[13]++; break;
                    case 14: chaincount[14]++; break;
                    case 15: chaincount[15]++; break;
                    case 16: chaincount[16]++; break;
                    case 17: chaincount[17]++; break;
                    case 18: chaincount[18]++; break;
                    case 19: chaincount[19]++; break;
                    case 20: chaincount[20]++; break;
                    case 21: chaincount[21]++; break;
                    case 22: chaincount[22]++; break;
                    case 23: chaincount[23]++; break;
                    case 24: chaincount[24]++; break;
                    case 25: chaincount[25]++; break;

                    default: break;
                }
            }
            for (int j = 0; j < 26; j++)
                chaincount[j] /= (iNums);
            for (int j = 0; j < 26; j++)
                histograme.Add(chaincount[j]);
        }
        return histograme;
    }
    ////////////

}

////////////////////