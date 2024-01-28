using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;


namespace TOD_Localization_Tool
{
    public class TextAsset : AssetHeader
    {

        static public Dictionary<char, string> CodesDictionary = new Dictionary<char, string>()
        {
           {'Θ', "[TOTAL_OVERDOSE_LOGO]"},
           {'α', "[XBOX_A_BUTTON]"},
           {'β', "[XBOX_B_BUTTON]"},
           {'λ', "[XBOX_X_BUTTON]"},
           {'π', "[XBOX_Y_BUTTON]"},
           {'†', "[XBOX_START_BUTTON]"},
           {'‡', "[XBOX_BACK_BUTTON]"},
           {'⅓', "[PS2_START_BUTTON]"},
           {'⅔', "[PS2_SELECT_BUTTON]"},
           {'⅛', "[PS2_L1_BUTTON]"},
           {'⅜', "[PS2_R1_BUTTON]"},
           {'⅝', "[PS2_R2_BUTTON]"},
           {'⅞', "[PS2_L2_BUTTON]"},
           {'←', "[XBOX_LEFT_BUTTON]"},
           {'↑', "[XBOX_UP_BUTTON]"},
           {'→', "[XBOX_RIGHT_BUTTON]"},
           {'↓', "[XBOX_BOTTOM_BUTTON]"},
           {'┌', "[PS2_X_BUTTON]"},
           {'┐', "[PS2_O_BUTTON]"},
           {'└', "[PS2_TRIANGLE_BUTTON]"},
           {'┘', "[PS2_SQUARE_BUTTON]"},
           {'├', "[PS2_RIGHT_BUTTON]"},
           {'┤', "[PS2_LEFT_BUTTON]"},
           {'┬', "[PS2_BOTTOM_BUTTON]"},
           {'┴', "[PS2_UP_BUTTON]"},
           {'╟', "[XBOX_DX_BUTTON]"},
           {'╠', "[XBOX_D_BUTTON]"},
           {'╡', "[XBOX_G_BUTTON]"},
           {'╢', "[XBOX_SX_BUTTON]"},
           {'╣', "[XBOX_I_BUTTON]"},
           {'░', "[TWO_GUNS_ICON]"},
           {'▒', "[RED_DIAMETER_ICON]"},
           {'▓', "[WHITE_DIAMETER_ICON]"},
           {'■', "[MAN_ICON]"},
           {'□', "[STAR_ICON]"},
           {'▪', "[RED_SQURE_ICON]"},
           {'▫', "[RED_CIRCLE_AND_WHITE_ICON]"},
           {'►', "[XBOX_L_BUTTON]"},
           {'◄', "[XBOX_R_BUTTON]"},
           {'○', "[RED_DOT_ICON]"},
           {'●', "[YELLOW_DOT_ICON"},
           {'◘', "[XBOX_WHITE_BUTTON]"},
           {'◙', "[XBOX_BLACK_BUTTON]"},
           {'☺', "[CITY_TOWERS_ICON]"},
           {'☻', "[CITY_CASTLE_ICON]"},
           {'☼', "[CITY_CHURCH_ICON]"},
           {'♀', "[CITY_TOWERS2_ICON]"},
           {'♂', "[CITY_FACTORES_ICON]"},
           {'♠', "[CITY_US_ICON]"},
           {'♣', "[CITY_SHIPPORT_ICON]"},
           {'♥', "[CITY_WRESTRING_ICON]"},
           {'♦', "[CITY_DESERT_ICON]"},
           {'♪', "[CITY_TOWERS3_ICON]"},
           {'♫', "[CITY_FACE_ICON]"}
        };





        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TextHeader
        {
            public int Unkown;

            //CharMap Block
            //From this offset + CharsMapOffset
            public int CharsMapOffset;
            public int Unkown_1;
            public int Unkown_2;
            public int Unkown_3;

            //CharInfo Block
            //From this offset + CharInfoOffset
            public int CharInfoOffset;
            public int TextCount;
            public int TextCount_1;
            public int Unkown_4;

            //GenralInfo Block
            public int Val1;
            public int Val2;
            public int Val3;
            public int Unkown_5;

            //From this offset + StartOffset
            public int StartOffset;

            //AssetHeader.Header.Unkown_1
            public int Unkown_6;
        }



