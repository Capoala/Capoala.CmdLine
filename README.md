# Capoala.CmdLine 2018

Capoala.CmdLine is a sophisticated command line library that allows developers to quickly define, evaluate, and manage the command line. The best part? Capoala.CmdLine is built upon the .NET Standard, so enjoy the freedom of cross-platform development!

## Defining a specification. 

The `ICommandLineSpecification` interface defines the characteristics of a command line argument, such as its delimiter and hierarchal order.

The following sample demenstrates how to create a new specification, where `TierOneSpec` represents a parent, and `TierTwoSpec` a child - or decorator - to further define optional parameters.

```csharp
ICommandLineSpecification TierOneSpec = new CommandLineSpecification(0, "--");
ICommandLineSpecification TierTwoSpec = new CommandLineSpecification(1, "-");
```

## Defining an argument. 

The `ICommandLineArgument` interface defines the actual command line argument. Additionally, you can also provide a description of the intention of the command in order to auto-generate a usage doc. (Auto-generated usage doc is currently in the works, but not yet implemented).
 
The following sample demenstrates how to create a new argument using the above specifications.

```csharp
ICommandLineArgument Convert 
	= new CommandLineArgument("convert", TierOneSpec, 
		"Converts a text file into a C# StringBuilder class.");

ICommandLineArgument InFilePath 
	= new CommandLineArgument("in", TierTwoSpec, 
		"The file path to the text file to convert.");

ICommandLineArgument OutFilePath 
	= new CommandLineArgument("out", TierTwoSpec, 
		"The target file path to write the C# class file.");
```

## Defining command line groupings (parent-child relationships).

The `ICommandLineGrouping` interface provides a way to create parent-child relationships further than the initial `ICommandLineSpecification.Hierarchy` number does.

The following example demonstrates how to group the above arguments, such that `Convert` is associated with both `InFilePath` and `OutFilePath`. Note, the below example specifies that both `InFilePath` and `OutFilePath` must be present in the grouping. Supplying only `InFilePath` or `OutFilePath` results in the grouping not being found. If you would like to allow `OutFilePath` to be left off, then you would define another grouping containing only `InFilePath` for the child collection.

```csharp
ICommandLineGrouping ConvertGroup = new CommandLineGrouping(
	new ICommandLineArgument[] { Convert },
	new ICommandLineArgument[] { InFilePath, OutFilePath}
	);
```

## Defining rules and restrictions.

The `ICommandLineRestriction<T>` interface provides an easy way to create re-usable restrictions and rule-sets. Built direclty into the framework are common, default restrictions that provide a quick and clear way of defining these rules.

The following example shows how to:

1. Enforce that unknown arguments are not allowed.
2. Ensure that at least one argument be supplied.
3. Ensure that both `InFilePath` and `OutFilePath` have one paramter.

```csharp
ICommandLineRestriction<CommandLineViolation> NoUnknownArgs 
	= new CommandLineRestrictions.UnknownArgumentsRestriction(
		CommandLine.KnownArguments, 
		CommandLine.KnownSpecifications);

ICommandLineRestriction<CommandLineViolation> AtLeastOneArg 
	= new CommandLineRestrictions.MustContainAtLeastOneArgumentRestriction();

ICommandLineRestriction<CommandLineViolation> InParam 
	new CommandLineRestrictions.ParameterCountRestriction(1,1, InFilePath)

ICommandLineRestriction<CommandLineViolation> OutParam 	
	new CommandLineRestrictions.ParameterCountRestriction(1,1, OutFilePath)
    
foreach (var restriction in new ICommandLineRestriction<CommandLineViolation>[] 
{
    NoUnknownArgs,
    AtLeastOneArg,
    InParam,
    OutParam,
})
{
    if (restriction.IsViolated)
    {
        foreach (var violation in restriction.GetViolations())
        {
            Console.WriteLine($"{violation.Violation} => {violation.Message}");
        }
    }
    else
    {
        // Retrieve working file paths.		
        string inFilePath = CommandLine.GetParams(new[] { Convert, InFilePath }).First();
        string outFilePath = CommandLine.GetParams(new[] { Convert, OutFilePath }).First();
        // Evaluate and do work....
    }
}

```

The above example is a very short, concise example on how one would implement a file conversion utility. At the command line, you would see the following:

