//// 파이썬 로드 + TDMS 로드 -> 클래스로 2차원 리스트 인자 전달해 보기
//// 함수 적용 https://nowonbun.tistory.com/690
//// 문제 발견: 2차원 배열을 인자로 전달 시 python에서 DF로 변환하는 데에 시간이 들 수 있음
//// 다음 시도: C#에서 데이터프레임을 선언 후 그대로 python으로 인자로 전달 -> program.backup4에서 시도 후 복귀.
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Python.Runtime;
//using System.IO;


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


//        // [메인] TDMS 로드 및 python 코드 로드
//        static void Main(string[] args)
//        {
//            // TDMS 파일 읽기
//            using (var tdms = new NationalInstruments.Tdms.File("./TDMS/sample2.tdms"))
//            {
//                tdms.Open();

//                // CNC 50개 데이터 생성(약 1초 분량)
//                // 채널 목록 확인 및 채널명 리스트 생성
//                var currentGroup_CNC = tdms.Groups["CNC"];

//                List<string> channelNameList_CNC = new List<string>();
//                foreach (var channel_name in currentGroup_CNC.Channels)
//                {
//                    Console.Write(channel_name.Key + ", ");
//                    channelNameList_CNC.Add(channel_name.Key);
//                }
//                Console.WriteLine("\n");


//                // 이차원 리스트로 만들기(50행 * 채널 수)
//                double[,] table_CNC = new double[50, channelNameList_CNC.Count];

//                // 선언한 리스트에 저장 : 다시 모든 채널에 대해 반복한다. 1열(index 0)은 Datetime 형태이므로 그냥 생략하였다
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




//                // DAQ 12800개 데이터 생성
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
//                    // python 코드를 string 형태로 수행시키기. 들여쓰기 중요!
//                    PythonEngine.RunSimpleString(@"
//import sys; 
//print('C# python Embedded Code, Preprocessing');
//print(sys.version);
//                        ");

//                    // 파이썬 패키지 폴더의 pycode/cs1_table_stats.py 코드 불러오기. import 하는 형태로 수행됨
//                    dynamic test = Py.Import("cs1_table_stats");

//                    // 파이썬 코드의 클래스 선언 및 CNC 테이블 전달해 보기
//                    dynamic function_test = test.EmbedTest(table_CNC, 10, 50);

//                    // EmbedTest 클래스 내부 테이블 출력 함수 수행
//                    //Console.WriteLine(function_test.print_parm());
//                }
//                // python 환경을 종료함
//                PythonEngine.Shutdown();

//                Console.WriteLine("Press any key...");
//                Console.ReadKey();


//            }
//        }
//    }
//}