        class CharNode
        {

            public CharNode Right;
            public CharNode Left;
            public int CharNodeOffset;

            public ushort Item;
            public ushort Item2;

            public long Offset;
            public MStream Stream;

            public CharNode PreviousNode;


            public CharNode ReadNodeValue(long offset)
            {
                Stream.SetPosition(offset);
                var charnode = new CharNode();
                charnode.Offset = offset;
                charnode.Right = new CharNode { CharNodeOffset = Stream.GetIntValue() };
                charnode.Left = new CharNode { CharNodeOffset = Stream.GetIntValue() };
                charnode.Item = Stream.GetUShortValue();
                charnode.Item2 = Stream.GetUShortValue();
                charnode.Stream = Stream;
                return charnode;
            }

            public CharNode GetRightCharNode()
            {
                CharNode temp = null;
                try
                {
                    temp = ReadNodeValue(Offset + Right.CharNodeOffset);
                    temp.PreviousNode = this;
                }
                catch
                {
                    return temp;
                }
                return temp;
            }

            public CharNode GetLeftCharNode()
            {
                CharNode temp = null;
                try
                {
                    temp = ReadNodeValue(Offset + Left.CharNodeOffset + 4);
                    temp.PreviousNode = this;
                }
                catch
                {
                    return temp;
                }
                return temp;
            }
        }



        TextHeader textheader;
        public long StartOffset;
        public long EndOffset;
        public long ExtraSize;


        CharNode MainCharNode;
        byte[] CharInfoBlock;
        ushort[] allTextStartoffset;

        public IStream TextStreamFile;
        public Entry AssetEntry;

        public TextAsset(IStream stream, Entry assetentry) : base(stream)
        {
            AssetEntry = assetentry;


            TextStreamFile = stream;

            //   Console.WriteLine(stream.GetPosition());
            textheader = stream.GetStructureValues<TextHeader>();

            StartOffset = (stream.GetPosition() - 8) + textheader.StartOffset;//block Start

            //  Console.WriteLine(stream.GetStringValueN());

            stream.Seek(StartOffset);

            CharNode charNode = new CharNode();
            charNode.Stream = new MStream(stream.GetBytes((int)(textheader.CharsMapOffset - (StartOffset - (BlockStartOffset + 32)))));
            MainCharNode = charNode.ReadNodeValue(0);


            CharInfoBlock = stream.GetBytes((int)(textheader.CharInfoOffset - ((StartOffset - (BlockStartOffset + 48)) + charNode.Stream.GetSize())));
            allTextStartoffset = stream.GetArray<ushort>(textheader.TextCount);

            SkipPadding();

            EndOffset = stream.GetPosition();

        }


        public string[] GetStringFormFile()
        {
            byte[] temp = CharInfoBlock;
            int sheft = 0;

            Func<CharNode> getCharacterInfo = () =>
            {
                CharNode node = MainCharNode;

                if (node.Item != 0xA74)
                {
                    return node;
                }

                do
                {
                    if (sheft >= 8)
                    {
                        Array.Copy(temp, 1, temp, 0, temp.Length - 1);
                        sheft -= 8;
                    }

                    byte currchar = (byte)(temp[0] >> sheft++);

                    node = (currchar & 1) != 0 ? node.GetLeftCharNode() : node.GetRightCharNode();

                } while (node.Item == 0xA74);

                return node;
            };

            List<string> strings = new List<string>();

            for (int i = 0; i < textheader.TextCount; i++)
            {
                sheft = 0;

                StringBuilder id = new StringBuilder(50);
                CharNode charinfo;

                do
                {
                    charinfo = getCharacterInfo();

                    char charItem = (char)charinfo.Item;
                    string textcode = CodesDictionary.TryGetValue(charItem, out var code) ? code : charItem.ToString();
                    id.Append(textcode);

                } while (charinfo.Item != 0);

                StringBuilder text = new StringBuilder(50);
                charinfo = null;

                do
                {
                    charinfo = getCharacterInfo();

                    char charItem = (char)charinfo.Item;
                    string textcode = CodesDictionary.TryGetValue(charItem, out var code) ? code : charItem.ToString();
                    text.Append(textcode);

                } while (charinfo.Item != 0);

                strings.Add(string.Concat(id.ToString().TrimEnd('\0'), "=", text.ToString().TrimEnd('\0')));

                if (sheft <= 8)
                {
                    sheft = 0;
                    Array.Copy(temp, 1, temp, 0, temp.Length - 1);
                }
            }

            return strings.ToArray();
        }



