﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace IxMilia.Dwg.Generator
{
    public class ObjectGenerator : GeneratorBase
    {
        private string _outputDir;
        private XElement _xml;
        private IEnumerable<XElement> _objects;

        public ObjectGenerator(string _outputDir)
        {
            this._outputDir = _outputDir;
            Directory.CreateDirectory(_outputDir);
        }

        public void Run()
        {
            _xml = XDocument.Load(Path.Combine("Spec", "Objects.xml")).Root;
            _objects = _xml.Elements("Object").Where(IsImplemented);

            OutputObjects();
        }

        private void OutputObjects()
        {
            OutputObjectTypeEnum();
            OutputObjectBaseClass();
            OutputObjectClasses();
        }

        private void OutputObjectTypeEnum()
        {
            CreateNewFile("IxMilia.Dwg.Objects");

            IncreaseIndent();
            AppendLine("public enum DwgObjectType : short");
            AppendLine("{");
            IncreaseIndent();
            foreach (var o in _objects)
            {
                AppendLine($"{Name(o)} = {Value(o)},");
            }
            DecreaseIndent();
            AppendLine("}");
            DecreaseIndent();

            FinishFile(Path.Combine(_outputDir, "DwgObjectType.Generated.cs"));
        }

        private void OutputObjectBaseClass()
        {
            CreateNewFile("IxMilia.Dwg.Objects", "System");

            IncreaseIndent();
            AppendLine("public abstract partial class DwgObject");
            AppendLine("{");
            IncreaseIndent();
            AppendLine("internal static DwgObject CreateObject(DwgObjectType type)");
            AppendLine("{");
            IncreaseIndent();
            AppendLine("switch (type)");
            AppendLine("{");
            IncreaseIndent();
            foreach (var o in _objects)
            {
                AppendLine($"case DwgObjectType.{Name(o)}:");
                IncreaseIndent();
                AppendLine($"return new Dwg{Name(o)}();");
                DecreaseIndent();
            }

            AppendLine("default:");
            IncreaseIndent();
            AppendLine("throw new InvalidOperationException(\"Unexpected object type.\");");
            DecreaseIndent();
            DecreaseIndent();
            AppendLine("}");
            DecreaseIndent();
            AppendLine("}");

            DecreaseIndent();
            AppendLine("}");
            DecreaseIndent();

            FinishFile(Path.Combine(_outputDir, "DwgObject.Generated.cs"));
        }

        private void OutputObjectClasses()
        {
            foreach (var o in _objects)
            {
                CreateNewFile("IxMilia.Dwg.Objects", "System", "System.Collections.Generic");

                IncreaseIndent();
                var baseClass = BaseClass(o) ?? (IsEntity(o) ? "DwgEntity" : "DwgObject");
                AppendLine($"{Accessibility(o)} partial class Dwg{Name(o)} : {baseClass}");
                AppendLine("{");
                IncreaseIndent();

                // properties
                AppendLine($"public override DwgObjectType Type => DwgObjectType.{Name(o)};");
                foreach (var p in o.Elements("Property"))
                {
                    if (SkipCreation(p))
                    {
                        continue;
                    }

                    var type = Type(p);
                    if (ReadCount(p) != null)
                    {
                        type = $"List<{type}>";
                    }

                    var comment = Comment(p);
                    if (!string.IsNullOrEmpty(comment))
                    {
                        AppendLine("/// <summary>");
                        AppendLine($"/// {comment}");
                        AppendLine("/// </summary>");
                    }

                    AppendLine($"{Accessibility(p)} {type} {Name(p)} {{ get; set; }}");
                }

                AppendLine();

                // flags
                foreach (var p in o.Elements("Property"))
                {
                    var flags = p.Elements("Flag").ToList();
                    if (flags.Count > 0)
                    {
                        AppendLine($"// {Name(p)} flags");
                        AppendLine();

                        foreach (var f in flags)
                        {
                            AppendLine($"public bool {Name(f)}");
                            AppendLine("{");
                            IncreaseIndent();
                            AppendLine($"get => Converters.GetFlag(this.{Name(p)}, {Mask(f)});");
                            AppendLine("set");
                            AppendLine("{");
                            IncreaseIndent();
                            AppendLine($"var flags = this.{Name(p)};");
                            AppendLine($"Converters.SetFlag(value, ref flags, {Mask(f)});");
                            AppendLine($"this.{Name(p)} = flags;");
                            DecreaseIndent();
                            AppendLine("}");
                            DecreaseIndent();
                            AppendLine("}");
                            AppendLine();
                        }
                    }
                }

                // .ctor
                AppendLine($"{ConstructorAccessibility(o)} Dwg{Name(o)}()");
                AppendLine("{");
                IncreaseIndent();
                AppendLine("SetDefaults();");
                DecreaseIndent();
                AppendLine("}");
                AppendLine();

                // defaults
                AppendLine("internal void SetDefaults()");
                AppendLine("{");
                IncreaseIndent();
                foreach (var p in o.Elements("Property"))
                {
                    AppendLine($"{Name(p)} = {DefaultValue(p)};");
                }

                DecreaseIndent();
                AppendLine("}");
                AppendLine();

                // writing
                if (!CustomReader(o))
                {
                    AppendLine("internal override void WriteSpecific(BitWriter writer, DwgVersionId version)");
                    AppendLine("{");
                    IncreaseIndent();
                    foreach (var p in o.Elements("Property"))
                    {
                        var condition = WriteCondition(p);
                        if (condition != null)
                        {
                            AppendLine($"if ({condition})");
                            AppendLine("{");
                            IncreaseIndent();
                        }

                        var readCount = ReadCount(p);
                        if (string.IsNullOrEmpty(readCount))
                        {
                            var value = ApplyWriteConverter(p, Name(p));
                            AppendLine($"writer.Write_{BinaryType(p)}({value});");
                        }
                        else
                        {
                            var value = ApplyWriteConverter(p, $"{Name(p)}[i]");
                            AppendLine($"for (int i = 0; i < {readCount}; i++)");
                            AppendLine("{");
                            IncreaseIndent();
                            AppendLine($"writer.Write_{BinaryType(p)}({value});");
                            DecreaseIndent();
                            AppendLine("}");
                        }

                        if (condition != null)
                        {
                            DecreaseIndent();
                            AppendLine("}");
                        }
                    }

                    DecreaseIndent();
                    AppendLine("}");
                    AppendLine();
                }

                // parsing
                if (!CustomWriter(o))
                {
                    AppendLine("internal override void ParseSpecific(BitReader reader, DwgVersionId version)");
                    AppendLine("{");
                    IncreaseIndent();
                    foreach (var p in o.Elements("Property"))
                    {
                        var condition = ReadCondition(p);
                        if (condition != null)
                        {
                            AppendLine($"if ({condition})");
                            AppendLine("{");
                            IncreaseIndent();
                        }

                        var readCount = ReadCount(p);
                        var value = ApplyReadConverter(p, $"reader.Read_{BinaryType(p)}({ReaderArgument(p)})");
                        if (string.IsNullOrEmpty(readCount))
                        {
                            AppendLine($"{Name(p)} = {value};");
                        }
                        else
                        {
                            AppendLine($"for (int i = 0; i < {readCount}; i++)");
                            AppendLine("{");
                            IncreaseIndent();
                            AppendLine($"{Name(p)}.Add({value});");
                            DecreaseIndent();
                            AppendLine("}");
                        }

                        if (condition != null)
                        {
                            DecreaseIndent();
                            AppendLine("}");
                        }
                    }

                    DecreaseIndent();
                    AppendLine("}");
                }

                DecreaseIndent();
                AppendLine("}");
                DecreaseIndent();

                FinishFile(Path.Combine(_outputDir, $"Dwg{Name(o)}.Generated.cs"));
            }
        }
    }
}
