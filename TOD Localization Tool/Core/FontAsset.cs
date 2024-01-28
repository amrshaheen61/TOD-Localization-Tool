using Helper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static Helper.DDSCooker;

namespace TOD_Localization_Tool
{
    public class FontAsset : AssetHeader
    {

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct FontHeader
        {
            public int Unkown;
            //From this offset + CharsMapOffset
            public int FontBufferOffset;

            //From this offset + StartOffset
            public int StartOffset;

        }


        FontHeader fontheader;
        public long StartOffset;
        public long EndOffset;
        public long ExtraSize;

        public IStream fontData;
        public byte[] Footer;

        public List<byte> Texture;

        public IStream StreamFile;
        public Entry AssetEntry;

        public FontAsset(IStream stream, Entry assetentry) : base(stream)
        {
            StreamFile = stream;
            fontheader = stream.GetStructureValues<FontHeader>();

            StartOffset = (stream.GetPosition() - 4) + fontheader.StartOffset;//block Start
            stream.Seek(StartOffset);

            fontData = new MStream(stream.GetBytes((int)(fontheader.FontBufferOffset - (StartOffset - (BlockStartOffset + 32)))));
            Footer = stream.GetBytes(20);
            SkipPadding();
            EndOffset = stream.GetPosition();

            AssetEntry = assetentry;
        }


        public static FontAsset ReadFont(IStream stream, Entry assetentry, List<byte> Texture)
        {
            var asset = new FontAsset(stream, assetentry);
            asset.Texture = Texture;
            return asset;
        }




        struct FontData
        {
            public int unkown;
            public int lineHeight;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            int[] Unkown;

            public float lineHeight2;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            int[] Unkown2;
            public int CharCount;
            public int CharCount2;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            int[] Unkown3;

            public int wight;
            public int Hight;

            public int wight2;
            public int Hight2;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            int[] Unkown4;

            public void SetWightAndHight(int scaleW, int scaleH)
            {
                this.wight = scaleW;
                this.Hight = scaleH;

                this.wight2 = scaleW;
                this.Hight2 = scaleH;
            }


            public void SetlineHeight(int lineHeight)
            {
                this.lineHeight = lineHeight;
                this.lineHeight2 = lineHeight / (float)Hight;
            }

            public void SetCharCount(int CharCount)
            {
                this.CharCount = CharCount;
                this.CharCount2 = CharCount;
            }


        }



        struct CharInfo
        {
            public uint Character;
            public float xoffset;
            public float yoffset;
            public float xadvace;
            public float x;
            public float y;
            public float width;
            public int IsSupportColors;
        }



        public string GetFontData()
        {
            fontData.Seek(0);
            FontData fontDataHeader = fontData.GetStructureValues<FontData>();


            List<CharInfo> chars = new List<CharInfo>();

            for (int i = 0; i < fontDataHeader.CharCount; i++)
            {
                chars.Add(fontData.GetStructureValues<CharInfo>());
            }

            chars = chars.OrderBy(x => x.Character).ToList();

            string XmlFile = $"<font>\r\n" +
                $"  <info />" +
                $"\r\n  <common lineHeight=\"{fontDataHeader.lineHeight}\" scaleW=\"{fontDataHeader.wight}\" scaleH=\"{fontDataHeader.Hight}\" pages=\"1\" />\r\n  " +
                $"<pages>\r\n    " +
                $"<page id=\"0\" file=\"{Path.GetFileNameWithoutExtension(FilePath)}.dds\" />\r\n  " +
                $"</pages>\r\n  " +
                $"<chars count=\"{fontDataHeader.CharCount}\">\r\n";

            foreach (CharInfo charInfo in chars)
            {
                XmlFile += $"    <char id=\"{charInfo.Character}\" x=\"{charInfo.x * fontDataHeader.wight}\" y=\"{charInfo.y * fontDataHeader.Hight}\" width=\"{charInfo.width * fontDataHeader.wight}\" height=\"{fontDataHeader.lineHeight}\" xoffset=\"{charInfo.xoffset}\" yoffset=\"{charInfo.yoffset}\" xadvance=\"{charInfo.xadvace}\" page=\"0\" chnl=\"15\" />\r\n";
            }

            XmlFile += "  </chars>\r\n</font>";
            return XmlFile;
        }



        public byte[] GetFontTexture()
        {
            fontData.Seek(0);
            FontData fontDataHeader = fontData.GetStructureValues<FontData>();
            return DDSCooker.TexToDds(Texture.ToArray(), DDSFormat.r8g8b8a8_unorm_srgb, fontDataHeader.Hight, fontDataHeader.wight);
        }