        //repack


        static Dictionary<char, string> CharCode;
        static int Id = 0;
        static string GetPath(ushort id)
        {
            Id += 1;
            return new string(Convert.ToString(Id, 2).Reverse().ToArray());
        }


        private CharNode CreateMap(string Text)
        {
            CharCode = new Dictionary<char, string>();

            HashSet<char> chars = new HashSet<char>();
            foreach (char c in Text)
            {
                chars.Add(c);

            }
            chars.Add('\0');

            //علي النعمه ما عارف ايه لزمة ام الخريطة دي
            Id = chars.Count - 1;


            chars = chars.OrderBy(x => x).ToHashSet();

            const ushort PointerValue = 0xA74;
            CharNode MainCharNode = new CharNode
            {
                Item = PointerValue
            };

            foreach (char _char in chars)
            {
            GetCode:
                string BinNum = GetPath(_char);






                //char[] strbytes = BinNum.ToCharArray();
                //Array.Reverse(strbytes);
                //BinNum = new string(strbytes);

                //  Console.WriteLine(_char + " -> " + (int)_char + " ->" + BinNum);

                //  CharacterMap;
                CharNode Temp = MainCharNode;
                foreach (char num in BinNum)
                {

                    if (Temp.Item != PointerValue)
                    {
                        goto GetCode;
                       // throw new Exception("INVALID MAP |" + Temp.Item + "->" + CharCode[(char)Temp.Item] + "| -> " + _char + " -> " + (int)_char + " ->" + BinNum);
                    }


                    if (num == '0')
                    {

                        if (Temp.Right == null)
                        {
                            Temp.Right = new CharNode
                            {
                                Item = PointerValue
                            };
                        }
                        Temp = Temp.Right;


                    }
                    else
                    {
                        if (Temp.Left == null)
                        {
                            Temp.Left = new CharNode
                            {
                                Item = PointerValue
                            };

                        }
                        Temp = Temp.Left;
                    }

                }

                Temp.Item = _char;
                CharCode.Add(_char, BinNum);
            }

            return MainCharNode;
        }


        private int NodeCount(CharNode node, bool Right = false, bool Left = false, bool All = false)
        {
            if (node == null)
            {
                return 0;
            }

            if (All)
            {
                return NodeCount(node.Right, Right, Left, All) + NodeCount(node.Left, Right, Left, All) + 1;
            }

            if ((node.Left == null && Right) || (node.Right == null && Left))
            {
                return 1;
            }

            else if (node.Left != null && node.Right != null)
            {
                return 1;
            }

            return Right ? NodeCount(node.Right, Right, Left, All) : Left ? NodeCount(node.Left, Right, Left, All) : 0;
        }

        private void WriteMap(CharNode node, MStream mStream)
        {


            if (node == null)
            {
                return;
            }


            if (node.Item != 0xA74)
            {
                mStream.SetIntValue(0);
                mStream.SetIntValue(0);
                mStream.SetUShortValue(node.Item);
                mStream.SetUShortValue(0xBAAC);

            }
            else
            {
                mStream.SetIntValue(NodeCount(node, Right: true) * 12);

                int leftValue = ((NodeCount(node, Left: true) + NodeCount(node.Right, All: true)) * 12) - 4;
                if (node.Left == null)
                {
                    leftValue = 0;
                }

                mStream.SetIntValue(leftValue);
                mStream.SetUShortValue(node.Item);
                mStream.SetUShortValue(0xBAAC);
            }

            WriteMap(node.Right, mStream);
            WriteMap(node.Left, mStream);
        }

