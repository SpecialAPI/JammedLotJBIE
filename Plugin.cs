using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace JammedLotJBIE
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "spapi.etg.jammedlotj";
        public const string NAME = "Jammed Lord of the Jammed";
        public const string VERSION = "1.1.0";

        public void Awake()
        {
            new Harmony(GUID).PatchAll();
        }
    }
}
