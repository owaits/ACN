using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LXProtocols.Citp.Packets.Msex
{
    public class CitpMsexLibraryId
    {
        #region Setup and Intialisation

        public CitpMsexLibraryId(byte number)
        {
            ParseNumber(number);
        }

        public CitpMsexLibraryId(byte level, byte level1, byte level2, byte level3)
        {
            Level = level;
            Level1 = level1;
            Level2 = level2;
            Level3 = level3;
        }

        public CitpMsexLibraryId()
        {
            Level = 0;
            Level1 = 0;
            Level2 = 0;
            Level3 = 0;
        }

        #endregion

        #region Operators

        public static bool operator == (CitpMsexLibraryId a,CitpMsexLibraryId b)
        {
            return a.Level == b.Level && a.Level1 == b.Level1 && a.Level2 == b.Level2 && a.Level3 == b.Level3;
        }

        public static bool operator !=(CitpMsexLibraryId a, CitpMsexLibraryId b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            CitpMsexLibraryId libraryId = obj as CitpMsexLibraryId;
            if (libraryId == null)
                return base.Equals(obj);

            return this == libraryId;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return new Tuple<byte,byte,byte,byte>(Level, Level1, Level2, Level3).GetHashCode();
        }

        #endregion



        public byte Level { get; set; }

        public byte Level1 { get; set; }

        public byte Level2 { get; set; }

        public byte Level3 { get; set; }

        public byte ToNumber()
        {
            if (Level != 1)
                throw new InvalidOperationException("Only Ids at level 1 can be converted to a number.");

            return Level1;
        }

        public void ParseNumber(byte number)
        {
            Level = 1;
            Level1 = number;
            Level2 = 0;
            Level3 = 0;
        }



        public static CitpMsexLibraryId ParseLibraryNumber(byte number)
        {
            CitpMsexLibraryId id = new CitpMsexLibraryId();
            id.ParseNumber(number);
            return id;
        }

        public static CitpMsexLibraryId Parse(string libraryId)
        {
            string[] elements = libraryId.Split(':','/','_','@');

            if (elements.Length != 4)
                throw new ArgumentException(@"The library id is not in the correct format. It must follow the format N:N/N/N", libraryId);

            CitpMsexLibraryId id = new CitpMsexLibraryId()
            {
                Level = byte.Parse(elements[0]),
                Level1 = byte.Parse(elements[1]),
                Level2 = byte.Parse(elements[2]),
                Level3 = byte.Parse(elements[3])
            };

            return id;           
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}/{2}/{3}",Level,Level1,Level2,Level3);
        }

        private static CitpMsexLibraryId empty = new CitpMsexLibraryId();

        public static CitpMsexLibraryId Empty
        {
            get { return empty; }
        }

    }

    public class CitpMsexLibraryIdComparer:IEqualityComparer<CitpMsexLibraryId>
    {

        public bool Equals(CitpMsexLibraryId x, CitpMsexLibraryId y)
        {
            return x.Level == y.Level && x.Level1 == y.Level1 && x.Level2 == y.Level2 && x.Level3 == y.Level3;
        }

        public int GetHashCode(CitpMsexLibraryId obj)
        {
            return obj.Level.GetHashCode() ^ obj.Level1.GetHashCode() ^obj.Level2.GetHashCode() ^obj.Level3.GetHashCode();
        }
    }
}
