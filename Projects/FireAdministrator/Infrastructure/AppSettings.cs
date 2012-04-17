﻿namespace Infrastructure
{
    public class AppSettings
    {
        public string ServiceAddress { get; set; }
        public string DefaultLogin { get; set; }
        public string DefaultPassword { get; set; }
        public bool AutoConnect { get; set; }
        public bool ShowVideo { get; set; }
        public string LibVlcDllsPath { get; set; }
        public bool ShowGK { get; set; }
        public bool ShowSKUD { get; set; }
        public bool IsDebug { get; set; }
    }
}