using System;
using System.Collections.Generic;
using SQLite.Net.Attributes;
using Newtonsoft.Json;

namespace Toggl.Phoebe._Data.Models
{
    public enum TimeEntryState {
        New,
        Running,
        Finished
    }

    public interface ITimeEntryData : ICommonData
    {
        TimeEntryState State { get; }
        string Description { get; }
        DateTime StartTime { get; }
        DateTime? StopTime { get; }
        bool DurationOnly { get; }
        bool IsBillable { get; }
        long UserRemoteId { get; }
        long WorkspaceRemoteId { get; }
        long? ProjectRemoteId { get; }
        long? TaskRemoteId { get; }
        Guid UserId { get; }
        Guid WorkspaceId { get; }
        Guid ProjectId { get; }
        Guid TaskId { get; }
        IReadOnlyList<string> Tags { get; }
        ITimeEntryData With (Action<TimeEntryData> transform);
    }

    [Table ("TimeEntryModel")]
    public class TimeEntryData : CommonData, ITimeEntryData
    {
        public TimeEntryData ()
        {
            State = TimeEntryState.New;
        }

        protected TimeEntryData (ITimeEntryData other) : base (other)
        {
            State = other.State;
            Description = other.Description;
            StartTime = other.StartTime;
            StopTime = other.StopTime;
            DurationOnly = other.DurationOnly;
            IsBillable = other.IsBillable;
            UserId = other.UserId;
            WorkspaceId = other.WorkspaceId;
            ProjectId = other.ProjectId;
            TaskId = other.TaskId;
            UserRemoteId = other.UserRemoteId;
            WorkspaceRemoteId = other.WorkspaceRemoteId;
            ProjectRemoteId = other.ProjectRemoteId;
            TaskRemoteId = other.TaskRemoteId;
        }

		public override object Clone ()
		{
			return new TimeEntryData (this);
		}
  
        public ITimeEntryData With (Action<TimeEntryData> transform)
        {
            return base.With (transform);
        }

        public TimeEntryState State { get; set; }

        public string Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? StopTime { get; set; }

        public bool DurationOnly { get; set; }

        public bool IsBillable { get; set; }

        public long UserRemoteId { get; set; }

        public long WorkspaceRemoteId { get; set; }

        public long? ProjectRemoteId { get; set; }

        public long? TaskRemoteId { get; set; }

        public Guid UserId { get; set; }

        public Guid WorkspaceId { get; set; }

        public Guid ProjectId { get; set; }

        public Guid TaskId { get; set; }

        [Ignore]
        public List<string> Tags { get; set; }

        IReadOnlyList<string> ITimeEntryData.Tags => Tags ?? new List<string> ();

        [JsonIgnore]
        public string RawTags
        {
            get {
				return JsonConvert.SerializeObject (Tags);
            }
            set {
				Tags = JsonConvert.DeserializeObject<List<string>> (value ?? "[]");
            }
        }

        public static string GetFormattedDuration (UserData user, TimeSpan duration)
        {
            string formattedString = duration.ToString (@"hh\:mm\:ss");
            if (user == null) {
                return formattedString;
            }

            if (user.DurationFormat == DurationFormat.Classic) {
                if (duration.TotalMinutes < 1) {
                    formattedString = duration.ToString (@"s\ \s\e\c");
                } else if (duration.TotalMinutes > 1 && duration.TotalMinutes < 60) {
                    formattedString = duration.ToString (@"mm\:ss\ \m\i\n");
                } else {
                    formattedString = duration.ToString (@"hh\:mm\:ss");
                }
            } else if (user.DurationFormat == DurationFormat.Decimal) {
                formattedString = String.Format ("{0:0.00} h", duration.TotalHours);
            }
            return formattedString;
        }
    }
}
