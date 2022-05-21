﻿using JetBrains.Annotations;
using Kingmaker.Blueprints;
using Kingmaker.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TabletopTweaks.Core.ModLogic;

namespace TabletopTweaks.Core.Config {
    public class Blueprints : IUpdatableSettings {
        [JsonProperty]
        private bool OverrideIds = false;
        [JsonProperty]
        private readonly SortedDictionary<string, Guid> NewBlueprints = new SortedDictionary<string, Guid>();
        [JsonProperty]
        private readonly SortedDictionary<string, Guid> DerivedBlueprintMasters = new SortedDictionary<string, Guid>();
        [JsonProperty]
        private readonly SortedDictionary<string, Guid> DerivedBlueprints = new SortedDictionary<string, Guid>();
        [JsonProperty]
        private readonly SortedDictionary<string, Guid> AutoGenerated = new SortedDictionary<string, Guid>();
        [JsonProperty]
        private readonly SortedDictionary<string, Guid> UnusedGUIDs = new SortedDictionary<string, Guid>();
        private readonly SortedDictionary<string, Guid> UsedGUIDs = new SortedDictionary<string, Guid>();
        [JsonIgnore]
        public bool Debug = false;
        [JsonIgnore]
        public ModContextBase Context;

        public void OverrideSettings(IUpdatableSettings userSettings) {
            var loadedSettings = userSettings as Blueprints;
            if (loadedSettings == null) { return; }
            if (loadedSettings.OverrideIds) {
                OverrideIds = loadedSettings.OverrideIds;
                loadedSettings.NewBlueprints.ForEach(entry => {
                    if (NewBlueprints.ContainsKey(entry.Key)) {
                        NewBlueprints[entry.Key] = entry.Value;
                    }
                });
                loadedSettings.DerivedBlueprintMasters.ForEach(entry => {
                    if (DerivedBlueprintMasters.ContainsKey(entry.Key)) {
                        DerivedBlueprintMasters[entry.Key] = entry.Value;
                    }
                });
            }
            loadedSettings.DerivedBlueprints.ForEach(entry => {
                DerivedBlueprints[entry.Key] = entry.Value;
            });
            loadedSettings.AutoGenerated.ForEach(entry => {
                AutoGenerated[entry.Key] = entry.Value;
            });
        }
        public BlueprintGuid GetGUID(string name) {
            Guid Id;
            if (!NewBlueprints.TryGetValue(name, out Id)) {
                if (!DerivedBlueprints.TryGetValue(name, out Id)) {
                    if (Context.Debug) {
                        if (!AutoGenerated.TryGetValue(name, out Id)) {
                            Id = Guid.NewGuid();
                            AutoGenerated.Add(name, Id);
                            Context.Logger.Log($"Generated new GUID: {name} - {Id}");
                        } else {
                            Context.Logger.Log($"WARNING: GUID: {name} - {Id} is autogenerated");
                        }
                    }
                }
            }
            if (Id == null) { Context.Logger.LogError($"ERROR: GUID for {name} not found"); }
            UsedGUIDs[name] = Id;
            return new BlueprintGuid(Id);
        }

        public BlueprintGuid GetDerivedMaster(string name) {

            Guid Id;
            if (!DerivedBlueprintMasters.TryGetValue(name, out Id)) {
                if (Context.Debug) {
                    if (!AutoGenerated.TryGetValue(name, out Id)) {
                        Id = Guid.NewGuid();
                        DerivedBlueprintMasters.Add(name, Id);
                        AutoGenerated.Add(name, Id);
                        Context.Logger.LogVerbose($"WARNING: MASTER GUID: {name} - {Id} is autogenerated");
                    }
                }
            }
            if (Id == null) { Context.Logger.LogError($"ERROR: MASTER GUID {name} not found"); }
            UsedGUIDs[name] = Id;
            return new BlueprintGuid(Id);
        }

        public BlueprintGuid GetDerivedGUID(string name, [NotNull] BlueprintGuid masterId, [NotNull] params BlueprintGuid[] componentIds) {
            Guid Id;
            if (!DerivedBlueprints.TryGetValue(name, out Id)) {
                NewBlueprints.TryGetValue(name, out Id);
            }
            if (Id == null || Id == Guid.Empty) {
                return DeriveGUID(name, masterId, componentIds);
            }
            UsedGUIDs[name] = Id;
            return new BlueprintGuid(Id);
        }

        public BlueprintGuid DeriveGUID(string name, [NotNull] BlueprintGuid masterId, [NotNull] params BlueprintGuid[] componentIds) {
            BlueprintGuid derivedID = componentIds.Aggregate(masterId, (aggregateID, componentID) => {
                byte[] aggregateBytes = aggregateID.ToByteArray();
                byte[] componentBytes = componentID.ToByteArray();
                for (int i = 0; i < aggregateBytes.Length; i++) {
                    aggregateBytes[i] = (byte)(aggregateBytes[i] ^ componentBytes[i]);
                }
                return new BlueprintGuid(new Guid(aggregateBytes));
            });
            UsedGUIDs[name] = derivedID.m_Guid;
            DerivedBlueprints.Add(name, derivedID.m_Guid);
            return derivedID;
        }

        public void GenerateUnused() {
            UnusedGUIDs.Clear();
            AutoGenerated.ForEach(entry => {
                if (!UsedGUIDs.ContainsKey(entry.Key)) {
                    UnusedGUIDs[entry.Key] = entry.Value;
                }
            });
            NewBlueprints.ForEach(entry => {
                if (!UsedGUIDs.ContainsKey(entry.Key)) {
                    UnusedGUIDs[entry.Key] = entry.Value;
                }
            });
        }

        public void RemoveUnused() {
            GenerateUnused();
            UnusedGUIDs.ForEach(entry => {
                if (AutoGenerated.ContainsKey(entry.Key)) { AutoGenerated.Remove(entry.Key); }
                if (NewBlueprints.ContainsKey(entry.Key)) { NewBlueprints.Remove(entry.Key); }
            });
            UnusedGUIDs.Clear();
        }

        public void Init() {
        }
    }
}
