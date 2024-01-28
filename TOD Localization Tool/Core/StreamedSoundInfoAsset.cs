using Helper;

namespace TOD_Localization_Tool
{
    public class StreamedSoundInfoAsset : AssetHeader
    {
        Entry AssetEntry;
        public StreamedSoundInfoAsset(IStream stream, Entry assetentry) : base(stream)
        {
            AssetEntry = assetentry;
            stream.Skip(4);
            stream.Skip(stream.GetIntValue(false));
            stream.Skip(60);
            string SoundName = stream.GetStringValueN();
            SkipPadding();
        }
    }
}
