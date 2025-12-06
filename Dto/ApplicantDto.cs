namespace ProjeOgrenciYonetim.Web.Dto
{
    public class ApplicantDto
    {
        public int Id { get; set; } // Application Id
        public string Status { get; set; } = string.Empty;
        public string ApplyDate { get; set; } = string.Empty;

        public StudentDto Student { get; set; } = default!;
    }

    public class StudentDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string KnownTechnologies { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
