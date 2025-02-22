public class PostItData
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public double Left { get; set; }
    public double Top { get; set; }
    public string Content { get; set; }
    public int GoalCount { get; set; }
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
}