> $ FileConverter.exe --convert -in "C:\test\inFile.txt" -out "C:\test\class.cs"


## What's a "call-chain", and how do we evaluate and retreive data from the command line?

A call-chain refers to the chain of arguments that make up a parent call hierarchy. Typically, we only make use of one or two levels of arguments, such as the previous example; however, what if what we are converting is a text file into an auto-generated .cs (class) file? What if we would like to determine how that class file is built? (e.g. StringBuilder, string[], List<string>, etc.);

Well, let's create a new child specification - or grandchild - to solve this issue.

```csharp
ICommandLineSpecification TierThreeSpec = new CommandLineSpecification(2, ":");
```

Now, let's add some grandchildren.

```csharp
ICommandLineArgument OutTypeStringBuilder 
	= new CommandLineArgument("stringbuilder", TierThreeSpec, 
		"Specifies that the file should be converted into a StringBuilder.");
	
ICommandLineArgument OutTypeArray 
	= new CommandLineArgument("array", TierThreeSpec, 
		"Specifies that the file should be converted into a string array.");		
```

Now let's add additional groupings to associate our new grandchildren. And since we want to mandate that a specific file type is supplied, we'll remove the original grouping.

```csharp
ICommandLineGrouping ConvertGroupStringBuilder = new CommandLineGrouping(
	// call-chain "--convert -out" is a parent call-chain to the grandchild using TierThreeSpec.
	new ICommandLineArgument[] { Convert, OutFilePath }, 
	// This states we will only allow this grandchild by itself.
	new ICommandLineArgument[] { OutTypeStringBuilder } 
	);	
	
ICommandLineGrouping ConvertGroupArray = new CommandLineGrouping(
	// call-chain "--convert -out" is a parent call-chain to the grandchild using TierThreeSpec.
	new ICommandLineArgument[] { Convert, OutFilePath }, 
	// This states we will only allow this grandchild by itself.
	new ICommandLineArgument[] { OutTypeArray }
	);		
	
ICommandLineGrouping ConvertGroupStringBuilderAndArray = new CommandLineGrouping(
	// call-chain "--convert -out" is a parent call-chain to the grandchild using TierThreeSpec.
	new ICommandLineArgument[] { Convert, OutFilePath }, 
	// This states we will allow both grandchildren to be called together.
	new ICommandLineArgument[] { OutTypeStringBuilder, OutTypeArray } 
	);			
```

Finally, we'll create a new restriction and add the three new groupings to a `LegalArgumentsRestriction` implementation.

```csharp
ICommandLineRestriction<CommandLineViolation> Legal
	= new CommandLineRestrictions.LegalArguments(
		ConvertGroupStringBuilder, 
		ConvertGroupArray, 
		ConvertGroupStringBuilderAndArray);
```	

And now, we can do something like this!

> $ FileConverter.exe --convert -in "C:\test\inFile.txt" -out "C:\test\class.cs" :stringbuilder :array


By changing the `InFilePathParam` and `OutFilePathParam` to be associated with `OutTypeStringBuilder` and `OutTypeArray`, we could then change where the paramter data is stored.

> $ FileConverter.exe --convert -in "C:\test\inFile.txt" -out :stringbuilder "C:\test\sb.cs" :array "C:\test\array.cs"

This makes more sense, as we can specify different files for each output type instead of overwriting the file or asking the user for additional input. 

Of course, since we can accept grandchildren solo or together, we'd first check if the command is present. To do so, we simply call:

```csharp
var arrayConvertRequested 
	= new ICommandLineArgument[] { Convert, OutFilePath, OutTypeArray}.Found(CmdLineSearchOptions.None);

var stringBuilderConvertRequested 
	= new ICommandLineArgument[] { Convert, OutFilePath, OutTypeStringBuilder}.Found(CmdLineSearchOptions.None);

if (arrayConvertRequested && stringBuilderConvertRequested)	
	// Do work with both
else if (arrayConvertRequested && !stringBuilderConvertRequested)
	// Do work with just OutTypeArray	
else if (!arrayConvertRequested && stringBuilderConvertRequested)
	// Do work with just ConvertGroupStringBuilder
else
	// May the force be with you
```


## Wrapping up

The Capoala.CmdLine library provides interfaces, detailed abstract implementations, and default sealed classes to get you up going and quick.  



