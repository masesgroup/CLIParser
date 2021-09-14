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

using MASES.CLIParser;
using System;

namespace MASES.CLIParserTest
{
    class Program
    {
        [Flags]
        public enum MyValues
        {
            First = 0x1,
            Second = 0x2,
            Third = 0x4
        };

        static void Main(string[] args)
        {
            Parser parser = Parser.CreateInstance();

            parser.Add(new ArgumentMetadata<MyValues>()
            {
                Name = "enum",
                Default = MyValues.First,
                Help = "this is an enum test",
                Type = ArgumentType.Double,
            });
            parser.Add(new ArgumentMetadata<bool>()
            {
                Name = "test",
                ShortName = "tst",
                Help = "this is a test",
                Type = ArgumentType.Double,
                ValueType = ArgumentValueType.Free,
            });
            parser.Add(new ArgumentMetadata<int>()
            {
                Name = "range",
                Default = 9,
                Type = ArgumentType.Double,
                ValueType = ArgumentValueType.Range,
                MinValue = 2,
                MaxValue = 10,
            });
            parser.Add(new ArgumentMetadata<string>()
            {
                Name = "multivalue",
                Type = ArgumentType.Double,
                IsMultiValue = true,
                Default = new string[] {"a", "b"}
            });
            parser.Add(new ArgumentMetadata<string>()
            {
                Name = "myval",
                Type = ArgumentType.Single,
            });

            var result =  parser.Parse(args);

            var fileInfo = parser.FromFile(result);

            parser.Override(result, fileInfo);

            var noFile = parser.RemoveFile(result);

            foreach (var item in parser.Exists(noFile))
            {
                if (!item.IsMultiValue)
                {
                    Console.WriteLine("{0} is {1}", item.Name, item.Value);
                }
                else
                {
                    Console.WriteLine("{0} is {1}", item.Name, string.Join(", ", (object[])item.Value));
                }
            }

            foreach (var item in parser.NotExists(noFile))
            {
                Console.WriteLine("{0} not exist", item.Name);
            }
        }
    }
}
