# DebugViewF
A self-contained, setup free debug view with colored threads, call stacks, filters, etc. 

In Desktop apps, CommonTools.InitializeDebugger() drops and launches the LogReader at current working folder. 

In UWP apps, LogReader has to be launched manually with the following command:
LogReader.exe RunAsServer [NameOfApp]
