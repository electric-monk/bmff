using System;
using System.IO;
using MatrixIO.IO.Numerics;

namespace MatrixIO.IO.Bmff.Boxes
{
    /// <summary>
    /// Track Header Box ('tkhd')
    /// </summary>
    [Box("tkhd", "Track Header Box")]
    public sealed class TrackHeaderBox : FullBox
    {
        public TrackHeaderBox()
            : base() { }

        public TrackHeaderBox(Stream stream)
            : base(stream) { }

        public DateTime CreationTime { get; set; } = MovieHeaderBox.Convert1904Time(0);

        public DateTime ModificationTime { get; set; } = MovieHeaderBox.Convert1904Time(0);

        public uint TrackID { get; set; }

        private uint Reserved1 { get; set; }

        public ulong Duration { get; set; }

        private ulong Reserved2 { get; set; }

        public short Layer { get; set; }

        public short AlternateGroup { get; set; }

        public short Volume { get; set; }

        private ushort Reserved3 { get; set; }

        public int[] Matrix { get; set; } = new int[9] { 0x00010000, 0, 0, 0, 0x00010000, 0, 0, 0, 0x40000000 };  // Unity Matrix
        
        public FixedPoint_16_16 Width { get; set; }
        
        public FixedPoint_16_16 Height { get; set; }

        private new TrackFlags Flags
        {
            get => (TrackFlags)_flags;
            set => _flags = (uint)value;
        }

        public bool Enabled
        {
            get {
                return (Flags & TrackFlags.Enabled) != 0;
            }
            set {
                if (value)
                    Flags |= TrackFlags.Enabled;
                else
                    Flags &= ~TrackFlags.Enabled;
            }
        }

        public bool InMovie
        {
            get {
                return (Flags & TrackFlags.InMovie) != 0;
            }
            set {
                if (value)
                    Flags |= TrackFlags.InMovie;
                else
                    Flags &= ~TrackFlags.InMovie;
            }
        }

        public bool InPreview
        {
            get {
                return (Flags & TrackFlags.InPreview) != 0;
            }
            set {
                if (value)
                    Flags |= TrackFlags.InPreview;
                else
                    Flags &= ~TrackFlags.InPreview;
            }
        }

        public bool InPoster
        {
            get {
                return (Flags & TrackFlags.InPoster) != 0;
            }
            set {
                if (value)
                    Flags |= TrackFlags.InPoster;
                else
                    Flags &= ~TrackFlags.InPoster;
            }
        }

        public override ulong CalculateSize()
        {
            return base.CalculateSize() +
                (ulong)(Version == 1 ? 8 + 8 + 4 + 4 + 8 : 4 + 4 + 4 + 4 + 4) + (2 * 4) + 2 + 2 + 2 + 2 + (9 * 4) + 4 + 4;
        }

        protected override void LoadFromStream(Stream stream)
        {
            base.LoadFromStream(stream);

            if (Version == 1)
            {
                CreationTime = MovieHeaderBox.Convert1904Time(stream.ReadBEUInt64());
                ModificationTime = MovieHeaderBox.Convert1904Time(stream.ReadBEUInt64());
                TrackID = stream.ReadBEUInt32();
                Reserved1 = stream.ReadBEUInt32();
                Duration = stream.ReadBEUInt64();
            }
            else // if (Version == 0)
            {
                CreationTime = MovieHeaderBox.Convert1904Time(stream.ReadBEUInt32());
                ModificationTime = MovieHeaderBox.Convert1904Time(stream.ReadBEUInt32());
                TrackID = stream.ReadBEUInt32();
                Reserved1 = stream.ReadBEUInt32();
                Duration = stream.ReadBEUInt32();
            }

            Reserved2 = stream.ReadBEUInt64();
            Layer = stream.ReadBEInt16();
            AlternateGroup = stream.ReadBEInt16();
            Volume = stream.ReadBEInt16();
            Reserved3 = stream.ReadBEUInt16();

            for (int i = 0; i < 9; i++)
            {
                Matrix[i] = stream.ReadBEInt32();
            }

            Width = stream.ReadBEUInt32();
            Height = stream.ReadBEUInt32();
        }

        protected override void SaveToStream(Stream stream)
        {
            base.SaveToStream(stream);

            if (Version == 1)
            {
                stream.WriteBEUInt64(MovieHeaderBox.Convert1904Time(CreationTime));
                stream.WriteBEUInt64(MovieHeaderBox.Convert1904Time(ModificationTime));
                stream.WriteBEUInt32(TrackID);
                stream.WriteBEUInt32(Reserved1);
                stream.WriteBEUInt64(Duration);
            }
            else // if (Version == 0)
            {
                stream.WriteBEUInt32((uint)MovieHeaderBox.Convert1904Time(CreationTime));
                stream.WriteBEUInt32((uint)MovieHeaderBox.Convert1904Time(ModificationTime));
                stream.WriteBEUInt32(TrackID);
                stream.WriteBEUInt32(Reserved1);
                stream.WriteBEUInt32((uint)Duration);
            }

            stream.WriteBEUInt64(Reserved2);

            stream.WriteBEInt16(Layer);
            stream.WriteBEInt16(AlternateGroup);
            stream.WriteBEInt16(Volume);
            stream.WriteBEUInt16(Reserved3);

            for (int i = 0; i < 9; i++)
            {
                stream.WriteBEInt32(Matrix[i]);
            }

            stream.WriteBEUInt32(Width.Value);
            stream.WriteBEUInt32(Height.Value);
        }

        [Flags]
        public enum TrackFlags : int
        {
            Enabled = 0x000001,
            InMovie = 0x000002,
            InPreview = 0x000004,
            InPoster = 0x000008,
        }
    }
}