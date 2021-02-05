﻿using PokeroleUI2.Databases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace PokeroleUI2
{
    public class PokemonData
    {
        public int PokemonID;
        private DexData _dexData;
        [XmlIgnoreAttribute]
        public DexData DexData {
            get { if(_dexData == null){
                    _dexData = new DexData(new ListData(PokemonID));
                }
                return _dexData;
            }
            set
            {
                _dexData = value;
            }
        }
        [XmlIgnoreAttribute]
        public string dexID { get { return DexData.DexID; } }

        //we're only using this for display purposes, dexname will never be needed on pokemondata if name is the same
        [XmlIgnoreAttribute]
        public string dexName { get {
                if(DexData.Name == Name)
                {
                    return "";
                }
                return DexData.Name;
            } }
        public string Name { get; set; }

        [XmlIgnoreAttribute]
        public string Type1 { get { return DexData.Type1; } }
        [XmlIgnoreAttribute]
        public string Type2 { get { return DexData.Type2; } }
        public int Rank { get; set; }
        [XmlIgnoreAttribute]
        public string TextRank { get { return PokemonUtils.RankFromInt(Rank); } }

        public float Height { get; set; }
        [XmlIgnoreAttribute]
        public string HeightString { get { return Height.ToString() + "m"; } }
        public float Weight { get; set; }
        [XmlIgnoreAttribute]
        public string WeightString { get { return Weight.ToString() + "kg"; } }

        public PkmnStatCollection Attributes;
        public PkmnStatCollection SocialAttributes;
        public PkmnStatCollection Skills;

        [XmlIgnoreAttribute]
        public int DEF { get { return Attributes.GetStatByTag("Vitality").Value; } }
        [XmlIgnoreAttribute]
        public int SPDEF { get { return Attributes.GetStatByTag("Insight").Value; } }

        public PkmnStat Happiness { get; set; }
        public PkmnStat Loyalty { get; set; }
        public int Battles { get; set; }
        public int Victories { get; set; }

        public string Nature { get; set; }
        public int Confidence { get; set; }
        public string Status { get; set; }
        public string Accessory { get; set; }
        public string Ribbons { get; set; }
        public string Item { get; set; }

        public PkmnSimpleStat HP { get; set; }
        public PkmnSimpleStat Will { get; set; }
        public List<string> Abilities;
        public int CurrentAbilityIndex;
        public int LearnableMoves = 4;
        public List<string> Moves;
        [XmlIgnoreAttribute]
        private LearnsetData _learnSet;
        [XmlIgnoreAttribute]
        public LearnsetData LearnSet
        {
            get
            {
                if (_learnSet == null) { _learnSet = new LearnsetData(PokemonID, Rank); }
                return _learnSet;        
            }
            set
            {
                _learnSet = value;
            }
        }

        public string Weaknesses;

        public PokemonData()
        {
        }

        public PokemonData(DexData dd)
        {
            this.DexData = dd;
            PokemonID = dd.ID;
            Name = dd.Name;
            Rank = PokemonUtils.RankFromString(dd.Rank);
            Weight = dd.Weight;
            Height = dd.Height;

            Abilities = dd.Abilities;
            Moves = new List<string>();
            PopulateStats(dd);
            UpdateDependencies();
        }

        public void PopulateStats(DexData dd)
        {
            Attributes = dd.Attributes.DeepCopy();
            PkmnStat TOU = new PkmnStat("Tough", 1, 5);
            PkmnStat COO = new PkmnStat("Cool", 1, 5);
            PkmnStat BEA = new PkmnStat("Beauty", 1, 5);
            PkmnStat CUT = new PkmnStat("Cute", 1, 5);
            PkmnStat CLE = new PkmnStat("Clever", 1, 5);
            List<PkmnStat> socattList = new List<PkmnStat> { TOU, COO, BEA, CUT, CLE };
            SocialAttributes = new PkmnStatCollection(socattList, 0, 0);

            PkmnStat BRAWL = new PkmnStat("Brawl", 0, 1);
            PkmnStat CHANN = new PkmnStat("Channel", 0, 1);
            PkmnStat CLASH = new PkmnStat("Clash", 0, 1);
            PkmnStat EVADE = new PkmnStat("Evasion", 0, 1);
            PkmnStat ALERT = new PkmnStat("Alert", 0, 1);
            PkmnStat ATHLE = new PkmnStat("Athletic", 0, 1);
            PkmnStat NATUR = new PkmnStat("Nature", 0, 1);
            PkmnStat STEAL = new PkmnStat("Stealth", 0, 1);
            PkmnStat ALLUR = new PkmnStat("Allure", 0, 1);
            PkmnStat ETIQU = new PkmnStat("Etiquette", 0, 1);
            PkmnStat INTIM = new PkmnStat("Intimidate", 0, 1);
            PkmnStat PERFO = new PkmnStat("Perform", 0, 1);
            List<PkmnStat> skillList = new List<PkmnStat> { BRAWL, CHANN, CLASH, EVADE, ALERT, ATHLE, NATUR, STEAL, ALLUR, ETIQU, INTIM, PERFO };
            Skills = new PkmnStatCollection(skillList, 0, 0);

            Happiness = new PkmnStat("Happiness", 0, 5);
            Loyalty = new PkmnStat("Happiness", 0, 5);

            Item = "Item";
            Nature = "Nature";
            Confidence = 0;
            Status = "Status";
            Accessory = "Accessory";
            Ribbons = "Ribbons";

            HP = new PkmnSimpleStat(dd.BaseHP, dd.BaseHP, dd.BaseHP);
            Will = new PkmnSimpleStat(0, Attributes.GetStatByTag("Insight").Value + 2, Attributes.GetStatByTag("Insight").Value + 2);
        }

        public void UpdateDependencies()
        {
            SocialAttributes.MaxPoints = PokemonUtils.GetSocialPointMax(Rank);
            Attributes.MaxPoints = PokemonUtils.GetAttributePointMax(Rank);
            Skills.MaxPoints = PokemonUtils.GetSkillPointMax(Rank);
            Skills.SetAllMax(PokemonUtils.GetSkillCap(Rank));
            HP.Max = HP.BaseVal + Attributes.GetStatByTag("Vitality").Value;
            Will.Max = Attributes.GetStatByTag("Insight").Value + 2;
            LearnSet = new LearnsetData(PokemonID, Rank);
            ReLengthMovesList();
        }

        public void ReLengthMovesList()
        {
            LearnableMoves = Attributes.GetStatByTag("Insight").Value + 2;
            List<string> theseMoves = new List<string>();
            for (int i = 0; i < LearnableMoves; i++)
            {
                if(Moves.Count > i)
                {
                    theseMoves.Add(Moves[i]);
                    continue;
                }
                theseMoves.Add(LearnSet.learnset[0].Name);
            }
            Moves = theseMoves;

        }
    }
}