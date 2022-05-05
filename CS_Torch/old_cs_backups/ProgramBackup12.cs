// 건희가 준 모델을 통해 실제 반환 결과 확인 및 비교하기 - TDMS 파일 하나 전체에 대해여 모델 결과값 비교(DAQ)
using Python.Runtime;  // 파이썬
using System;
using System.Collections.Generic;  // 스트링 및
using System.Diagnostics;  // 코드 수행 시간 측정
using System.IO;  // IO 및
using System.Linq;  // path 및


namespace TDMS_Reader_dotnet5._0
{
    class Program
    {
        // [함수]python Path를 설정
        public static void AddEnvPath(params string[] paths)
        {
            // 시스템 환경 변수 가져오기
            var envPaths = Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator).ToList();
            // 중복 환경 변수가 없으면 list에 넣기
            envPaths.InsertRange(0, paths.Where(x => x.Length > 0 && !envPaths.Contains(x)).ToArray());
            // 환경 변수를 적용하기
            Environment.SetEnvironmentVariable("PATH", string.Join(Path.PathSeparator.ToString(), envPaths), EnvironmentVariableTarget.Process);
        }


        // [함수]파이썬 작업 환경 지정
        public static void AddPythonPath(params string[] paths)
        {
            // 파이썬이 설치된 폴더 지정
            var PYTHON_HOME = Environment.ExpandEnvironmentVariables(@"C:\Users\sungs\Anaconda3\envs\KITECH_A\");
            // Python Home path 경로
            AddEnvPath(PYTHON_HOME, Path.Combine(PYTHON_HOME, @"./"));
            // Python Home path 지정
            PythonEngine.PythonHome = PYTHON_HOME;
            // 모듈 패키지 path 지정
            PythonEngine.PythonPath = string.Join(
              Path.PathSeparator.ToString(),
              new string[] {
          PythonEngine.PythonPath,
          // pip install directory
          Path.Combine(PYTHON_HOME, @"Lib\site-packages"),
          // 파이썬 패키지 폴더(직접 만든 코드들)
          "./pycode"
              }
            );
        }


        // [함수] TDMS 로드 함수화 시키기
        //(아래내용 함수화 예정)
        //public static void func(?){

        //}


        // [메인] TDMS 로드 및 python 코드 로드
        static void Main(string[] args)
        {
            // TDMS 파일 읽기
            using (var tdms = new NationalInstruments.Tdms.File("./TDMS/190507_180041_SM45C_Shouldering.tdms"))
            {
                tdms.Open();

                // 전체 그룹 표시
                Console.WriteLine("그룹: ");
                foreach (var group in tdms)
                    Console.Write(group.Name + " ");
                Console.WriteLine("\n");


                //// DAQ 데이터 처리
                // 채널 목록 확인 및 채널명 리스트 생성
                var currentGroup_DAQ = tdms.Groups["Sensor"];

                List<string> channelNameList_DAQ = new List<string>();
                Console.WriteLine("DAQ 채널: ");
                foreach (var channel_name in currentGroup_DAQ.Channels)
                {
                    Console.Write(channel_name.Key + ", ");
                    channelNameList_DAQ.Add(channel_name.Key);
                }
                Console.WriteLine("\n");


                // TDMS 파일의 전체 행 수 확인
                var num_rows = currentGroup_DAQ.Channels[channelNameList_DAQ[0]].DataCount;
                //num_rows = 50000; //테스트용///////////////////////////////////////////////////////////////////////////////////////////
                
                // TDMS 파일 분량의 데이터를 이차원 리스트에 담기 전, 빈 이 차원 리스트 만들기
                dynamic[,] table_DAQ = new dynamic[num_rows, channelNameList_DAQ.Count];

                // 이 차원 리스트 채우기
                for (int j = 0; j < channelNameList_DAQ.Count; j++) //칼럼
                {
                    // 데이터 확인 및 데이터 리스트 생성
                    var currentChannel = currentGroup_DAQ.Channels[channelNameList_DAQ[j]];

                    int k = 0;//행 --> 실시간 알고리즘 작성
                    foreach (var data in currentChannel.GetData<dynamic>())
                    {
                        table_DAQ[k, j] = data;
                        k++;
                        //if (k == 30000 ) { break; } //테스트용/////////////////////////////////////////////////////////////////////////
                    }
                }

                // 출력(상위 10개만 출력)
                Console.WriteLine("[테이블 생성 완료]DAQ 이 차원 리스트");
                Console.WriteLine("-------------------------------------------");
                for (int k = 0; k < 10; k++)
                {
                    for (int j = 0; j < channelNameList_DAQ.Count; j++)
                    {
                        Console.Write(table_DAQ[k, j] + " ");
                    }
                    Console.WriteLine("");
                }
                Console.WriteLine("-------------------------------------------");

                Console.Write("[TDMS파일 처리 완료]\n\n\n");


                /*-------------------------------파이썬 처리하기-----------------------------------------*/
                // 1초 분량의 데이터 담을 빈 리스트 만들기
                dynamic[,] second_DAQ = new dynamic[12800, channelNameList_DAQ.Count];

                // 모델 반환 결과 담을 리스트 만들기(크기: 전체 행 수 / 12800 = 초 수)
                long sec_count = num_rows / 12800;
                double[] result_list = new double[sec_count];


                // python 경로 설정 함수 호출
                AddPythonPath();

                // Python 엔진 초기화
                PythonEngine.Initialize();

                // get Global Interpreter Lock
                using (Py.GIL())
                {
                    // 파이썬 패키지 폴더의 pycode/cs4_model_test_daq.py 코드 불러오기. import 하는 형태로 수행됨
                    dynamic test = Py.Import("cs4_model_test_daq");

                    // 1초 분량의 데이터 잘라서 파이썬에 인자 전달 --> 모델 결과 반환
                    for (int i = 0; i < sec_count; i++)
                    {
                        // 코드 수행시간 측정을 위한 객체 선언(TEST용)
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start(); // TEST용

                        // 1초 분량 자르기
                        for (int j = 0; j < 12800; j++)
                        {
                            for (int k = 0; k < channelNameList_DAQ.Count; k++)
                            {
                                second_DAQ[j, k] = table_DAQ[(i * 12800)+j, k];
                            }   
                        }

                        // 클래스 선언, 전달 인자 - 칼럼명(ch_name), 데이터(2d array), 데이터 사이즈(col*row)
                        dynamic function_test = test.EmbedTest(channelNameList_DAQ, second_DAQ, channelNameList_DAQ.Count, 12800);

                        // EmbedTest 클래스 내부 테이블 출력 함수 수행
                        result_list[i] = function_test.preprocess();

                        stopwatch.Stop(); // TEST용
                        System.Console.WriteLine("time : " + stopwatch.ElapsedMilliseconds + "ms" + "\n, model res: " + result_list[i]); // 1초간의 데이터에 대하여 돌린 모델 결과 확인
                    }

                }

                //python 환경을 종료함
                PythonEngine.Shutdown();

                Console.WriteLine("Press any key...");
                Console.ReadKey();


                // 모델 결과 한번에 출력하기
                for (int i = 0; i < sec_count; i++)
                {
                    Console.WriteLine(result_list[i]);
                }
            }
        }
    }
}