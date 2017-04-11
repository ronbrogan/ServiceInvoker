# ServiceInvoker
A command utility to watch something for changes, and perform an action when that occurs.

The intention behind this is to aid in local development of microservice-like architectures; where you have many individual services running concurrently in seperate processes.

## Plugin System
The beginnigns of a plugin system are in place. The goal is to ease adding of functionality by allowing the addition of new watch types. 
A new `Watcher` must:
 - Reference the InvokerCore assembly
 - Inherit from the `Watcher` abstract class
 - Declare a class-level `RegisterWatcher` attribute and specify the Name of the desired type there

Refer to the `DotNetCoreWatcher` project/implementation to see an example.
