using UnityEngine;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class Program : MonoBehaviour
{
    private const int NumberOfTestInputs1 = 1000000;
    private const int NumberOfTestInputs2 = 10000;

    private const int NumberOfTestRuns = 10;

    void Awake()
    {
        Application.runInBackground = true;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 100), "DoTest"))
        {
            DoTest();
        }
    }

    private void DoTest()
    {
        DoTest1();
        DoTest2();
        DoTest3();
        DoTest4();
        DoTest5();
        DoTest6();
    }

    private static void DoTest1()
    {
        var inputs = Enumerable.Range(1, NumberOfTestInputs1).ToList();

        DoAddTest(1, "Add " + NumberOfTestInputs1 + " value types", inputs);
    }

    private static void DoTest2()
    {
        var inputs = Enumerable.Range(1, NumberOfTestInputs1)
                        .Select(i => new Person { Name = "TestMe", Age = i })
                        .ToList();

        DoAddTest(2, "Add " + NumberOfTestInputs1 + " reference types", inputs);
    }

    private static void DoTest3()
    {
        var inputs = Enumerable.Range(1, NumberOfTestInputs2).ToList();

        // look for even numbers
        var targets = inputs.Where(n => n % 2 == 0).ToList();

        DoContainsTest(3, "Run Contains() on half of " + NumberOfTestInputs2 + " value types", inputs, targets);
    }

    private static void DoTest4()
    {
        var inputs = Enumerable.Range(1, NumberOfTestInputs2)
                        .Select(i => new Person { Name = "TestMe", Age = i }).ToList();

        // look for Person with even age
        var targets = inputs.Where(p => p.Age % 2 == 0).ToList();

        DoContainsTest(4, "Run Contains() on half of " + NumberOfTestInputs2 + " reference types", inputs, targets);
    }

    private static void DoTest5()
    {
        var inputs = Enumerable.Range(1, NumberOfTestInputs2).ToList();

        // remove even numbers
        var targets = inputs.Where(n => n % 2 == 0).ToList();

        DoRemoveTest(5, "Remove half of " + NumberOfTestInputs2 + " value types", inputs, targets);
    }

    private static void DoTest6()
    {
        var inputs = Enumerable.Range(1, NumberOfTestInputs2)
                        .Select(i => new Person { Name = "TestMe", Age = i }).ToList();

        // remove Person with even age
        var targets = inputs.Where(p => p.Age % 2 == 0).ToList();

        DoRemoveTest(6, "Remove half of " + NumberOfTestInputs2 + " reference types", inputs, targets);
    }

    /// <summary>
    /// Helper method that performs an Add test using HashSet, List and Dictionary
    /// </summary>
    private static void DoAddTest<T>(int testNumber, string description, List<T> inputs)
    {
        var hashsetResult = PerfTest.DoTest(
            inputs, () => new HashSet<T>(), (hs, i) => hs.Add(i), NumberOfTestRuns);

        var listResult = PerfTest.DoTest(
            inputs, () => new List<T>(), (l, i) => l.Add(i), NumberOfTestRuns);

        var dictResult = PerfTest.DoTest(
            inputs, () => new Dictionary<T, T>(), (dict, i) => dict.Add(i, i), NumberOfTestRuns);

        var dictResult2 = PerfTest.DoTest(
            inputs, () => new Dictionary<T, T>(), (dict, i) => dict[i] = i, NumberOfTestRuns);

        var c5hashsetResult = PerfTest.DoTest(
            inputs, () => new C5.HashSet<T>(), (hs, i) => hs.Add(i), NumberOfTestRuns);

        var c5listResult = PerfTest.DoTest(
            inputs, () => new C5.ArrayList<T>(), (l, i) => l.Add(i), NumberOfTestRuns);

        var c5dictResult = PerfTest.DoTest(
            inputs, () => new C5.HashDictionary<T, T>(), (dict, i) => dict.Add(i, i), NumberOfTestRuns);

        var c5dictResult2 = PerfTest.DoTest(
            inputs, () => new C5.HashDictionary<T, T>(), (dict, i) => dict[i] = i, NumberOfTestRuns);

        UnityEngine.Debug.Log(string.Format(@"
Test {0} ({1}) Result:
------------------------------------------------
HashSet.Add : {2}
List.Add : {3}
Dictionary.Add : {4}
Dictionary[n] = n : {5}
C5.HashSet.Add : {6}
C5.ArrayList.Add : {7}
C5.HashDictionary.Add : {8}
C5.HashDictionary[n] = n : {9}
------------------------------------------------",
            testNumber,
            description,
            hashsetResult,
            listResult,
            dictResult,
            dictResult2,
            c5hashsetResult,
            c5listResult,
            c5dictResult,
            c5dictResult2
            )
            );
    }

    /// <summary>
    /// Helper method that performs a Contains test using HashSet, List and Dictionary
    /// </summary>
    private static void DoContainsTest<T>(
        int testNumber, string description, List<T> inputs, List<T> targets)
    {
        var hashsetResult = PerfTest.DoTest(
            targets, () => new HashSet<T>(inputs), (hs, t) => hs.Contains(t), NumberOfTestRuns);

        var listResult = PerfTest.DoTest(
            targets, () => new List<T>(inputs), (l, t) => l.Contains(t), NumberOfTestRuns);

        var dictResult = PerfTest.DoTest(
            targets, () => inputs.ToDictionary(i => i, i => i), (dict, t) => dict.ContainsKey(t), NumberOfTestRuns);

        var dictResult2 = PerfTest.DoTest(
            targets, () => inputs.ToDictionary(i => i, i => i), (dict, t) => dict.ContainsValue(t), NumberOfTestRuns);

        var c5hashsetResult = PerfTest.DoTest(
            targets, () => new C5.HashSet<T>(), (hs, li) => hs.AddAll(li), (hs, t) => hs.Contains(t), NumberOfTestRuns);

        var c5listResult = PerfTest.DoTest(
            targets, () => new C5.ArrayList<T>(), (l, li) => l.AddAll(li), (l, t) => l.Contains(t), NumberOfTestRuns);

        var c5dictResult = PerfTest.DoTest(
            targets, () => new C5.HashDictionary<T, T>(),
            (dict, li) =>
            {
                List<C5.KeyValuePair<T, T>> lk = new List<C5.KeyValuePair<T, T>>();
                foreach (T i in li) lk.Add(new C5.KeyValuePair<T, T>(i));

                dict.AddAll<T, T>(lk);
            },
            (dict, t) => dict.Contains(t), NumberOfTestRuns);

        UnityEngine.Debug.Log(string.Format(@"
Test {0} ({1}) Result:
------------------------------------------------
HashSet.Contains : {2}
List.Contains : {3}
Dictionary.ContainsKey : {4}
Dictionary.ContainsValue : {5}
C5.HashSet.Contains : {6}
C5.ArrayList.Contains : {7}
C5.HashDictionary.ContainsKey : {8}
C5.HashDictionary.ContainsValue : x
------------------------------------------------",
            testNumber,
            description,
            hashsetResult,
            listResult,
            dictResult,
            dictResult2,
            c5hashsetResult,
            c5listResult,
            c5dictResult
            )
            );
    }

    private static void DoRemoveTest<T>(
        int testNumber, string description, List<T> inputs, List<T> targets)
    {
        var hashsetResult = PerfTest.DoTest(
            targets, () => new HashSet<T>(inputs), (hs, t) => hs.Remove(t), NumberOfTestRuns);

        var listResult = PerfTest.DoTest(
            targets, () => new List<T>(inputs), (l, t) => l.Remove(t), NumberOfTestRuns);

        var dictResult = PerfTest.DoTest(
            targets, () => inputs.ToDictionary(i => i, i => i), (dict, t) => dict.Remove(t), NumberOfTestRuns);

        var c5hashsetResult = PerfTest.DoTest(
            targets, () => new C5.HashSet<T>(), (hs, li) => hs.AddAll(li), (hs, t) => hs.Remove(t), NumberOfTestRuns);

        var c5listResult = PerfTest.DoTest(
            targets, () => new C5.ArrayList<T>(), (l, li) => l.AddAll(li), (l, t) => l.Remove(t), NumberOfTestRuns);

        var c5dictResult = PerfTest.DoTest(
            targets, () => new C5.HashDictionary<T, T>(),
            (dict, li) =>
            {
                List<C5.KeyValuePair<T, T>> lk = new List<C5.KeyValuePair<T, T>>();
                foreach (T i in li) lk.Add(new C5.KeyValuePair<T, T>(i));

                dict.AddAll<T, T>(lk);
            }, (dict, t) => dict.Remove(t), NumberOfTestRuns);

        UnityEngine.Debug.Log(string.Format(@"
Test {0} ({1}) Result:
------------------------------------------------
HashSet.Remove : {2}
List.Remove : {3}
Dictionary.Remove : {4}
C5.HashSet.Remove : {5}
C5.ArrayList.Remove : {6}
C5.HashDictionary.Remove : {7}
------------------------------------------------",
            testNumber,
            description,
            hashsetResult,
            listResult,
            dictResult,
            c5hashsetResult,
            c5listResult,
            c5dictResult
            )
            );
    }

    /// <summary>
    /// A static class for executing the performance tests
    /// </summary>
    public static class PerfTest
    {
        public static long DoTest<TCol, TInput>(
            List<TInput> inputs,             // the inputs for the test
            Func<TCol> initCollection,      // initialize a new collection for the test
            Action<TCol, TInput> action,    // the action to perform against the input
            int numberOfRuns)               // how many times do we need to repeat the test?
            where TCol : class
        {
            long totalTime = 0;
            var stopwatch = new Stopwatch();

            for (var i = 0; i < numberOfRuns; i++)
            {
                // get a new collection for this test run
                var collection = initCollection();

                // start the clock and execute the test
                stopwatch.Start();
                inputs.ForEach(n => action(collection, n));
                stopwatch.Stop();

                // add to the total time
                totalTime += stopwatch.ElapsedMilliseconds;

                // reset the stopwatch for the next run
                stopwatch.Reset();
            }

            var avgTime = totalTime / numberOfRuns;

            return avgTime;
        }

        public static long DoTest<TCol, TInput>(
            List<TInput> inputs,             // the inputs for the test
            Func<TCol> initCollection,      // initialize a new collection for the test
            Action<TCol, List<TInput>> addAction,
            Action<TCol, TInput> action,    // the action to perform against the input
            int numberOfRuns)               // how many times do we need to repeat the test?
            where TCol : class
        {
            long totalTime = 0;
            var stopwatch = new Stopwatch();

            for (var i = 0; i < numberOfRuns; i++)
            {
                // get a new collection for this test run
                var collection = initCollection();
                addAction(collection, inputs);

                // start the clock and execute the test
                stopwatch.Start();
                inputs.ForEach(n => action(collection, n));
                stopwatch.Stop();

                // add to the total time
                totalTime += stopwatch.ElapsedMilliseconds;

                // reset the stopwatch for the next run
                stopwatch.Reset();
            }

            var avgTime = totalTime / numberOfRuns;

            return avgTime;
        }

        //public static long DoTestC5HashDictionary<TInput>(
        //    List<TInput> inputs,             // the inputs for the test
        //    Action<C5.HashDictionary<TInput, TInput>, TInput> action,    // the action to perform against the input
        //    int numberOfRuns)               // how many times do we need to repeat the test?
        //{
        //    long totalTime = 0;
        //    var stopwatch = new Stopwatch();

        //    for (var i = 0; i < numberOfRuns; i++)
        //    {
        //        // get a new collection for this test run
        //        var collection = new C5.HashDictionary<TInput, TInput>();

        //        // start the clock and execute the test
        //        stopwatch.Start();
        //        inputs.ForEach(n => action(collection, n));
        //        stopwatch.Stop();

        //        // add to the total time
        //        totalTime += stopwatch.ElapsedMilliseconds;

        //        // reset the stopwatch for the next run
        //        stopwatch.Reset();
        //    }

        //    var avgTime = totalTime / numberOfRuns;

        //    return avgTime;
        //}
    }

    public class Person
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }
}