        public void EditFile(string FntFile)
        {

            var font = bmfont.GetFont(FntFile, true);
            this.Texture.Clear();

            var dds = DDSCooker.DdsToTex(font.Textures[0]);


            this.Texture.AddRange(dds.Data);

            fontData.Seek(0);
            FontData fontDataHeader = fontData.GetStructureValues<FontData>();
            fontDataHeader.SetWightAndHight(font.common.scaleW, font.common.scaleH);
            fontDataHeader.SetlineHeight(font.common.lineHeight);
            fontDataHeader.SetCharCount(font.Chars.Count);

            fontData.Seek(0);
            fontData.SetStructureValus(fontDataHeader);
            fontData.SetLength(fontData.GetPosition());


            for (int i = 0; i < font.Chars.Count; i++)
            {
                fontData.SetIntValue(font.Chars[i].id);
                fontData.SetFloatValue(font.Chars[i].xoffset);
                fontData.SetFloatValue(font.Chars[i].yoffset);

#if ARABIC
                if (tashkeelChars.Contains(font.Chars[i].id))
                {
                    fontData.SetFloatValue(0);
                }
                else if (!IgnoreChars.Contains(font.Chars[i].id))
                {
                    fontData.SetFloatValue(font.Chars[i].xadvance);
                }
                else if (FntFile.EndsWith("ODPS2.font", System.StringComparison.InvariantCulture) || FntFile.EndsWith("ODPC.font", System.StringComparison.InvariantCulture))
                {
                    fontData.SetFloatValue(font.Chars[i].xadvance - 1);
                }
                else
                    fontData.SetFloatValue(font.Chars[i].xadvance - 4);
#else
                fontData.SetFloatValue(font.Chars[i].xadvance);
#endif



                fontData.SetFloatValue(font.Chars[i].x / (float)font.common.scaleW);
                fontData.SetFloatValue(font.Chars[i].y / (float)font.common.scaleH);
                fontData.SetFloatValue(font.Chars[i].width / (float)font.common.scaleW);

                if (TextAsset.CodesDictionary.ContainsKey((char)font.Chars[i].id))
                    fontData.SetIntValue(1);
                else
                    fontData.SetIntValue(0);

            }




            StreamFile.Seek(StartOffset);
            StreamFile.DeleteBytes((int)(EndOffset - StartOffset));
            StreamFile.InsertBytes(fontData.ToArray());
            StreamFile.InsertBytes(Footer);

            long End = StreamFile.GetPosition();

            StreamFile.Seek(BlockStartOffset + 32);
            StreamFile.SetIntValue((int)(End - BlockStartOffset) - 32 - 20);
            StreamFile.Seek(End);

            ExtraSize = End - EndOffset;

            AssetEntry.UpdateOffsets(ExtraSize);
        }


#if ARABIC
        List<int> IgnoreChars = new List<int>() {
        0xFEB3,
0xFEB3,
0xFEB3,
0xFEB3,
0xFEB3,
0xFE8C,
0xFEB3,
0xFE92,
0xFEB3,
0xFE98,
0xFE9C,
0xFEA0,
0xFEA4,
0xFEA8,
0xFEB3,
0xFEB3,
0xFEB3,
0xFEB3,
0xFEB4,
0xFEB8,
0xFEBC,
0xFEC0,
0xFEC4,
0xFEC8,
0xFECC,
0xFED0,
0x0640,
0xFED4,
0xFED8,
0xFEDC,
0xFEE0,
0xFEE4,
0xFEE8,
0xFEEC,
0xFEB3,
0xFEB3,
0xFEF4,
0xFEB3,
0xFB69,
0xFB61,
0xFB55,
0xFB59,
0xFB65,
0xFB5D,
0xFB79,
0xFB75,
0xFB7D,
0xFB81,
0xFEB3,
0xFEB3,
0xFEB3,
0xFEB3,
0xFEB3,
0xFEB3,
0xFB6D,
0xFB71,
0xFB91,
0xFBD6,
0xFB95,
0xFB9D,
0xFB99,
0xFBA3,
0xFBAD,
0xFEB3,
0xFBA9,
0xFEB3,
0xFEB3,
0xFEB3,
0xFEB3,
0xFEB3,
0xFEB3,
0xFBFF,
0xFBE7,
0xFEB3,
0xFEB3,
0xFEB3,
0xFE82,
0xFE84,
0xFE86,
0xFE88,
0xFE8A,
0xFE8E,
0xFE90,
0xFE94,
0xFE96,
0xFE9A,
0xFE9E,
0xFEA2,
0xFEA6,
0xFEAA,
0xFEAC,
0xFEAE,
0xFEB0,
0xFEB2,
0xFEB6,
0xFEBA,
0xFEBE,
0xFEC2,
0xFEC6,
0xFECA,
0xFECE,
0x0640,
0xFED2,
0xFED6,
0xFEDA,
0xFEDE,
0xFEE2,
0xFEE6,
0xFEEA,
0xFEEE,
0xFEF0,
0xFEF2,
0xFB51,
0xFB67,
0xFB5F,
0xFB53,
0xFB57,
0xFB63,
0xFB5B,
0xFB77,
0xFB73,
0xFB7B,
0xFB7F,
0xFB89,
0xFB85,
0xFB83,
0xFB87,
0xFB8D,
0xFB8B,
0xFB6B,
0xFB6F,
0xFB8F,
0xFBD4,
0xFB93,
0xFB9B,
0xFB97,
0xFBA1,
0xFBAB,
0xFBA5,
0xFBA7,
0xFBE1,
0xFBDA,
0xFBD8,
0xFBDC,
0xFBE3,
0xFBDF,
0xFBFD,
0xFBE5,
0xFBAF,
0xFBB1,
      };
        //حروف التشكيل
        List<int> tashkeelChars = new List<int>() {

0x064C,
0x064D,
0x064B,
0x064C,
0x064D,
0x064F,
0x0650,
0x064D,
0x0650,

        };

#endif






    }
}
