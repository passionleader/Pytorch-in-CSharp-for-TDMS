//// 2차원 리스트 대신 Microsoft Dataframe 형태로 전달한다 -> 파이썬에서 Pandas로 변환한다
//// 반복해서 처리해 보기
//using System;
//using System.IO;  // IO 및
//using System.Linq;  // path 및
//using System.Threading;  // sleep함수 사용
//using System.Diagnostics;  // 코드 수행 시간 측정
//using System.Collections.Generic;  // 스트링 및
//using Python.Runtime;  // 파이썬
//using Microsoft.Data.Analysis; // MS Dataframe 및 데이터 분석 도구


//namespace TDMS_Reader_dotnet5._0
//{
//    class Program
//    {
//        // [함수]python Path를 설정
//        public static void AddEnvPath(params string[] paths)
//        {
//            // 시스템 환경 변수 가져오기
//            var envPaths = Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator).ToList();
//            // 중복 환경 변수가 없으면 list에 넣기
//            envPaths.InsertRange(0, paths.Where(x => x.Length > 0 && !envPaths.Contains(x)).ToArray());
//            // 환경 변수를 적용하기
//            Environment.SetEnvironmentVariable("PATH", string.Join(Path.PathSeparator.ToString(), envPaths), EnvironmentVariableTarget.Process);
//        }


//        // [함수]파이썬 작업 환경 지정
//        public static void AddPythonPath(params string[] paths)
//        {
//            // 파이썬이 설치된 폴더 지정
//            var PYTHON_HOME = Environment.ExpandEnvironmentVariables(@"C:\Program Files\Python38\");
//            // Python Home path 경로
//            AddEnvPath(PYTHON_HOME, Path.Combine(PYTHON_HOME, @"./"));
//            // Python Home path 지정
//            PythonEngine.PythonHome = PYTHON_HOME;
//            // 모듈 패키지 path 지정
//            PythonEngine.PythonPath = string.Join(
//              Path.PathSeparator.ToString(),
//              new string[] {
//          PythonEngine.PythonPath,
//          // pip install directory
//          Path.Combine(PYTHON_HOME, @"Lib\site-packages"),
//          // 파이썬 패키지 폴더(직접 만든 코드들)
//          "./pycode"
//              }
//            );
//        }


//        // [함수] TDMS 로드 함수화 시키기
//        //(아래내용 함수화 예정)


//        // [메인] TDMS 로드 및 python 코드 로드
//        static void Main(string[] args)
//        {
//            // TDMS 파일 읽기
//            using (var tdms = new NationalInstruments.Tdms.File("./TDMS/sample2.tdms"))
//            {
//                tdms.Open();

//                //// CNC 50개 데이터 생성(약 1초 분량)
//                // 채널 목록 확인 및 채널명 리스트 생성["Time", "X_FT"...]
//                var currentGroup_CNC = tdms.Groups["CNC"];
//                List<string> channelNameList_CNC = new List<string>();


//                // 각 채널에 대해 반복
//                foreach (var channel_name in currentGroup_CNC.Channels)
//                {
//                    Console.Write(channel_name.Key + ", ");
//                    channelNameList_CNC.Add(channel_name.Key);
//                }
//                Console.WriteLine("\n");


//                // 이차원 리스트로 만들기(50행 * 채널 수)
//                double[,] table_CNC = new double[50, channelNameList_CNC.Count];

                
//                // 리스트 말고 DataFrame으로 시도해 본다
//                PrimitiveDataFrameColumn <DateTime> dateTimes = new PrimitiveDataFrameColumn<DateTime>("Date");
//                DataFrame df = new DataFrame();

//                // 각 칼럼 타입 확인 -> 해당 타입으로 구성된 데이터프레임 생성 -> 저장 -> 데이터프레임 결합하기
//                // 너무 어려울 것 ㅠ.ㅠ

//                for (int j = 1; j < channelNameList_CNC.Count; j++) {
//                    PrimitiveDataFrameColumn<double> datas = new PrimitiveDataFrameColumn<double>(channelNameList_CNC[j]);
//                    foreach (var data in currentGroup_CNC.Channels[channelNameList_CNC[j]].GetData<double>()) 
//                    {
//                        datas.Append(data);
//                    }

//                    df.Append(datas);
//                }




