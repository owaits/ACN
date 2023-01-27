using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using LXProtocols.Acn.Rdm;
using LXProtocols.Acn.Rdm.Packets.Parameters;

namespace RdmSnoop.Brokers
{
    public class ParameterInformation
    {
        private ParameterDescription.GetReply parameter = null;

        public ParameterInformation(RdmParameters parameterId)
        {
            this.parameter = new ParameterDescription.GetReply() { ParameterId = parameterId };
        }

        public ParameterInformation(ParameterDescription.GetReply parameter)
        {
            this.parameter = parameter;
        }

        public RdmParameters ParameterId { get { return parameter.ParameterId;  } }

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
            if (!string.IsNullOrEmpty(parameter?.Description))
                return parameter.Description;
            return ParameterId.ToString();
        }
    }
}
