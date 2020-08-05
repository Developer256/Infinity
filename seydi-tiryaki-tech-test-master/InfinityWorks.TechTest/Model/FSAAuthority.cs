namespace InfinityWorks.TechTest.Model
{
    public class FsaAuthority
    {
        public int LocalAuthorityIdCode { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return $"FSAAuthority[id={LocalAuthorityIdCode}, name='{Name}']";
        }
    }
}
