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
using System.IO;
using System.Text;

namespace MASES.CLIParser
{
    /// <summary>
    /// Prefixes of the argument
    /// </summary>
    public enum ArgumentPrefix
    {
        /// <summary>
        /// Represents an argument without a prefix
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Represents a "-"
        /// </summary>
        Dash = 0x1,
        /// <summary>
        /// Represents a "--"
        /// </summary>
        DoubleDash = 0x2,
        /// <summary>
        /// Represents a "/"
        /// </summary>
        Slash = 0x4,
        /// <summary>
        /// Represents a custom prefix
        /// </summary>
        Custom = 0x8,
    }
    /// <summary>
    /// Represent the argument format
    /// </summary>
    public enum ArgumentType
    {
        /// <summary>
        /// It is a single value
        /// </summary>
        Single,
        /// <summary>
        /// It is a key-value pair
        /// </summary>
        KeyValue,
        /// <summary>
        /// Represents an argument whose value is the next command-line argument
        /// </summary>
        Double
    }
    /// <summary>
    /// Represent the argument possible values to check
    /// </summary>
    public enum ArgumentValueType
    {
        /// <summary>
        /// It is a free value
        /// </summary>
        Free,
        /// <summary>
        /// Represents a possible value into an array
        /// </summary>
        Array,
        /// <summary>
        /// Represents a range of values
        /// </summary>
        Range
    }
    /// <summary>
    /// Metadata associated to the argument, used during the parse
    /// </summary>
    public interface IArgumentMetadata
    {
        /// <summary>
        /// <see cref="ArgumentPrefix"/> prefix associated to <see cref="IArgumentMetadata"/>
        /// </summary>
        ArgumentPrefix? Prefix { get; set; }
        /// <summary>
        /// If <see cref="Prefix"/> contains <see cref="ArgumentPrefix.Custom"/> use <see cref="CustomPrefix"/> to analyze arguments
        /// </summary>
        string CustomPrefix { get; set; }
        /// <summary>
        /// The string representing the actual prefix from configuration
        /// </summary>
        string PrefixInUse { get; }
        /// <summary>
        /// Argument name
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Argument short name
        /// </summary>
        string ShortName { get; set; }
        /// <summary>
        /// Argument help
        /// </summary>
        string Help { get; set; }
        /// <summary>
        /// Set how to manage the <see cref="IArgumentMetadata"/>
        /// </summary>
        ArgumentType? Type { get; set; }
        /// <summary>
        /// <see cref="ArgumentValueType"/> to check input
        /// </summary>
        ArgumentValueType? ValueType { get; set; }
        /// <summary>
        /// Separator used when <see cref="Type"/> is set to <see cref="ArgumentType.KeyValue"/>
        /// </summary>
        string KeyValuePairSeparator { get; set; }
        /// <summary>
        /// True if the argument shall be treated as case invariant
        /// </summary>
        bool? IsCaseInvariant { get; set; }
        /// <summary>
        /// True if the argument shall be mandatory
        /// </summary>
        bool IsMandatory { get; set; }
        /// <summary>
        /// True if the argument is an extended argument
        /// </summary>
        bool IsExtended { get; set; }
        /// <summary>
        /// The argument contains multiple values
        /// </summary>
        bool IsMultiValue { get; set; }
        /// <summary>
        /// The argument contains an enumeration value
        /// </summary>
        bool IsEnum { get; }
        /// <summary>
        /// The argument contains an enumeration value with <see cref="FlagsAttribute"/>
        /// </summary>
        bool IsFlag { get; }
        /// <summary>
        /// The separator used in the multiple value argument
        /// </summary>
        char? MultiValueSeparator { get; set; }
        /// <summary>
        /// The dafault value to use if the value is not found
        /// </summary>
        object Default { get; set; }
        /// <summary>
        /// The array of possible values for the argument if <see cref="ValueType"/> is <see cref="ArgumentValueType.Array"/>
        /// </summary>
        object[] ArrayValues { get; set; }
        /// <summary>
        /// The minimum value for the argument if <see cref="ValueType"/> is <see cref="ArgumentValueType.Range"/>
        /// </summary>
        object MinValue { get; set; }
        /// <summary>
        /// The maximum value for the argument if <see cref="ValueType"/> is <see cref="ArgumentValueType.Range"/>
        /// </summary>
        object MaxValue { get; set; }
        /// <summary>
        /// An <see cref="Action"/> to cross check multiple values. If something is not correct during check an <see cref="ArgumentException"/> shall be raised.
        /// </summary>
        Action<IEnumerable<IArgumentMetadataParsed>> CrossCheck { get; set; }
        /// <summary>
        /// The <see cref="Type"/> of the parameter.
        /// </summary>
        Type DataType { get; }
    }
    /// <summary>
    /// Result of the argument analysis after the parse
    /// </summary>
    public interface IArgumentMetadataParsed : IArgumentMetadata
    {
        /// <summary>
        /// True if the argument was found during parse
        /// </summary>
        bool Exist { get; }
        /// <summary>
        /// True if the argument was found and identifies a file
        /// </summary>
        bool IsFile { get; }
        /// <summary>
        /// The value found during the parse
        /// </summary>
        object Value { get; }
        /// <summary>
        /// Overrides with values in <paramref name="src"/>
        /// </summary>
        /// <param name="src">The source <see cref="IArgumentMetadataParsed"/></param>
        /// <returns>True if override happened</returns>
        bool Override(IArgumentMetadataParsed src);
    }

