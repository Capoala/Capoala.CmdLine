# Welcome to Capoala.CmdLine!

Capoala.CmdLine is a sophisticated command line library that allows developers to quickly define, evaluate, and manage... well, the command line! The best part? Capoala.CmdLine is built upon the .NET Standard, so enjoy the freedom of cross-platform development!

## Defining a specification. 

The ICommandLineSpecification interface defines the characteristics of a command line argument, such as its delimiter and hierarchal order.

The following sample demenstrates how to create a new specification, where MainSpec represents a parent, and OptionalSpec a child - or decorator - to further define optional parameters.

```csharp
ICommandLineSpecification MainSpec = new CommandLineSpecification(0, '/');
ICommandLineSpecification OptionalSpec = new CommandLineSpecification(1, '-');
```

## Defining an argument. 

The ICommandLineArgument interface defines the actual command line argument. Additionally, you can also provide a description of the intention of the command in order to auto-generate a usage doc.
 
The following sample demenstrates how to create a new argument using the above specifications.

```csharp
ICommandLineArgument Convert = new CommandLineArgument("convert", MainSpec, "Converts a test file into a C# StringBuilder class");
ICommandLineArgument InFilePath = new CommandLineArgument("in", OptionalSpec, "The file path to the text file to convert.");
ICommandLineArgument OutFilePath = new CommandLineArgument("out", OptionalSpec, "The target file path to write the C# class file.");
```

## Defining command line groupings (parent-child relationships).

The ICommandLineGrouping interface provides a way to create parent-child relationships further than the initial ICommandLineSpecification.Hierarchy number does.

The following examples demonstrates how to group the above arguments, such that Convert is associated with both InFilePath and OutFilePath.

```csharp
ICommandLineGrouping ConvertGroup = new CommandLineGrouping(
  new ICommandLineArgument[] { Convert },
  new ICommandLineArgument[] { InFilePath, OutFilePath}
  );
```

## Defining rules and restrictions.

The ICommandLineRestriction<T> interface provides an easy way to create re-usable restrictions and rule-sets. Built direclty into the framework are common, default restrictions that provide a quick and clear way of defining restrictions.

The following example shows how to:

1. Enforce that unknown arguments are not allowed.
2. Ensure that correct hierarchal order is enforced.
3. Ensure that both InFilePath and OutFilePath are used in conjuction with Convert.
4. And finally, that each optional argument must contain exactly one paramter. 

```csharp
ICommandLineRestriction<CommandLineViolation> NoUnknownArgsRestriction 
  = new CommandLineRestrictions.UnknownArgumentRestriction();

ICommandLineRestriction<CommandLineViolation> EnforceHierarchy 
  = new CommandLineRestrictions.HierarchyRestriction();

ICommandLineRestriction<CommandLineViolation> InFilePathParam 
  = new CommandLineRestrictions.ParameterCountRestriction(1, 1, InFilePath);

ICommandLineRestriction<CommandLineViolation> OutFilePathParam 
  = new CommandLineRestrictions.ParameterCountRestriction(1, 1, OutFilePath);

ICommandLineRestriction<CommandLineViolation> LegalConvert 
  = new CommandLineRestrictions.LegalArguments(ConvertGroup);

foreach (var restriction in new ICommandLineRestriction<CommandLineViolation>[] 
{
  NoUnknownArgsRestriction,
  EnforceHierarchy,
  InFilePathParam,
  OutFilePathParam,
  LegalConvert
})
{
  if (restriction.IsViolated)
  {
   foreach (var violation in restriction.GetViolations())
    {
      Console.WriteLine($"{violation.Violation} => {violation.Message}");
    }
  }
}

```

## Wrapping up

The above example is a very short, concise example on how one would implement a file conversion utility. At the command line, you would see the following:

$ FileConvert.exe /convert -in "C:\test\inFile.txt" -out "C:\test\class.cs"

The Capoala.CmdLine library provides interfaces, detailed abstract implementations, and default sealed classes to get you going up and quick. The sealed classes were used throughout the entire example; however, creating your own implementation is as easy as inheriting from one of the abstract classes located in CommandLine.BaseImplementations. 

Well, that's it! Download now and experience how easy it can be write even the most complex of command line utilities. 

P.S. More documentation and examples coming soon. Stay tuned!




