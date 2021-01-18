using System;
using System.IO;
using System.Text;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Data Entry Url Box ("url ")
    /// </summary>
    [Box("url ", "Data Entry Url Box")]
    public sealed class DataEntryUrlBox : FullBox
    {
        public DataEntryUrlBox()
            : base() { }

        public DataEntryUrlBox(Stream stream)
            : base(stream) { }

        public DataEntryUrlBox(string location)
        {
            Location = location;
        }

        public string Location { get; set; }

        private new DataEntryUrlFlags Flags
        {
            get => (DataEntryUrlFlags)_flags;
            set => _flags = (uint)value;
        }

        public bool MovieIsSelfContained
        {
            get {
                return (Flags & DataEntryUrlFlags.MovieIsSelfContained) != 0;
            }
            set {
                if (value)
                    Flags |= DataEntryUrlFlags.MovieIsSelfContained;
                else
                    Flags &= ~DataEntryUrlFlags.MovieIsSelfContained;
            }
        }

        public override ulong CalculateSize()
        {
            return base.CalculateSize() + (string.IsNullOrEmpty(Location) ? 0 : (ulong)Encoding.UTF8.GetByteCount(Location));
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            Location = stream.ReadNullTerminatedUTF8String();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            if (!string.IsNullOrEmpty(Location))
            {
                stream.WriteUTF8String(Location);
            }
        }

        [Flags]
        public enum DataEntryUrlFlags : int
        {
            MovieIsSelfContained = 0x000001,
        }
    }
}