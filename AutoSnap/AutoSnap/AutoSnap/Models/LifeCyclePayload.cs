using System;
using System.Collections.Generic;
using System.Text;

namespace AutoSnap.Models
{

    //CustomRendererと通信するためのコンテナ
    public class LifeCyclePayload
    {
        public LifeCycle Status { get; set; }
    }

    public enum LifeCycle
    {
        OnStart = 0,
        OnSleep = 1,
        OnResume = 2,
    }
}
