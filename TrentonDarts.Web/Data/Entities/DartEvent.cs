namespace TrentonDarts.Web.Data.Entities;

public class DartEvent
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? EventContact { get; set; }
    public string? EventContact2 { get; set; }
    public int? EventTypeId { get; set; }
    public DateTime? EventDate { get; set; }
    public DateTime? EventEndDate { get; set; }
    public string? DartType { get; set; }
    public int? ImageFileId { get; set; }
    public int? PosterFileId { get; set; }
    public string? PosterFile { get; set; }
    public string? Url { get; set; }
    public string? FacebookUrl { get; set; }
    public string? HostName { get; set; }
    public string? HostUrl { get; set; }
    public string? HostPhone { get; set; }
    public string? LocationName { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public string? RegistrationStartTime { get; set; }
    public string? RegistrationEndTime { get; set; }
    public string? DartStart { get; set; }
    public string? MapUrl { get; set; }
    public string? Description { get; set; }
    public bool? IsTitleEvent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<DartEventResult> Results { get; set; } = new List<DartEventResult>();

    public static readonly Dictionary<int, string> EventTypes = new()
    {
        { 1, "GTDL Event" }, { 2, "Charity Dart Event" }, { 3, "Regional Event" },
        { 4, "GTDL Sponsored Event" }, { 5, "GTDL All Stars" }, { 6, "GTDL Player Event" },
        { 7, "Charity Event" }, { 8, "DPNY Series" }, { 9, "CDC Series" },
        { 10, "DPNJ Series" }, { 11, "Qualifier" }
    };
}
