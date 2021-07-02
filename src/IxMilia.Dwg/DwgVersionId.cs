using System;

namespace IxMilia.Dwg
{
    public enum DwgVersionId
    {
        R1_0,
        R1_2,
        R1_40,
        R2_05,
        R2_10,
        R2_21,
        R2_22,
        R2_5,
        R2_6,
        R19,
        R10,
        R11_12,
        R13,
        R14,
        Dwg2000,
        Dwg2004,
        Dwg2007,
        Dwg2010,
        Dwg2013,
        Dwg2018,
        Default = R14
    }

    public static class DwgVersionIdExtensions
    {
        public const string R1_0String     = "MC0.0";
        public const string R1_2String     = "AC1.2";
        public const string R1_40String    = "AC1.40";
        public const string R2_05String    = "AC1.50";
        public const string R2_10String    = "AC2.10";
        public const string R2_21String    = "AC2.21";
        public const string R2_22_V1String = "AC2.22";
        public const string R2_22_V2String = "AC1001";
        public const string R2_5String     = "AC1002";
        public const string R2_6String     = "AC1003";
        public const string R19String      = "AC1004";
        public const string R10String      = "AC1006";
        public const string R11_12String   = "AC1009";
        public const string R13String      = "AC1012";
        public const string R14String      = "AC1014";
        public const string Dwg2000String  = "AC1015";
        public const string Dwg2004String  = "AC1018";
        public const string Dwg2007String  = "AC1021";
        public const string Dwg2010String  = "AC1024";
        public const string Dwg2013String  = "AC1027";
        public const string Dwg2018String  = "AC1032";

        public static string VersionString(this DwgVersionId versionId)
        {
            switch (versionId)
            {
                case DwgVersionId.R1_0:
                    return R1_0String;
                case DwgVersionId.R1_2:
                    return R1_2String;
                case DwgVersionId.R1_40:
                    return R1_40String;
                case DwgVersionId.R2_05:
                    return R2_05String;
                case DwgVersionId.R2_10:
                    return R2_10String;
                case DwgVersionId.R2_21:
                    return R2_21String;
                case DwgVersionId.R2_22:
                    return R2_22_V2String;
                case DwgVersionId.R2_5:
                    return R2_5String;
                case DwgVersionId.R2_6:
                    return R2_6String;
                case DwgVersionId.R19:
                    return R19String;
                case DwgVersionId.R10:
                    return R10String;
                case DwgVersionId.R11_12:
                    return R11_12String;
                case DwgVersionId.R13:
                    return R13String;
                case DwgVersionId.R14:
                    return R14String;
                case DwgVersionId.Dwg2000:
                    return Dwg2000String;
                case DwgVersionId.Dwg2004:
                    return Dwg2004String;
                case DwgVersionId.Dwg2007:
                    return Dwg2007String;
                case DwgVersionId.Dwg2010:
                    return Dwg2010String;
                case DwgVersionId.Dwg2013:
                    return Dwg2013String;
                case DwgVersionId.Dwg2018:
                    return Dwg2018String;
                default:
                    throw new NotSupportedException($"Unrecognized version {versionId}");
            }
        }

        public static DwgVersionId VersionIdFromString(string version)
        {
            switch (version)
            {
                case R1_0String:
                    return DwgVersionId.R1_0;
                case R1_2String:
                    return DwgVersionId.R1_2;
                case R1_40String:
                    return DwgVersionId.R1_40;
                case R2_05String:
                    return DwgVersionId.R2_05;
                case R2_10String:
                    return DwgVersionId.R2_10;
                case R2_21String:
                    return DwgVersionId.R2_21;
                case R2_22_V1String:
                case R2_22_V2String:
                    return DwgVersionId.R2_22;
                case R2_5String:
                    return DwgVersionId.R2_05;
                case R2_6String:
                    return DwgVersionId.R2_6;
                case R19String:
                    return DwgVersionId.R19;
                case R10String:
                    return DwgVersionId.R10;
                case R11_12String:
                    return DwgVersionId.R11_12;
                case R13String:
                    return DwgVersionId.R13;
                case R14String:
                    return DwgVersionId.R14;
                case Dwg2000String:
                    return DwgVersionId.Dwg2000;
                case Dwg2004String:
                    return DwgVersionId.Dwg2004;
                case Dwg2007String:
                    return DwgVersionId.Dwg2007;
                case Dwg2010String:
                    return DwgVersionId.Dwg2010;
                case Dwg2013String:
                    return DwgVersionId.Dwg2013;
                case Dwg2018String:
                    return DwgVersionId.Dwg2018;
                default:
                    throw new NotSupportedException($"Unrecognized version {version}");
            }
        }
    }
}
