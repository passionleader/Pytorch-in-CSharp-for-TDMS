//// 함수화, 파이썬 로드 최종 코드(TDMS 미적용)
//using System;
//using System.Linq;
//using Python.Runtime;
//using System.IO;

//namespace PythonExecutor
//{
//    class Program
//    {
//        // 환경설정 Path를 설정하는 함수
//        public static void AddEnvPath(params string[] paths)
//        {
//            // 시스템 환경 변수 가져오기
//            var envPaths = Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator).ToList();
//            // 중복 환경 변수가 없으면 list에 넣기
//            envPaths.InsertRange(0, paths.Where(x => x.Length > 0 && !envPaths.Contains(x)).ToArray());
//            // 환경 변수를 적용하기
//            Environment.SetEnvironmentVariable("PATH", string.Join(Path.PathSeparator.ToString(), envPaths), EnvironmentVariableTarget.Process);
//        }


//        // 파이썬 작업 환경 지정하는 함수
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

//        //메인 함수
//        static void Main(string[] args)
//        {
//            // python 경로 설정 함수 호출
//            AddPythonPath();

//            // Python 엔진 초기화
//            PythonEngine.Initialize();

//            // get Global Interpreter Lock
//            using (Py.GIL())
//            {
//                // python 코드를 string 형태로 수행시키기
//                PythonEngine.RunSimpleString(@"
//import sys;
 
//print('C# python Embedded Code, Preprocessing');
//print(sys.version);
 
//");
//                // 파이썬 패키지 폴더의 pycode/premapping_run.py 실행. import 하는 형태로 수행됨
//                dynamic test = Py.Import("premapping_run");
//            }
//            // python 환경을 종료함
//            PythonEngine.Shutdown();

//            Console.WriteLine("Press any key...");
//            Console.ReadKey();

//        }
//    }
//}

