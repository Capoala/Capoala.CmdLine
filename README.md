# Welcome to Capoala.CmdLine!

Capoala.CmdLine is a sophisticated command line utility that allows developers to quickly define, evaluate, and manage, well, the command line!

## Defining a specification. 

The ICommandLineSpecification interface defines the characteristics of a command line argument, such as its delimiter and hierarchal order.

The following sample demenstrates how to create a new specification, where MainSpec is a parent-level argument, and OptionalSpec being a child argument - or decorator - to further define optional paramters.

```csharp
static CommandLineSpecification MainSpec = new CommandLineSpecification(0, '/');
static CommandLineSpecification OptionalSpec = new CommandLineSpecification(1, '-');
```

## Defining an argument. 

The ICommandLineArgument interface defines the actual command line argument.

The following sample demenstrates how to create a new argument using the above specifications.

```csharp
static CommandLineArgument Convert = new CommandLineArgument("convert", MainSpec);
static CommandLineArgument InFilePath = new CommandLineArgument("in", OptionalSpec);
static CommandLineArgument OutFilePath = new CommandLineArgument("out", OptionalSpec);
```
