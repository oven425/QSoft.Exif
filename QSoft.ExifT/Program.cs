// See https://aka.ms/new-console-template for more information


using System.IO;
using System.Text;

var stream = System.IO.File.OpenRead("../../../54362611843_84a763ea2c_o.jpg");
long start_pos = stream.Position;
BinaryReader br = new BinaryReader(stream);

//https://www.media.mit.edu/pia/Research/deepview/exif.html

// find APP1
int applen = 0;
while (true)
{
    byte[] bb = br.ReadBytes(2);
    if (bb[0] == 0xFF && bb[1] == 0xE1)
    {
        applen = br.ReadInt16();
        var exif_header = br.ReadBytes(6);
        var ss = Encoding.ASCII.GetString(exif_header);
        var tiff_header = br.ReadBytes(8);
        var ss1 = Encoding.ASCII.GetString(tiff_header);
        break;
    }
}

void ParseTiff()
{

}



