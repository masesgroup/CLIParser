[![CI_BUILD](https://github.com/masesgroup/CLIParser/actions/workflows/build.yaml/badge.svg)](https://github.com/masesgroup/CLIParser/actions/workflows/build.yaml) [![CI_PULLREQUEST](https://github.com/masesgroup/CLIParser/actions/workflows/pullrequest.yaml/badge.svg)](https://github.com/masesgroup/CLIParser/actions/workflows/pullrequest.yaml) [![CI_RELEASE](https://github.com/masesgroup/CLIParser/actions/workflows/release.yaml/badge.svg)](https://github.com/masesgroup/CLIParser/actions/workflows/release.yaml)

# CLI Parser

A library to manage command-line arguments in a simple way. It was an internal MASES project, now it is available to anyone.

This project adheres to the Contributor [Covenant code of conduct](https://github.com/masesgroup/DataDistributionManager/blob/master/CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code. Please report unacceptable behavior to coc_reporting@masesgroup.com.

## How it works

The library is very simple in its usage. The definition of command-line switches is based on generic C# types. The parser fully analyze the command line searching for switches and arguments. A special case is the one where the swithes can be inserted within an external file listing the switches line-by-line.
To see a real application of the library look at project [JCOReflectorCLI](https://github.com/masesgroup/JCOReflector/tree/master/JCOReflector/CLI) and [JCOReflectorEngine](https://github.com/masesgroup/JCOReflector/blob/master/JCOReflector/engine/SharedClasses.cs)

### Argument definition

An argument can be defined using the following syntax sinppets:

```C#
arg = new ArgumentMetadata<bool>()
{
	Name = "test",
	ShortName = "tst",
	Help = "this is a test",
	Type = ArgumentType.Double,
	ValueType = ArgumentValueType.Free,
}

arg1 = new ArgumentMetadata<int>()
{
	Name = "range",
	Default = 9,
	Type = ArgumentType.Double,
	ValueType = ArgumentValueType.Range,
	MinValue = 2,
	MaxValue = 10,
}
```

### Parser initialization

Upon arguments are defined they can be added to the list managed from the parser using:

```C#
Parser.Add(arg);
```

or the compact version:

```C#
arg1.Add();
```

### Parser use

Then it is possible to use the parser on command-line arguments:

```C#
Parser.Parse(args);
```

or the compact version:

```C#
args.Parse();
```

### Argument check

When the `Parse` method returns, a list of prepared arguments is available. The list can be used to get value or check for existence.

