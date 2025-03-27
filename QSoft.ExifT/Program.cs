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
(TagFormat format, int length, int begin) ParseTiff()
{
    //https://www.media.mit.edu/pia/Research/deepview/exif.html
    var enterycount = BitConverter.ToInt16(br.ReadBytes(2).Reverse().ToArray(), 0);
    var tagname = BitConverter.ToInt16(br.ReadBytes(2).Reverse().ToArray(), 0);
    var tagformat = (TagFormat)BitConverter.ToInt16(br.ReadBytes(2).Reverse().ToArray(), 0);
    var tagatalength = BitConverter.ToInt32(br.ReadBytes(4).Reverse().ToArray(), 0);
    var tagoffset = BitConverter.ToInt32(br.ReadBytes(4).Reverse().ToArray(), 0);
    var curpos = br.BaseStream.Position;
    br.BaseStream.Position = exifstart+ tagoffset;
    //var ascii = br.ReadBytes(tagcount);
    //var asciiStr = Encoding.ASCII.GetString(ascii);

    return (tagformat, tagatalength, 0);
}


void ascii()
{

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


