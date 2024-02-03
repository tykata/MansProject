namespace EfectivoWork.Entities;

public class Meeting
{
    public string Topic { get; set; }
    public List<string> Participants { get; set; }
    public string Location { get; set; }
    public DateTime StartTime { get; set; }
    public TimeSpan Duration { get; set; }

    public Meeting(string topic, List<string> participants, string location, DateTime startTime, TimeSpan duration)
    {
        Topic = topic;
        Participants = participants;
        Location = location;
        StartTime = startTime;
        Duration = duration;
    }
}
