//// 파이썬 로드 + TDMS 로드 작동 테스트 only
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



//                /*-------------------------------파이썬 처리하기-----------------------------------------*/
//                // python 경로 설정 함수 호출
//                AddPythonPath();

//                // Python 엔진 초기화
//                PythonEngine.Initialize();

//                // get Global Interpreter Lock
//                using (Py.GIL())
//                {
//                    // python 코드를 string 형태로 수행시키기
//                    PythonEngine.RunSimpleString(@"
//import sys;
 
//print('C# python Embedded Code, Preprocessing');
//print(sys.version);
 
//");
//                    // 파이썬 패키지 폴더의 pycode/ premapping_run.py 실행.import 하는 형태로 수행됨
//                    dynamic test = Py.Import("premapping_run");
//                }
//                // python 환경을 종료함
//                PythonEngine.Shutdown();

//                Console.WriteLine("Press any key...");
//                Console.ReadKey();






//            }
//        }
//    }
//}



