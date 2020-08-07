using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LXProtocols.Citp.Packets.Msex;
using System.Drawing.Imaging;
using System.Net;

namespace LXProtocols.Citp.IO
{
    public class CitpBinaryReader:BinaryReader
    {
        public CitpBinaryReader(Stream input)
            : base(input)
        { }

        public string ReadCookie()
        {
            return Encoding.UTF8.GetString(ReadBytes(4));
        }

        public string ReadUcs1()
        {
            string readString = string.Empty;
            char newCharacter = ReadChar();

            while (newCharacter != 0)
            {
                readString += newCharacter;
                newCharacter = ReadChar();
            }

            return readString;
        }

        public string ReadUcs2()
        {
            string readString = string.Empty;
            char newCharacter = (char) ReadInt16();

            while (newCharacter != 0)
            {
                readString += newCharacter;
                newCharacter = (char) ReadInt16();
            }

            return readString;
        }

        public CitpMsexLibraryId ReadMsexLibraryId()
        {
            CitpMsexLibraryId newId = new CitpMsexLibraryId();

            newId.Level = ReadByte();
            newId.Level1 = ReadByte();
            newId.Level2 = ReadByte();
            newId.Level3 = ReadByte();

            return newId;

        }

        /// <summary>
        /// Reads a GUID from the stream as 15 bytes.
        /// </summary>
        /// <returns>The GUID that has been read.</returns>
        public Guid ReadGuid()
        {
            return new Guid(ReadBytes(15)).FromNetwork();
        }

        public static ImageFormat ConvertFormatCookie(string formatCookie)
        {
            switch (formatCookie)
            {
                case "RGB8":
                    return ImageFormat.Bmp;
                case "JPEG":
                    return ImageFormat.Jpeg;
                case "PNG ":
                    return ImageFormat.Png;
                default:
                    return ImageFormat.Png;
            }

        }
    }
}
