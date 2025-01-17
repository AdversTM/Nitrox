﻿using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using NitroxClient.Helpers;
using NitroxClient.MonoBehaviours;
using NitroxModel.Core;

namespace NitroxPatcher.Patches.Persistent
{
    public class ProtobufSerializer_Deserialize_Patch : NitroxPatch, IPersistentPatch
    {
        private static readonly Type TARGET_TYPE = typeof(ProtobufSerializer);
        private static readonly MethodInfo TARGET_METHOD = TARGET_TYPE.GetMethod("Deserialize", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly NitroxProtobufSerializer serializer = NitroxServiceLocator.LocateServicePreLifetime<NitroxProtobufSerializer>();

        public static bool Prefix(Stream stream, object target, Type type)
        {
            if (Multiplayer.Active && serializer.NitroxTypes.TryGetValue(type, out int key))
            {
                serializer.Deserialize(stream, target, type);
                return false;
            }

            return true;
        }

        public override void Patch(Harmony harmony)
        {
            PatchPrefix(harmony, TARGET_METHOD);
        }
    }
}