//                // 선언한 리스트에 저장 : 다시 모든 채널에 대해 반복한다. 1열(index 0)은 Datetime 형태이므로 에러 나서 생략함
//                for (int j = 1; j < channelNameList_CNC.Count; j++)
//                {
//                    // 데이터 확인 및 데이터 리스트 생성
//                    var currentChannel = currentGroup_CNC.Channels[channelNameList_CNC[j]];

//                    int k = 0;
//                    foreach (var data in currentChannel.GetData<double>())
//                    {
//                        table_CNC[k, j] = data;
//                        k++;
//                        if (k == 50)
//                            break;
//                    }
//                }
//                Console.WriteLine("[테이블 생성 완료]CNC 1초 분량");

//                // 출력
//                Console.WriteLine("-------------------------------------------");
//                for (int k = 0; k < 50; k++)
//                {
//                    for (int j = 0; j < channelNameList_CNC.Count; j++)
//                    {
//                        Console.Write(table_CNC[k, j] + " ");
//                    }
//                    Console.WriteLine("");
//                }
//                Console.WriteLine("-------------------------------------------\n\n\n\n");




//                //// DAQ 12800개 데이터 생성
//                // 채널 목록 확인 및 채널명 리스트 생성
//                var currentGroup_DAQ = tdms.Groups["SENSOR"];

//                List<string> channelNameList_DAQ = new List<string>();
//                foreach (var channel_name in currentGroup_DAQ.Channels)
//                {
//                    Console.Write(channel_name.Key + ", ");
//                    channelNameList_DAQ.Add(channel_name.Key);
//                }
//                Console.WriteLine("\n");


//                // 이차원 리스트로 만들기(12800행 * 채널 수)
//                double[,] table_DAQ = new double[12800, channelNameList_DAQ.Count];

//                // 선언한 리스트에 저장 : 다시 모든 채널에 대해 반복한다. 1열은 Datetime 형태이므로 그냥 생략하였다
//                for (int j = 1; j < channelNameList_DAQ.Count; j++)
//                {
//                    // 데이터 확인 및 데이터 리스트 생성
//                    var currentChannel = currentGroup_DAQ.Channels[channelNameList_DAQ[j]];

//                    int k = 0;
//                    foreach (var data in currentChannel.GetData<double>())
//                    {
//                        table_DAQ[k, j] = data;
//                        k++;
//                        if (k == 12800)
//                            break;
//                    }
//                }
//                Console.WriteLine("[테이블 생성 완료]DAQ 1초 분량 중 일부");

//                // 출력(상위 50개만 출력)
//                Console.WriteLine("-------------------------------------------");
//                for (int k = 0; k < 50; k++)
//                {
//                    for (int j = 0; j < channelNameList_DAQ.Count; j++)
//                    {
//                        Console.Write(table_DAQ[k, j] + " ");
//                    }
//                    Console.WriteLine("");
//                }
//                Console.WriteLine("-------------------------------------------\n\n\n");


//                Console.Write("[TDMS파일 처리 완료]\n");
//                // 타입이 뭐지 Console.WriteLine(table_DAQ.GetType());


//                /*-------------------------------파이썬 처리하기-----------------------------------------*/
//                // python 경로 설정 함수 호출
//                AddPythonPath();

//                // Python 엔진 초기화
//                PythonEngine.Initialize();

//                // get Global Interpreter Lock
//                using (Py.GIL())
//                {
//                    // 파이썬 패키지 폴더의 pycode/cs1_table_stats.py 코드 불러오기. import 하는 형태로 수행됨
//                    dynamic test = Py.Import("cs2_sec_preprocess");



//                    // 파이썬 코드의 클래스 선언 및 반복 전달해 보기(추후 비동기 방식으로 전환시킬 것-async)
//                    while (true)
//                    {
//                        // 코드 수행시간 측정을 위한 객체 선언(TEST용)
//                        Stopwatch stopwatch = new Stopwatch();
//                        stopwatch.Start(); // TEST용

//                        // 클래스 선언
//                        dynamic function_test = test.EmbedTest(table_CNC, table_DAQ, 10, 50, 10, 12800);
//                        // EmbedTest 클래스 내부 테이블 출력 함수 수행
//                        function_test.preprocess();


//                        stopwatch.Stop(); // TEST용
//                        System.Console.WriteLine("time : " + stopwatch.ElapsedMilliseconds + "ms"); // TEST용
//                    }
//                }

//                // python 환경을 종료함
//                PythonEngine.Shutdown();

//                Console.WriteLine("Press any key...");
//                Console.ReadKey();


//            }
//        }
//    }
//}


