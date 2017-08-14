## Welcome to Capoala.CmdLine!

Capoala.CmdLine is a sophisticated command line utility that allows developers to quickly define, evaluate, and manage, well, the command line!

## Defining a specification. 

The ICommandLineSpecification interface defines the characteristics of a command line argument, such as its delimiter and hierarchal order.

The following sample demenstrates how to create a new specification, where SpecA is a parent-level argument, and SpecB being a child, argument - or decorator - to further define optional paramters.

```csharp
static CommandLineSpecification SpecA = new CommandLineSpecification(0, '/');
static CommandLineSpecification SpecB = new CommandLineSpecification(1, '-');
```

For more details see [GitHub Flavored Markdown](https://guides.github.com/features/mastering-markdown/).

### Jekyll Themes

Your Pages site will use the layout and styles from the Jekyll theme you have selected in your [repository settings](https://github.com/Capoala/Capoala.CmdLine/settings). The name of this theme is saved in the Jekyll `_config.yml` configuration file.

### Support or Contact

Having trouble with Pages? Check out our [documentation](https://help.github.com/categories/github-pages-basics/) or [contact support](https://github.com/contact) and we’ll help you sort it out.
