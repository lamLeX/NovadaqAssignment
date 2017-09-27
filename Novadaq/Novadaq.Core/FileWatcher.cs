using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Novadaq.Core
{
    public class FileWatcher : IDisposable
    {
        private readonly string _inputFile;
        private readonly string _outputFile;

        private CancellationTokenSource _cts;

        private Task _currentTask;

        private readonly List<IObserver<string>> _observers = new List<IObserver<string>>();
        public IObservable<string> InputValue { get; }

        public string InputFolder { get; }

        public FileWatcher(string inputPath)
        {
            InputFolder = inputPath;
            _inputFile = Path.Combine(inputPath, "input");
            _outputFile = Path.Combine(inputPath, "output");

            InputValue = Observable.Create<string>(observer =>
            {
                _observers.Add(observer);
                return () => _observers.Remove(observer);
            });
        }

        public void Dispose()
        {
            _cts?.Cancel();
        }

        public void StartMonitoring()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            SignalObserver("Start observing");
            _currentTask = AccquireInput(_cts.Token);
        }

        public void StopMonitoring()
        {
            _cts?.Cancel();
            SignalObserver("Observation stopped");
        }

        private async Task AccquireInput(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (File.Exists(_inputFile))
                {
                    //if task is cancelled, don't bother to carry on
                    ct.ThrowIfCancellationRequested();
                    SignalObserver("Found input file");

                    try
                    {
                        var inputs = File.ReadAllLines(_inputFile);
                        ct.ThrowIfCancellationRequested();
                        SignalObserver("File read");

                        int currentInput = 1;
                        if (inputs.Any())
                        {
                            if (int.TryParse(inputs[0], out currentInput))
                            {
                                SignalObserver("Parse input successfully");
                            }
                        }

                        ct.ThrowIfCancellationRequested();
                        File.Delete(_inputFile);
                        SignalObserver("Input file deleted");

                        ct.ThrowIfCancellationRequested();

                        SignalObserver("Start ProceedToOutput");
                        await ProceedToOutput(ct, currentInput);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        SignalObserver("Cannot access input file");
                    }
                }
                await Task.Delay(500, ct);
            }
        }

        private async Task ProceedToOutput(CancellationToken ct, int inputValue)
        {
            SignalObserver($"Looking up Fibonacci number at {inputValue}");
            //The process of finding Fibo nummber could take very long. Therefore, we offload it to new task and await on the current task
            //so that the current task don't block the thread from thread pool
            var fibocNumber = await Task.Run(() => FibonacciFinder.GetAt(inputValue, ct), ct);

            SignalObserver($"Found Fibonacci number {fibocNumber}. Start writing to file");
            try
            {
                File.WriteAllLines(_outputFile, new[] { fibocNumber.ToString() });
                SignalObserver("Finished writing to file");

            }
            catch (UnauthorizedAccessException)
            {
                SignalObserver("Cannot access output file");
            }
        }

        private void SignalObserver(string msg)
        {
            //Signal all subscriber that there is new value
            foreach (var observer in _observers)
            {
                observer.OnNext(msg);
            }
        }
    }
}