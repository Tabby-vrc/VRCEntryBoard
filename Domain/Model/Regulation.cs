namespace VRCEntryBoard.Domain.Model
{
    internal sealed class Regulation
    {
        public int ID { get; set; }
        public string RegulationName { get; set; }

        public Regulation(int id, string regulationName)
        {
            ID = id;
            RegulationName = regulationName;
        }
    }
}