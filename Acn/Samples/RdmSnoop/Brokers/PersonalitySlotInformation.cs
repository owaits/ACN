using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Acn.Rdm.Packets.DMX;

namespace RdmSnoop
{
    public class PersonalitySlotInformation: INotifyPropertyChanged
    {
        #region Information

        [Category("Information")]
        public short Offset { get; set; }

        [Category("Information")]
        public SlotTypes Type { get; set; }

        [Category("Information")]
        public SlotIds Id { get; set; }

        [Category("Information")]
        public int SlotLink { get; set; }

        [Category("Information")]
        public string Description { get; set; }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Description))
                return Description;
            return Id.ToString();
        }
    }
}
