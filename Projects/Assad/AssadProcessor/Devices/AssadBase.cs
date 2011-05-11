﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using FiresecClient;

namespace AssadProcessor.Devices
{
    public abstract class AssadBase
    {
        public AssadBase()
        {
            Children = new List<AssadBase>();
        }

        public AssadBase Parent { get; set; }
        public List<AssadBase> Children { get; set; }
        public string DeviceId { get; set; }

        public abstract void SetInnerDevice(Assad.MHconfigTypeDevice innerDevice);
        public abstract Assad.DeviceType GetStates();
        public abstract void FireEvent(string eventName);
        public abstract Assad.DeviceType QueryAbility();

        List<AssadBase> allChildren;
        public List<AssadBase> FindAllChildren()
        {
            allChildren = new List<AssadBase>();
            allChildren.Add(this);
            FindChildren(this);
            return allChildren;
        }

        void FindChildren(AssadBase parent)
        {
            if (parent.Children != null)
                foreach (AssadBase child in parent.Children)
                {
                    allChildren.Add(child);
                    FindChildren(child);
                }
        }
    }
}