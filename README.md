# Welcome to Capoala.CmdLine!

Capoala.CmdLine is a sophisticated command line library that allows developers to quickly define, evaluate, and manage... well, the command line! The best part? Capoala.CmdLine is built upon the .NET Standard, so enjoy the freedom of cross-platform development!

## Defining a specification. 

The ICommandLineSpecification interface defines the characteristics of a command line argument, such as its delimiter and hierarchal order.

The following sample demenstrates how to create a new specification, where TierOneSpec represents a parent, and TierTwoSpec a child - or decorator - to further define optional parameters.

```csharp
ICommandLineSpecification TierOneSpec = new CommandLineSpecification(0, '/');
ICommandLineSpecification TierTwoSpec = new CommandLineSpecification(1, '-');
```

## Defining an argument. 

The ICommandLineArgument interface defines the actual command line argument. Additionally, you can also provide a description of the intention of the command in order to auto-generate a usage doc.
 
The following sample demenstrates how to create a new argument using the above specifications.

```csharp
ICommandLineArgument Convert 
	= new CommandLineArgument("convert", 
	                          TierOneSpec, 
							  "Converts a text file into a C# StringBuilder class.");

ICommandLineArgument InFilePath 
	= new CommandLineArgument("in", 
	                          TierTwoSpec, 
							  "The file path to the text file to convert.");

ICommandLineArgument OutFilePath 
	= new CommandLineArgument("out", 
	                          TierTwoSpec, 
							  "The target file path to write the C# class file.");
```

## Defining command line groupings (parent-child relationships).

The ICommandLineGrouping interface provides a way to create parent-child relationships further than the initial ICommandLineSpecification.Hierarchy number does.

The following example demonstrates how to group the above arguments, such that Convert is associated with both InFilePath and OutFilePath. Note, the below example specifies that both InFilePath and OutFilePath must be present in the grouping. Supplying only InFilePath or OutFilePath results in the grouping not being found. If you would like to allow OutFilePath to be left off, then you would define another grouping containing only InFilePath for the child collection.

```csharp
ICommandLineGrouping ConvertGroup = new CommandLineGrouping(
	new ICommandLineArgument[] { Convert },
	new ICommandLineArgument[] { InFilePath, OutFilePath}
	);
```

## Defining rules and restrictions.

The ICommandLineRestriction<T> interface provides an easy way to create re-usable restrictions and rule-sets. Built direclty into the framework are common, default restrictions that provide a quick and clear way of defining these rules.

The following example shows how to:

1. Enforce that unknown arguments are not allowed.
2. Ensure that correct hierarchal order is enforced.
3. Ensure that both InFilePath and OutFilePath are used in conjuction with Convert.
4. And finally, that InFilePath and OutFilePath arguments must each contain exactly one paramter. 

```csharp
ICommandLineRestriction<CommandLineViolation> NoUnknownArgsRestriction 
	= new CommandLineRestrictions.UnknownArgumentRestriction();

ICommandLineRestriction<CommandLineViolation> EnforceHierarchy 
	= new CommandLineRestrictions.HierarchyRestriction();

ICommandLineRestriction<CommandLineViolation> InFilePathParam 
	= new CommandLineRestrictions.ParameterCountRestriction(1, 1, InFilePath);

ICommandLineRestriction<CommandLineViolation> OutFilePathParam 
	= new CommandLineRestrictions.ParameterCountRestriction(1, 1, OutFilePath);

ICommandLineRestriction<CommandLineViolation> Legal
	= new CommandLineRestrictions.LegalArguments(ConvertGroup);

foreach (var restriction in new ICommandLineRestriction<CommandLineViolation>[] 
{
  NoUnknownArgsRestriction,
  EnforceHierarchy,
  InFilePathParam,
  OutFilePathParam,
  Legal
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
	  string inFilePath = CommandLine.GetParams(Convert, InFilePath).First();
	  string outFilePath = CommandLine.GetParams(Convert, OutFilePath).First();
		
	  // Evaluate and do work....
	}
}

```

The above example is a very short, concise example on how one would implement a file conversion utility. At the command line, you would see the following:

$ FileConverter.exe /convert -in "C:\test\inFile.txt" -out "C:\test\class.cs"


