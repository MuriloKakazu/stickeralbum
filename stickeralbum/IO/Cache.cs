﻿using System;
using System.IO;
using Newtonsoft.Json;
using stickeralbum.Design;
using stickeralbum.Entities;
using stickeralbum.Generics;
using stickeralbum.Debug;
using STDGEN = System.Collections.Generic;
using stickeralbum.Extensions;
using stickeralbum.Audio;

namespace stickeralbum.IO
{
    public static class Cache {
        private static STDGEN.Dictionary<String, Cacheable> CachedObjects
                 = new STDGEN.Dictionary<String, Cacheable>();

        private class ObjectNotFoundInCacheException : Exception {
            private String Key;
            public new String Message
                => $"No object with key <{Key}> found in cache.";
            public ObjectNotFoundInCacheException(String key)
                => Key = key;
        }

        public static void Clear()
            => CachedObjects.Clear();

        public static void Load() {
            try {
                DebugUtils.LogIO("Populating cache...");
                LoadDefaults();
                LoadCustoms();
                DebugUtils.LogIO($"Cache populated. {CachedObjects.Keys.Count} Objects Loaded.");
            } catch (Exception e) {
                DebugUtils.LogError($"Error populating cache. Reason: {e.Message}");
            }
        }

        public static void DumpLog() {
            DebugUtils.LogCache("Dumping Cache State...");
            CachedObjects.Values.ToLinkedList()
           .ForEach(x => DebugUtils.LogCache($"{x.ID} =>\n " +
            $"{JsonConvert.SerializeObject(x, Formatting.Indented)}"));
            DebugUtils.LogCache("Cache State Dumped!");
        }

        public static void LoadDefaults() {
            LoadGods();
            LoadChaos();
            LoadIcons();
            LoadTitans();
            LoadSprites();
            LoadSoundFX();
            LoadSemiGods();
            LoadCreatures();
        }

        public static void LoadCustoms() {
            LoadCustomGods();
            LoadCustomTitans();
            LoadCustomSprites();
            LoadCustomSemiGods();
            LoadCustomCreatures();
        }

        private static Chaos LoadChaos() {
            try {
                var x = JsonConvert.DeserializeObject<Chaos>(
                    File.ReadAllText(Paths.ChaosMetadata)
                );
                Add(x);
                return x;
            } catch (Exception e) {
                DebugUtils.LogError($"Could not load Chaos. Reason => {e.Message}");
                return null;
            }
        }
        private static LinkedList<Creature> LoadCustomCreatures()
            => JsonConvert.DeserializeObject<LinkedList<Creature>>
              (File.ReadAllText(Paths.CustomCreaturesMetadata))
              .ForEach(x => {
                  try {
                      x.IsCustom = true;
                      Add(x);
                  } catch (Exception e) {
                      DebugUtils.LogError($"Could not load custom creature <{x?.ID}>. Reason => {e.Message}");
                  }
              });

        private static LinkedList<SemiGod> LoadCustomSemiGods()
            => JsonConvert.DeserializeObject<LinkedList<SemiGod>>
              (File.ReadAllText(Paths.CustomSemiGodsMetadata))
              .ForEach(x => {
                  try {
                      x.IsCustom = true;
                      Add(x);
                  } catch (Exception e) {
                      DebugUtils.LogError($"Could not load custom semigod <{x?.ID}>. Reason => {e.Message}");
                  }
              });

        private static LinkedList<Sprite> LoadCustomSprites()
            => JsonConvert.DeserializeObject<LinkedList<Sprite>>
              (File.ReadAllText(Paths.CustomSpritesMetadata))
              .ForEach(x => {
                  try {
                      x.Path = Paths.CustomSpritesDirectory + x.Path;
                      x.LoadImage();
                      x.IsCustom = true;
                      Add(x);
                  } catch (Exception e) {
                      DebugUtils.LogError($"Could not load custom sprite <{x?.ID}>. Reason => {e.Message}");
                  }
              });

        private static LinkedList<Titan> LoadCustomTitans()
            => JsonConvert.DeserializeObject<LinkedList<Titan>>
              (File.ReadAllText(Paths.CustomTitansMetadata))
              .ForEach(x => {
                  try {
                      x.IsCustom = true;
                      Add(x);
                  } catch (Exception e) {
                      DebugUtils.LogError($"Could not load titan <{x?.ID}>. Reason => {e.Message}");
                  }
              });

