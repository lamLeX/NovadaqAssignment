## Project structure
The logic for Fibonacci and file IO is separated into a shared library targeting NetStandard 2.0 to maximize the potability. 
By doing this the Novadaq.Core could be reused by any other platform such as Xamarin, ASP.NET. 
However, to be able to use it portably on other platform, the FileIO need to be abstracted. This feature is out of scope for this assignment.

* **Novadaq.Core** : Shared code library targeting NetStandard 2.0
  * This project depends on [System.Reative](https://www.nuget.org/packages/System.Reactive/). 
It will be restored automatically by Nuget Package Manager if the project is built by Visual Studio
  * This project contains:
    * `FibonacciFinder`: a static class provides a recursive method to find Fibonacci number. 
    This method is not optimized in order to demonstrate how the threading management done in FileWatcher.
    Inded, the ammount of time it takes to calculate Fibonacci(50) is 8 minutes as in this picture
    
        ![Benchmark picture](Timing.PNG "Benchmark picture")
    * `FileWatcher`: The file watcher represents a session with a given observed folder.
    Whenever a new folder is given, a new instance of FileWatcher should be created.
    This file watcher uses [Task Parallel Libary (TPL)](https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-parallel-library-tpl)
    to schedule tasks that will be excuted by the default task scheduler (ThreadPool).
    The task will be interupted using CancellationTokenSource which is explained in [Cancellation in Managed Threads](https://docs.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads).
    When the user asks to observe the folder by calling `StartMonitoring()` on an instance of `FileWatcher`, a new task will be spawn and run in the `while` loop to look for a file named `input` in the given folder.
    If the user decides to terminate the task by either calling `StopMonitoring()` or disposing the instance of `FileWatcher`, the cancellation token will be propagated correctly to cancel all outstanding tasks.
    By using TPL, the input observation runs on a seperate thread and doesn't block the main thread at all. 
    Even when the Fibonacci is calculated, the current task will spawn the new task, schedule a callback by using `await` keyword and return the current thread to threadpool.
    The whole behavior could be observed using breakpoint at certain stages; when a breakpoint is hit, use `Parallel Stacks` and `Tasks` windows under `Debug/Windows` menu.
        
        ![Parallel Stacks](ParallelStacks.PNG "Parallel Stacks")

    The `FileWatcher` uses [ReactiveX](https://github.com/Reactive-Extensions/Rx.NET) to provide a fluent IObservable to signal any event happens in the process. 
More on this will be explained in the Novadaq.UI.


   
* **Novadaq.Core.Test** : xUnit test project for testing Novadaq.Core.
* **Novadaq.UI** : UI application using WPF targeting .Net Framework 4.7.
  * The UI thread is not blocked by the `FileWatcher` and `FibonacciFinder`.
    Indeed, the UI thread is free to do any operation even cancelling the running task for file observation and Fibonacci finding.
    The UI thread subscribes to the IObservable declared in `FileWatcher` to listen to any events signaled by the watcher.
    This approach is similiar to the .NET event delegate. However, it is more preferable because removing event delegate requires manually removing the delegate.
    With IObservable, we only need to call `Dispose` on subscription object. Also, it will allow many more complex logic applied on the event stream such as filtering, throttling, delaying, etc.
    Finally, the state of Start/Stop is managed manually using the `_isFileWatcherRunning`. This could be improved by implementing proper lifecycle manager for the file watcher instance.

## Requirement
* Visual Studio 2017 version 15.3. Download the free community version at [https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community&rel=15](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community&rel=15)
Make sure the `.Net Desktop Development` and `.Net Core Cross-platform development` workload is installed.
Also choose `.NET Framework 4.7 SDK` and `.NET Framework 4.7 targeting pack` in `Individual components` to be able to build the WPF project. 

## Time reports
|Task               |Time
|-------------------|----------|
|Designing code     | 30 minutes
|Implement code     | 1.5 hours
|Debuging and test  | 1.5 hours
|Documentation      | 1 hours
