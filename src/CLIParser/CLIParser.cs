/*
 *  MIT License
 *
 *  Copyright (c) 2021 MASES s.r.l.
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
using System.Text;

namespace MASES.CLIParser
{
    /// <summary>
    /// Helper methods for the <see cref="Parser"/>
    /// </summary>
    public static class ParserExtension
    {
        /// <summary>
        /// Adds an <see cref="IArgumentMetadata"/>
        /// </summary>
        /// <param name="metadatas">The <see cref="IArgumentMetadata"/> to add</param>
        public static void Add(this IArgumentMetadata metadata)
        {
            IArgumentMetadataHelper helper = metadata as IArgumentMetadataHelper;
            if (helper.Parser == null) throw new ArgumentException(string.Format("Parameter {0} does not have any associated Parser", metadata.Name));
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
            if (arguments.ContainsKey(metadata.Name)) throw new ArgumentException(string.Format("Parameter {0} is duplicated", metadata.Name));
            IArgumentMetadataHelper helper = metadata as IArgumentMetadataHelper;
            if (helper.Parser == null)
            {
                helper.Parser = this;
                helper.SetDefault(this);
            }
            if (!ReferenceEquals(helper.Parser, this))
            {
                throw new ArgumentException(string.Format("Parser mismatch: {0} is not associated to this parser instance.", metadata.Name));
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

                if (parsedArgs.ContainsKey(dataParsed.Name)) throw new ArgumentException(string.Format("Parameter {0} is duplicated", dataParsed.Name));

                if (dataParsed != null)
                {
                    parsedArgs.Add(dataParsed.Name, dataParsed);
                }
            }

            if (Settings.CheckUnwanted && lstArgs.Count != 0)
            {
                throw new ArgumentException(string.Format("Parameter{0} {1} are not managed", lstArgs.Count == 1 ? string.Empty : "s", string.Join(", ", lstArgs)));
            }

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

            foreach (IArgumentMetadataHelper item in Arguments)
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
            return new List<IArgumentMetadataParsed>(args)[index];
        }

        /// <summary>
        /// Return the <see cref="IArgumentMetadataParsed"/> at <paramref name="name"/>
        /// </summary>
        /// <param name="args">An ensemble of <see cref="IArgumentMetadataParsed"/> to parse</param>
        /// <param name="name">The argument name, or short name, to get</param>
        /// <returns>The selected <see cref="IArgumentMetadataParsed"/></returns>
        public IArgumentMetadataParsed Get(IEnumerable<IArgumentMetadataParsed> args, string name)
        {
            foreach (var item in new List<IArgumentMetadataParsed>(args))
            {
                if (item.Name == name || item.ShortName == name) return item;
            }
            throw new ArgumentException("name is not a valid argument.");
        }

        /// <summary>
        /// Return the <see cref="IArgumentMetadataParsed"/> at <paramref name="name"/>
        /// </summary>
        /// <param name="args">An ensemble of <see cref="IArgumentMetadataParsed"/> to parse</param>
        /// <param name="name">The argument name, or short name, to get</param>
        /// <returns>The selected <see cref="IArgumentMetadataParsed"/></returns>
        public T Get<T>(IEnumerable<IArgumentMetadataParsed> args, string name)
        {
            foreach (var item in new List<IArgumentMetadataParsed>(args))
            {
                if (item.Name == name || item.ShortName == name) return Get<T>(item);
            }
            throw new ArgumentException("name is not a valid argument.");
        }

        /// <summary>
        /// Return the <see cref="IArgumentMetadataParsed"/> at <paramref name="name"/>
        /// </summary>
        /// <param name="args">An ensemble of <see cref="IArgumentMetadataParsed"/> to parse</param>
        /// <param name="name">The argument name, or short name, to get</param>
        /// <returns>The selected <see cref="IArgumentMetadataParsed"/></returns>
        public T Get<T>(IArgumentMetadataParsed arg)
        {
            if (arg == null) throw new ArgumentNullException("arg cannot be null.");
            if (!typeof(T).IsAssignableFrom(arg.DataType)) throw new ArgumentException(string.Format("{0} is incomplatible wirh {1}.", typeof(T), arg.DataType));
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
        public IEnumerable<IArgumentMetadataParsed> RemoveFile(IEnumerable<IArgumentMetadataParsed> args)
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
        public IEnumerable<IArgumentMetadataParsed> Override(IEnumerable<IArgumentMetadataParsed> destination, IEnumerable<IArgumentMetadataParsed> source, bool rawReplace = false)
        {
            List<IArgumentMetadataParsed> newSrc = new List<IArgumentMetadataParsed>(source);
            foreach (var item in destination)
            {
                foreach (var item2 in newSrc.ToArray())
                {
                    if (rawReplace ? true : item2.Exist && item.Override(item2))
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
        /// <param name="args">Arguments to test using the list prepared using <see cref="Parse(string[])"/></param>
        /// <returns>A filtered list of <see cref="IArgumentMetadataParsed"/></returns>
        public IEnumerable<IArgumentMetadataParsed> Exists(IEnumerable<IArgumentMetadataParsed> args)
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
        /// <param name="args">Arguments to test using the list prepared using <see cref="Parse(string[])"/></param>
        /// <param name="name">Argument name to search</param>
        /// <returns>true if the the argument with <paramref name="name"/> name exist</returns>
        public bool Exist(IEnumerable<IArgumentMetadataParsed> args, string name)
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
        /// <param name="args">Arguments to test using the list prepared using <see cref="Parse(string[])"/></param>
        /// <returns>A filtered list of <see cref="IArgumentMetadataParsed"/></returns>
        public IEnumerable<IArgumentMetadataParsed> NotExists(IEnumerable<IArgumentMetadataParsed> args)
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
        /// <summary>
        /// Returns the padding calculated on argument length
        /// </summary>
        /// <returns>The calculated length</returns>
        public int PaddingFromArguments()
        {
            int len = 0;
            foreach (IArgumentMetadataHelper item in Arguments)
            {
                len = Math.Max(len, item.Parameter().Length);
            }
            return len;
        }

        /// <summary>
        /// Returns the help information
        /// </summary>
        /// <param name="width">The width of the help to write</param>
        /// <returns>A <see cref="string with help information"/></returns>
        public string HelpInfo(int? width = null)
        {
            int newWidth = Console.WindowWidth;
            if (width.HasValue) newWidth = width.Value;
            StringBuilder builder = new StringBuilder();
            foreach (IArgumentMetadataHelper item in Arguments)
            {
                builder.AppendLine(item.DescriptionBuilder(newWidth));
            }

            return builder.ToString();
        }
    }
}