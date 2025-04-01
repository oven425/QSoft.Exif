// See https://aka.ms/new-console-template for more information


using System.Buffers.Binary;
using System.IO;
using System.Text;


var stream = System.IO.File.OpenRead("../../../54362611843_84a763ea2c_o.jpg");

BinaryReader br = new BinaryReader(stream);

//https://www.media.mit.edu/pia/Research/deepview/exif.html

// find APP1
int applen = 0;
long exifstart = 0;
while (true)
{
    byte[] bb = br.ReadBytes(2);
    if (bb[0] == 0xFF && bb[1] == 0xE1)
    {
        applen = br.ReadInt16();
        var exif_header = br.ReadBytes(6);
        var ss = Encoding.ASCII.GetString(exif_header);
        exifstart = br.BaseStream.Position;
        //TIFF Header
        var align = br.ReadBytes(2);
        var tagmark = br.ReadBytes(2);
        var offsetfirstifd =BitConverter.ToInt32(br.ReadBytes(4).Reverse().ToArray(), 0);
        //br.ReadBytes(offsetfirstifd);
        break;
    }
}


ParseTiff();
void ParseTiff()
{
    //https://www.media.mit.edu/pia/Research/deepview/exif.html
    var enterycount = BitConverter.ToInt16(br.ReadBytes(2).Reverse().ToArray(), 0);
    List<(TagName name, TagFormat format, int length, long begin)> ll = [];
    for(int i=0;i<enterycount;i++)
    {
        var tagname_1 = BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0);
        var tagname = (TagName)tagname_1;
        var tagformat = (TagFormat)BitConverter.ToInt16(br.ReadBytes(2).Reverse().ToArray(), 0);
        var tagatalength = BitConverter.ToInt32(br.ReadBytes(4).Reverse().ToArray(), 0);
        var tagoffset = BitConverter.ToInt32(br.ReadBytes(4).Reverse().ToArray(), 0);
        ll.Add((tagname, tagformat, tagatalength, exifstart + tagoffset));
    }

    foreach(var oo in ll)
    {
        switch(oo.format)
        {
            case TagFormat.RATIONAL:
                {
                    br.BaseStream.Position = oo.begin;
                    var buf = br.ReadBytes(oo.length*8).Reverse();
                    var v1 = BitConverter.ToUInt32(buf.Take(4).ToArray(), 0);
                    var v2 = BitConverter.ToUInt32(buf.Skip(4).Take(4).ToArray(), 0);
                    Console.WriteLine($"{oo.name} : {v1}/{v2}");
                }
                break;

            case TagFormat.AsciiString:
                {
                    br.BaseStream.Position = oo.begin;
                    var str = Encoding.ASCII.GetString(br.ReadBytes(oo.length),0, oo.length-1);
                    System.Diagnostics.Trace.WriteLine($"{oo.name} : {str}");
                }
                break;
            case TagFormat.UShort:
                {
                    br.BaseStream.Position = oo.begin;
                    var buf = br.ReadBytes(oo.length * 2);
                    var str = BitConverter.ToUInt16(br.ReadBytes(oo.length*2).Reverse().ToArray(), 0);
                    Console.WriteLine($"{oo.name} : {str}");
                }
                break;

        }
    }
}


void ascii()
{

}


public enum TagName
{
    // IFD0 (main image) Tags
    ImageDescription = 0x010e,
    Make = 0x010f,
    Model = 0x0110,
    Orientation = 0x0112,
    XResolution = 0x011a,
    YResolution = 0x011b,
    ResolutionUnit = 0x0128,
    Software = 0x0131,
    DateTime = 0x0132,
    HostComputer = 0x13C,
    WhitePoint = 0x013e,
    PrimaryChromaticities = 0x013f,
    YCbCrCoefficients = 0x0211,
    YCbCrPositioning = 0x0213,
    ReferenceBlackWhite = 0x0214,
    Copyright = 0x8298,
    ExifOffset = 0x8769,

