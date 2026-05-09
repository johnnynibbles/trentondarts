namespace TrentonDarts.Web.Data;

public static class Roles
{
    public const string Owner = "Owner";
    public const string Admin = "Admin";
    public const string BoardMember = "BoardMember";
    public const string Member = "Member";
    public const string User = "User";

    public static readonly IReadOnlyList<string> All = [Owner, Admin, BoardMember, Member, User];
}
