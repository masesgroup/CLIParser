/*
 *  MIT License
 *
 *  Copyright (c) 2024 MASES s.r.l.
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in all
 *  copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *  SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASES.CLIParser
{
    /// <summary>
    /// Helper methods for the <see cref="Parser"/> class
    /// </summary>
    public static class ParserExtension
    {
        /// <summary>
        /// Convert an <see cref="ArgumentPrefix"/> to the equivalent <see cref="string"/> representation
        /// </summary>
        /// <param name="prefix">The <see cref="ArgumentPrefix"/> to convert in string</param>
        /// <param name="customPrefix">The custom prefix associated to <paramref name="prefix"/> with a value of <see cref="ArgumentPrefix.Custom"/></param>
        public static string Prefix(this ArgumentPrefix prefix, string customPrefix = null)
        {
            switch (prefix)
            {
                case ArgumentPrefix.Dash:
                    return InternalConst.ArgumentPrefix.Dash;
                case ArgumentPrefix.DoubleDash:
                    return InternalConst.ArgumentPrefix.DoubleDash;
                case ArgumentPrefix.Slash:
                    return InternalConst.ArgumentPrefix.Slash;
                case ArgumentPrefix.Custom:
                    return customPrefix;
                case ArgumentPrefix.None:
                default:
                    return string.Empty;
            }
        }
        /// <summary>
        /// Creates the command line switch associated to the switch with <paramref name="name"/> and <paramref name="value"/>
        /// </summary>
        /// <param name="metadatas">An ensemble of <see cref="IArgumentMetadata"/> with all possible arguments</param>
        /// <param name="name">The command-line switch name</param>
        /// <param name="values">The value(s) to use. Multiple value can be used in case of argument reports <see cref="IArgumentMetadata.IsMultiValue"/> as <see langword="true"/></param>
        /// <returns>The string equivalent for command-line</returns>
        public static string ToString(this IEnumerable<IArgumentMetadata> metadatas, string name, params object[] values)
        {
            var cmdArg = metadatas.Get(name);
            return cmdArg.ToString(values);
        }

        /// <summary>
        /// Creates the command line switch associated to the <paramref name="metadata"/> and <paramref name="values"/>
        /// </summary>
        /// <param name="metadata">The <see cref="IArgumentMetadata"/> switch</param>
        /// <param name="values">The value(s) to use. Multiple value can be used in case of argument reports <see cref="IArgumentMetadata.IsMultiValue"/> to be <see langword="true"/></param>
        /// <returns>The string equivalent for command-line</returns>
        public static string ToString(this IArgumentMetadata metadata, params object[] values)
        {
            string valueStr = string.Empty;
            if (metadata.Type != ArgumentType.Single)
            {
                if (values == null || values.Length == 0) throw new ArgumentException("Cannot be null or empty", "values");
                if (metadata.IsMultiValue)
                {
                    foreach (var item in values)
                    {
                        valueStr += $"{item}{metadata.MultiValueSeparator}";
                    }
                    valueStr = valueStr.Substring(0, valueStr.Length - 1);
                }
                else
                {
                    valueStr = values[0].ToString();
                }
            }

            var prefix = metadata.PrefixInUse;

            switch (metadata.Type)
            {
                case ArgumentType.Single: return $"{prefix}{metadata.Name}";
                case ArgumentType.Double: return $"{prefix}{metadata.Name} {valueStr}";
                case ArgumentType.KeyValue: return $"{prefix}{metadata.Name}{metadata.KeyValuePairSeparator}{valueStr}";
                default: throw new ArgumentException($"Parameter {metadata.Name} does not have a correct Type: {metadata.Type}");
            }
        }

        /// <summary>
        /// Adds an <see cref="IArgumentMetadata"/>
        /// </summary>
        /// <param name="metadatas">The <see cref="IArgumentMetadata"/> to add</param>
        public static void Add(this IArgumentMetadata metadata)
        {
            IArgumentMetadataHelper helper = metadata as IArgumentMetadataHelper;
            if (helper.Parser == null) throw new ArgumentException($"Parameter {metadata.Name} does not have any associated Parser");
            helper.Parser.Add(metadata);
        }
        /// <summary>
        /// Adds a collection of <see cref="IArgumentMetadata"/>
        /// </summary>
        /// <param name="metadatas">The collection of <see cref="IArgumentMetadata"/> to add</param>
        public static void Add(this IEnumerable<IArgumentMetadata> metadatas)
        {
            foreach (var item in metadatas)
            {
                Add(item);
            }
        }
        /// <summary>
        /// Return the <see cref="IArgumentMetadata"/> at <paramref name="index"/>
        /// </summary>
        /// <param name="args">An ensemble of <see cref="IArgumentMetadata"/> to parse</param>
        /// <param name="index">Index to get</param>
        /// <returns>The selected <see cref="IArgumentMetadata"/></returns>
        public static IArgumentMetadata Get(this IEnumerable<IArgumentMetadata> args, int index)
        {
            return new List<IArgumentMetadata>(args)[index];
        }

        /// <summary>
        /// Return the <see cref="IArgumentMetadata"/> at <paramref name="name"/>
        /// </summary>
        /// <param name="args">An ensemble of <see cref="IArgumentMetadata"/> to parse</param>
        /// <param name="name">The argument name, or short name, to get</param>
        /// <returns>The selected <see cref="IArgumentMetadata"/></returns>
        public static IArgumentMetadata Get(this IEnumerable<IArgumentMetadata> args, string name)
        {
            foreach (var item in new List<IArgumentMetadata>(args))
            {
                if (item.Name == name || item.ShortName == name) return item;
            }
            throw new ArgumentException("name is not a valid argument.");
        }
        /// <summary>
        /// Return the <see cref="IArgumentMetadataParsed"/> at <paramref name="index"/>
        /// </summary>
        /// <param name="args">An ensemble of <see cref="IArgumentMetadataParsed"/> to parse</param>
        /// <param name="index">Index to get</param>
        /// <returns>The selected <see cref="IArgumentMetadataParsed"/></returns>
        public static IArgumentMetadataParsed Get(this IEnumerable<IArgumentMetadataParsed> args, int index)
        {
            return new List<IArgumentMetadataParsed>(args)[index];
        }

        /// <summary>
        /// Return the <see cref="IArgumentMetadataParsed"/> at <paramref name="name"/>
        /// </summary>
        /// <param name="args">An ensemble of <see cref="IArgumentMetadataParsed"/> to parse</param>
        /// <param name="name">The argument name, or short name, to get</param>
        /// <returns>The selected <see cref="IArgumentMetadataParsed"/></returns>
        public static IArgumentMetadataParsed Get(this IEnumerable<IArgumentMetadataParsed> args, string name)
        {
            foreach (var item in new List<IArgumentMetadataParsed>(args))
            {
                if (item.Name == name || item.ShortName == name) return item;
            }
            throw new ArgumentException("name is not a valid argument.");
        }

        /// <summary>
        /// Return the value of the <see cref="IArgumentMetadataParsed"/> ensemble at <paramref name="name"/>
        /// </summary>
        /// <param name="args">An ensemble of <see cref="IArgumentMetadataParsed"/> to parse</param>
        /// <param name="name">The argument name, or short name, to get</param>
        /// <returns>The value from <see cref="IArgumentMetadataParsed"/></returns>
        public static T Get<T>(this IEnumerable<IArgumentMetadataParsed> args, string name)
        {
            foreach (var item in new List<IArgumentMetadataParsed>(args))
            {
                if (item.Name == name || item.ShortName == name) return Get<T>(item);
            }
            throw new ArgumentException("name is not a valid argument.");
        }

        /// <summary>
        /// Return the value of the <see cref="IArgumentMetadataParsed"/> ensemble at <paramref name="index"/>
        /// </summary>
        /// <param name="args">An ensemble of <see cref="IArgumentMetadataParsed"/> to parse</param>
        /// <param name="index">Index to get</param>
        /// <returns>The value from <see cref="IArgumentMetadataParsed"/></returns>
        public static T Get<T>(this IEnumerable<IArgumentMetadataParsed> args, int index)
        {
            return Get(args, index).Get<T>();
        }

        /// <summary>
        /// Return the value from <see cref="IArgumentMetadataParsed"/> or the default value
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to use to retrieve value</typeparam>
        /// <param name="arg">The <see cref="IArgumentMetadataParsed"/> to use to get value</param>
        /// <returns>The value from <see cref="IArgumentMetadataParsed"/> or <see cref="IArgumentMetadata.Default"/> if value was not set</returns>
        public static T Get<T>(this IArgumentMetadataParsed arg)
        {
            if (arg == null) throw new ArgumentNullException("arg cannot be null.");
            if (!typeof(T).IsAssignableFrom(arg.DataType)) throw new ArgumentException($"{typeof(T)} is incomplatible wirh {arg.DataType}.");
            if (arg.Value != null)
            {
                return (T)arg.Value;
            }
            return (T)arg.Default;
        }

        /// <summary>
        /// Return the <see cref="IArgumentMetadataParsed"/> without any accurrence of file argument (i.e. <see cref="IArgumentMetadataParsed.IsFile"/> is true)
        /// </summary>
        /// <param name="args">An ensemble of <see cref="IArgumentMetadataParsed"/> to check</param>
        /// <returns>A list of <see cref="IArgumentMetadataParsed"/></returns>
        public static IEnumerable<IArgumentMetadataParsed> RemoveFile(this IEnumerable<IArgumentMetadataParsed> args)
        {
            List<IArgumentMetadataParsed> parsedArgs = new List<IArgumentMetadataParsed>(args);

            foreach (var item in parsedArgs.ToArray())
            {
                if (item.IsFile) parsedArgs.Remove(item);
            }
            return parsedArgs;
        }

        /// <summary>
        /// Overrides values in <paramref name="destination"/> with values found in <paramref name="source"/>
        /// </summary>
        /// <param name="destination">An ensemble of <see cref="IArgumentMetadataParsed"/></param>
        /// <param name="source">An ensemble of <see cref="IArgumentMetadataParsed"/></param>
        /// <param name="rawReplace">Replace destionation without check if argument exist in the source</param>
        /// <returns>The updated <paramref name="destination"/></returns>
        public static IEnumerable<IArgumentMetadataParsed> Override(this IEnumerable<IArgumentMetadataParsed> destination, IEnumerable<IArgumentMetadataParsed> source, bool rawReplace = false)
        {
            List<IArgumentMetadataParsed> newSrc = new List<IArgumentMetadataParsed>(source);
            foreach (var item in destination)
            {
                foreach (var item2 in newSrc.ToArray())
                {
                    if (rawReplace || item2.Exist && item.Override(item2))
                    {
                        newSrc.Remove(item2);
                        break;
                    }
                }
            }
            return destination;
        }

        /// <summary>
        /// Filter the <paramref name="args"/> for existing <see cref="IArgumentMetadataParsed"/>
        /// </summary>
        /// <param name="args">Arguments to test using the list prepared using <see cref="Parse"/></param>
        /// <returns>A filtered list of <see cref="IArgumentMetadataParsed"/></returns>
        public static IEnumerable<IArgumentMetadataParsed> Exists(this IEnumerable<IArgumentMetadataParsed> args)
        {
            List<IArgumentMetadataParsed> existArgs = new List<IArgumentMetadataParsed>();
            foreach (IArgumentMetadataParsed item in args)
            {
                if (item.Exist)
                {
                    existArgs.Add(item);
                }
            }

            return existArgs;
        }

        /// <summary>
        /// Check the <paramref name="args"/> for existing <paramref name="name"/>
        /// </summary>
        /// <param name="args">Arguments to test using the list prepared using <see cref="Parse"/></param>
        /// <param name="name">Argument name to search</param>
        /// <returns>true if the the argument with <paramref name="name"/> name exist</returns>
        public static bool Exist(this IEnumerable<IArgumentMetadataParsed> args, string name)
        {
            foreach (var item in Exists(args))
            {
                if (item.Name == name || item.ShortName == name) return true;
            }

            return false;
        }

        /// <summary>
        /// Filter the <paramref name="args"/> for non existing <see cref="IArgumentMetadataParsed"/>
        /// </summary>
        /// <param name="args">Arguments to test using the list prepared using <see cref="Parse"/></param>
        /// <returns>A filtered list of <see cref="IArgumentMetadataParsed"/></returns>
        public static IEnumerable<IArgumentMetadataParsed> NotExists(this IEnumerable<IArgumentMetadataParsed> args)
        {
            List<IArgumentMetadataParsed> existArgs = new List<IArgumentMetadataParsed>();
            foreach (IArgumentMetadataParsed item in args)
            {
                if (!item.Exist)
                {
                    existArgs.Add(item);
                }
            }

            return existArgs;
        }
    }

    /// <summary>
    /// The class managing the settings of <see cref="Parser"/>
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Initializa a new instance of <see cref="Settings"/>
        /// </summary>
        public Settings()
        {
            DefaultConsoleWindowWidth = 80;
            DefaultFileNameIdentifier = InternalConst.DefaultFileNameIdentifier;
            DefaultPrefix = ArgumentPrefix.Dash;
            DefaultCustomPrefix = string.Empty;
            DefaultType = ArgumentType.Single;
            DefaultValueType = ArgumentValueType.Free;
            DefaultMultiValueSeparator = InternalConst.DefaultMultiValueSeparator;
            DefaultKeyValuePairSeparator = InternalConst.DefaultKeyValuePairSeparator;
            DefaultIsCaseInvariant = true;
            DefaultDescriptionPadding = 30;
        }
        /// <summary>
        /// The string representing the actual prefix from configuration
        /// </summary>
        public string PrefixInUse { get { return DefaultPrefix.Prefix(DefaultCustomPrefix); } }
        /// <summary>
        /// Default value to be used when a real console device is not available. Default is 80
        /// </summary>
        public int DefaultConsoleWindowWidth { get; set; }
        /// <summary>
        /// Default value of identifier used when an argument represent a file containing the arguments
        /// </summary>
        public char DefaultFileNameIdentifier { get; set; }
        /// <summary>
        /// Default value of <see cref="IArgumentMetadata.Prefix"/> used when a new instance of <see cref="ArgumentMetadataBase"/> is created
        /// </summary>
        public ArgumentPrefix DefaultPrefix { get; set; }
        /// <summary>
        /// Default value of <see cref="IArgumentMetadata.CustomPrefix"/> used when a new instance of <see cref="ArgumentMetadataBase"/> is created
        /// </summary>
        public string DefaultCustomPrefix { get; set; }
        /// <summary>
        /// Default value of <see cref="IArgumentMetadata.Type"/> used when a new instance of <see cref="ArgumentMetadataBase"/> is created
        /// </summary>
        public ArgumentType DefaultType { get; set; }
        /// <summary>
        /// Default value of <see cref="IArgumentMetadata.ValueType"/> used when a new instance of <see cref="ArgumentMetadataBase"/> is created
        /// </summary>
        public ArgumentValueType DefaultValueType { get; set; }
        /// <summary>
        /// Default value of <see cref="IArgumentMetadata.MultiValueSeparator"/> used when a new instance of <see cref="ArgumentMetadataBase"/> is created
        /// </summary>
        public char DefaultMultiValueSeparator { get; set; }
        /// <summary>
        /// Default value of <see cref="IArgumentMetadata.KeyValuePairSeparator"/> used when a new instance of <see cref="ArgumentMetadataBase"/> is created
        /// </summary>
        public string DefaultKeyValuePairSeparator { get; set; }
        /// <summary>
        /// Default value of <see cref="IArgumentMetadata.IsCaseInvariant"/> used when a new instance of <see cref="ArgumentMetadataBase"/> is created
        /// </summary>
        public bool DefaultIsCaseInvariant { get; set; }
        /// <summary>
        /// Default value to use on padding the help information
        /// </summary>
        public int DefaultDescriptionPadding { get; set; }
        /// <summary>
        /// True to test if there are unwanted switches
        /// </summary>
        public bool CheckUnwanted { get; set; }
    }

    /// <summary>
    /// Public entry point for the parser
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// Creates a new instance of <see cref="Parser"/>
        /// </summary>
        /// <param name="settings">The <see cref="Settings"/> to use or the default</param>
        /// <returns>The newly created instance</returns>
        public static Parser CreateInstance(Settings settings = null)
        {
            return new Parser(settings);
        }

        readonly IDictionary<string, IArgumentMetadata> arguments;

        Parser(Settings settings)
        {
            if (settings == null) settings = new Settings();
            Settings = settings;
            arguments = new Dictionary<string, IArgumentMetadata>();
        }
        /// <summary>
        /// The settings in use
        /// </summary>
        public Settings Settings { get; private set; }
        /// <summary>
        /// The arguments in command-line not parsed, i.e. the possible extra arguments to be used from the application for other scopes
        /// </summary>
        public string[] UnparsedArgs { get; private set; }
        /// <summary>
        /// Available <see cref="IArgumentMetadata"/> for parsing
        /// </summary>
        public IReadOnlyList<IArgumentMetadata> Arguments { get { return new List<IArgumentMetadata>(arguments.Values); } }
        /// <summary>
        /// Adds <paramref name="metadata"/> to the argument to be parsed from <see cref="Parse(string[])"/>
        /// </summary>
        /// <param name="metadata"><see cref="IArgumentMetadata"/> to be added</param>
        public void Add(IArgumentMetadata metadata)
        {
            if (metadata == null) throw new ArgumentNullException("metadata cannot be null.");
            if (string.IsNullOrEmpty(metadata.Name)) throw new ArgumentException("Parameter Name shall be set.");
            if (arguments.ContainsKey(metadata.Name)) throw new ArgumentException($"Parameter {metadata.Name} is duplicated");
            IArgumentMetadataHelper helper = metadata as IArgumentMetadataHelper;
            if (helper.Parser == null)
            {
                helper.Parser = this;
                helper.SetDefault(this);
            }
            if (!ReferenceEquals(helper.Parser, this))
            {
                throw new ArgumentException($"Parser mismatch: {metadata.Name} is not associated to this parser instance.");
            }
            helper.Check();
            arguments.Add(metadata.Name, metadata);
        }
        /// <summary>
        /// Adds <paramref name="metadata"/> to the argument to be parsed from <see cref="Parse(string[])"/>
        /// </summary>
        /// <param name="metadata"><see cref="IArgumentMetadata"/> to be added</param>
        public void Add(IEnumerable<IArgumentMetadata> metadatas)
        {
            foreach (var item in metadatas)
            {
                Add(item);
            }
        }
        /// <summary>
        /// Parse the arguments and return a list of parsed <see cref="IArgumentMetadataParsed"/>
        /// </summary>
        /// <param name="args">Arguments to parse using the list prepared using <see cref="Add(IArgumentMetadata)"/></param>
        /// <returns>A list of <see cref="IArgumentMetadataParsed"/></returns>
        public IEnumerable<IArgumentMetadataParsed> Parse(string[] args)
        {
            Dictionary<string, IArgumentMetadataParsed> parsedArgs = new Dictionary<string, IArgumentMetadataParsed>();

            IList<string> lstArgs = new List<string>(args);

            List<IArgumentMetadata> argsToCheck = new List<IArgumentMetadata>();
            argsToCheck.Add(ArgumentMetadataBase.DefaultFileArgumentMetadata(this));
            argsToCheck.AddRange(Arguments);

            foreach (IArgumentMetadataHelper item in argsToCheck)
            {
                IArgumentMetadataParsed dataParsed = item.Parse(lstArgs);

                if (parsedArgs.ContainsKey(dataParsed.Name)) throw new ArgumentException($"Parameter {dataParsed.Name} is duplicated");

                if (dataParsed != null)
                {
                    parsedArgs.Add(dataParsed.Name, dataParsed);
                }
            }

            if (Settings.CheckUnwanted && lstArgs.Count != 0)
            {
                throw new ArgumentException($"Parameter {(lstArgs.Count == 1 ? string.Empty : "s")} {string.Join(", ", lstArgs)} are not managed");
            }

            foreach (var item in parsedArgs.Values)
            {
                item.CrossCheck?.Invoke(parsedArgs.Values);
            }

            UnparsedArgs = new List<string>(lstArgs).ToArray();

            return parsedArgs.Values;
        }

        /// <summary>
        /// Parse the <see cref="IArgumentMetadataParsed"/> if it represent a file argument (i.e. <see cref="IArgumentMetadataParsed.IsFile"/> is true.)
        /// </summary>
        /// <param name="arg"><see cref="IArgumentMetadataParsed"/> to parse</param>
        /// <returns>A list of <see cref="IArgumentMetadataParsed"/></returns>
        public IEnumerable<IArgumentMetadataParsed> Parse(IArgumentMetadataParsed arg)
        {
            if (arg == null) throw new ArgumentNullException("arg cannot be null.");
            if (!arg.IsFile) throw new ArgumentException("arg does not represent a file argument.");
            List<IArgumentMetadataParsed> parsedArgs = new List<IArgumentMetadataParsed>();

            IList<string> lstArgs = new List<string>(arg.Value as IEnumerable<string>);

            foreach (IArgumentMetadataHelper item in Arguments.Cast<IArgumentMetadataHelper>())
            {
                IArgumentMetadataParsed dataParsed = item.Parse(lstArgs);

                if (dataParsed != null)
                {
                    parsedArgs.Add(dataParsed);
                }
            }

            return parsedArgs;
        }

        /// <summary>
        /// Convert the <see cref="IArgumentMetadataParsed"/> file argument (i.e. <see cref="IArgumentMetadataParsed.IsFile"/> is true) and return the converted arguments
        /// </summary>
        /// <param name="arg">The <see cref="IArgumentMetadataParsed"/> to parse</param>
        /// <returns>A list of <see cref="IArgumentMetadataParsed"/></returns>
        public IEnumerable<IArgumentMetadataParsed> FromFile(IArgumentMetadataParsed arg)
        {
            if (arg == null) throw new ArgumentNullException("arg cannot be null.");
            return Parse(arg);
        }

        /// <summary>
        /// Convert the ensemble of <see cref="IArgumentMetadataParsed"/> searching file argument (i.e. <see cref="IArgumentMetadataParsed.IsFile"/> is true) and return the converted arguments
        /// </summary>
        /// <param name="args">An ensemble of <see cref="IArgumentMetadataParsed"/> to parse</param>
        /// <returns>A list of <see cref="IArgumentMetadataParsed"/></returns>
        public IEnumerable<IArgumentMetadataParsed> FromFile(IEnumerable<IArgumentMetadataParsed> args)
        {
            foreach (var item in args)
            {
                if (item.IsFile)
                {
                    return Parse(item);
                }
            }

            return args;
        }

        /// <summary>
        /// Return the <see cref="IArgumentMetadataParsed"/> at <paramref name="index"/>
        /// </summary>
        /// <param name="args">An ensemble of <see cref="IArgumentMetadataParsed"/> to parse</param>
        /// <param name="index">Index to get</param>
        /// <returns>The selected <see cref="IArgumentMetadataParsed"/></returns>
        public IArgumentMetadataParsed Get(IEnumerable<IArgumentMetadataParsed> args, int index)
        {
            return args.Get(index);
        }

        /// <summary>
        /// Return the <see cref="IArgumentMetadataParsed"/> at <paramref name="name"/>
        /// </summary>
        /// <param name="args">An ensemble of <see cref="IArgumentMetadataParsed"/> to parse</param>
        /// <param name="name">The argument name, or short name, to get</param>
        /// <returns>The selected <see cref="IArgumentMetadataParsed"/></returns>
        public IArgumentMetadataParsed Get(IEnumerable<IArgumentMetadataParsed> args, string name)
        {
            return args.Get(name);
        }

        /// <summary>
        /// Return the value of the <see cref="IArgumentMetadataParsed"/> ensemble at <paramref name="index"/>
        /// </summary>
        /// <param name="args">An ensemble of <see cref="IArgumentMetadataParsed"/> to parse</param>
        /// <param name="index">Index to get</param>
        /// <returns>The value from <see cref="IArgumentMetadataParsed"/></returns>
        public T Get<T>(IEnumerable<IArgumentMetadataParsed> args, int index)
        {
            return args.Get<T>(index);
        }

        /// <summary>
        /// Return the value of the <see cref="IArgumentMetadataParsed"/> ensemble at <paramref name="name"/>
        /// </summary>
        /// <param name="args">An ensemble of <see cref="IArgumentMetadataParsed"/> to parse</param>
        /// <param name="name">The argument name, or short name, to get</param>
        /// <returns>The value from <see cref="IArgumentMetadataParsed"/></returns>
        public T Get<T>(IEnumerable<IArgumentMetadataParsed> args, string name)
        {
            return args.Get<T>(name);
        }

        /// <summary>
        /// Return the value from <see cref="IArgumentMetadataParsed"/>
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to use to retrieve value</typeparam>
        /// <param name="arg">The <see cref="IArgumentMetadataParsed"/> to use to get value</param>
        /// <returns>The value from <see cref="IArgumentMetadataParsed"/></returns>
        public T Get<T>(IArgumentMetadataParsed arg)
        {
            return arg.Get<T>();
        }

        /// <summary>
        /// Return the <see cref="IArgumentMetadataParsed"/> without any accurrence of file argument (i.e. <see cref="IArgumentMetadataParsed.IsFile"/> is true)
        /// </summary>
        /// <param name="args">An ensemble of <see cref="IArgumentMetadataParsed"/> to check</param>
        /// <returns>A list of <see cref="IArgumentMetadataParsed"/></returns>
        public IEnumerable<IArgumentMetadataParsed> RemoveFile(IEnumerable<IArgumentMetadataParsed> args)
        {
            return args.RemoveFile();
        }

        /// <summary>
        /// Overrides values in <paramref name="destination"/> with values found in <paramref name="source"/>
        /// </summary>
        /// <param name="destination">An ensemble of <see cref="IArgumentMetadataParsed"/></param>
        /// <param name="source">An ensemble of <see cref="IArgumentMetadataParsed"/></param>
        /// <param name="rawReplace">Replace destionation without check if argument exist in the source</param>
        /// <returns>The updated <paramref name="destination"/></returns>
        public IEnumerable<IArgumentMetadataParsed> Override(IEnumerable<IArgumentMetadataParsed> destination, IEnumerable<IArgumentMetadataParsed> source, bool rawReplace = false)
        {
            return destination.Override(source, rawReplace);
        }

        /// <summary>
        /// Filter the <paramref name="args"/> for existing <see cref="IArgumentMetadataParsed"/>
        /// </summary>
        /// <param name="args">Arguments to test using the list prepared using <see cref="Parse(string[])"/></param>
        /// <returns>A filtered list of <see cref="IArgumentMetadataParsed"/></returns>
        public IEnumerable<IArgumentMetadataParsed> Exists(IEnumerable<IArgumentMetadataParsed> args)
        {
            return args.Exists();
        }

        /// <summary>
        /// Check the <paramref name="args"/> for existing <paramref name="name"/>
        /// </summary>
        /// <param name="args">Arguments to test using the list prepared using <see cref="Parse(string[])"/></param>
        /// <param name="name">Argument name to search</param>
        /// <returns>true if the the argument with <paramref name="name"/> name exist</returns>
        public bool Exist(IEnumerable<IArgumentMetadataParsed> args, string name)
        {
            return args.Exist(name);
        }

        /// <summary>
        /// Filter the <paramref name="args"/> for non existing <see cref="IArgumentMetadataParsed"/>
        /// </summary>
        /// <param name="args">Arguments to test using the list prepared using <see cref="Parse(string[])"/></param>
        /// <returns>A filtered list of <see cref="IArgumentMetadataParsed"/></returns>
        public IEnumerable<IArgumentMetadataParsed> NotExists(IEnumerable<IArgumentMetadataParsed> args)
        {
            return args.NotExists();
        }
        /// <summary>
        /// Returns the padding calculated on argument length
        /// </summary>
        /// <returns>The calculated length</returns>
        public int PaddingFromArguments()
        {
            int len = 0;
            foreach (IArgumentMetadataHelper item in Arguments.Cast<IArgumentMetadataHelper>())
            {
                len = Math.Max(len, item.Parameter().Length);
            }
            return len;
        }

        /// <summary>
        /// Returns the help information
        /// </summary>
        /// <param name="width">The width of the help to write</param>
        /// <returns>A <see cref="string"/> with help information</returns>
        public string HelpInfo(int? width = null)
        {
            int newWidth = Settings.DefaultConsoleWindowWidth;
            try
            {
                newWidth = Console.WindowWidth;
            }
            catch { }
            if (width.HasValue) newWidth = width.Value;
            StringBuilder builder = new StringBuilder();
            foreach (IArgumentMetadataHelper item in Arguments.Cast<IArgumentMetadataHelper>())
            {
                builder.AppendLine(item.DescriptionBuilder(newWidth));
            }

            return builder.ToString();
        }
    }
}