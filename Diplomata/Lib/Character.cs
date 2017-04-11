﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace DiplomataLib {

    [Serializable]
    public class DictAttr {
        public string key;
        public byte value;

        public DictAttr(string key) {
            this.key = key;
            value = 50;
        }
    }

    [Serializable]
    public class Character {
        public string name;
        public string description = "";
        public bool startOnPlay;
        public List<DictAttr> attributes = new List<DictAttr>();
        public byte influence = 50;
        public List<Message> messages = new List<Message>();

        public Character(string name) {
            this.name = name;

            Diplomata.Instantiate();
            SetAttributes();
            CheckRepeatedCharacter();

            JSONHandler.Create(this, name, "Diplomata/Characters/");
        }

        public void SetAttributes() {
            if (attributes.Count == 0) {
                foreach (string attrName in Diplomata.preferences.attributes) {
                    attributes.Add(new DictAttr(attrName));
                }
            }
        }

        public void CheckRepeatedCharacter() {
            bool canAdd = true;

            foreach (string characterName in Diplomata.preferences.characterList) {
                if (characterName == name) {
                    canAdd = false;
                    break;
                }
            }

            if (canAdd) {
                Diplomata.characters.Add(this);
                Diplomata.preferences.characterList.Add(name);
                JSONHandler.Update(Diplomata.preferences, "preferences", "Diplomata/");
            }
        }

        public static void UpdateList() {
            JSONHandler.CreateFolder("Diplomata/Characters/");

            var charactersFiles = Resources.LoadAll("Diplomata/Characters/");

            Diplomata.characters = new List<Character>();
            Diplomata.preferences.characterList = new List<string>();
            
            foreach (UnityEngine.Object obj in charactersFiles) {
                var json = (TextAsset)obj;
                var character = JsonUtility.FromJson<Character>(json.text);

                Diplomata.characters.Add(character);
                Diplomata.preferences.characterList.Add(obj.name);
            }
        }
    }

}