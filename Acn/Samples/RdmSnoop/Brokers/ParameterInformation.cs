using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Acn.Rdm.Packets.Parameters;

namespace RdmSnoop.Brokers
{
    public class ParameterInformation
    {
        private ParameterDescription.GetReply parameter = null;

        public ParameterInformation(ParameterDescription.GetReply parameter)
        {
            this.parameter = parameter;
        }

        [Category("Limits")]
        public int MaxValue
        {
            get { return parameter.MaxValidValue; }
        }

        [Category("Limits")]
        public int MinValue
        {
            get { return parameter.MinValidValue; }
        }

        [Category("Limits")]
        public int DefaultValue
        {
            get { return parameter.DefaultValue; }
        }

        private int value = 0;

        [Category("Value")]
        public int Value
        {
            get { return value; }
            set { this.value = value;  }
        }

        public override string ToString()
        {
            if (parameter != null)
                return parameter.Description;
            return base.ToString();
        }
    }
}
