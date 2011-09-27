using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Rdm
{
    public class UId:IComparable
    {
        protected UId()
        {
        }

        public UId(ushort manufacturerId,uint deviceId)
        {
            ManufacturerId = manufacturerId;
            DeviceId = deviceId;
        }

        public ushort ManufacturerId { get; protected set; }

        public uint DeviceId { get; protected set; }

        private static UId broadcast = new UId(0xFFFF, 0xFFFFFFFF);

        public static UId Broadcast 
        {
            get { return broadcast; }
        }

        private static UId empty = new UId();

        public static UId Empty
        {
            get { return empty; }
        }

        public static UId ManfacturerBroadcast(ushort manufacturerId)
        {
            return new UId(manufacturerId, 0xFFFFFFFF);
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", ManufacturerId.ToString("X4"), DeviceId.ToString("X8"));
        }

        public static UId NewUId(ushort manufacturerId)
        {
            Random randomId = new Random();
            return new UId(manufacturerId, (uint) randomId.Next(1,0x7FFFFFFF));
        }

        public static UId Parse(string value)
        {
            string[] parts = value.Split(':');
            return new UId((ushort) int.Parse(parts[0], System.Globalization.NumberStyles.HexNumber), (uint) int.Parse(parts[1], System.Globalization.NumberStyles.HexNumber));
        }

        public static UId ParseUrl(string url)
        {           
            string[] parts = url.Split('/');
            string idPart = parts[parts.Length - 1];
            
            //Normalize the string
            idPart = idPart.Replace("0x",string.Empty).Replace(":",string.Empty);

            return new UId((ushort) int.Parse(idPart.Substring(0, 4), System.Globalization.NumberStyles.HexNumber), (uint) int.Parse(idPart.Substring(4, 8), System.Globalization.NumberStyles.HexNumber));
        }

        public override int GetHashCode()
        {
            return ManufacturerId.GetHashCode() + DeviceId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            UId id = obj as UId;
            if (!object.ReferenceEquals(id, null))
                return id.ManufacturerId.Equals(ManufacturerId) && id.DeviceId.Equals(DeviceId);

            return base.Equals(obj);
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            UId id = obj as UId;

            if (id != null)
                return ManufacturerId.CompareTo(id.ManufacturerId) + DeviceId.CompareTo(id.DeviceId);

            return -1;
        }

        #endregion
    }
}