    interface IArgumentMetadataHelper
    {
        Parser Parser { get; set; }

        string StartWith { get; }

        string ShortStartWith { get; }

        void SetDefault(Parser parser);

        string GetPrefix();

        string Parameter();

        string DescriptionBuilder(int width);

        void Check();

        IArgumentMetadataParsed Parse(IList<string> args);

        void TestValue(object value);
    }

    /// <summary>
    /// Class managing the argument information
    /// </summary>
    /// <typeparam name="T">Expected data <see cref="Type"/> from the argument</typeparam>
    public class ArgumentMetadataBase : IArgumentMetadata
    {
        /// <summary>
        /// Dafault used to check for file argument
        /// </summary>
        public static IArgumentMetadata DefaultFileArgumentMetadata(Parser parser)
        {
            return new ArgumentMetadata<string>(parser)
            {
                Name = string.Empty,
                ShortName = string.Empty,
                Default = null,
                Prefix = ArgumentPrefix.Custom,
                CustomPrefix = parser.Settings.DefaultFileNameIdentifier.ToString(),
                Type = ArgumentType.Single,
                ValueType = ArgumentValueType.Free,
            };
        }

        internal ArgumentMetadataBase()
        {
        }

        /// <inheritdoc/>
        public virtual ArgumentPrefix? Prefix { get; set; }
        /// <inheritdoc/>
        public virtual string CustomPrefix { get; set; }
        /// <inheritdoc/>
        public string PrefixInUse => Prefix?.Prefix(CustomPrefix);
        /// <inheritdoc/>
        public virtual string Name { get; set; }
        /// <inheritdoc/>
        public virtual string ShortName { get; set; }
        /// <inheritdoc/>
        public virtual string Help { get; set; }
        /// <inheritdoc/>
        public virtual ArgumentType? Type { get; set; }
        /// <inheritdoc/>
        public virtual ArgumentValueType? ValueType { get; set; }
        /// <inheritdoc/>
        public virtual string KeyValuePairSeparator { get; set; }
        /// <inheritdoc/>
        public virtual bool? IsCaseInvariant { get; set; }
        /// <inheritdoc/>
        public virtual bool IsMandatory { get; set; }
        /// <inheritdoc/>
        public virtual bool IsExtended { get; set; }
        /// <inheritdoc/>
        public virtual bool IsMultiValue { get; set; }
        /// <inheritdoc/>
        public virtual bool IsEnum { get; protected set; }
        /// <inheritdoc/>
        public virtual bool IsFlag { get; protected set; }
        /// <inheritdoc/>
        public virtual char? MultiValueSeparator { get; set; }
        /// <inheritdoc/>
        public virtual object Default { get; set; }
        /// <inheritdoc/>
        public virtual object[] ArrayValues { get; set; }
        /// <inheritdoc/>
        public virtual object MinValue { get; set; }
        /// <inheritdoc/>
        public virtual object MaxValue { get; set; }
        /// <inheritdoc/>
        public virtual Action<IEnumerable<IArgumentMetadataParsed>> CrossCheck { get; set; }
        /// <inheritdoc/>
        public virtual Type DataType { get; protected set; }
    }

