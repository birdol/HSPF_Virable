using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSPF_Variable
{
    class Program
    {
        //数组打印函数
        static void print_array(Double[] A)
        {
            for (int i = 0; i < A.Length; i++)
            {
                Console.WriteLine(A[i]);
            }
        }

        //DHR的元整函数
        static double RoundDHR(double DHR)
        {
            if(DHR <= 40000)
            {
                DHR = Math.Round(DHR / 5000, 0) * 5000;
            }
            else if(DHR <= 110000)
            {
                DHR = Math.Round(DHR / 10000, 0) * 10000;
            }
            else if (DHR <= 120000)
            {
                DHR = 110000;
            }
            else
            {
                DHR = 130000;
            }

                return DHR;
        }

        static void Main(string[] args)
        {

            //Input

            //H01
            Double Q_h_k_1_62 = 24769.25;
            Double E_h_k_1_62 = 1226.87;

            //H1N,不测试留空
            Double Q_h_k_N_47 = 0.0;
            Double E_h_k_N_47 = 0.0;
            Boolean H1N_H32_samespeed = true;

            //H12
            Double Q_h_k_2_47 = 47859.25;
            Double E_h_k_2_47 = 3568.55;

            //H11
            Double Q_h_k_1_47 = 20376.46;
            Double E_h_k_1_47 = 1184.96;

            //H2v
            Double Q_h_k_v_35 = 21723.43;
            Double E_h_k_v_35 = 1891.99;

            //H22,不测试留空
            Double Q_h_k_2_35 = 0.0;
            Double E_h_k_2_35 = 0.0;

            //H32
            Double Q_h_k_2_17 = 27570.29;
            Double E_h_k_2_17 = 3035.06;

            //计算47F高频数据；

            Double Q_hcalc_k_2_47 = 0.0;
            Double E_hcalc_k_2_47 = 0.0;

            if (Q_h_k_2_47 != 0 && E_h_k_2_47 != 0) //如果测试了H12工况,使用H12的测试数据
            {
                Q_hcalc_k_2_47 = Q_h_k_2_47;
                E_hcalc_k_2_47 = E_h_k_2_47;
            }
            else if (H1N_H32_samespeed)//此外，如果H1N的频率和H32相同，使用H1N数据
            {
                Q_hcalc_k_2_47 = Q_h_k_N_47;
                E_hcalc_k_2_47 = E_h_k_N_47;
            }
            else//以上两个条件均不满足，使用H32的数据进行折算
            {
                Q_hcalc_k_2_47 = 1.612 * Q_h_k_2_17;//系数对于分体机和一体机有差异
                E_hcalc_k_2_47 = 1.1365 * E_h_k_2_17;//系数对于分体机和一体机有差异
            }



            //以下为折算的工况参数
            Double Q_h_k_1_35 = Q_h_k_1_47 + (Q_h_k_1_62 - Q_h_k_1_47) / (62 - 47) * (35 - 47);
            Double E_h_k_1_35 = E_h_k_1_47 + (E_h_k_1_62 - E_h_k_1_47) / (62 - 47) * (35 - 47);

            if(Q_h_k_2_35==0 || E_h_k_2_35==0)//如果H22工况没有进行测试，则需要折算
            {
                Q_h_k_2_35 = 0.90 * (Q_h_k_2_17 + 0.6 * (Q_hcalc_k_2_47 - Q_h_k_2_17));
                E_h_k_2_35 = 0.985 * (E_h_k_2_17 + 0.6 * (E_hcalc_k_2_47 - E_h_k_2_17));
            }



            //Console.WriteLine(Q_h_k_2_35);



            Double Cd_h = 0.25;

            string RegionNumber = "IV";
            Boolean DemandDefrost = true;

            Double T_off = -45;  //热泵低温停机环境温度
            Double T_on = -45;  //热泵停机后重新开启环境温度


            //Calculation



            Double[] TemperatureBin = new double[18] { 62, 57, 52, 47, 42, 37, 32, 27, 22, 17, 12, 7, 2, -3, -8, -13, -18, -23 };

            //以下为默认第IV区域的数据
            Double HLH = 2250;
            Double T_OD = 5;
            Double[] FractionOfTemperatureBin = new double[18] { .132, .111, .103, .093, .100, .109, .126, .087, .055, .036, .026, .013, .006, .002, .001, 0, 0, 0 };
            
            //根据不同气候区域选择计算参数
            switch (RegionNumber)
            {
                case "I":
                    HLH = 750;
                    T_OD = 37;
                    FractionOfTemperatureBin = new double[18] { .291, .239, .194, .129, .081, .041, .019, .005, .001, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    break;

                case "II":
                    HLH = 1250;
                    T_OD = 27;
                    FractionOfTemperatureBin = new double[18] { .215, .189, .163, .143, .112, .088, .056, .024, .008, .002, 0, 0, 0, 0, 0, 0, 0, 0 };
                    break;

                case "III":
                    HLH = 1750;
                    T_OD = 17;
                    FractionOfTemperatureBin = new double[18] { .153, .142, .138, .137, .135, .118, .092, .042, .021, .009, .005, .002, .001, 0, 0, 0, 0, 0 };

                    break;

                case "IV":
                    break;

                case "V":
                    HLH = 2750;
                    T_OD = -10;
                    FractionOfTemperatureBin = new double[18] { .106, .092, .086, .076, .078, .087, .102, .094, .074, .055, .047, .038, .029, .018, .010, .005, .002, .001 };
                    break;
                case "VI":
                    HLH = 2750;
                    T_OD = 30;
                    FractionOfTemperatureBin = new double[18] { .113, .206, .215, .204, .141, .076, .034, .008, .003, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                    break;
            }

            Double DHR_min = Q_hcalc_k_2_47 * (65 - T_OD) / 60;
            Double DHR_max = 2*Q_hcalc_k_2_47 * (65 - T_OD) / 60;

            if(RegionNumber=="V")
            {
                DHR_min = Q_hcalc_k_2_47;
                DHR_max = 2.2*Q_hcalc_k_2_47;
            }

            DHR_min = RoundDHR(DHR_min);
            DHR_max = RoundDHR(DHR_max);

            //Console.WriteLine("DHR_min= {0},DHR_max= {1}", DHR_min,DHR_max);

            Double[] BL_Tj_nj_N = new double[18];
            Double[] Eh_Tj_N = new double[18];
            Double[] Rh_Tj_N = new double[18];

            Double[] Q_h_k_1 = new double[18];
            Double[] E_h_k_1 = new double[18];
            Double[] COP_h_k_1 = new double[18];

            Double[] Q_h_k_v = new double[18];
            Double[] E_h_k_v = new double[18];
            Double[] COP_h_k_v = new double[18];

            Double[] Q_h_k_2 = new double[18];
            Double[] E_h_k_2 = new double[18];
            Double[] COP_h_k_2 = new double[18];

            Double[] X = new double[18];
            Double[] PLF = new double[18];
            Double[] Delta_Tj = new double[18];
            Double[] COP_h_k_i = new double[18];
            Double[] E_h_k_i = new double[18];


            Double N_Q = (Q_h_k_v_35 - Q_h_k_1_35) / (Q_h_k_2_35 - Q_h_k_1_35);
            Double N_E = (E_h_k_v_35 - E_h_k_1_35) / (E_h_k_2_35 - E_h_k_1_35);

            Double M_Q = (Q_h_k_1_62 - Q_h_k_1_47) / (62 - 47) * (1 - N_Q) + N_Q * (Q_h_k_2_35- Q_h_k_2_17) / (35-17);
            Double M_E = (E_h_k_1_62 - E_h_k_1_47) / (62 - 47) * (1 - N_E) + N_E * (E_h_k_2_35 - E_h_k_2_17) / (35 - 17);


            Double[] BuildingLoad = new double[18];

            for (int i = 0; i < 18; i++)
            {
                BuildingLoad[i] = (65 - TemperatureBin[i]) / (65 - T_OD) * 0.77 *DHR_min;
                BL_Tj_nj_N[i] = BuildingLoad[i] * FractionOfTemperatureBin[i];

                //低中频的停机系数
                if (TemperatureBin[i]<=T_off)
                {
                    Delta_Tj[i] = 0;
                }
                else if(T_off < TemperatureBin[i] && TemperatureBin[i] <= T_on)
                {
                    Delta_Tj[i] = 0.5;
                }
                else
                {
                    Delta_Tj[i] = 1.0;
                }




                //机组低频运行数据
                Q_h_k_1[i] = Q_h_k_1_47+(Q_h_k_1_62- Q_h_k_1_47) /(62-47)*(TemperatureBin[i] - 47);
                E_h_k_1[i] = E_h_k_1_47 + (E_h_k_1_62 - E_h_k_1_47) / (62 - 47) * (TemperatureBin[i] - 47);
                //机组中频运行数据
                Q_h_k_v[i] = Q_h_k_v_35 + M_Q * (TemperatureBin[i] - 35);
                E_h_k_v[i] = E_h_k_v_35 + M_E * (TemperatureBin[i] - 35);
                //机组高频运行数据
                if(TemperatureBin[i]>=45 || TemperatureBin[i] <= 17)
                {
                    Q_h_k_2[i] = Q_h_k_2_17 + (Q_h_k_2_47 - Q_h_k_2_17) / (47 - 17) * (TemperatureBin[i] - 17);
                    E_h_k_2[i] = E_h_k_2_17 + (E_h_k_2_47 - E_h_k_2_17) / (47 - 17) * (TemperatureBin[i] - 17);
                }
                else
                {
                    Q_h_k_2[i] = Q_h_k_2_17 + (Q_h_k_2_35 - Q_h_k_2_17) / (35 - 17) * (TemperatureBin[i] - 17);
                    E_h_k_2[i] = E_h_k_2_17 + (E_h_k_2_35 - E_h_k_2_17) / (35 - 17) * (TemperatureBin[i] - 17);
                }


                if (BuildingLoad[i] <= Q_h_k_1[i])
                {
                    X[i] = Math.Min(1, BuildingLoad[i] / Q_h_k_1[i]);
                    PLF[i] = 1 - Cd_h * (1 - X[i]);
                    Eh_Tj_N[i] = E_h_k_1[i] * FractionOfTemperatureBin[i] * X[i] * Delta_Tj[i] / PLF[i];
                    Rh_Tj_N[i] = 0;
                }
                else if(BuildingLoad[i] > Q_h_k_1[i] && BuildingLoad[i] < Q_h_k_2[i])
                {
                    COP_h_k_1[i] = Q_h_k_1[i] / E_h_k_1[i] / 3.413;
                    COP_h_k_v[i] = Q_h_k_v[i] / E_h_k_v[i] / 3.413;
                    COP_h_k_2[i] = Q_h_k_2[i] / E_h_k_2[i] / 3.413;

                    if (Q_h_k_1[i]< BuildingLoad[i] && BuildingLoad[i]< Q_h_k_v[i])
                    {
                        COP_h_k_i[i] = COP_h_k_1[i] + (COP_h_k_v[i] - COP_h_k_1[i]) / (Q_h_k_v[i] - Q_h_k_1[i]) * (BuildingLoad[i] - Q_h_k_1[i]);
                    }
                    else
                    {
                        COP_h_k_i[i] = COP_h_k_v[i] + (COP_h_k_2[i] - COP_h_k_v[i]) / (Q_h_k_2[i] - Q_h_k_v[i]) * (BuildingLoad[i] - Q_h_k_v[i]);
                    }

                    E_h_k_i[i] = BuildingLoad[i] / 3.413 / COP_h_k_i[i];


                    Eh_Tj_N[i] = E_h_k_i[i] * Delta_Tj[i] * FractionOfTemperatureBin[i];

                    Rh_Tj_N[i] = 0;
                }
                else    //机组持续以高频运行
                {
                    //高频时停机系数不同，需要覆盖中低频数据
                    if (TemperatureBin[i] <= T_off || Q_h_k_2[i]/3.413/ E_h_k_2[i] < 1)
                    {
                        Delta_Tj[i] = 0;
                    }
                    else if (T_off < TemperatureBin[i] && TemperatureBin[i] <= T_on && Q_h_k_2[i] / 3.413 / E_h_k_2[i] < 1)
                    {
                        Delta_Tj[i] = 0.5;
                    }
                    else
                    {
                        Delta_Tj[i] = 1.0;
                    }



                    Eh_Tj_N[i] = E_h_k_2[i] * Delta_Tj[i] * FractionOfTemperatureBin[i];

                    Rh_Tj_N[i] = (BuildingLoad[i] - Q_h_k_2[i]* Delta_Tj[i])/3.413* FractionOfTemperatureBin[i];

                }
            }

            print_array(FractionOfTemperatureBin);


        }





    }
}
