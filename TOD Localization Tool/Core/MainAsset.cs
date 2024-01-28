using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;


namespace TOD_Localization_Tool
{
    public class Entry
    {
        public Entry NextNode;
        public Entry PrevNode;
        public MainAsset MainAsset;

        public long ExtraSize;
        public int index;
        long _entryoffset;
        public long EntryOffset
        {
            get
            {
                return _entryoffset + ExtraSize;
            }
            set
            {
                _entryoffset = value;
            }

        }
        public eAssetType type;

        public void UpdateOffsets(long size)
        {
            Entry Update(Entry entry)
            {
                if (entry.NextNode != null)
                {
                    entry.NextNode.EntryOffset += size;
                    Update(entry.NextNode);
                }
                return entry;
            }
            Update(this);
        }

    }
    public class MainAsset : IDisposable
    {

        public Dictionary<string, Entry> FilesEntres = new Dictionary<string, Entry>();

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct MainAssetHeader
        {
            public int m_EngineTimestamp;
            public int m_PropertyChecksum;
            public int m_CommandsChecksum;
            public int m_ResourcesTotal;
            public int m_AssetsHeaderSize;
            public int m_MaxBufferSize;
        };

        MainAssetHeader assetheader;

        IStream AssetBuffer;
        List<List<byte>> AssetsDataBuffers = new List<List<byte>>();

        public IStream MainAssetStream;

        public MainAsset(IStream stream)
        {
            MainAssetStream = stream;

            assetheader = stream.GetStructureValues<MainAssetHeader>();
            AssetBuffer = new MStream(stream.GetBytes(assetheader.m_AssetsHeaderSize));

            var m_AssetsSizes = stream.GetArray<int>(assetheader.m_ResourcesTotal);

            foreach (int BufferSize in m_AssetsSizes)
            {
                AssetsDataBuffers.Add(stream.GetBytes(BufferSize).ToList());
            }

            Entry PrevNode = null;
            for (int i = 0; i < assetheader.m_ResourcesTotal; i++)
            {
                eAssetType type = (eAssetType)AssetBuffer.GetIntValue(false);
                AssetHeader asset = GetAsset(type, i);
                //    Console.WriteLine(type);

                if (asset == null)
                {
                    break;
                }

                FilesEntres.Add(asset.FilePath.Trim('/'), new Entry() { EntryOffset = asset.BlockStartOffset, index = i, type = asset.header.assettype, MainAsset = this });

                if (PrevNode != null)
                {
                    PrevNode.NextNode = FilesEntres.Last().Value;
                    FilesEntres[asset.FilePath.Trim('/')].PrevNode = PrevNode;
                }

                PrevNode = FilesEntres.Last().Value;
            }

        }

        public void Dispose()
        {
            if (MainAssetStream != null)
            {
                MainAssetStream.Dispose();
                AssetsDataBuffers.Clear();
                FilesEntres.Clear();
                GC.SuppressFinalize(this);
            }
        }
        ~MainAsset()
        {
            Dispose();
        }
        private AssetHeader GetAsset(eAssetType type, int index, Entry entry = null)
        {
            AssetHeader asset = null;
            switch (type)
            {
                case eAssetType.STREAMEDSOUNDINFO:
                    asset = new StreamedSoundInfoAsset(AssetBuffer, entry);
                    break;

                case eAssetType.FONT:
                    asset = FontAsset.ReadFont(AssetBuffer, entry, AssetsDataBuffers[index]);
                    break;

                case eAssetType.TEXT:
                    asset = new TextAsset(AssetBuffer, entry);
                    break;
            }
            return asset;
        }


        public AssetHeader GetFile(Entry entry)
        {
            AssetBuffer.Seek(entry.EntryOffset);
            return GetAsset(entry.type, entry.index, entry);
        }

        public void UpdateOffsets(int start, long size)
        {
            //for (int i = start; i < FilesEntres.Count; i++)
            //{
            //    FilesEntres.ElementAt(i).Value.EntryOffset += size;
            //}
        }

        public Entry UpdateOffsets(Entry entry, long size)
        {
            if (entry.NextNode != null)
            {
                entry.NextNode.EntryOffset += size;
                UpdateOffsets(entry.NextNode, size);
            }
            return entry;
        }


        int MaxBuffer(List<List<byte>> bytes)
        {
            int output = 0;

            foreach (var buffer in bytes)
            {
                if (output < buffer.Count)
                {
                    output = buffer.Count;
                }
            }

            return output;

        }



        public void SaveFile(string FilePath)
        {

            assetheader.m_AssetsHeaderSize = (int)AssetBuffer.GetSize();
            assetheader.m_MaxBufferSize = MaxBuffer(AssetsDataBuffers);

            MainAssetStream.SetSize(24);
            MainAssetStream.SetPosition(0);

            MainAssetStream.SetStructureValus(assetheader);
            MainAssetStream.SetBytes(AssetBuffer.ToArray());

            foreach (var buffer in AssetsDataBuffers)
            {
                MainAssetStream.SetIntValue(buffer.Count);
            }

            foreach (var buffer in AssetsDataBuffers)
            {
                MainAssetStream.SetBytes(buffer.ToArray());
            }

            MainAssetStream.WriteFile(FilePath);
        }


    }
}