    class ArgumentMetadataParsed : ArgumentMetadataBase, IArgumentMetadataParsed
    {
        public ArgumentMetadataParsed(IArgumentMetadata reference)
        {
            Prefix = reference.Prefix;
            CustomPrefix = reference.CustomPrefix;
            Name = reference.Name;
            ShortName = reference.ShortName;
            Help = reference.Help;
            Type = reference.Type;
            ValueType = reference.ValueType;
            KeyValuePairSeparator = reference.KeyValuePairSeparator;
            IsCaseInvariant = reference.IsCaseInvariant;
            IsMandatory = reference.IsMandatory;
            IsExtended = reference.IsExtended;
            IsMultiValue = reference.IsMultiValue;
            IsEnum = reference.IsEnum;
            MultiValueSeparator = reference.MultiValueSeparator;
            Default = reference.Default;
            ArrayValues = reference.ArrayValues;
            MinValue = reference.MinValue;
            MaxValue = reference.MaxValue;
            CrossCheck = reference.CrossCheck;
            DataType = reference.DataType;
        }

        public bool Exist { get; internal set; }

        public bool IsFile { get; internal set; }

        public object Value { get; internal set; }

        public bool Override(IArgumentMetadataParsed src)
        {
            if (Name == src.Name)
            {
                Exist = src.Exist;
                IsFile = src.IsFile;
                Value = src.Value;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Class managing the argument information
    /// </summary>
    /// <typeparam name="T">Expected data <see cref="Type"/> from the argument</typeparam>
    public class ArgumentMetadata<T> : ArgumentMetadataBase, IArgumentMetadataHelper
    {
        /// <summary>
        /// Default value for multi value argment
        /// </summary>
        public static IEnumerable<T> DefaultMultiValue { get { return new T[] { }; } }

        public ArgumentMetadata()
        {
            Prepare();
        }

        public ArgumentMetadata(Parser parser)
        {
            Prepare();
            Helper.Parser = parser;
            Helper.SetDefault(parser);
        }

        void Prepare()
        {
            Default = default(T);
            DataType = typeof(T);
            IsEnum = DataType.IsEnum;
            foreach (var _ in DataType.CustomAttributes)
            {
                IsFlag = true; break;
            }
        }

        IArgumentMetadataHelper Helper { get { return this; } }

        string IArgumentMetadataHelper.GetPrefix()
        {
            return PrefixInUse;
        }

        Parser IArgumentMetadataHelper.Parser { get; set; }

        string IArgumentMetadataHelper.StartWith
        {
            get
            {
                string arg = Helper.GetPrefix();
                switch (Type)
                {
                    case ArgumentType.Single:
                    case ArgumentType.Double:
                        arg += Name;
                        break;
                    case ArgumentType.KeyValue:
                        arg += Name + KeyValuePairSeparator;
                        break;

                    default:
                        break;
                }

                if (IsCaseInvariant.Value) arg = arg.ToLowerInvariant();

                return arg;
            }
        }

        string IArgumentMetadataHelper.ShortStartWith
        {
            get
            {
                if (string.IsNullOrEmpty(ShortName)) return null;

                string arg = Helper.GetPrefix();
                switch (Type)
                {
                    case ArgumentType.Single:
                    case ArgumentType.Double:
                        arg += ShortName;
                        break;
                    case ArgumentType.KeyValue:
                        arg += ShortName + KeyValuePairSeparator;
                        break;

                    default:
                        break;
                }

                if (IsCaseInvariant.Value) arg = arg.ToLowerInvariant();

                return arg;
            }
        }

        void IArgumentMetadataHelper.SetDefault(Parser parser)
        {
            if (!Prefix.HasValue) Prefix = parser.Settings.DefaultPrefix;
            if (CustomPrefix == null) CustomPrefix = parser.Settings.DefaultCustomPrefix;
            if (!Type.HasValue) Type = parser.Settings.DefaultType;
            if (!ValueType.HasValue) ValueType = parser.Settings.DefaultValueType;
            if (KeyValuePairSeparator == null) KeyValuePairSeparator = parser.Settings.DefaultKeyValuePairSeparator;
            if (!IsCaseInvariant.HasValue) IsCaseInvariant = parser.Settings.DefaultIsCaseInvariant;
            if (!MultiValueSeparator.HasValue) MultiValueSeparator = parser.Settings.DefaultMultiValueSeparator;
        }

        string IArgumentMetadataHelper.Parameter()
        {
            string description = Name;
            if (!string.IsNullOrEmpty(ShortName))
            {
                description += $" ({ShortName})";
            }
            return description;
        }

        string IArgumentMetadataHelper.DescriptionBuilder(int width)
        {
            if (width <= 0) width = 80; // set a new default since received one is not Parser.HelpInfo is not correct
            Helper.Check();
            string description = Helper.Parameter();
            description = description.PadRight(Helper.Parser.Settings.DefaultDescriptionPadding, ' ');
            description += ": ";
            if (!string.IsNullOrEmpty(Help))
            {
                description += Help;
                if (!description.EndsWith(".")) description += ". ";
                if (!description.EndsWith(" ")) description += " ";
            }

            description += $"The argument is {(IsMandatory ? "mandatory" : "optional")}. ";

            if (Default != null)
            {
                description += $"Default: {Default}. ";
            }
            string range = null;
            if (IsEnum)
            {
                range = string.Join(", ", Enum.GetNames(DataType));
            }
            else
            {
                switch (ValueType)
                {
                    case ArgumentValueType.Array:
                        if (ArrayValues != null)
                        {
                            range = string.Join(", ", ArrayValues);
                        }
                        break;
                    case ArgumentValueType.Range:
                        if (MinValue != null && MaxValue != null)
                        {
                            range = $"[{MinValue}...{MaxValue}]";
                        }
                        break;
                    case ArgumentValueType.Free:
                    default:
                        break;
                }
            }

            switch (Type)
            {
                case ArgumentType.KeyValue:
                    if (!string.IsNullOrEmpty(range))
                    {
                        description += $"{Helper.GetPrefix()}{Name}=<{range}>.";
                    }
                    break;
                case ArgumentType.Double:
                    if (!string.IsNullOrEmpty(range))
                    {
                        description += $"{Helper.GetPrefix()}{Name} <{range}>.";
                    }
                    break;
                case ArgumentType.Single:
                default:
                    break;
            }

            string trimming = description;
            StringBuilder builder = new StringBuilder();
            while (trimming.Length > width)
            {
                int cuttingPoint = width - 2;
                if (cuttingPoint < 0) cuttingPoint = 0;
                builder.AppendLine(trimming.Substring(0, cuttingPoint) + "-");
                trimming = string.Empty.PadRight(Helper.Parser.Settings.DefaultDescriptionPadding + 2, ' ') + trimming.Remove(0, cuttingPoint);
            }
            builder.AppendLine(trimming);

            return builder.ToString();
        }

        void IArgumentMetadataHelper.Check()
        {
            if (string.IsNullOrEmpty(Name)) throw new ArgumentException("Argument needs to set Name.");
            if (Default != null)
            {
                if (IsMultiValue)
                {
                    var dType = Default.GetType();
                    if (dType.GetInterface(typeof(System.Collections.IEnumerable).Name) != null)
                    {
                        var elemType = dType.GetElementType() ?? throw new ArgumentException($"Default type shall be an instance of {typeof(System.Collections.IEnumerable).Name}<{DataType}>.");
                        if (elemType != DataType) throw new ArgumentException($"Default type shall be equal to {DataType}.");
                    }
                    else throw new ArgumentException($"Default type shall be an instance of {typeof(System.Collections.IEnumerable).Name}<{DataType}>.");
                }
                else
                {
                    if (Default.GetType() != DataType) throw new ArgumentException($"Default type shall be equal to {DataType}.");
                }
            }
            switch (ValueType)
            {
                case ArgumentValueType.Array:
                    if (ArrayValues == null || ArrayValues.Length == 0) throw new ArgumentException("Argument needs to set ArrayValues.");
                    foreach (var item in ArrayValues)
                    {
                        if (item.GetType() != DataType) throw new ArgumentException($"ArrayValues type shall be equal to {DataType}.");
                    }
                    break;
                case ArgumentValueType.Range:
                    if (MinValue == null && MaxValue == null) throw new ArgumentException("Argument needs to set both MinValue and MaxValue.");
                    if (!(DataType.IsValueType)) throw new ArgumentException($"DataType shall be a ValueType, found {DataType}.");
                    if (MinValue.GetType() != DataType) throw new ArgumentException($"MinValue type shall be equal to {DataType}.");
                    if (MaxValue.GetType() != DataType) throw new ArgumentException("MaxValue type shall be equal to {DataType}.");
                    break;
                case ArgumentValueType.Free:
                default:
                    break;
            }
            if (Default != null) Helper.TestValue(Default);
        }

        bool CheckParam(string stringToTest)
        {
            bool result;
            switch (Type)
            {
                case ArgumentType.KeyValue:
                    result = stringToTest.StartsWith(Helper.StartWith) || (!string.IsNullOrEmpty(Helper.ShortStartWith) && stringToTest.StartsWith(Helper.ShortStartWith));
                    break;
                default:
                    result = stringToTest == Helper.StartWith || (!string.IsNullOrEmpty(Helper.ShortStartWith) && stringToTest == Helper.ShortStartWith);
                    break;
            }
            return result;
        }

        IArgumentMetadataParsed IArgumentMetadataHelper.Parse(IList<string> args)
        {
            for (int i = 0; i < args.Count; i++)
            {
                string stringToTest = (IsCaseInvariant.Value) ? args[i].ToLowerInvariant() : args[i];
                if (stringToTest.StartsWith(Helper.Parser.Settings.DefaultFileNameIdentifier.ToString()))
                {
                    // represent a file
                    ArgumentMetadataParsed parsedData = new ArgumentMetadataParsed(this)
                    {
                        Exist = true,
                        IsFile = true,
                    };
                    parsedData.Value = File.ReadLines(args[i].Substring(1));
                    args.RemoveAt(i);
                    return parsedData;
                }
                else if (CheckParam(stringToTest))
                {
                    ArgumentMetadataParsed parsedData = new ArgumentMetadataParsed(this)
                    {
                        Exist = true
                    };
                    switch (Type)
                    {
                        case ArgumentType.Single:
                            {
                                parsedData.Value = Convert.ChangeType(args[i].Substring(Helper.GetPrefix().Length), DataType);
                                Helper.TestValue(parsedData.Value);
                                args.RemoveAt(i);
                            }
                            break;
                        case ArgumentType.KeyValue:
                            {
                                _ = args[i];
                                string value;
                                if (stringToTest.StartsWith(Helper.StartWith))
                                {
                                    value = args[i].Substring(Helper.StartWith.Length);
                                }
                                else
                                {
                                    value = args[i].Substring(Helper.ShortStartWith.Length);
                                }
                                if (string.IsNullOrEmpty(value))
                                {
                                    throw new ArgumentException($"Parameter {Name} needs a value");
                                }
                                if (IsEnum)
                                {
                                    try
                                    {
                                        parsedData.Value = Enum.Parse(DataType, value, IsCaseInvariant.Value);
                                        Helper.TestValue(parsedData.Value);
                                    }
                                    catch
                                    {
                                        throw new ArgumentException($"Argument {Name} shall be in {string.Join(", ", Enum.GetNames(DataType))}: {value} was found.");
                                    }
                                }
                                else if (IsMultiValue)
                                {
                                    List<T> values = new List<T>();
                                    string[] datas = value.Split(MultiValueSeparator.Value);
                                    foreach (var item in datas)
                                    {
                                        object oVal = Convert.ChangeType(value, DataType);
                                        Helper.TestValue(oVal);
                                        values.Add((T)oVal);
                                    }
                                    parsedData.Value = values.ToArray();
                                }
                                else
                                {
                                    parsedData.Value = Convert.ChangeType(value, DataType);
                                    Helper.TestValue(parsedData.Value);
                                }
                                args.RemoveAt(i);
                            }
                            break;
                        case ArgumentType.Double:
                            {
                                if (args.Count > i + 1)
                                {
                                    string value = args[i + 1];
                                    if (IsEnum)
                                    {
                                        try
                                        {
                                            parsedData.Value = Enum.Parse(DataType, value, IsCaseInvariant.Value);
                                            Helper.TestValue(parsedData.Value);
                                        }
                                        catch
                                        {
                                            throw new ArgumentException($"Argument {Name} shall be in {string.Join(", ", Enum.GetNames(DataType))}: {value} was found.");
                                        }
                                    }
                                    else if (IsMultiValue)
                                    {
                                        List<T> values = new List<T>();
                                        string[] datas = value.Split(MultiValueSeparator.Value);
                                        foreach (var item in datas)
                                        {
                                            object oVal = Convert.ChangeType(item, DataType);
                                            Helper.TestValue(oVal);
                                            values.Add((T)oVal);
                                        }
                                        parsedData.Value = values.ToArray();
                                    }
                                    else
                                    {
                                        parsedData.Value = Convert.ChangeType(value, DataType);
                                        Helper.TestValue(parsedData.Value);
                                    }
                                    args.RemoveAt(i);
                                    args.RemoveAt(i);
                                }
                                else throw new ArgumentException($"Parameter {Name} needs a value");
                            }
                            break;
                        default:
                            break;
                    }

                    return parsedData;
                }
                else continue;
            }

            if (IsMandatory) throw new ArgumentException($"Parameter {Name} is mandatory");

            return new ArgumentMetadataParsed(this)
            {
                Exist = false
            };
        }

        void IArgumentMetadataHelper.TestValue(object value)
        {
            if (IsEnum)
            {
                if (!IsFlag && !Enum.IsDefined(DataType, value))
                {
                    throw new ArgumentException($"Argument {Name} shall be in {string.Join(", ", Enum.GetNames(DataType))}: {value} was found.");
                }
            }
            else
            {
                switch (ValueType)
                {
                    case ArgumentValueType.Array:
                        {
                            bool found = false;
                            foreach (var item in ArrayValues)
                            {
                                found |= Equals(value, item);
                            }
                            if (!found)
                            {
                                throw new ArgumentException($"Argument {Name} shall be in {string.Join(", ", ArrayValues)}, {value} was found.");
                            }
                        }
                        break;
                    case ArgumentValueType.Range:
                        {
                            if (Comparer<T>.Default.Compare((T)value, (T)MinValue) >= 0 && Comparer<T>.Default.Compare((T)value, (T)MaxValue) <= 0)
                            {
                                break;
                            }
                            else throw new ArgumentException($"Argument {Name} shall be in {MinValue} - {MaxValue}, {value} was found.");
                        }
                    case ArgumentValueType.Free:
                    default:
                        break;
                }
            }
        }
    }
}