## What's a "call-chain", and how do we evaluate and retrieve data from the command line?

A call-chain refers to the chain of arguments that make up a parent call hierarchy. Typically, we only make use of one or two levels of arguments, such as the previous example; however, what if what we are converting is a text file into an auto-generated .cs (class) file? What if we would like to determine how that class file is built? (e.g. StringBuilder, string[], List<string>, etc.);

Well, let's create a new child specification - or grandchild - to solve this issue.

```csharp
ICommandLineSpecification TierThreeSpec = new CommandLineSpecification(2, ':');
```

Now, let's add some grandchildren.

```csharp
ICommandLineArgument OutTypeStringBuilder 
	= new CommandLineArgument("stringbuilder", 
	                          TierThreeSpec, 
							  "Specifies that the file should be converted into a StringBuilder.");
	
ICommandLineArgument OutTypeArray 
	= new CommandLineArgument("array", 
	                          TierThreeSpec, 
							  "Specifies that the file should be converted into a string array.");		
```

Now let's add additional groupings to associate our new grandchildren. And since we want to mandate that a specific file type is supplied, we'll remove the original grouping.

```csharp
ICommandLineGrouping ConvertGroupStringBuilder = new CommandLineGrouping(
	// call-chain "/convert -out" is a parent call-chain to the grandchild using TierThreeSpec.
	new ICommandLineArgument[] { Convert, OutFilePath }, 
	// This states we will allow this grandchild by itself.
	new ICommandLineArgument[] { OutTypeStringBuilder} 
	);	
	
ICommandLineGrouping ConvertGroupArray = new CommandLineGrouping(
	// call-chain "/convert -out" is a parent call-chain to the grandchild using TierThreeSpec.
	new ICommandLineArgument[] { Convert, OutFilePath }, 
	// This states we will allow this grandchild by itself.
	new ICommandLineArgument[] { OutTypeArray} // This states we will allow this grandchild by itself.
	);		
	
ICommandLineGrouping ConvertGroupStringBuilderAndArray = new CommandLineGrouping(
	// call-chain "/convert -out" is a parent call-chain to the grandchild using TierThreeSpec.
	new ICommandLineArgument[] { Convert, OutFilePath }, 
	// This states we will allow both grandchildren to be called together.
	new ICommandLineArgument[] { OutTypeStringBuilder, OutTypeArray} 
	);			
```

Finally, we'll add the three new groupings to our Legal restriction, and remove the original grouping that we remvoed during our previous step.

```csharp
ICommandLineRestriction<CommandLineViolation> Legal
	= new CommandLineRestrictions.LegalArguments(ConvertGroupStringBuilder, 
											     ConvertGroupArray, 
												 ConvertGroupStringBuilderAndArray);
```	

And now, we can do something like this!

$ FileConverter.exe /convert -in "C:\test\inFile.txt" -out "C:\test\class.cs" :stringbuilder :array


By changing the InFilePathParam and OutFilePathParam to be associated with OutTypeStringBuilder and OutTypeArray, we could then change where the paramter data is stored.

$ FileConverter.exe /convert -in "C:\test\inFile.txt" -out :stringbuilder "C:\test\class.cs" :array "C:\test\class.cs"

Now, instead of accessing the paramter data via "string inFilePath = CommandLine.GetParams(Convert, InFilePath).First();", we now access the data via "string inFilePath = CommandLine.GetParams(Convert, InFilePath, OutTypeArray).First();"

Of course, since we can accept grandchildren solo or together, we'd first check if the command is present. Since we've already setup the groupings, we can simply call:

```csharp
if (CommandLine.Found(ConvertGroupArray))
	// Do work with just OutTypeArray
else if (CommandLine.Found(ConvertGroupStringBuilder))
	// Do work with just ConvertGroupStringBuilder
else if (CommandLine.Found(ConvertGroupStringBuilderAndArray))
	// Do work with both!
```


## Wrapping up

The Capoala.CmdLine library provides interfaces, detailed abstract implementations, and default sealed classes to get you up going and quick. The sealed classes were used throughout the entire example; however, creating your own implementation is as easy as inheriting from one of the abstract classes located in CommandLine.BaseImplementations. 

Well, that's it... Enjoy!

