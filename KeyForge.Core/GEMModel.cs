using System;
using System.Collections.Generic;
using System.Text;

namespace KeyForge.Core
{
    public class GEMModel
    {
        public Data data { get; set; }
        public int? ffg_league_lastpk { get; set; }
        public string app_version { get; set; }
    }
    public class Data
    {
        public List<object> tempKeys { get; set; }
        public EntityGroupMap entityGroupMap { get; set; }
        public string metadataVersion { get; set; }
    }
    public class EntityGroupMap
    {
        //public object Tournament { get; set; } 
        public Participant Participant { get; set; } 
        //public object GameSettings { get; set; } 
        //public object Tiebreaker { get; set; } 
        //public object Deck { get; set; } 
        //public object Match { get; set; } 
        public MatchParticipant MatchParticipant { get; set; } 
    }
    public class Participant
    {
        public List<ParticipantEntity> entities { get; set; }
    }
    public class MatchParticipant
    {
        public List<MatchParticipantEntity> entities { get; set; }
    }
    public class ParticipantEntity
    {
        public int? pk { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public int? ffg_id { get; set; }
        public string username { get; set; }
    }
    public class MatchParticipantEntity
    {
        public int? points_earned { get; set; }
        public int? match_pk { get; set; }
        public int? participant_pk { get; set; }
    }
}