        private static LinkedList<God> LoadCustomGods()
            => JsonConvert.DeserializeObject<LinkedList<God>>
              (File.ReadAllText(Paths.CustomGodsMetadata))
              .ForEach(x => {
                  try {
                      x.IsCustom = true;
                      Add(x);
                  } catch (Exception e) {
                      DebugUtils.LogError($"Could not load custom god <{x?.ID}>. Reason => {e.Message}");
                  }
              });

        private static LinkedList<Sprite> LoadIcons()
            => JsonConvert.DeserializeObject<LinkedList<Sprite>>
              (File.ReadAllText(Paths.IconsMetadata))
              .ForEach(x => {
                  try {
                  x.Path = Paths.IconsDirectory + x.Path;
                  x.LoadImage();
                  Add(x);
                  } catch (Exception e) {
                      DebugUtils.LogError($"Could not load icon <{x?.ID}>. Reason => {e.Message}");
                  }
              });

        private static LinkedList<God> LoadGods() 
            => JsonConvert.DeserializeObject<LinkedList<God>>
              (File.ReadAllText(Paths.GodsMetadata))
              .ForEach(x => {
                  try {
                      Add(x);
                  } catch (Exception e) {
                      DebugUtils.LogError($"Could not load god <{x?.ID}>. Reason => {e.Message}");
                  }
              });

        private static LinkedList<Titan> LoadTitans()
            => JsonConvert.DeserializeObject<LinkedList<Titan>>
              (File.ReadAllText(Paths.TitansMetadata))
              .ForEach(x => {
                  try {
                      Add(x);
                  } catch (Exception e) {
                      DebugUtils.LogError($"Could not load titan <{x?.ID}>. Reason => {e.Message}");
                  }
              });

        private static LinkedList<Sprite> LoadSprites()
            => JsonConvert.DeserializeObject<LinkedList<Sprite>>
              (File.ReadAllText(Paths.SpritesMetadata))
              .ForEach(x => {
                  try {
                      x.Path = Paths.SpritesDirectory + x.Path;
                      x.LoadImage();
                      Add(x);
                  } catch(Exception e) {
                      DebugUtils.LogError($"Could not load sprite <{x?.ID}>. Reason => {e.Message}");
                  }
              });

        private static LinkedList<SoundTrack> LoadSoundFX()
            => JsonConvert.DeserializeObject<LinkedList<SoundTrack>>
              (File.ReadAllText(Paths.SoundFXMetadata))
              .ForEach(x => {
                  try {
                      x.Path = Paths.AudioDirectory + x.Path;
                      x.Setup();
                      Add(x);
                  } catch (Exception e) {
                      DebugUtils.LogError($"Could not load sfx <{x?.ID}>. Reason => {e.Message}");
                  }
              });

        private static LinkedList<Creature> LoadCreatures() 
            => JsonConvert.DeserializeObject<LinkedList<Creature>>
              (File.ReadAllText(Paths.CreaturesMetadata))
              .ForEach(x => {
                  try {
                      Add(x);
                  } catch (Exception e) {
                      DebugUtils.LogError($"Could not load creature <{x?.ID}>. Reason => {e.Message}");
                  }
              });

        private static LinkedList<SemiGod> LoadSemiGods() 
            => JsonConvert.DeserializeObject<LinkedList<SemiGod>>
              (File.ReadAllText(Paths.SemiGodsMetadata))
              .ForEach(x => {
                  try {
                      Add(x);
                  } catch (Exception e) {
                      DebugUtils.LogError($"Could not load semigod <{x?.ID}>. Reason => {e.Message}");
                  }
              });

        private static void Add(Cacheable value) {
            CachedObjects.Add(value.ID.ToString(), value);
            DebugUtils.LogCache($"Object <{value.ID}> added to cache.");
        }

        private static void AddRange(LinkedList<Cacheable> values)
            => values.ForEach(x => {
                Add(x);
            });

        public static Boolean ContainsKey(String key)  
            => CachedObjects.ContainsKey(key);

        public static Cacheable Get(String key) {
            if (key == null) {
                DebugUtils.LogWarning("Cache can not return null key Object");
                return null;
            }
            if (CachedObjects.TryGetValue(key, out Cacheable value)) {
                return value;
            } else {
                DebugUtils.LogWarning($"No Object with key <{key}> found in cache.");
                return null;
            }
        }

        public static LinkedList<Cacheable> GetAll()
            => CachedObjects.Values.ToLinkedList();
    }
}
