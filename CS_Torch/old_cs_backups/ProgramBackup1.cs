/* 참고 사이트
 * used python 3.8 and pip install pythonnet
 * https://noteforstudy.tistory.com/20
 * https://github.com/pythonnet/pythonnet/blob/master/README.rst#embedding-python-in-net
 * http://pythonnet.github.io/
 * 1. need to add python.runtime.dll to reference(it will be created after pip install pythinnet)
 * 2. need to copy and paste python38.dll to your debug / release folder of project
*/


//// 가장 기본적인 사용
//using System;
//using System.Collections.Generic;
//using Python.Runtime;

//namespace ConsoleApp1
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            using (Py.GIL())
//            {
//                dynamic np = Py.Import("numpy");
//                Console.WriteLine(np.cos(np.pi * 2));

//                dynamic sin = np.sin;
//                Console.WriteLine(sin(5));

//                double c = np.cos(5) + sin(5);
//                Console.WriteLine(c);

//                dynamic a = np.array(new List<float> { 1, 2, 3 });
//                Console.WriteLine(a.dtype);

//                dynamic b = np.array(new List<float> { 6, 5, 4 }, dtype: np.int32);
//                Console.WriteLine(b.dtype);

//                Console.WriteLine(a * b);
//                Console.ReadKey();
//            }
//        }
//    }

//}


////블로그 원본, https://nowonbun.tistory.com/690
//using System;
//using System.Linq;
//using Python.Runtime;
//using System.IO;

//namespace PythonExecutor
//{
//    class Program
//    {
//        // 환경설정 Path를 설정하는 함수이다. 실제 Path가 바뀌는 건 아니고 프로그램 세션 안에서만 path를 변경해서 사용한다.
//        public static void AddEnvPath(params string[] paths)
//        {
//            // PC에 설정되어 있는 환경 변수를 가져온다.
//            var envPaths = Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator).ToList();
//            // 중복 환경 변수가 없으면 list에 넣는다.
//            envPaths.InsertRange(0, paths.Where(x => x.Length > 0 && !envPaths.Contains(x)).ToArray());
//            // 환경 변수를 다시 설정한다.
//            Environment.SetEnvironmentVariable("PATH", string.Join(Path.PathSeparator.ToString(), envPaths), EnvironmentVariableTarget.Process);
//        }
//        // 시작 함수입니다.
//        static void Main(string[] args)
//        {
//            // 아까 where python으로 나온 anaconda 설치 경로를 설정
//            var PYTHON_HOME = Environment.ExpandEnvironmentVariables(@"D:\anaconda3-32\");
//            // 환경 변수 설정
//            AddEnvPath(PYTHON_HOME, Path.Combine(PYTHON_HOME, @"Library\bin"));
//            // Python 홈 설정.
//            PythonEngine.PythonHome = PYTHON_HOME;
//            // 모듈 패키지 패스 설정.
//            PythonEngine.PythonPath = string.Join(
//              Path.PathSeparator.ToString(),
//              new string[] {
//          PythonEngine.PythonPath,
//          // pip하면 설치되는 패키지 폴더.
//          Path.Combine(PYTHON_HOME, @"Lib\site-packages"),
//          // 개인 패키지 폴더
//          "d:\\Python\\MyLib"
//              }
//            );
//            // Python 엔진 초기화
//            PythonEngine.Initialize();
//            // Global Interpreter Lock을 취득
//            using (Py.GIL())
//            {
//                // String 식으로 python 식을 작성, 실행
//                PythonEngine.RunSimpleString(@"
//import sys;

//print('hello world');
//print(sys.version);

//");
//                // 개인 패키지 폴더의 example/test.py를 읽어드린다.
//                dynamic test = Py.Import("example.test");

//                // example/test.py의 Calculator 클래스를 선언
//                dynamic f = test.Calculator(1, 2);
//                // Calculator의 add함수를 호출
//                Console.WriteLine(f.add());
//            }
//            // python 환경을 종료한다.
//            PythonEngine.Shutdown();

//            Console.WriteLine("Press any key...");
//            Console.ReadKey();

//        }
//    }
//}



//개인 경로에 맞도록 설정
using System;
using System.Linq;
using Python.Runtime;
using System.IO;

namespace PythonExecutor
{
    class Program
    {
        // 환경설정 Path를 설정하는 함수
        public static void AddEnvPath(params string[] paths)
        {
            // 시스템 환경 변수 가져오기
            var envPaths = Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator).ToList();
            // 중복 환경 변수가 없으면 list에 넣기
            envPaths.InsertRange(0, paths.Where(x => x.Length > 0 && !envPaths.Contains(x)).ToArray());
            // 환경 변수를 적용하기
            Environment.SetEnvironmentVariable("PATH", string.Join(Path.PathSeparator.ToString(), envPaths), EnvironmentVariableTarget.Process);
        }


        //메인 함수
        static void Main(string[] args)
        {
            // 파이썬이 설치된 폴더 지정
            var PYTHON_HOME = Environment.ExpandEnvironmentVariables(@"C:\Program Files\Python38\");
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
            // Python 엔진 초기화
            PythonEngine.Initialize();
            // Global Interpreter LocK
            using (Py.GIL())
            {
                // python 코드를 string 형태로 수행시키기
                PythonEngine.RunSimpleString(@"
import sys;
 
print('C# python Embedded Code');
print(sys.version);
 
");
                // 파이썬 패키지 폴더의 pycode/premapping_run.py 실행. import 하는 형태로 수행됨
                dynamic test = Py.Import("premapping_run");
            }
            // python 환경을 종료함
            PythonEngine.Shutdown();

            Console.WriteLine("Press any key...");
            Console.ReadKey();

        }
    }
}