    // Exif SubIFD Tags
    ExposureTime = 0x829a,
    FNumber = 0x829d,
    ExposureProgram = 0x8822,
    ISOSpeedRatings = 0x8827,
    ExifVersion = 0x9000,
    DateTimeOriginal = 0x9003,
    DateTimeDigitized = 0x9004,
    ComponentConfiguration = 0x9101,
    CompressedBitsPerPixel = 0x9102,
    ShutterSpeedValue = 0x9201,
    ApertureValue = 0x9202,
    BrightnessValue = 0x9203,
    ExposureBiasValue = 0x9204,
    MaxApertureValue = 0x9205,
    SubjectDistance = 0x9206,
    MeteringMode = 0x9207,
    LightSource = 0x9208,
    Flash = 0x9209,
    FocalLength = 0x920a,
    MakerNote = 0x927c,
    UserComment = 0x9286,
    FlashPixVersion = 0xa000,
    ColorSpace = 0xa001,
    ExifImageWidth = 0xa002,
    ExifImageHeight = 0xa003,
    RelatedSoundFile = 0xa004,
    ExifInteroperabilityOffset = 0xa005,
    FocalPlaneXResolution = 0xa20e,
    FocalPlaneYResolution = 0xa20f,
    FocalPlaneResolutionUnit = 0xa210,
    SensingMethod = 0xa217,
    FileSource = 0xa300,
    SceneType = 0xa301,
    CFAPattern = 0xa302,

    // IFD1 (thumbnail image) Tags
    ImageWidth = 0x0100,
    ImageLength = 0x0101,
    BitsPerSample = 0x0102,
    Compression = 0x0103,
    PhotometricInterpretation = 0x0106,
    StripOffsets = 0x0111,
    SamplesPerPixel = 0x0115,
    RowsPerStrip = 0x0116,
    StripByteCounts = 0x0117,
    PlanarConfiguration = 0x011c,
    JpegIFOffset = 0x0201,
    JpegIFByteCount = 0x0202,
    YCbCrSubSampling = 0x0212,

    // Misc Tags
    NewSubfileType = 0x00fe,
    SubfileType = 0x00ff,
    TransferFunction = 0x012d,
    Artist = 0x013b,
    Predictor = 0x013d,
    TileWidth = 0x0142,
    TileLength = 0x0143,
    TileOffsets = 0x0144,
    TileByteCounts = 0x0145,
    SubIFDs = 0x014a,
    JPEGTables = 0x015b,
    CFARepeatPatternDim = 0x828d,
    CFAPattern2 = 0x828e, // Renamed to avoid conflict with IFD0 tag
    BatteryLevel = 0x828f,
    IPTC_NAA = 0x83bb,
    InterColorProfile = 0x8773,
    SpectralSensitivity = 0x8824,
    GPSInfo = 0x8825,
    OECF = 0x8828,
    Interlace = 0x8829,
    TimeZoneOffset = 0x882a,
    SelfTimerMode = 0x882b,
    FlashEnergy2 = 0x920b, // Renamed to avoid conflict with Exif SubIFD tag
    SpatialFrequencyResponse = 0x920c,
    Noise = 0x920d,
    ImageNumber = 0x9211,
    SecurityClassification = 0x9212,
    ImageHistory = 0x9213,
    SubjectLocation2 = 0x9214, // Renamed to avoid conflict with Exif SubIFD tag
    ExposureIndex2 = 0x9215, // Renamed to avoid conflict with Exif SubIFD tag
    TIFF_EPStandardID = 0x9216,
    SubSecTime = 0x9290,
    SubSecTimeOriginal = 0x9291,
    SubSecTimeDigitized = 0x9292,
    FlashEnergy3 = 0xa20b, // Renamed to avoid conflict
    SpatialFrequencyResponse2 = 0xa20c, // Renamed to avoid conflict
    SubjectLocation3 = 0xa214, // Renamed to avoid conflict
    ExposureIndex3 = 0xa215 // Renamed to avoid conflict
}

enum TagFormat
{
    Byte = 1,
    AsciiString = 2,
    UShort = 3,
    LONG = 4,
    RATIONAL = 5,
    SBYTE = 6,
    UNDEFINED = 7,
    SSHORT = 8,
    SLONG = 9,
    SRATIONAL = 10,
    FLOAT = 11,
    DOUBLE = 12
};


