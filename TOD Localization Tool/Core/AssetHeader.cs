using Helper;
using System.Runtime.InteropServices;

namespace TOD_Localization_Tool
{
    public enum eAssetType : int
    {
        TEXTURE = 0,
        FONT,
        TEXT,
        MODEL,
        FRAGMENT,
        MOVIE,
        CUTSCENE,
        SOUND,
        STREAMEDSOUNDINFO,
        ANIMATION,
        MESHCOLOR
    }



    public class AssetHeader
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public eAssetType assettype;
            //From this offset + NameOffset
            public int NameOffset;
            public int GlobalId;
            public int Unkown_1;
            public long EngineTimestamp;
            public int Flags;
        }





        public Header header;
        public long BlockStartOffset;

        public string FilePath;

        IStream MStream;
        public AssetHeader(IStream stream)
        {
            MStream = stream;
            BlockStartOffset = stream.GetPosition();
            header = stream.GetStructureValues<Header>();


            FilePath = stream.GetStringValueN(false, (int)(BlockStartOffset + 4 + header.NameOffset));

            //   Console.WriteLine(FilePath);
        }



        public void SkipPadding()
        {
            if ((MStream.GetPosition() % 16) != 0)
            {
                int paddding = (int)(16 - (MStream.GetPosition() % 16));
                MStream.Skip(paddding);

            }
        }

    }
}