        private byte[] CreateCharMap(string text)
        {
            MStream stream = new MStream();
            WriteMap(CreateMap(text), stream);
            return stream.ToArray();
        }
        ///////////////
        public static (byte[], List<ushort>) ConvertTextToInfoTable(string Strings)
        {
            List<ushort> Offsets = new List<ushort>();
            StringBuilder tempBuilder = new StringBuilder();

            foreach (string str1 in Strings.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Where(x => x.Contains("=")))
            {
                if ((tempBuilder.Length / 8) > ushort.MaxValue)
                {
                    throw new Exception("Text is too long");
                }

                Offsets.Add((ushort)(tempBuilder.Length / 8));


                string[] Line = str1.Split(new[] { '=' }, 2);
                Line[0] += '\0';
                Line[1] += '\0';

                if (!string.IsNullOrEmpty(Line[0]))
                {
                    foreach (char ch in Line[0])
                    {
                        tempBuilder.Append(CharCode[ch]);
                    }
                }

                if (!string.IsNullOrEmpty(Line[1]))
                {
                    foreach (char ch in Line[1])
                    {
                        tempBuilder.Append(CharCode[ch]);
                    }
                }

                int padding = (8 - (tempBuilder.Length % 8));
                if (padding != 8)
                {
                    tempBuilder.Append('0', padding);
                }
            }

            string temp = tempBuilder.ToString();
            byte[] byteArray = new byte[temp.Length / 8];

            for (int i = 0; i < byteArray.Length; i++)
            {
                int sheft = 0;
                for (int j = 0; j < 8; j++)
                {
                    byteArray[i] |= (byte)(((byte)(temp[i * 8 + j] == '1' ? 1 : 0)) << sheft++);
                }
            }


            tempBuilder.Clear();
            tempBuilder = null;
            temp = null;

            return (byteArray, Offsets);
        }







        public void EditFile(string Strings)
        {
            foreach (var charinfo in CodesDictionary)
            {
                Strings = Strings.Replace(charinfo.Value, charinfo.Key + "");
            }



            var CharMap = CreateCharMap(Strings);
            (var TextInfo, var TextOffsetMap) = ConvertTextToInfoTable(Strings);

            textheader.CharsMapOffset = textheader.StartOffset + 48 + CharMap.Length;
            textheader.CharInfoOffset = textheader.StartOffset + 32 + CharMap.Length + TextInfo.Length;
            textheader.TextCount = TextOffsetMap.Count;
            textheader.TextCount_1 = TextOffsetMap.Count;
            textheader.Val1 = textheader.CharsMapOffset - 32;
            textheader.Val2 = TextInfo.Length;
            textheader.Val3 = TextInfo.Length;


            MStream mStream = new MStream();

            mStream.SetBytes(CharMap);
            mStream.SetBytes(TextInfo);
            foreach (ushort s in TextOffsetMap)
                mStream.SetUShortValue(s);

            if ((mStream.GetPosition() % 16) != 0)
                mStream.SetBytes(new byte[16 - (mStream.GetPosition() % 16)]);



            TextStreamFile.Seek(StartOffset);
            TextStreamFile.DeleteBytes((int)(EndOffset - StartOffset));
            TextStreamFile.InsertBytes(mStream.ToArray());

            long End = TextStreamFile.GetPosition();



            TextStreamFile.Seek(BlockStartOffset + 28);
            TextStreamFile.SetStructureValus(textheader);
            TextStreamFile.Seek(End);

            ExtraSize = End - EndOffset;

            AssetEntry.UpdateOffsets(ExtraSize);
        }









    }
}
