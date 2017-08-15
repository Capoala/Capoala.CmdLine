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
ICommandLineArgument Convert 
	= new CommandLineArgument("convert", MainSpec, "Converts a text file into a C# StringBuilder class.");

ICommandLineArgument InFilePath 
	= new CommandLineArgument("in", OptionalSpec, "The file path to the text file to convert.");

ICommandLineArgument OutFilePath 
	= new CommandLineArgument("out", OptionalSpec, "The target file path to write the C# class file.");
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

A call-chain refers to the chain of arguments that make up a parent call hierarchy. Typically, we only make use of one or two levels of arguments, such as the previous examples; however, what if what we are converting is a text file into an auto-generated .cs (class) file? What if we would like to determine how that class file is built? (e.g. StringBuilder, string[], List<string>, etc.);

Well, let's create a new child specification - or grandchild - to solve this issue.

```csharp
ICommandLineSpecification FormatSpec = new CommandLineSpecification(2, ':');
```

Now, let's add some grandchildren.

```csharp
ICommandLineArgument OutTypeStringBuilder 
	= new CommandLineArgument("stringbuilder", FormatSpec, "Specifies that the file should be converted into a StringBuilder.");
	
ICommandLineArgument OutTypeArray 
	= new CommandLineArgument("array", FormatSpec, "Specifies that the file should be converted into a string array.");		
```

Now let's add additional groupings to associate our new grandchildren. And since we want to mandate that a specific file type is supplied, we'll remove the original grouping.

```csharp
ICommandLineGrouping ConvertGroupStringBuilder = new CommandLineGrouping(
	new ICommandLineArgument[] { Convert, OutFilePath }, // call-chain "/convert -out" is a parent call-chain to the grandchild using FormatSpec.
	new ICommandLineArgument[] { OutTypeStringBuilder} // This states we will allow this grandchild by itself.
	);	
	
ICommandLineGrouping ConvertGroupArray = new CommandLineGrouping(
	new ICommandLineArgument[] { Convert, OutFilePath }, // call-chain "/convert -out" is a parent call-chain to the grandchild using FormatSpec.
	new ICommandLineArgument[] { OutTypeArray} // This states we will allow this grandchild by itself.
	);		
	
ICommandLineGrouping ConvertGroupStringBuilderAndArray = new CommandLineGrouping(
	new ICommandLineArgument[] { Convert, OutFilePath }, // call-chain "/convert -out" is a parent call-chain to the grandchild using FormatSpec.
	new ICommandLineArgument[] { OutTypeStringBuilder, OutTypeArray} // This states we will allow both grandchildren to be called together.
	);			
```

Finally, we'll add the three new groupings to our Legal restriction, and remove the original grouping that we remvoed during our previous step.

```csharp
ICommandLineRestriction<CommandLineViolation> Legal
	= new CommandLineRestrictions.LegalArguments(ConvertGroupStringBuilder, ConvertGroupArray, ConvertGroupStringBuilderAndArray);
```	

And now, we can do something like this!

$ FileConverter.exe /convert -in "C:\test\inFile.txt" -out "C:\test\class.cs" :stringbuilder -out "C:\test\class.cs" :array


By changing the InFilePathParam and OutFilePathParam to be associated with OutTypeStringBuilder and OutTypeArray, we could then change where the paramter data is stored.

$ FileConverter.exe /convert -in "C:\test\inFile.txt" -out :stringbuilder "C:\test\class.cs" -out :array "C:\test\class.cs"

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

Well, that's it! Download now and experience how easy it can be to write even the most complex of command line utilities!




