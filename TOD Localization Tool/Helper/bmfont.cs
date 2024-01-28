using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace Helper
{
    public class bmfont
    {

        /// <summary>
        /// This tag holds information on how the font was generated.
        /// </summary>
        public struct info
        {
            byte chunkID;//1
            int chunkDataSize;

            /// <summary>
            /// size	The size of the true type font.
            /// </summary>
            public short fontSize;

            /// <summary>
            /// italic	The font is italic.
            /// charset The name of the OEM charset used(when not unicode).
            /// unicode Set to 1 if it is the unicode charset.
            /// </summary>
            public byte bitField;  //bit 0: smooth, bit 1: unicode, bit 2: italic, bit 3: bold, bit 4: fixedHeigth, bits 5-7: reserved

            /// <summary>
            /// The font is bold.
            /// </summary>
            public bool bold;

            /// <summary>
            ///  The font is italic.
            /// </summary>
            public bool italic;

            /// <summary>
            /// Set to 1 if smoothing was turned on.
            /// </summary>
            public bool smooth;

            /// <summary>
            /// Set to 1 if it is the unicode charset.
            /// </summary>
            public bool unicode;

            /// <summary>
            /// charset	The name of the OEM charset used (when not unicode).
            /// </summary>
            public byte charSet;

            /// <summary>
            /// stretchH	The font height stretch in percentage. 100% means no stretch.
            /// </summary>
            public short stretchH;

            /// <summary>
            /// aa	The supersampling level used. 1 means no supersampling was used.
            /// </summary>
            public byte aa;
            /// <summary>
            /// padding	The padding for each character (up, right, down, left).
            /// </summary>
            public byte paddingUp;

            /// <summary>
            /// padding	The padding for each character (up, right, down, left).
            /// </summary>
            public byte paddingRight;

            /// <summary>
            /// padding	The padding for each character (up, right, down, left).
            /// </summary>
            public byte paddingDown;

            /// <summary>
            /// padding	The padding for each character (up, right, down, left).
            /// </summary>
            public byte paddingLeft;

            /// <summary>
            /// spacing	The spacing for each character (horizontal, vertical).
            /// </summary>
            public byte spacingHoriz;

            /// <summary>
            /// spacing	The spacing for each character (horizontal, vertical).
            /// </summary>
            public byte spacingVert;

            /// <summary>
            /// outline	The outline thickness for the characters.
            /// </summary>
            public byte outline;

            /// <summary>
            /// face This is the name of the true type font.
            /// </summary>
            public string fontName;
        }

        /// <summary>
        /// This tag holds information common to all characters.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct common
        {
            byte chunkID;//2
            int chunkDataSize;
            /// <summary>
            /// This is the distance in pixels between each line of text.
            /// </summary>
            public short lineHeight;

            /// <summary>
            /// The number of pixels from the absolute top of the line to the base of the characters.
            /// </summary>
            public short Base;

            /// <summary>
            /// The width of the texture, normally used to scale the x pos of the character image.
            /// </summary>
            public short scaleW;

            /// <summary>
            /// The height of the texture, normally used to scale the y pos of the character image.
            /// </summary>
            public short scaleH;

            /// <summary>
            /// The number of texture pages included in the font.
            /// </summary>
            public short pages;

            /// <summary>
            /// Set to 1 if the monochrome characters have been packed into each of the texture channels. In this case alphaChnl describes what is stored in each channel.
            /// </summary>
            public byte packed;

            /// <summary>
            /// Set to 0 if the channel holds the glyph data, 1 if it holds the outline, 2 if it holds the glyph and the outline, 3 if its set to zero, and 4 if its set to one.
            /// </summary>
            public byte alphaChnl;
            /// <summary>
            /// Set to 0 if the channel holds the glyph data, 1 if it holds the outline, 2 if it holds the glyph and the outline, 3 if its set to zero, and 4 if its set to one.
            /// </summary>
            public byte redChnl;
            /// <summary>
            /// Set to 0 if the channel holds the glyph data, 1 if it holds the outline, 2 if it holds the glyph and the outline, 3 if its set to zero, and 4 if its set to one.
            /// </summary>
            public byte greenChnl;
            /// <summary>
            /// Set to 0 if the channel holds the glyph data, 1 if it holds the outline, 2 if it holds the glyph and the outline, 3 if its set to zero, and 4 if its set to one.
            /// </summary>
            public byte blueChnl;
        }


        /// <summary>
        /// This tag gives the name of a texture file. There is one for each page in the font.
        /// </summary>
        public struct page
        {
            /// <summary>
            /// 	The texture file name.
            /// </summary>
            public string fileName;
        }

        /// <summary>
        /// This tag describes on character in the font. There is one for each included character in the font.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class Char
        {

            /// <summary>
            /// The character id.
            /// </summary>
            public int id;

            /// <summary>
            /// The left position of the character image in the texture.
            /// </summary>
            public short x;

            /// <summary>
            /// The top position of the character image in the texture.
            /// </summary>
            public short y;

            /// <summary>
            /// The width of the character image in the texture.
            /// </summary>
            public short width;

            /// <summary>
            /// The height of the character image in the texture.
            /// </summary>
            public short height;

            /// <summary>
            /// How much the current position should be offset when copying the image from the texture to the screen.
            /// </summary>
            public short xoffset;

            /// <summary>
            /// 	How much the current position should be offset when copying the image from the texture to the screen.
            /// </summary>
            public short yoffset;

            /// <summary>
            /// 	How much the current position should be advanced after drawing the character.
            /// </summary>
            public short xadvance;

            /// <summary>
            /// The texture page where the character image is found.
            /// </summary>
            public byte page;

            /// <summary>
            /// The texture channel where the character image is found (1 = blue, 2 = green, 4 = red, 8 = alpha, 15 = all channels).
            /// </summary>
            public byte chnl;


            public Char() { }

            public Char(Char Table)
            {
                this.id = Table.id;
                this.x = Table.x;
                this.y = Table.y;
                this.width = Table.width;
                this.height = Table.height;
                this.xoffset = Table.xoffset;
                this.yoffset = Table.yoffset;
                this.xadvance = Table.xadvance;
                this.page = Table.page;
                this.chnl = Table.chnl;
            }
        }



        public struct Font
        {
            public info info;
            public common common;
            public List<page> pages;
            public List<Char> Chars;
            public List<byte[]> Textures;
        }




        public static Font GetFont(string FntPath, bool LoadTextures = false)
        {

            FStream mStream = new FStream(FntPath, FileMode.Open, FileAccess.Read);
            if (mStream.GetIntValue(false) == 0x3464D42)//binary
            {
                return LoadPages(FntPath, ReadBinaryFile(mStream), LoadTextures);
            }

            mStream.Position = 0;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(mStream);//if can't load it so it will be text or something else
                return LoadPages(FntPath, ReadXmlFile(doc), LoadTextures);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            //text File
            throw new Exception("No implementation for this font type!\nfile: " + FntPath);
            // no need for it :)
        }


        private static Font LoadPages(string fntPath, Font font, bool LoadTextures)
        {
            if (LoadTextures)
            {
                string Dir = Path.GetDirectoryName(fntPath);
                font.Textures = new List<byte[]>();

                foreach (var page in font.pages)
                {
                    font.Textures.Add(File.ReadAllBytes(Path.Combine(Dir, page.fileName)));
                }
            }
            return font;
        }

        private static Font ReadBinaryFile(IStream stream)
        {
            stream.Seek(4);//BFM\x3

            Font font = new Font();
            font.info = new info();

            //info
            stream.Skip(5);
            font.info.fontSize = stream.GetShortValue();
            font.info.bitField = stream.GetByteValue();

            font.info.bold = (font.info.bitField & 0x08) != 0;
            font.info.italic = (font.info.bitField & 0x04) != 0;
            font.info.smooth = (font.info.bitField & 0x01) != 0;
            font.info.unicode = (font.info.bitField & 0x02) != 0;

            font.info.charSet = stream.GetByteValue();
            font.info.stretchH = stream.GetShortValue();
            font.info.aa = stream.GetByteValue();
            font.info.paddingUp = stream.GetByteValue();
            font.info.paddingRight = stream.GetByteValue();
            font.info.paddingDown = stream.GetByteValue();
            font.info.paddingLeft = stream.GetByteValue();
            font.info.spacingHoriz = stream.GetByteValue();
            font.info.spacingVert = stream.GetByteValue();
            font.info.outline = stream.GetByteValue();
            font.info.fontName = stream.GetStringValueN();

            //common
            font.common = stream.GetStructureValues<common>();

            //pages
            stream.Skip(5);
            font.pages = new List<page>();

            for (int i = 0; i < font.common.pages; i++)
            {
                font.pages.Add(new page() { fileName = stream.GetStringValueN() });
            }

            //chars
            stream.Skip(1);
            int CharsCount = stream.GetIntValue() / 20;
            font.Chars = new List<Char>();

            for (int i = 0; i < CharsCount; i++)
            {
                font.Chars.Add(stream.GetStructureValues<Char>());
            }


            AddArabicIsolatedChars(font);
            return font;

        }

        protected static string GetAttributesValue(XmlNode xmlnode, string Attribute)
        {
            if (xmlnode == null || xmlnode.Attributes[Attribute] == null)
                return null;

            return xmlnode.Attributes[Attribute].Value;
        }

        private static Font ReadXmlFile(XmlDocument doc)
        {
            Font font = new Font();
            font.info = new info();

            var infoNode = doc.SelectSingleNode("/font/info");
            font.info.fontName = GetAttributesValue(infoNode, "face");
            font.info.fontSize = short.TryParse(GetAttributesValue(infoNode, "size"), out short fontSize) ? fontSize : (short)0;
            font.info.bold = GetAttributesValue(infoNode, "bold") == "1";
            font.info.italic = GetAttributesValue(infoNode, "italic") == "1";
            font.info.unicode = GetAttributesValue(infoNode, "unicode") == "1";
            font.info.smooth = GetAttributesValue(infoNode, "smooth") == "1";
            font.info.charSet = byte.TryParse(GetAttributesValue(infoNode, "charset"), out byte charSet) ? charSet : (byte)0;
            font.info.stretchH = short.TryParse(GetAttributesValue(infoNode, "stretchH"), out short stretchH) ? stretchH : (short)0;
            font.info.aa = byte.TryParse(GetAttributesValue(infoNode, "aa"), out byte aa) ? aa : (byte)0;

            if (GetAttributesValue(infoNode, "padding") != null)
            {
                string[] Padding = (GetAttributesValue(infoNode, "padding") + ",,,,").Split(',');
                font.info.paddingUp = byte.TryParse(Padding[0], out byte paddingUp) ? paddingUp : (byte)0;
                font.info.paddingRight = byte.TryParse(Padding[1], out byte paddingRight) ? paddingRight : (byte)0;
                font.info.paddingDown = byte.TryParse(Padding[2], out byte paddingDown) ? paddingDown : (byte)0;
                font.info.paddingLeft = byte.TryParse(Padding[3], out byte paddingLeft) ? paddingLeft : (byte)0;
            }

            if (GetAttributesValue(infoNode, "spacing") != null)
            {
                string[] spacing = (GetAttributesValue(infoNode, "spacing") + ",,").Split(',');
                font.info.spacingHoriz = byte.TryParse(spacing[0], out byte spacingHoriz) ? spacingHoriz : (byte)0;
                font.info.spacingVert = byte.TryParse(spacing[1], out byte spacingVert) ? spacingVert : (byte)0;
            }

            font.info.outline = byte.TryParse(GetAttributesValue(infoNode, "outline"), out byte outline) ? outline : (byte)0;

            //common
            font.common = new common();
            var commonNode = doc.SelectSingleNode("/font/common");
            font.common.lineHeight = short.TryParse(GetAttributesValue(commonNode, "lineHeight"), out short lineHeight) ? lineHeight : (short)0;
            font.common.Base = short.TryParse(GetAttributesValue(commonNode, "base"), out short baseValue) ? baseValue : (short)0;
            font.common.scaleW = short.TryParse(GetAttributesValue(commonNode, "scaleW"), out short scaleW) ? scaleW : (short)0;
            font.common.scaleH = short.TryParse(GetAttributesValue(commonNode, "scaleH"), out short scaleH) ? scaleH : (short)0;
            font.common.pages = short.TryParse(GetAttributesValue(commonNode, "pages"), out short pages) ? pages : (short)0;
            font.common.packed = byte.TryParse(GetAttributesValue(commonNode, "packed"), out byte packed) ? packed : (byte)0;
            font.common.alphaChnl = byte.TryParse(GetAttributesValue(commonNode, "alphaChnl"), out byte alphaChnl) ? alphaChnl : (byte)0;
            font.common.redChnl = byte.TryParse(GetAttributesValue(commonNode, "redChnl"), out byte redChnl) ? redChnl : (byte)0;
            font.common.greenChnl = byte.TryParse(GetAttributesValue(commonNode, "greenChnl"), out byte greenChnl) ? greenChnl : (byte)0;
            font.common.blueChnl = byte.TryParse(GetAttributesValue(commonNode, "blueChnl"), out byte blueChnl) ? blueChnl : (byte)0;

            //Pages
            var pagesNode = doc.SelectSingleNode("/font/pages");
            font.pages = new List<page>();
            for (int i = 0; i < font.common.pages; i++)
            {
                var pageNode = pagesNode.ChildNodes[i];
                font.pages.Add(new page()
                {
                    fileName = GetAttributesValue(pageNode, "file")
                });
            }

            //chars
            var charsNode = doc.SelectSingleNode("/font/chars");
            font.Chars = new List<Char>();

            for (int i = 0; i < charsNode.ChildNodes.Count; i++)
            {
                var charNode = charsNode.ChildNodes[i];
                Char charinfo = new Char();

                charinfo.id = int.TryParse(GetAttributesValue(charNode, "id"), out int id) ? id : 0;
                charinfo.x = short.TryParse(GetAttributesValue(charNode, "x"), out short x) ? x : (short)0;
                charinfo.y = short.TryParse(GetAttributesValue(charNode, "y"), out short y) ? y : (short)0;
                charinfo.width = short.TryParse(GetAttributesValue(charNode, "width"), out short width) ? width : (short)0;
                charinfo.height = short.TryParse(GetAttributesValue(charNode, "height"), out short height) ? height : (short)0;
                charinfo.xoffset = short.TryParse(GetAttributesValue(charNode, "xoffset"), out short xoffset) ? xoffset : (short)0;
                charinfo.yoffset = short.TryParse(GetAttributesValue(charNode, "yoffset"), out short yoffset) ? yoffset : (short)0;
                charinfo.xadvance = short.TryParse(GetAttributesValue(charNode, "xadvance"), out short xadvance) ? xadvance : (short)0;
                charinfo.page = byte.TryParse(GetAttributesValue(charNode, "page"), out byte page) ? page : (byte)0;
                charinfo.chnl = byte.TryParse(GetAttributesValue(charNode, "chnl"), out byte chnl) ? chnl : (byte)0;

                font.Chars.Add(charinfo);
            }
            AddArabicIsolatedChars(font);
            return font;
        }




        private static Dictionary<char, char> ArabicChars = new Dictionary<char, char>()
        {
        //Arabic Presentation Forms-B
        {'\u0621','\uFE80'},//ﺀ
        {'\u0622','\uFE81'},//ﺁ
        {'\u0623','\uFE83'},//ﺃ
        {'\u0624','\uFE85'},//ﺅ
        {'\u0625','\uFE87'},//ﺇ
        {'\u0626','\uFE89'},//ﺉ
        {'\u0627','\uFE8D'},//ﺍ
        {'\u0628','\uFE8F'},//ﺏ
        {'\u062A','\uFE95'},//ﺕ
        {'\u062B','\uFE99'},//ﺙ
        {'\u062C','\uFE9D'},//ﺝ
        {'\u062D','\uFEA1'},//ﺡ
        {'\u062E','\uFEA5'},//ﺥ
        {'\u062F','\uFEA9'},//ﺩ
        {'\u0630','\uFEAB'},//ﺫ
        {'\u0631','\uFEAD'},//ﺭ
        {'\u0632','\uFEAF'},//ﺯ
        {'\u0633','\uFEB1'},//ﺱ
        {'\u0634','\uFEB5'},//ﺵ
        {'\u0635','\uFEB9'},//ﺹ
        {'\u0636','\uFEBD'},//ﺽ
        {'\u0637','\uFEC1'},//ﻁ
        {'\u0638','\uFEC5'},//ﻅ
        {'\u0639','\uFEC9'},//ﻉ
        {'\u063A','\uFECD'},//ﻍ
        {'\u0641','\uFED1'},//ﻑ
        {'\u0642','\uFED5'},//ﻕ
        {'\u0643','\uFED9'},//ﻙ
        {'\u0644','\uFEDD'},//ﻝ
        {'\u0645','\uFEE1'},//ﻡ
        {'\u0646','\uFEE5'},//ﻥ
        {'\u0647','\uFEE9'},//ﻩ
        {'\u0648','\uFEED'},//ﻭ
        {'\u064A','\uFEF1'},//ﻱ
        {'\u0649','\uFEEF'},//ﻯ
        {'\u0629','\uFE93'}, //ﺓ
            
        // Arabic Presentation Forms-A
        {'\u0671', '\uFB50'},//ٱ
        {'\u0679', '\uFB66'},//ٹ
        {'\u067A', '\uFB5E'},//ٺ
        {'\u067B' ,'\uFB52'},//ٻ
        {'\u067E', '\uFB56'},//پ
        {'\u067F', '\uFB62'},//ٿ
        {'\u0680', '\uFB5A'},//ڀ
        {'\u0683', '\uFB76'},//ڃ
        {'\u0684', '\uFB72'},//ڄ
        {'\u0686', '\uFB7A'},//چ
        {'\u0687', '\uFB7E'},//ڇ
        {'\u0688', '\uFB88'},//ڈ
        {'\u068C', '\uFB84'},//ڌ
        {'\u068D', '\uFB82'},//ڍ
        {'\u068E', '\uFB86'},//ڎ
        {'\u0691', '\uFB8C'},//ڑ
        {'\u0698', '\uFB8A'},//ژ
        {'\u06A4', '\uFB6A'},//ڤ
        {'\u06A6', '\uFB6E'},//ڦ
        {'\u06A9', '\uFB8E'},//ک
        {'\u06AD', '\uFBD3'},//ڭ
        {'\u06AF', '\uFB92'},//گ
        {'\u06B1', '\uFB9A'},//ڱ
        {'\u06B3', '\uFB96'},//ڳ
        {'\u06BB', '\uFBA0'},//ڻ
        {'\u06BE', '\uFBAA'},//ھ
        {'\u06C0', '\uFBA4'},//ۀ
        {'\u06C1', '\uFBA6'},//ہ
        {'\u06C5', '\uFBE0'},//ۅ
        {'\u06C6', '\uFBD9'},//ۆ
        {'\u06C7', '\uFBD7'},//ۇ
        {'\u06C8', '\uFBDB'},//ۈ
        {'\u06C9', '\uFBE2'},//ۉ
        {'\u06CB', '\uFBDE'},//ۋ
        {'\u06CC', '\uFBFC'},//ی
        {'\u06D0', '\uFBE4'},//ې
        {'\u06D2', '\uFBAE'},//ے
        {'\u06D3', '\uFBB0'} //ۓ
        };


        private static void AddArabicIsolatedChars(Font font)
        {
            foreach (var ch in ArabicChars)
            {
                var IsContainGeneralUnicodeChar = font.Chars.FindIndex(x => x.id == ch.Key);
                if (IsContainGeneralUnicodeChar != -1)
                {

                    var IsContainIsolatedChar = font.Chars.FindIndex(x => x.id == ch.Value);
                    if (IsContainIsolatedChar == -1)
                    {
                        var newChar = new Char(font.Chars[IsContainGeneralUnicodeChar]);
                        newChar.id = ch.Value;
                        font.Chars.Add(newChar);
                    }
                }
            }
        }












    }
}
